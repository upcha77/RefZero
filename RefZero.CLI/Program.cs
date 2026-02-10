using System;
using System.IO;
using System.CommandLine;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using RefZero.Core; // Still used for data models if any, or we can move ReferenceItem here.

namespace RefZero.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            // Root Command
            var rootCommand = new RootCommand("Ref-Zero: Reference Auditor and Collector (Out-of-Process)");

            var projectOption = new Option<FileInfo>(
                name: "--project",
                description: "The path to the project file.")
            { IsRequired = true };
            projectOption.AddAlias("-p");

            var outputOption = new Option<DirectoryInfo>(
                name: "--output",
                description: "The directory to output the collected DLLs.")
            { IsRequired = true };
            outputOption.AddAlias("-o");

            // Commands
            var collectCommand = new Command("collect", "Collect all project references.");
            collectCommand.AddOption(projectOption);
            collectCommand.AddOption(outputOption);
            collectCommand.SetHandler((FileInfo p, DirectoryInfo o) => CollectReferences(p, o), projectOption, outputOption);

            var analyzeCommand = new Command("analyze", "Analyze project references and output to JSON.");
            analyzeCommand.AddOption(projectOption);
            analyzeCommand.SetHandler((FileInfo p) => AnalyzeReferences(p), projectOption);

            var diagnosticsCommand = new Command("diagnostics", "Run system diagnostics.");
            diagnosticsCommand.AddOption(projectOption);
            diagnosticsCommand.SetHandler((FileInfo p) => RunDiagnostics(p), projectOption);

            rootCommand.AddCommand(collectCommand);
            rootCommand.AddCommand(analyzeCommand);
            rootCommand.AddCommand(diagnosticsCommand);

            // Clean Command
            var cleanCommand = new Command("clean", "Analyze unused references.");
            cleanCommand.AddOption(projectOption);
            var dryRunOption = new Option<bool>("--dry-run", "Simulate without changes.");
            var jsonOption = new Option<bool>("--json", "Output results as JSON.");
            cleanCommand.AddOption(dryRunOption);
            cleanCommand.AddOption(jsonOption);
            cleanCommand.SetHandler((FileInfo p) => RunCleanAnalysis(p), projectOption);
            rootCommand.AddCommand(cleanCommand);

            // Remove Command
            var removeCommand = new Command("remove", "Remove specific references.");
            removeCommand.AddOption(projectOption);
            var refsOption = new Option<string[]>(
                name: "--references",
                description: "References to remove.")
            { IsRequired = true, AllowMultipleArgumentsPerToken = true };
            refsOption.AddAlias("-r");
            removeCommand.AddOption(refsOption);
            removeCommand.SetHandler((FileInfo p, string[] r) => RemoveReferences(p, r), projectOption, refsOption);
            rootCommand.AddCommand(removeCommand);

            return rootCommand.Invoke(args);
        }

        // --- Core Logic: Out-of-Process Analysis ---

        private static List<ReferenceItem> ExecuteMSBuildAnalysis(FileInfo projectFile)
        {
            if (!projectFile.Exists) throw new FileNotFoundException("Project file not found", projectFile.FullName);

            // 1. Prepare Targets File
            string cliDir = AppDomain.CurrentDomain.BaseDirectory;
            string targetsSource = Path.Combine(cliDir, "RefZero.targets");
            if (!File.Exists(targetsSource))
            {
                throw new FileNotFoundException("RefZero.targets not found in CLI directory.", targetsSource);
            }

            // Create a wrapper project in the same directory as the target project
            // to preserve relative paths.
            string wrapperProject = Path.Combine(projectFile.DirectoryName, $"RefZeroWrapper_{Guid.NewGuid()}.proj");
            
            try
            {
                string wrapperContent = $@"<Project>
  <Import Project=""{projectFile.Name}"" />
  <Import Project=""{targetsSource}"" />
</Project>";
                File.WriteAllText(wrapperProject, wrapperContent);

                // 2. Build MSBuild Command
                // We build the wrapper project, targeting RefZeroDump
                var psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"msbuild \"{wrapperProject}\" /t:RefZeroDump /nologo /v:m /m:1",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // 3. Execute
                using (var proc = Process.Start(psi))
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    string error = proc.StandardError.ReadToEnd();
                    proc.WaitForExit();

                    if (proc.ExitCode != 0)
                    {
                        throw new Exception($"MSBuild execution failed (Exit Code {proc.ExitCode}).\nError: {error}\nOutput: {output}");
                    }

                    // 4. Parse Output
                    return ParseAnalysisOutput(output);
                }
            }
            finally
            {
                if (File.Exists(wrapperProject)) File.Delete(wrapperProject);
            }
        }

        private static List<ReferenceItem> ParseAnalysisOutput(string output)
        {
            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var results = new List<ReferenceItem>();
            bool capturing = false;

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (line.Contains("REFZERO_START")) // Relaxed check
                {
                    capturing = true;
                    continue;
                }
                if (line.Contains("REFZERO_END")) // Relaxed check
                {
                    capturing = false;
                    // Don't break immediately, in case REFZERO_END is on the same line as an item
                }

                if (capturing && line.StartsWith("ITEM|"))
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 4)
                    {
                        results.Add(new ReferenceItem
                        {
                            SourceType = parts[1], // Binary or Project
                            Name = parts[2],
                            PhysicalPath = parts[3] // Mapped to PhysicalPath
                        });
                    }
                }
            }
            return results;
        }

        // --- Command Handlers ---

        static void AnalyzeReferences(FileInfo projectFile)
        {
            try
            {
                var references = ExecuteMSBuildAnalysis(projectFile);
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                string json = System.Text.Json.JsonSerializer.Serialize(references, options);
                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Analysis Failed: {ex.Message}");
                Environment.Exit(1);
            }
        }

        static void CollectReferences(FileInfo projectFile, DirectoryInfo outputDir)
        {
            try
            {
                var references = ExecuteMSBuildAnalysis(projectFile);
                
                if (!outputDir.Exists) outputDir.Create();

                Console.WriteLine($"Collecting {references.Count} references to {outputDir.FullName}...");

                foreach (var refItem in references)
                {
                    if (File.Exists(refItem.PhysicalPath))
                    {
                        string destDir = outputDir.FullName;
                        
                        if (refItem.SourceType.Equals("Project", StringComparison.OrdinalIgnoreCase))
                        {
                            destDir = Path.Combine(outputDir.FullName, "refByPrj");
                            if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                        }

                        string dest = Path.Combine(destDir, Path.GetFileName(refItem.PhysicalPath));
                        File.Copy(refItem.PhysicalPath, dest, true);
                    }
                }
                Console.WriteLine("Collection completed.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Collection Failed: {ex.Message}");
                Environment.Exit(1);
            }
        }

        static void RunDiagnostics(FileInfo projectFile)
        {
            Console.WriteLine("=== RefZero Diagnostics (Out-of-Process) ===");
            Console.WriteLine($"Checking 'dotnet' availability...");
            
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var p = Process.Start(psi);
                string v = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                Console.WriteLine($"dotnet version: {v.Trim()}");
            }
            catch 
            {
                Console.WriteLine("CRITICAL: 'dotnet' command not found in PATH.");
            }

            Console.WriteLine($"\n[Analyzing Project: {projectFile.Name}]");
            try
            {
                var refs = ExecuteMSBuildAnalysis(projectFile);
                Console.WriteLine($"Analysis Successful. Found {refs.Count} references.");
                foreach(var r in refs.Take(5))
                {
                    Console.WriteLine($"- {r.Name} ({r.SourceType}) -> {r.PhysicalPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAnalysis FAILED:");
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("\n=== End Diagnostics ===");
        }

        static void RunCleanAnalysis(FileInfo projectFile)
        {
             try
             {
                 var allReferences = ExecuteMSBuildAnalysis(projectFile);
                 var cleaner = new ReferenceCleaner();
                 // We need to pass IEnumerable<IReferenceItem>, List<ReferenceItem> satisfies this.
                 var unused = cleaner.AnalyzeUnusedReferences(projectFile.FullName, allReferences);
                 
                 var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                 string json = System.Text.Json.JsonSerializer.Serialize(unused, options);
                 Console.WriteLine(json);
             }
             catch (Exception ex)
             {
                 Console.Error.WriteLine($"Clean Analysis Failed: {ex.Message}");
                 Environment.Exit(1);
             }
        }

        static void RemoveReferences(FileInfo projectFile, string[] references)
        {
            try
            {
                var cleaner = new ReferenceCleaner();
                cleaner.RemoveReferences(projectFile.FullName, references);
                Console.WriteLine($"Successfully removed {references.Length} references.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Remove Failed: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }
}
