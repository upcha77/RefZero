
namespace RefZero.GUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblProject = new System.Windows.Forms.Label();
            this.txtProjectPath = new System.Windows.Forms.TextBox();
            this.btnBrowseProject = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabAggregator = new System.Windows.Forms.TabPage();
            this.btnAnalyzeOnly = new System.Windows.Forms.Button();
            this.btnCollect = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.btnDiagnostics = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.btnBrowseOutput = new System.Windows.Forms.Button();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.lblOutput = new System.Windows.Forms.Label();
            this.tabCleaner = new System.Windows.Forms.TabPage();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.clbUnusedRefs = new System.Windows.Forms.CheckedListBox();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabAggregator.SuspendLayout();
            this.tabCleaner.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblProject
            // 
            this.lblProject.AutoSize = true;
            this.lblProject.Location = new System.Drawing.Point(14, 15);
            this.lblProject.Name = "lblProject";
            this.lblProject.Size = new System.Drawing.Size(58, 15);
            this.lblProject.TabIndex = 0;
            this.lblProject.Text = "Project:";
            // 
            // txtProjectPath
            // 
            this.txtProjectPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProjectPath.Location = new System.Drawing.Point(80, 12);
            this.txtProjectPath.Name = "txtProjectPath";
            this.txtProjectPath.Size = new System.Drawing.Size(687, 25);
            this.txtProjectPath.TabIndex = 1;
            // 
            // btnBrowseProject
            // 
            this.btnBrowseProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseProject.Location = new System.Drawing.Point(779, 11);
            this.btnBrowseProject.Name = "btnBrowseProject";
            this.btnBrowseProject.Size = new System.Drawing.Size(86, 25);
            this.btnBrowseProject.TabIndex = 2;
            this.btnBrowseProject.Text = "Browse...";
            this.btnBrowseProject.UseVisualStyleBackColor = true;
            this.btnBrowseProject.Click += new System.EventHandler(this.btnBrowseProject_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabAggregator);
            this.tabControl1.Controls.Add(this.tabCleaner);
            this.tabControl1.Location = new System.Drawing.Point(14, 50);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(870, 302);
            this.tabControl1.TabIndex = 3;
            // 
            // tabAggregator
            // 
            this.tabAggregator.Controls.Add(this.btnAnalyzeOnly);
            this.tabAggregator.Controls.Add(this.btnCollect);
            this.tabAggregator.Controls.Add(this.btnOpenFolder);
            this.tabAggregator.Controls.Add(this.btnDiagnostics);
            this.tabAggregator.Controls.Add(this.btnClearLog);
            this.tabAggregator.Controls.Add(this.rtbLog);
            this.tabAggregator.Controls.Add(this.btnBrowseOutput);
            this.tabAggregator.Controls.Add(this.txtOutputPath);
            this.tabAggregator.Controls.Add(this.lblOutput);
            this.tabAggregator.Location = new System.Drawing.Point(4, 25);
            this.tabAggregator.Name = "tabAggregator";
            this.tabAggregator.Padding = new System.Windows.Forms.Padding(3);
            this.tabAggregator.Size = new System.Drawing.Size(862, 273);
            this.tabAggregator.TabIndex = 0;
            this.tabAggregator.Text = "DLL Collector";
            this.tabAggregator.UseVisualStyleBackColor = true;
            // 
            // btnAnalyzeOnly
            // 
            this.btnAnalyzeOnly.Location = new System.Drawing.Point(21, 48);
            this.btnAnalyzeOnly.Name = "btnAnalyzeOnly";
            this.btnAnalyzeOnly.Size = new System.Drawing.Size(114, 30);
            this.btnAnalyzeOnly.TabIndex = 6;
            this.btnAnalyzeOnly.Text = "Analyze Only";
            this.btnAnalyzeOnly.UseVisualStyleBackColor = true;
            this.btnAnalyzeOnly.Click += new System.EventHandler(this.btnAnalyzeOnly_Click);
            // 
            // btnCollect
            // 
            this.btnCollect.Location = new System.Drawing.Point(281, 48);
            this.btnCollect.Name = "btnCollect";
            this.btnCollect.Size = new System.Drawing.Size(133, 30);
            this.btnCollect.TabIndex = 3;
            this.btnCollect.Text = "Collect DLLs";
            this.btnCollect.UseVisualStyleBackColor = true;
            this.btnCollect.Click += new System.EventHandler(this.btnCollect_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Location = new System.Drawing.Point(420, 48);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(137, 30);
            this.btnOpenFolder.TabIndex = 5;
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // btnDiagnostics
            // 
            this.btnDiagnostics.Location = new System.Drawing.Point(141, 48);
            this.btnDiagnostics.Name = "btnDiagnostics";
            this.btnDiagnostics.Size = new System.Drawing.Size(134, 30);
            this.btnDiagnostics.TabIndex = 7;
            this.btnDiagnostics.Text = "Run Diagnostics";
            this.btnDiagnostics.UseVisualStyleBackColor = true;
            this.btnDiagnostics.Click += new System.EventHandler(this.btnDiagnostics_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(563, 48);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(108, 30);
            this.btnClearLog.TabIndex = 8;
            this.btnClearLog.Text = "Clear";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // rtbLog
            // 
            this.rtbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLog.Location = new System.Drawing.Point(21, 90);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(818, 172);
            this.rtbLog.TabIndex = 4;
            this.rtbLog.Text = "";
            // 
            // btnBrowseOutput
            // 
            this.btnBrowseOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseOutput.Location = new System.Drawing.Point(721, 16);
            this.btnBrowseOutput.Name = "btnBrowseOutput";
            this.btnBrowseOutput.Size = new System.Drawing.Size(86, 25);
            this.btnBrowseOutput.TabIndex = 2;
            this.btnBrowseOutput.Text = "Browse...";
            this.btnBrowseOutput.UseVisualStyleBackColor = true;
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputPath.Location = new System.Drawing.Point(80, 17);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(629, 25);
            this.txtOutputPath.TabIndex = 1;
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(17, 20);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(56, 15);
            this.lblOutput.TabIndex = 0;
            this.lblOutput.Text = "Output:";
            // 
            // tabCleaner
            // 
            this.tabCleaner.Controls.Add(this.lblStatus);
            this.tabCleaner.Controls.Add(this.btnRemove);
            this.tabCleaner.Controls.Add(this.clbUnusedRefs);
            this.tabCleaner.Controls.Add(this.btnAnalyze);
            this.tabCleaner.Controls.Add(this.chkSelectAll);
            this.tabCleaner.Location = new System.Drawing.Point(4, 25);
            this.tabCleaner.Name = "tabCleaner";
            this.tabCleaner.Padding = new System.Windows.Forms.Padding(3);
            this.tabCleaner.Size = new System.Drawing.Size(862, 273);
            this.tabCleaner.TabIndex = 1;
            this.tabCleaner.Text = "Cleaner (Remove Unused)";
            this.tabCleaner.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(17, 243);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(54, 15);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Ready.";
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(440, 235);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(171, 30);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove Selected";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // clbUnusedRefs
            // 
            this.clbUnusedRefs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbUnusedRefs.FormattingEnabled = true;
            this.clbUnusedRefs.Location = new System.Drawing.Point(17, 60);
            this.clbUnusedRefs.Name = "clbUnusedRefs";
            this.clbUnusedRefs.Size = new System.Drawing.Size(594, 164);
            this.clbUnusedRefs.TabIndex = 1;
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Location = new System.Drawing.Point(17, 15);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(206, 30);
            this.btnAnalyze.TabIndex = 0;
            this.btnAnalyze.Text = "Analyze Unused References";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.Location = new System.Drawing.Point(538, 6);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(73, 24);
            this.chkSelectAll.TabIndex = 4;
            this.chkSelectAll.Text = "ALL";
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 363);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnBrowseProject);
            this.Controls.Add(this.txtProjectPath);
            this.Controls.Add(this.lblProject);
            this.Name = "Form1";
            this.Text = "Ref-Zero GUI";
            this.tabControl1.ResumeLayout(false);
            this.tabAggregator.ResumeLayout(false);
            this.tabAggregator.PerformLayout();
            this.tabCleaner.ResumeLayout(false);
            this.tabCleaner.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProject;
        private System.Windows.Forms.TextBox txtProjectPath;
        private System.Windows.Forms.Button btnBrowseProject;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabAggregator;
        private System.Windows.Forms.TabPage tabCleaner;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Button btnBrowseOutput;
        private System.Windows.Forms.Button btnCollect;
        private System.Windows.Forms.Button btnAnalyzeOnly;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.CheckedListBox clbUnusedRefs;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnDiagnostics;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.CheckBox chkSelectAll;
    }
}
