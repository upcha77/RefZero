using System;
using System.IO;
using System.CommandLine;
using System.Linq;
using RefZero.Core;
using Microsoft.Build.Locator;
using System.Collections.Generic;

namespace RefZero.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            // Register MSBuild
            try
            {
                // Simple command line parsing to find project path early
                string projectPath = null;
                for (int i = 0; i < args.Length - 1; i++)
                {
                    if (args[i] == "-p" || args[i] == "--project")
                    {
                        projectPath = args[i + 1];
                        break;
                    }
                }

                // Heuristic: Check if project is SDK-style or Legacy
                // SDK-style projects (Visual Studio 2017+) should use DotNetSdk MSBuild if possible.
                // Legacy projects (verbose csproj) MUST use VisualStudio MSBuild.
                bool isSdkStyle = false;
                if (!string.IsNullOrEmpty(projectPath) && File.Exists(projectPath))
                {
                    try
                    {
                        // Quick text check. A full XML parse is safer but heavier.
                        // SDK projects start with <Project Sdk="...">
                        string content = File.ReadAllText(projectPath);
                        if (content.Contains("Sdk=\"") || content.Contains("Sdk = \"")) 
                        {
                            isSdkStyle = true;
                        }
                    }
                    catch { /* Ignore */ }
                }

                var query = MSBuildLocator.QueryVisualStudioInstances();
                IEnumerable<VisualStudioInstance> instances;

                if (isSdkStyle)
                {
                    // Prefer DotNetSdk for SDK-style projects (solves NETSDK1073)
                    instances = query
                        .OrderByDescending(i => i.DiscoveryType == DiscoveryType.DotNetSdk)
                        .ThenByDescending(i => i.Version);
                }
                else
                {
                    // Prefer VisualStudio for legacy projects
                    instances = query
                        .OrderByDescending(i => i.DiscoveryType == DiscoveryType.VisualStudioSetup)
                        .ThenByDescending(i => i.Version);
                }
                
                var instance = instances.FirstOrDefault();

                if (instance == null)
                {
                    Console.WriteLine("Error: No MSBuild instances found. Please install Visual Studio or Build Tools.");
                    return 1;
                }

                // Console.WriteLine($"Using MSBuild at: {instance.MSBuildPath}");
                MSBuildLocator.RegisterInstance(instance);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to register MSBuild: {ex.Message}");
                return 1;
            }

            // Define Commands
            var rootCommand = new RootCommand("Ref-Zero: Reference Auditor and Collector");

            var collectCommand = new Command("collect", "Collect all project references to a destination folder.");

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

            collectCommand.AddOption(projectOption);
            collectCommand.AddOption(outputOption);

            collectCommand.SetHandler((FileInfo projectFile, DirectoryInfo outputDir) =>
            {
                CollectReferences(projectFile, outputDir);
            }, projectOption, outputOption);

            rootCommand.AddCommand(collectCommand);

            var analyzeCommand = new Command("analyze", "Analyze project references and output to JSON.");
            analyzeCommand.AddOption(projectOption);
            
            analyzeCommand.SetHandler((FileInfo projectFile) =>
            {
                AnalyzeReferences(projectFile);
            }, projectOption);

            rootCommand.AddCommand(analyzeCommand);

            var cleanCommand = new Command("clean", "Identify and optionally remove unused references from a project.");
            cleanCommand.AddOption(projectOption);
            var dryRunOption = new Option<bool>("--dry-run", "Simulate the cleaning process without modifying files.");
            cleanCommand.AddOption(dryRunOption);
            var jsonOption = new Option<bool>("--json", "Output results in JSON format.");
            cleanCommand.AddOption(jsonOption);

            cleanCommand.SetHandler((FileInfo projectFile, bool dryRun, bool json) =>
            {
                CleanReferences(projectFile, dryRun, json);
            }, projectOption, dryRunOption, jsonOption);

            rootCommand.AddCommand(cleanCommand);

            var removeCommand = new Command("remove", "Remove specific references from a project.");
            removeCommand.AddOption(projectOption);
            var refsOption = new Option<string[]>(
                name: "--references",
                description: "The list of references to remove.")
            { IsRequired = true, AllowMultipleArgumentsPerToken = true };
            refsOption.AddAlias("-r");
            removeCommand.AddOption(refsOption);

            removeCommand.SetHandler((FileInfo projectFile, string[] references) =>
            {
                RemoveSpecificReferences(projectFile, references);
            }, projectOption, refsOption);

            rootCommand.AddCommand(removeCommand);

            var diagnosticsCommand = new Command("diagnostics", "Run system diagnostics to debug reference issues.");
            diagnosticsCommand.AddOption(projectOption);

            diagnosticsCommand.SetHandler((FileInfo projectFile) =>
            {
                RunDiagnostics(projectFile);
            }, projectOption);

            rootCommand.AddCommand(diagnosticsCommand);

            return rootCommand.Invoke(args);
        }

        static void RunDiagnostics(FileInfo projectFile)
        {
            Console.WriteLine("=== RefZero Diagnostics Mode ===");
            Console.WriteLine($"Time: {DateTime.Now}");
            Console.WriteLine($"OS: {Environment.OSVersion}");
            Console.WriteLine($"CLI Path: {AppDomain.CurrentDomain.BaseDirectory}");
            
            // 1. MSBuild Info
            try
            {
                // Re-run heuristic logic simply to display it (code duplication for display purposes, or we could refactor)
                // For now, let's just query all and show active.
                // Actually, Main() selects the instance. Here we can just show what MSBuildLocator *thinks* is registered?
                // MSBuildLocator doesn't expose "GetRegisteredInstance". 
                // But since we registered one, loading it into process, we can check a type?
                // A simpler way: The list we display here is just "Query". It doesn't tell us which one is *actually* used by the engine.
                // However, since we registered the FIRST one from our sorted list in Main,
                // we should replicate the sorting logic here to indicate which one is likely active.

                // Logic replication from Main:
                string projectPath = projectFile.FullName;
                bool isSdkStyle = false;
                if (projectFile.Exists)
                {
                    try
                    {
                        string content = File.ReadAllText(projectPath);
                        if (content.Contains("Sdk=\"") || content.Contains("Sdk = \"")) 
                        {
                            isSdkStyle = true;
                        }
                    }
                    catch {}
                }

                Console.WriteLine($"\n[Heuristic] Is SDK Style? {isSdkStyle}");
                
                var query = MSBuildLocator.QueryVisualStudioInstances();
                IEnumerable<VisualStudioInstance> instances;

                if (isSdkStyle)
                {
                    instances = query.OrderByDescending(i => i.DiscoveryType == DiscoveryType.DotNetSdk).ThenByDescending(i => i.Version);
                }
                else
                {
                    instances = query.OrderByDescending(i => i.DiscoveryType == DiscoveryType.VisualStudioSetup).ThenByDescending(i => i.Version);
                }

                Console.WriteLine($"[MSBuild Instances Found: {instances.Count()}]");
                int index = 1;
                foreach (var inst in instances)
                {
                    string activeMarker = (index == 1) ? " [ACTIVE (Selected)]" : "";
                    Console.WriteLine($"- Instance #{index++}{activeMarker}");
                    Console.WriteLine($"  Version: {inst.Version}");
                    Console.WriteLine($"  Path:    {inst.MSBuildPath}");
                    Console.WriteLine($"  Type:    {inst.DiscoveryType}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error querying MSBuild: {ex.Message}");
            }

            // 2. Project Analysis (Verbose)
            if (projectFile.Exists)
            {
                Console.WriteLine($"\n[Analyzing Project: {projectFile.Name}]");
                try
                {
                    var analyzer = new DependencyAnalyzer();
                    var projectInfo = analyzer.Analyze(projectFile.FullName);
                    
                    int count = projectInfo.References.Count();
                    Console.WriteLine($"Reference Count: {count}");
                    
                    if (count == 0)
                    {
                        Console.WriteLine("WARNING: 0 references found. Possible causes:");
                        Console.WriteLine("- Project not restored (try 'dotnet restore').");
                        Console.WriteLine("- Target framework not installed on this machine.");
                        Console.WriteLine("- MSBuild failed to resolve references silently.");
                    }
                    else
                    {
                        Console.WriteLine("First 5 references detected:");
                        foreach(var r in projectInfo.References.Take(5))
                        {
                            Console.WriteLine($"- {r.Name} ({r.SourceType})");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CRITICAL ERROR during analysis:");
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                 Console.WriteLine($"\nProject file not found: {projectFile.FullName}");
            }

            Console.WriteLine("\n=== End Diagnostics ===");
        }

        static void RemoveSpecificReferences(FileInfo projectFile, string[] references)
        {
            if (!projectFile.Exists)
            {
                Console.WriteLine($"Error: Project file '{projectFile.FullName}' does not exist.");
                return;
            }

            try
            {
                Console.WriteLine($"Removing {references.Length} references from {projectFile.Name}...");
                var cleaner = new ReferenceCleaner();
                cleaner.RemoveReferences(projectFile.FullName, references);
                Console.WriteLine("Removal completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing references: {ex.Message}");
            }
        }

        static void AnalyzeReferences(FileInfo projectFile)
        {
            if (!projectFile.Exists)
            {
                Console.WriteLine($"Error: Project file '{projectFile.FullName}' does not exist.");
                return;
            }

            try
            {
                var analyzer = new DependencyAnalyzer();
                var projectInfo = analyzer.Analyze(projectFile.FullName);
                
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                string json = System.Text.Json.JsonSerializer.Serialize(projectInfo.References, options);
                Console.WriteLine(json);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }

        static void CollectReferences(FileInfo projectFile, DirectoryInfo outputDir)
        {
            if (!projectFile.Exists)
            {
                Console.WriteLine($"Error: Project file '{projectFile.FullName}' does not exist.");
                return;
            }

            try
            {
                // Console.WriteLine($"Starting collection for: {projectFile.Name}"); // Suppress for cleaner output if needed, or keep.

                var analyzer = new DependencyAnalyzer();
                var projectInfo = analyzer.Analyze(projectFile.FullName);
                
                // Console.WriteLine($"Found {projectInfo.References.Count()} references.");

                var copier = new ReferenceCopier();
                copier.CopyReferences(projectInfo.References, outputDir.FullName);
                
                Console.WriteLine("Collection completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        static void CleanReferences(FileInfo projectFile, bool dryRun, bool json)
        {
            if (!projectFile.Exists)
            {
                Console.WriteLine($"Error: Project file '{projectFile.FullName}' does not exist.");
                return;
            }

            try
            {
                if (!json) Console.WriteLine($"Starting cleanup analysis for: {projectFile.Name}");

                var analyzer = new DependencyAnalyzer();
                var projectInfo = analyzer.Analyze(projectFile.FullName);
                var allRefs = projectInfo.References;

                if (!json) Console.WriteLine($"Total References: {allRefs.Count()}");

                var cleaner = new ReferenceCleaner();
                // We pass the project path to let cleaner find source files
                var unusedRefs = cleaner.AnalyzeUnusedReferences(projectFile.FullName, allRefs);

                if (json)
                {
                    var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(unusedRefs, options));
                    return;
                }

                Console.WriteLine($"\n--- Unused References Analysis ---");
                if (!unusedRefs.Any())
                {
                    Console.WriteLine("No unused references detected.");
                }
                else
                {
                    foreach (var unused in unusedRefs)
                    {
                        Console.WriteLine($"[Unused] {unused}");
                    }
                    Console.WriteLine($"Total Unused: {unusedRefs.Count()}");

                    if (dryRun)
                    {
                        Console.WriteLine("\n[Dry Run] No files modified.");
                    }
                    else
                    {
                        Console.WriteLine("\n[Action] Removing unused references from .csproj...");
                        cleaner.RemoveReferences(projectFile.FullName, unusedRefs);
                        Console.WriteLine("Cleanup completed.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during cleanup: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
