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

        private void btnBrowseProject_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Project Files (*.csproj)|*.csproj";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtProjectPath.Text = ofd.FileName;
                }
            }
        }

        private void btnAnalyzeOnly_Click(object sender, EventArgs e)
        {
            string projectPath = txtProjectPath.Text;

            if (string.IsNullOrEmpty(projectPath) || !File.Exists(projectPath))
            {
                MessageBox.Show("Please select a valid project file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            rtbLog.AppendText($"Starting analysis for {Path.GetFileName(projectPath)}...\n");
            
            try
            {
                var references = CliWrapper.Analyze(projectPath);
                
                rtbLog.AppendText($"Found {references.Count} references.\n");
                
                foreach(var refItem in references)
                {
                    rtbLog.AppendText($"[REF] {refItem.Name} ({refItem.Version}) -> {refItem.PhysicalPath} [{refItem.SourceType}]\n");
                }
                
                rtbLog.AppendText("Analysis completed.\n");
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
            string projectPath = txtProjectPath.Text;
            string outputPath = txtOutputPath.Text;

            if (string.IsNullOrEmpty(projectPath) || !File.Exists(projectPath))
            {
                MessageBox.Show("Please select a valid project file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                MessageBox.Show("Please select an output directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            rtbLog.AppendText($"Starting collection for {projectPath}...\n");
            
            try
            {
                // CliWrapper.Collect executes the CLI 'collect' command
                CliWrapper.Collect(projectPath, outputPath);
                
                rtbLog.AppendText("Collection completed successfully.\n");
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
            string projectPath = txtProjectPath.Text;

            if (string.IsNullOrEmpty(projectPath) || !File.Exists(projectPath))
            {
                MessageBox.Show("Please select a valid project file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblStatus.Text = "Analyzing unused references...";
            clbUnusedRefs.Items.Clear();

            try
            {
                var unusedRefs = CliWrapper.RunCleanAnalysis(projectPath);

                if (unusedRefs.Count == 0)
                {
                    MessageBox.Show("No unused references found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblStatus.Text = "No unused references found.";
                }
                else
                {
                    foreach (var item in unusedRefs)
                    {
                        clbUnusedRefs.Items.Add(item, true); // Default checked
                    }
                    chkSelectAll.Checked = true;
                    lblStatus.Text = $"Found {unusedRefs.Count} unused references.";
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

            string projectPath = txtProjectPath.Text;
            if (string.IsNullOrEmpty(projectPath) || !File.Exists(projectPath))
            {
                MessageBox.Show("Please select a valid project file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to remove {clbUnusedRefs.CheckedItems.Count} references from the project file?",
                "Confirm Removal",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    var refsToRemove = clbUnusedRefs.CheckedItems.Cast<string>().ToList();
                    
                    CliWrapper.RemoveReferences(projectPath, refsToRemove);

                    MessageBox.Show("Selected references have been removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Refresh GUI
                    // We remove items from the list box that were checked and successfully removed
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
