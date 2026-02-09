
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
            this.tabCleaner = new System.Windows.Forms.TabPage();
            
            // Aggregator Controls
            this.lblOutput = new System.Windows.Forms.Label();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.btnBrowseOutput = new System.Windows.Forms.Button();
            this.btnAnalyzeOnly = new System.Windows.Forms.Button();
            this.btnCollect = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.rtbLog = new System.Windows.Forms.RichTextBox();

            // Cleaner Controls
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.clbUnusedRefs = new System.Windows.Forms.CheckedListBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();

            this.tabControl1.SuspendLayout();
            this.tabAggregator.SuspendLayout();
            this.tabCleaner.SuspendLayout();
            this.SuspendLayout();

            // 
            // lblProject
            // 
            this.lblProject.AutoSize = true;
            this.lblProject.Location = new System.Drawing.Point(12, 15);
            this.lblProject.Name = "lblProject";
            this.lblProject.Size = new System.Drawing.Size(49, 15);
            this.lblProject.TabIndex = 0;
            this.lblProject.Text = "Project:";

            // 
            // txtProjectPath
            // 
            this.txtProjectPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProjectPath.Location = new System.Drawing.Point(70, 12);
            this.txtProjectPath.Name = "txtProjectPath";
            this.txtProjectPath.Size = new System.Drawing.Size(400, 23);
            this.txtProjectPath.TabIndex = 1;

            // 
            // btnBrowseProject
            // 
            this.btnBrowseProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseProject.Location = new System.Drawing.Point(480, 11);
            this.btnBrowseProject.Name = "btnBrowseProject";
            this.btnBrowseProject.Size = new System.Drawing.Size(75, 25);
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
            this.tabControl1.Location = new System.Drawing.Point(12, 50);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(560, 300);
            this.tabControl1.TabIndex = 3;

            // 
            // tabAggregator
            // 
            this.tabAggregator.Controls.Add(this.btnAnalyzeOnly);
            this.tabAggregator.Controls.Add(this.btnOpenFolder);
            this.tabAggregator.Controls.Add(this.rtbLog);
            this.tabAggregator.Controls.Add(this.btnCollect);
            this.tabAggregator.Controls.Add(this.btnBrowseOutput);
            this.tabAggregator.Controls.Add(this.txtOutputPath);
            this.tabAggregator.Controls.Add(this.lblOutput);
            this.tabAggregator.Location = new System.Drawing.Point(4, 24);
            this.tabAggregator.Name = "tabAggregator";
            this.tabAggregator.Padding = new System.Windows.Forms.Padding(3);
            this.tabAggregator.Size = new System.Drawing.Size(552, 272);
            this.tabAggregator.TabIndex = 0;
            this.tabAggregator.Text = "DLL Collector";
            this.tabAggregator.UseVisualStyleBackColor = true;

            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(15, 20);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(48, 15);
            this.lblOutput.TabIndex = 0;
            this.lblOutput.Text = "Output:";

            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputPath.Location = new System.Drawing.Point(70, 17);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(350, 23);
            this.txtOutputPath.TabIndex = 1;

            // 
            // btnBrowseOutput
            // 
            this.btnBrowseOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseOutput.Location = new System.Drawing.Point(430, 16);
            this.btnBrowseOutput.Name = "btnBrowseOutput";
            this.btnBrowseOutput.Size = new System.Drawing.Size(75, 25);
            this.btnBrowseOutput.TabIndex = 2;
            this.btnBrowseOutput.Text = "Browse...";
            this.btnBrowseOutput.UseVisualStyleBackColor = true;
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);

            // 
            // btnAnalyzeOnly
            // 
            this.btnAnalyzeOnly.Location = new System.Drawing.Point(18, 50);
            this.btnAnalyzeOnly.Name = "btnAnalyzeOnly";
            this.btnAnalyzeOnly.Size = new System.Drawing.Size(100, 30);
            this.btnAnalyzeOnly.TabIndex = 6;
            this.btnAnalyzeOnly.Text = "Analyze Only";
            this.btnAnalyzeOnly.UseVisualStyleBackColor = true;
            this.btnAnalyzeOnly.Click += new System.EventHandler(this.btnAnalyzeOnly_Click);

            // 
            // btnCollect
            // 
            this.btnCollect.Location = new System.Drawing.Point(130, 50);
            this.btnCollect.Name = "btnCollect";
            this.btnCollect.Size = new System.Drawing.Size(150, 30);
            this.btnCollect.TabIndex = 3;
            this.btnCollect.Text = "Collect DLLs";
            this.btnCollect.UseVisualStyleBackColor = true;
            this.btnCollect.Click += new System.EventHandler(this.btnCollect_Click);

            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Location = new System.Drawing.Point(290, 50);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(120, 30);
            this.btnOpenFolder.TabIndex = 5;
            this.btnOpenFolder.Text = "Open Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);

            // 
            // rtbLog
            // 
            this.rtbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLog.Location = new System.Drawing.Point(18, 90);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(515, 170);
            this.rtbLog.TabIndex = 4;
            this.rtbLog.Text = "";

            // 
            // tabCleaner
            // 
            this.tabCleaner.Controls.Add(this.lblStatus);
            this.tabCleaner.Controls.Add(this.btnRemove);
            this.tabCleaner.Controls.Add(this.clbUnusedRefs);
            this.tabCleaner.Controls.Add(this.btnAnalyze);
            this.tabCleaner.Location = new System.Drawing.Point(4, 24);
            this.tabCleaner.Name = "tabCleaner";
            this.tabCleaner.Padding = new System.Windows.Forms.Padding(3);
            this.tabCleaner.Size = new System.Drawing.Size(552, 272);
            this.tabCleaner.TabIndex = 1;
            this.tabCleaner.Text = "Cleaner (Remove Unused)";
            this.tabCleaner.UseVisualStyleBackColor = true;

            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Location = new System.Drawing.Point(15, 15);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(180, 30);
            this.btnAnalyze.TabIndex = 0;
            this.btnAnalyze.Text = "Analyze Unused References";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);

            // 
            // clbUnusedRefs
            // 
            this.clbUnusedRefs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbUnusedRefs.FormattingEnabled = true;
            this.clbUnusedRefs.Location = new System.Drawing.Point(15, 60);
            this.clbUnusedRefs.Name = "clbUnusedRefs";
            this.clbUnusedRefs.Size = new System.Drawing.Size(520, 166);
            this.clbUnusedRefs.TabIndex = 1;

            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(385, 235);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(150, 30);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove Selected";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);

            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(15, 243);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(42, 15);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Ready.";

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
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
    }
}
