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
                var instances = MSBuildLocator.QueryVisualStudioInstances().OrderByDescending(i => i.Version);
                var instance = instances.FirstOrDefault();

                if (instance == null)
                {
                    Console.WriteLine("Error: No MSBuild instances found. Please install Visual Studio or Build Tools.");
                    return 1;
                }

                Console.WriteLine($"Using MSBuild at: {instance.MSBuildPath}");
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

            var cleanCommand = new Command("clean", "Identify and optionally remove unused references from a project.");
            cleanCommand.AddOption(projectOption);
            var dryRunOption = new Option<bool>("--dry-run", "Simulate the cleaning process without modifying files.");
            cleanCommand.AddOption(dryRunOption);

            cleanCommand.SetHandler((FileInfo projectFile, bool dryRun) =>
            {
                CleanReferences(projectFile, dryRun);
            }, projectOption, dryRunOption);

            rootCommand.AddCommand(cleanCommand);

            return rootCommand.Invoke(args);
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
                Console.WriteLine($"Starting collection for: {projectFile.Name}");

                var analyzer = new DependencyAnalyzer();
                var projectInfo = analyzer.Analyze(projectFile.FullName);

                Console.WriteLine($"Found {projectInfo.References.Count()} references.");

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

        static void CleanReferences(FileInfo projectFile, bool dryRun)
        {
            if (!projectFile.Exists)
            {
                Console.WriteLine($"Error: Project file '{projectFile.FullName}' does not exist.");
                return;
            }

            try
            {
                Console.WriteLine($"Starting cleanup analysis for: {projectFile.Name}");

                var analyzer = new DependencyAnalyzer();
                var projectInfo = analyzer.Analyze(projectFile.FullName);
                var allRefs = projectInfo.References;

                Console.WriteLine($"Total References: {allRefs.Count()}");

                var cleaner = new ReferenceCleaner();
                // We pass the project path to let cleaner find source files
                var unusedRefs = cleaner.AnalyzeUnusedReferences(projectFile.FullName, allRefs);

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
