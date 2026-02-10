using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RefZero.GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private bool IsSolution(string path)
        {
            return path.EndsWith(".sln", StringComparison.OrdinalIgnoreCase);
        }

        private void btnBrowseProject_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Project/Solution Files (*.csproj;*.sln)|*.csproj;*.sln|Project Files (*.csproj)|*.csproj|Solution Files (*.sln)|*.sln";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtProjectPath.Text = ofd.FileName;
                }
            }
        }

        private void btnAnalyzeOnly_Click(object sender, EventArgs e)
        {
            string path = txtProjectPath.Text;

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                MessageBox.Show("Please select a valid project or solution file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            rtbLog.AppendText($"Starting analysis for {Path.GetFileName(path)}...\n");
            
            try
            {
                var projects = new List<string>();
                if (IsSolution(path))
                {
                    projects = CliWrapper.GetProjectsInSolution(path);
                    rtbLog.AppendText($"Solution contains {projects.Count} projects.\n");
                }
                else
                {
                    projects.Add(path);
                }

                foreach (var proj in projects)
                {
                    rtbLog.AppendText($"--- Analyzing Project: {Path.GetFileName(proj)} ---\n");
                    try 
                    {
                        var references = CliWrapper.Analyze(proj);
                        rtbLog.AppendText($"Found {references.Count} references.\n");
                        foreach(var refItem in references)
                        {
                            rtbLog.AppendText($"[REF] {refItem.Name} ({refItem.Version}) -> {refItem.PhysicalPath} [{refItem.SourceType}]\n");
                        }
                    }
                    catch(Exception ex)
                    {
                        rtbLog.AppendText($"Failed to analyze {Path.GetFileName(proj)}: {ex.Message}\n");
                    }
                    rtbLog.AppendText("\n");
                }
                
                rtbLog.AppendText("Analysis completed for all targets.\n");
            }
            catch (Exception ex)
            {
                rtbLog.AppendText($"Error: {ex.Message}\n");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDiagnostics_Click(object sender, EventArgs e)
        {
            string projectPath = txtProjectPath.Text;

            // Allow empty path for diagnostics (it might checking general environment)
            // But CLI expects -p, so maybe we should enforce it or pass dummy?
            // Let's enforce it for now as per CLI requirement.
            if (string.IsNullOrEmpty(projectPath))
            {
                 rtbLog.AppendText("Running diagnostics without project file...\n");
                 // If CLI supported no-project diagnostics, we would call it. 
                 // Current CLI requires -p. Let's just ask user to pick one or just run on current dir if possible?
                 // For now, let's just warn.
                 MessageBox.Show("Please select a project file to analyze.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 return;
            }

            rtbLog.AppendText($"\nTODO: Running Diagnostics on {Path.GetFileName(projectPath)}...\n");
            rtbLog.AppendText("=============================================\n");
            
            try
            {
                // Force UI repainting
                Application.DoEvents(); 

                var result = CliWrapper.TryRunDiagnostics(projectPath);
                rtbLog.AppendText(result);
            }
            catch (Exception ex)
            {
                rtbLog.AppendText($"Diagnostics failed validation: {ex.Message}\n");
            }

            rtbLog.AppendText("\n=============================================\n");
            rtbLog.ScrollToCaret();
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtOutputPath.Text = fbd.SelectedPath;
                }
            }
        }

        private void btnCollect_Click(object sender, EventArgs e)
        {
            string path = txtProjectPath.Text;
            string outputPath = txtOutputPath.Text;

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                MessageBox.Show("Please select a valid project or solution file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                MessageBox.Show("Please select an output directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            rtbLog.AppendText($"Starting collection for {Path.GetFileName(path)}...\n");
            
            try
            {
                var projects = new List<string>();
                if (IsSolution(path))
                {
                    projects = CliWrapper.GetProjectsInSolution(path);
                    rtbLog.AppendText($"Solution contains {projects.Count} projects.\n");
                }
                else
                {
                    projects.Add(path);
                }

                foreach (var proj in projects)
                {
                    rtbLog.AppendText($"--- Collecting: {Path.GetFileName(proj)} ---\n");
                    try
                    {
                        // CliWrapper.Collect executes the CLI 'collect' command
                        CliWrapper.Collect(proj, outputPath);
                        rtbLog.AppendText($"Collection for {Path.GetFileName(proj)} completed.\n");
                    }
                    catch (Exception ex)
                    {
                        rtbLog.AppendText($"Failed to collect {Path.GetFileName(proj)}: {ex.Message}\n");
                    }
                }
                
                rtbLog.AppendText("All collections completed.\n");
                MessageBox.Show("Collection completed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                rtbLog.AppendText($"Error: {ex.Message}\n");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            string outputPath = txtOutputPath.Text;
            if (string.IsNullOrEmpty(outputPath) || !Directory.Exists(outputPath))
            {
                MessageBox.Show("Output directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            try
            {
                Process.Start("explorer.exe", outputPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            string path = txtProjectPath.Text;

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                MessageBox.Show("Please select a valid project or solution file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblStatus.Text = "Analyzing unused references...";
            clbUnusedRefs.Items.Clear();

            try
            {
                var projects = new List<string>();
                if (IsSolution(path))
                {
                    projects = CliWrapper.GetProjectsInSolution(path);
                }
                else
                {
                    projects.Add(path);
                }

                int totalUnused = 0;
                foreach (var proj in projects)
                {
                    try
                    {
                        var unusedRefs = CliWrapper.RunCleanAnalysis(proj);
                        if (unusedRefs.Count > 0)
                        {
                            string projName = Path.GetFileName(proj);
                            foreach (var item in unusedRefs)
                            {
                                // Store as "[ProjectName] ReferenceName" to identify source project
                                // We might need full path to distinguish same-named projects, but usually filename is enough context 
                                // or we can use a delimited string and just display differently.
                                // For simplicity: "[ProjectFileName] ReferenceName"
                                clbUnusedRefs.Items.Add($"[{projName}] {item}", true); // Default checked
                            }
                            totalUnused += unusedRefs.Count;
                        }
                    }
                    catch (Exception ex)
                    {
                        rtbLog.AppendText($"Clean analysis failed for {Path.GetFileName(proj)}: {ex.Message}\n");
                    }
                }

                if (totalUnused == 0)
                {
                    MessageBox.Show("No unused references found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblStatus.Text = "No unused references found.";
                }
                else
                {
                    chkSelectAll.Checked = true;
                    lblStatus.Text = $"Found {totalUnused} unused references.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Analysis failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Analysis failed.";
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (clbUnusedRefs.CheckedItems.Count == 0)
            {
                MessageBox.Show("No items selected to remove.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = txtProjectPath.Text;
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                MessageBox.Show("Please select a valid project or solution file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to remove {clbUnusedRefs.CheckedItems.Count} references?",
                "Confirm Removal",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    // Group removals by project
                    // Item format: "[ProjectFileName] ReferenceName"
                    var removalMap = new Dictionary<string, List<string>>(); // ProjectName -> List<Ref>
                    
                    // Note: If single project analyze was run, format might be just "ReferenceName" IF we didn't change logic there.
                    // BUT we changed logic above to always use projects list so even single project has "[Proj] Ref" format now?
                    // Let's re-verify logic above.
                    // Yes: even for single project, we add it to list and loop, so it gets "[Proj] " prefix.
                    // Wait, earlier logic: projects.Add(path). So yes.
                    
                    // We need to map ProjectFileName back to full path if possible, or just assume unique filenames in solution?
                    // The CliWrapper.GetProjectsInSolution returns full paths.
                    // But we stored just FileName in the listbox.
                    // We need to map back.
                    
                    var allProjects = new List<string>();
                    if (IsSolution(path))
                    {
                        allProjects = CliWrapper.GetProjectsInSolution(path);
                    }
                    else
                    {
                        allProjects.Add(path);
                    }
                    
                    // Map: FileName -> FullPath
                    var projectFileMap = allProjects.ToDictionary(p => Path.GetFileName(p), p => p, StringComparer.OrdinalIgnoreCase);

                    foreach (string item in clbUnusedRefs.CheckedItems)
                    {
                        int bracketEnd = item.IndexOf(']');
                        if (bracketEnd > 1 && item.StartsWith("["))
                        {
                            string projName = item.Substring(1, bracketEnd - 1); // "ProjectFileName"
                            string refName = item.Substring(bracketEnd + 1).Trim();
                            
                            if (projectFileMap.TryGetValue(projName, out string fullPath))
                            {
                                if (!removalMap.ContainsKey(fullPath))
                                    removalMap[fullPath] = new List<string>();
                                
                                removalMap[fullPath].Add(refName);
                            }
                            else
                            {
                                // Should not happen if solution didn't change
                                rtbLog.AppendText($"Warning: Could not find project path for {projName}\n");
                            }
                        }
                        else
                        {
                            // Fallback for legacy format or error
                             rtbLog.AppendText($"Warning: invalid item format {item}\n");
                        }
                    }

                    foreach (var kvp in removalMap)
                    {
                        string projectFullPath = kvp.Key;
                        List<string> refs = kvp.Value;
                        
                        CliWrapper.RemoveReferences(projectFullPath, refs);
                        rtbLog.AppendText($"Removed {refs.Count} references from {Path.GetFileName(projectFullPath)}\n");
                    }

                    MessageBox.Show("Selected references have been removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Refresh GUI
                    for (int i = clbUnusedRefs.Items.Count - 1; i >= 0; i--)
                    {
                        if (clbUnusedRefs.GetItemChecked(i))
                        {
                            clbUnusedRefs.Items.RemoveAt(i);
                        }
                    }
                    lblStatus.Text = "Removal completed.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Removal failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
        }

        private void chkSelectAll_CheckedChanged_1(object sender, EventArgs e)
        {
            bool state = chkSelectAll.Checked;
            for (int i = 0; i < clbUnusedRefs.Items.Count; i++)
            {
                clbUnusedRefs.SetItemChecked(i, state);
            }
        }
    }
}
