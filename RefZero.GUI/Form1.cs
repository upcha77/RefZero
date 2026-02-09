using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using RefZero.Core;

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
                ofd.Filter = "Project Files (*.csproj)|*.csproj|All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtProjectPath.Text = ofd.FileName;
                    lblStatus.Text = $"Loaded: {Path.GetFileName(ofd.FileName)}";
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
                var analyzer = new DependencyAnalyzer();
                var projectInfo = analyzer.Analyze(projectPath);
                
                rtbLog.AppendText($"Found {projectInfo.References.Count()} references.\n");
                
                foreach(var refItem in projectInfo.References)
                {
                    rtbLog.AppendText($"[REF] {refItem.Name} ({refItem.Version}) -> {refItem.PhysicalPath}\n");
                }
                
                rtbLog.AppendText("Analysis completed.\n");
            }
            catch (Exception ex)
            {
                rtbLog.AppendText($"Error: {ex.Message}\n");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                var analyzer = new DependencyAnalyzer();
                var projectInfo = analyzer.Analyze(projectPath);
                
                rtbLog.AppendText($"Found {projectInfo.References.Count()} references.\n");

                var copier = new ReferenceCopier();
                // Redirect console output to RichTextBox (Simplified: ReferenceCopier logs to Console currently)
                // Ideally, we should pass a logger or event, but for now capturing Console is hard in WinForms in-proc.
                // We will just run it.
                
                copier.CopyReferences(projectInfo.References, outputPath);
                
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
                System.Diagnostics.Process.Start("explorer.exe", outputPath);
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

            clbUnusedRefs.Items.Clear();
            lblStatus.Text = "Analyzing unused references...";
            Application.DoEvents();

            try
            {
                var analyzer = new DependencyAnalyzer();
                var projectInfo = analyzer.Analyze(projectPath);
                var cleaner = new ReferenceCleaner();
                var unusedRefs = cleaner.AnalyzeUnusedReferences(projectPath, projectInfo.References);

                if (!unusedRefs.Any())
                {
                    lblStatus.Text = "No unused references found.";
                    MessageBox.Show("No unused references found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    foreach (var refName in unusedRefs)
                    {
                        clbUnusedRefs.Items.Add(refName, true); // Default checked
                    }
                    lblStatus.Text = $"Found {unusedRefs.Count()} unused references.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Analysis failed.";
                MessageBox.Show($"Analysis failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    var cleaner = new ReferenceCleaner();
                    cleaner.RemoveReferences(projectPath, refsToRemove);

                    MessageBox.Show("Selected references have been removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Refresh GUI
                    foreach (var item in refsToRemove)
                    {
                        clbUnusedRefs.Items.Remove(item);
                    }
                    lblStatus.Text = "Removal completed.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Removal failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
