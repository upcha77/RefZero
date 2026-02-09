using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Locator;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System.Collections.Generic;

namespace RefZero.PoC
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: RefZero.PoC <Path to .csproj>");
                return;
            }

            string projectPath = args[0];
            if (!File.Exists(projectPath))
            {
                Console.WriteLine($"Error: File not found: {projectPath}");
                return;
            }

            // 1. MSBuild Locator 초기화
            // 가장 최신의 MSBuild 인스턴스를 찾아서 등록합니다.
            // Visual Studio가 설치되어 있다면 해당 MSBuild를 사용합니다.
            var instances = MSBuildLocator.QueryVisualStudioInstances().OrderByDescending(i => i.Version);
            var instance = instances.FirstOrDefault();

            if (instances.Count() == 0)
            {
                Console.WriteLine("Error: No MSBuild instances found. Please install Visual Studio or Build Tools.");
                return;
            }

            Console.WriteLine($"Found {instances.Count()} MSBuild instances:");
            foreach (var inst in instances)
            {
                Console.WriteLine($" - {inst.Name} ({inst.Version}) @ {inst.MSBuildPath}");
            }

            if (instance == null)
            {
                // This should not happen due to the check above, but satisfies the compiler
                return;
            }
            Console.WriteLine($"Using MSBuild at: {instance.MSBuildPath}");
            MSBuildLocator.RegisterInstance(instance);

            AnalyzeProject(projectPath);
        }

        static void AnalyzeProject(string projectPath)
        {
            try
            {
                var projectCollection = new ProjectCollection();
                // 로거 추가
                projectCollection.RegisterLogger(new Microsoft.Build.Logging.ConsoleLogger(Microsoft.Build.Framework.LoggerVerbosity.Minimal));

                var project = projectCollection.LoadProject(projectPath);

                Console.WriteLine($"Project Loaded: {Path.GetFileName(projectPath)}");

                var references = project.GetItems("Reference");
                Console.WriteLine($"Found {references.Count} references (Static Analysis).Status: resolving...");

                var projectInstance = project.CreateProjectInstance();
                var buildRequest = new BuildRequestData(projectInstance, new[] { "ResolveAssemblyReferences" });

                var buildParameters = new BuildParameters(projectCollection);
                // 콘솔 로거 활성화 (상세 오류 확인용)
                buildParameters.Loggers = new List<ILogger> { new Microsoft.Build.Logging.ConsoleLogger(Microsoft.Build.Framework.LoggerVerbosity.Normal) };

                var buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequest);

                if (buildResult.OverallResult == BuildResultCode.Success)
                {
                    // ResolveAssemblyReferences 타겟의 결과 항목들은 보통 
                    // 'ReferencePath', 'ReferenceDependencyPaths' 등에 담깁니다.
                    // 확인 결과: 'ResolveAssemblyReferences' 타겟의 Output Item은 '_ResolvedProjectReferencePaths' 등이 있지만,
                    // 가장 확실한 것은 빌드 후 생성된 Item 그룹 중 'ReferencePath'를 확인하는 것입니다.
                    
                    // 주의: ResolveAssemblyReferences 타겟 실행 후 ProjectInstance의 아이템들이 갱신되었는지 확인 필요.
                    // 빌드 결과(TargetResult)에서 Output을 가져오는 것이 더 정확할 수 있습니다.
                    
                    if (buildResult.ResultsByTarget.TryGetValue("ResolveAssemblyReferences", out var targetResult))
                    {
                        // ReferencePath 아이템들이 해석된 참조 경로들입니다.
                        var resolvedReferences = targetResult.Items; // ITaskItem[]

                        Console.WriteLine($"\n--- Resolved References ({resolvedReferences.Length}) ---");
                        
                        foreach (var item in resolvedReferences)
                        {
                            string assemblyName = item.ItemSpec; // 보통 전체 경로가 됨
                            string originalSpec = item.GetMetadata("OriginalItemSpec");
                            string version = item.GetMetadata("Version");
                            string fusionName = item.GetMetadata("FusionName");
                            string hintPath = item.GetMetadata("HintPath");
                            
                            // ItemSpec 자체가 물리적 절대 경로인 경우가 많음 (ResolveAssemblyReferences 결과)
                            string physicalPath = assemblyName; 

                            Console.WriteLine($"[Reference]: {Path.GetFileName(assemblyName)}");
                            Console.WriteLine($"  - Path: {physicalPath}");
                            Console.WriteLine($"  - Version: {version}");
                            if(!string.IsNullOrEmpty(originalSpec))
                                Console.WriteLine($"  - Original: {originalSpec}");
                            // GAC 여부 판단 (경로 기반 또는 FusionName 확인)
                            if (physicalPath.Contains("Microsoft.NET\\assembly") || physicalPath.Contains("GAC_MSIL"))
                            {
                                Console.WriteLine($"  - Source: GAC/Framework");
                            }
                            else
                            {
                                Console.WriteLine($"  - Source: Local/NuGet");
                            }
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Target 'ResolveAssemblyReferences' did not produce expected output.");
                    }
                }
                else
                {
                    Console.WriteLine("Build failed during reference resolution.");
                    if(buildResult.Exception != null)
                        Console.WriteLine($"Exception: {buildResult.Exception}");
                }

                // Unload
                projectCollection.UnloadProject(project);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
