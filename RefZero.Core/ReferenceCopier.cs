using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RefZero.Core
{
    public class ReferenceCopier
    {
        public void CopyReferences(IEnumerable<IReferenceItem> references, string outputDirectory)
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var copiedFiles = new List<object>();

            // Group by filename to handle version conflicts
            var groupedReferences = references.GroupBy(r => Path.GetFileName(r.PhysicalPath));

            foreach (var group in groupedReferences)
            {
                string fileName = group.Key;
                
                // Conflict handling: Pick the one with the highest version or simple first one
                // For now, let's pick the first one and log if there are multiple
                var selectedRef = group.First();

                if (group.Count() > 1)
                {
                    Console.WriteLine($"Warning: Multiple versions found for {fileName}. Using {selectedRef.Version} from {selectedRef.PhysicalPath}");
                }

                if (string.IsNullOrEmpty(selectedRef.PhysicalPath) || !File.Exists(selectedRef.PhysicalPath))
                {
                    Console.WriteLine($"Error: File not found for reference {selectedRef.Name} at {selectedRef.PhysicalPath}");
                    continue;
                }

                string destPath = Path.Combine(outputDirectory, fileName);
                
                try
                {
                    File.Copy(selectedRef.PhysicalPath, destPath, true);
                    Console.WriteLine($"Copied: {fileName}");

                    copiedFiles.Add(new
                    {
                        Name = selectedRef.Name,
                        Version = selectedRef.Version,
                        Source = selectedRef.SourceType,
                        OriginalPath = selectedRef.PhysicalPath,
                        DestinationPath = destPath
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to copy {fileName}: {ex.Message}");
                }
            }

            // Generate Report
            string reportPath = Path.Combine(outputDirectory, "metrics.json");
            string json = JsonSerializer.Serialize(copiedFiles, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(reportPath, json);
            Console.WriteLine($"Report generated at: {reportPath}");
        }
    }
}
