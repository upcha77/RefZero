using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RefZero.Core
{
    public class ReferenceCleaner
    {
        public IEnumerable<string> AnalyzeUnusedReferences(string projectPath, IEnumerable<IReferenceItem> allReferences)
        {
            var usedReferences = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var projectDir = Path.GetDirectoryName(projectPath);

            if (string.IsNullOrEmpty(projectDir) || !Directory.Exists(projectDir))
            {
                Console.WriteLine($"Error: Project directory not found for {projectPath}");
                return Enumerable.Empty<string>();
            }

            // 1. Collect all .cs files
            var csFiles = Directory.GetFiles(projectDir, "*.cs", SearchOption.AllDirectories);
            Console.WriteLine($"Analyzing {csFiles.Length} source files...");

            foreach (var file in csFiles)
            {
                // Skip obj and bin folders
                if (file.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar) ||
                    file.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar))
                {
                    continue;
                }

                var code = File.ReadAllText(file);
                var syntaxTree = CSharpSyntaxTree.ParseText(code);
                var root = syntaxTree.GetRoot();

                // 2. Extract Usings
                var usings = root.DescendantNodes().OfType<UsingDirectiveSyntax>();
                foreach (var u in usings)
                {
                    string namespaceName = u.Name.ToString();
                    usedReferences.Add(namespaceName);
                }

                // 3. Extract Type Usage (Simple identification)
                // Note: Without a full Compilation and SemanticModel, we can only guess based on names.
                // For a robust solution, we would need to load the project into a Roslyn Workspace, 
                // but that requires more heavy setup (MSBuildWorkspace).
                // Here, we try to match namespaces from references to using directives.
            }

            // 4. Map References to Usings/Types
            // This is a heuristic approach. Real references (DLLs) have assembly names,
            // but code uses Namespaces. We need a way to map Assembly -> Namespaces.
            // Since we don't have that map easily without loading the assemblies, 
            // we will assume that if a Reference Name appears in the "Used Namespaces", it is used.
            // *Limitation*: "Newtonsoft.Json.dll" usually provides "Newtonsoft.Json" namespace.
            // "System.Windows.Forms.dll" -> "System.Windows.Forms".
            
            // To make this better, we should inspect the Reference DLLs to get their exported namespaces.
            // But for now, let's use a simple name matching heuristic + common patterns.

            var unusedReferences = new List<string>();

            foreach (var refItem in allReferences)
            {
                string refName = refItem.Name; // e.g., "Newtonsoft.Json", "System.Data"
                
                // Heuristic: Check if any used namespace starts with the reference name
                // e.g. Used: "Newtonsoft.Json.Linq" -> Matches Reference "Newtonsoft.Json"
                bool isUsed = usedReferences.Any(u => u.StartsWith(refName, StringComparison.OrdinalIgnoreCase));

                if (!isUsed)
                {
                    // Special handling for core frameworks that might be implicitly used or have different namespace roots
                    if (IsCoreFramework(refName)) continue;

                    unusedReferences.Add(refName);
                }
            }

            return unusedReferences;
        }

        private bool IsCoreFramework(string name)
        {
            // Always keep these basics to avoid breaking the world
            var whitelist = new[]
            {
                "mscorlib",
                "System",
                "System.Core",
                "System.Runtime",
                "System.Collections",
                "System.Linq"
            };
            return whitelist.Contains(name, StringComparer.OrdinalIgnoreCase);
        }

        public void RemoveReferences(string projectPath, IEnumerable<string> referencesToRemove)
        {
            try
            {
                // We need to load it as a new collection to avoid conflicts with previous loads
                var collection = new Microsoft.Build.Evaluation.ProjectCollection();
                var project = collection.LoadProject(projectPath);

                var itemsToRemove = new List<Microsoft.Build.Evaluation.ProjectItem>();

                foreach (var refName in referencesToRemove)
                {
                    // Find items with ItemType 'Reference' or 'PackageReference'
                    var refs = project.GetItems("Reference").Where(i => i.EvaluatedInclude.StartsWith(refName, StringComparison.OrdinalIgnoreCase));
                    var packageRefs = project.GetItems("PackageReference").Where(i => i.EvaluatedInclude.Equals(refName, StringComparison.OrdinalIgnoreCase));
                    
                    itemsToRemove.AddRange(refs);
                    itemsToRemove.AddRange(packageRefs);
                }

                if (itemsToRemove.Count > 0)
                {
                    foreach (var item in itemsToRemove)
                    {
                        project.RemoveItem(item);
                    }
                    project.Save();
                }

                collection.UnloadProject(project);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update project file: {ex.Message}");
                throw;
            }
        }
    }
}
