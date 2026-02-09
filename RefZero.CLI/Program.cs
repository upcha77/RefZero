using System;
using System.IO;
using System.CommandLine;
using System.Linq;
using RefZero.Core;
using Microsoft.Build.Locator;

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
    }
}
