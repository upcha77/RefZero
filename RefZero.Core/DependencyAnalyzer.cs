using System.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution; // Ensure this is present
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace RefZero.Core
{
    public interface IProjectInfo
    {
        string ProjectPath { get; }
        IEnumerable<IReferenceItem> References { get; }
    }

    public interface IReferenceItem
    {
        string Name { get; }
        string PhysicalPath { get; }
        string Version { get; }
        string SourceType { get; } // "GAC", "NuGet", "Local", etc.
    }

    public class ReferenceItem : IReferenceItem
    {
        public string Name { get; set; } = string.Empty;
        public string PhysicalPath { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string SourceType { get; set; } = "Unknown";
    }

    public class ProjectInfo : IProjectInfo
    {
        public string ProjectPath { get; set; } = string.Empty;
        public IEnumerable<IReferenceItem> References { get; set; } = new List<IReferenceItem>();
    }

    public class DependencyAnalyzer
    {
        private readonly ProjectCollection _projectCollection;

        public DependencyAnalyzer()
        {
            _projectCollection = new ProjectCollection();
            // 로거 설정 (필요 시)
             _projectCollection.RegisterLogger(new ConsoleLogger(LoggerVerbosity.Quiet));
        }

        public IProjectInfo Analyze(string projectPath)
        {
            if (!File.Exists(projectPath))
                throw new FileNotFoundException("Project file not found.", projectPath);

            var project = _projectCollection.LoadProject(projectPath);
            Console.WriteLine($"Analyzing: {Path.GetFileName(projectPath)}");

            var references = ResolveReferences(project);

            return new ProjectInfo
            {
                ProjectPath = projectPath,
                References = references
            };
        }

        private IEnumerable<IReferenceItem> ResolveReferences(Project project)
        {
            var results = new List<IReferenceItem>();

            // 1. Project Instance 생성 및 빌드 실행 (ResolveAssemblyReferences)
            var projectInstance = project.CreateProjectInstance();
            var buildRequest = new BuildRequestData(projectInstance, new[] { "ResolveAssemblyReferences" });
            var buildParameters = new BuildParameters(_projectCollection)
            {
                Loggers = new List<ILogger> { new ConsoleLogger(LoggerVerbosity.Quiet) }
            };

            var buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequest);

            if (buildResult.OverallResult != BuildResultCode.Success)
            {
                Console.WriteLine($"Build failed for resolution on {project.FullPath}");
                // 실패 시 정적 분석 결과라도 반환하거나 예외 처리?
                // 여기서는 빈 리스트 반환 후 진행 (또는 에러 로그)
                return results;
            }

            if (buildResult.ResultsByTarget.TryGetValue("ResolveAssemblyReferences", out var targetResult))
            {
                foreach (var item in targetResult.Items)
                {
                    string physicalPath = item.ItemSpec;
                    string name = Path.GetFileNameWithoutExtension(physicalPath);
                    string version = item.GetMetadata("Version");
                    
                    // GAC/Framework 판단
                    bool isGac = physicalPath.Contains("Microsoft.NET\\assembly") || 
                                 physicalPath.Contains("GAC_MSIL") ||
                                 physicalPath.Contains("Microsoft.NETCore.App.Ref") ||
                                 physicalPath.Contains("Microsoft.WindowsDesktop.App.Ref");

                    results.Add(new ReferenceItem
                    {
                        Name = name,
                        PhysicalPath = physicalPath,
                        Version = version,
                        SourceType = isGac ? "Framework/GAC" : "Local/NuGet"
                    });
                }
            }

            // 2. ProjectReference 재귀 추적
            // ResolveAssemblyReferences 결과에는 전이적 종속성(Transitive Dependencies)이 보통 포함되지만,
            // ProjectReference로 연결된 프로젝트의 출력물(DLL)도 포함되는지 확인 필요.
            // 보통 '_ResolvedProjectReferencePaths' 아이템에 프로젝트 출력이 있음.
            
            // 만약 재귀적으로 소스 프로젝트를 분석해야 한다면 여기서 재귀 호출.
            // 하지만 'ResolveAssemblyReferences' 타겟이 성공했다면, 
            // 해당 프로젝트가 참조하는 모든 DLL(프로젝트 참조 결과물 포함)의 경로가 Output Items에 포함되어 있어야 함.
            
            return results;
        }
        
        public void Unload()
        {
            _projectCollection.UnloadAllProjects();
            _projectCollection.Dispose();
        }
    }
}
