using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RefZero.GUI
{
    public static class CliWrapper
    {
        // Try to find the CLI executable relative to the GUI, or in a known location
        private static string GetCliPath()
        {
            // 1. Look for RefZero.CLI.exe in the same folder (Release/Publish scenario)
            string localExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RefZero.CLI.exe");
            if (File.Exists(localExe)) return localExe;

            // 2. Development scenario: Look in the source build output
            // Assumes we are in RefZero.GUI\bin\Debug\net48
            // CLI is in RefZero.CLI\bin\Debug\net8.0\RefZero.CLI.exe
            string devExe = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\RefZero.CLI\bin\Debug\net8.0\RefZero.CLI.exe"));
            if (File.Exists(devExe)) return devExe;

            throw new FileNotFoundException("Could not find RefZero.CLI.exe", devExe);
        }

        public static List<ReferenceItem> Analyze(string projectPath)
        {
            string cliPath = GetCliPath();
            var output = RunProcess(cliPath, $"analyze -p \"{projectPath}\"");
            
            // Extract JSON part
            int jsonStart = output.IndexOf('[');
            if (jsonStart == -1) 
            {
                 throw new Exception($"No JSON output found in CLI response.\nOutput: {output}");
            }
            string json = output.Substring(jsonStart);

            try
            {
                return JsonSerializer.Deserialize<List<ReferenceItem>>(json);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse analysis result: {ex.Message}\nOutput: {output}");
            }
        }

        public static FileInfo Collect(string projectPath, string outputDir)
        {
            string cliPath = GetCliPath();
            string output = RunProcess(cliPath, $"collect -p \"{projectPath}\" -o \"{outputDir}\"");
            // Collect just returns text log
            return new FileInfo(Path.Combine(outputDir, "metrics.json"));
        }

        public static List<string> RunCleanAnalysis(string projectPath)
        {
            string cliPath = GetCliPath();
            string output = RunProcess(cliPath, $"clean -p \"{projectPath}\" --dry-run --json");

            // Extract JSON part
            int jsonStart = output.IndexOf('[');
            if (jsonStart == -1) 
            {
                 throw new Exception($"No JSON output found in CLI response.\nOutput: {output}");
            }
            string json = output.Substring(jsonStart);

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse cleanup analysis: {ex.Message}\nOutput: {output}");
            }
        }

        public static void RemoveReferences(string projectPath, IEnumerable<string> references)
        {
            string cliPath = GetCliPath();
            // Build arguments: -r "Ref1" -r "Ref2" ...
            string refsArgs = "";
            foreach (var r in references)
            {
                refsArgs += $"-r \"{r}\" ";
            }

            string output = RunProcess(cliPath, $"remove -p \"{projectPath}\" {refsArgs}");
            // Output is log
        }

        public static string TryRunDiagnostics(string projectPath)
        {
            try
            {
                string cliPath = GetCliPath();
                return RunProcess(cliPath, $"diagnostics -p \"{projectPath}\"");
            }
            catch (Exception ex)
            {
                return $"Diagnostics execution failed: {ex.Message}";
            }
        }

        private static string RunProcess(string exePath, string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var proc = Process.Start(psi))
            {
                string stdout = proc.StandardOutput.ReadToEnd();
                string stderr = proc.StandardError.ReadToEnd();
                proc.WaitForExit();

                if (proc.ExitCode != 0)
                {
                    throw new Exception($"CLI execution failed (Exit Code {proc.ExitCode}).\nError: {stderr}\nLog: {stdout}");
                }

                return stdout;
            }
        }
    }
}
