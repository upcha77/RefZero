using System.Collections.Generic;

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
}
