namespace InitFS_Editor
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxRebuildOptions = new System.Windows.Forms.GroupBox();
            this.checkBoxUseOriginalDiceKeys = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBoxToolOptions = new System.Windows.Forms.GroupBox();
            this.checkBoxAutoOpenInitFS = new System.Windows.Forms.CheckBox();
            this.textBoxAutoLoadInitFSPath = new System.Windows.Forms.TextBox();
            this.buttonAutoOpenInitFSBrowse = new System.Windows.Forms.Button();
            this.openFileDialogInitFS = new System.Windows.Forms.OpenFileDialog();
            this.groupBoxRebuildOptions.SuspendLayout();
            this.groupBoxToolOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxRebuildOptions
            // 
            this.groupBoxRebuildOptions.Controls.Add(this.checkBoxUseOriginalDiceKeys);
            this.groupBoxRebuildOptions.Location = new System.Drawing.Point(12, 12);
            this.groupBoxRebuildOptions.Name = "groupBoxRebuildOptions";
            this.groupBoxRebuildOptions.Size = new System.Drawing.Size(378, 46);
            this.groupBoxRebuildOptions.TabIndex = 0;
            this.groupBoxRebuildOptions.TabStop = false;
            this.groupBoxRebuildOptions.Text = "Rebuild Options";
            // 
            // checkBoxUseOriginalDiceKeys
            // 
            this.checkBoxUseOriginalDiceKeys.AutoSize = true;
            this.checkBoxUseOriginalDiceKeys.Location = new System.Drawing.Point(10, 19);
            this.checkBoxUseOriginalDiceKeys.Name = "checkBoxUseOriginalDiceKeys";
            this.checkBoxUseOriginalDiceKeys.Size = new System.Drawing.Size(134, 17);
            this.checkBoxUseOriginalDiceKeys.TabIndex = 1;
            this.checkBoxUseOriginalDiceKeys.Text = "Use original DICE keys";
            this.checkBoxUseOriginalDiceKeys.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(317, 156);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // groupBoxToolOptions
            // 
            this.groupBoxToolOptions.Controls.Add(this.buttonAutoOpenInitFSBrowse);
            this.groupBoxToolOptions.Controls.Add(this.textBoxAutoLoadInitFSPath);
            this.groupBoxToolOptions.Controls.Add(this.checkBoxAutoOpenInitFS);
            this.groupBoxToolOptions.Location = new System.Drawing.Point(12, 64);
            this.groupBoxToolOptions.Name = "groupBoxToolOptions";
            this.groupBoxToolOptions.Size = new System.Drawing.Size(378, 80);
            this.groupBoxToolOptions.TabIndex = 2;
            this.groupBoxToolOptions.TabStop = false;
            this.groupBoxToolOptions.Text = "Tool Options";
            // 
            // checkBoxAutoOpenInitFS
            // 
            this.checkBoxAutoOpenInitFS.AutoSize = true;
            this.checkBoxAutoOpenInitFS.Location = new System.Drawing.Point(10, 19);
            this.checkBoxAutoOpenInitFS.Name = "checkBoxAutoOpenInitFS";
            this.checkBoxAutoOpenInitFS.Size = new System.Drawing.Size(171, 17);
            this.checkBoxAutoOpenInitFS.TabIndex = 0;
            this.checkBoxAutoOpenInitFS.Text = "Auto-open InitFS file on startup";
            this.checkBoxAutoOpenInitFS.UseVisualStyleBackColor = true;
            // 
            // textBoxAutoLoadInitFSPath
            // 
            this.textBoxAutoLoadInitFSPath.Location = new System.Drawing.Point(10, 42);
            this.textBoxAutoLoadInitFSPath.Name = "textBoxAutoLoadInitFSPath";
            this.textBoxAutoLoadInitFSPath.ReadOnly = true;
            this.textBoxAutoLoadInitFSPath.Size = new System.Drawing.Size(261, 20);
            this.textBoxAutoLoadInitFSPath.TabIndex = 1;
            // 
            // buttonAutoOpenInitFSBrowse
            // 
            this.buttonAutoOpenInitFSBrowse.Location = new System.Drawing.Point(277, 40);
            this.buttonAutoOpenInitFSBrowse.Name = "buttonAutoOpenInitFSBrowse";
            this.buttonAutoOpenInitFSBrowse.Size = new System.Drawing.Size(27, 23);
            this.buttonAutoOpenInitFSBrowse.TabIndex = 2;
            this.buttonAutoOpenInitFSBrowse.Text = "...";
            this.buttonAutoOpenInitFSBrowse.UseVisualStyleBackColor = true;
            this.buttonAutoOpenInitFSBrowse.Click += new System.EventHandler(this.buttonAutoOpenInitFSBrowse_Click);
            // 
            // openFileDialogInitFS
            // 
            this.openFileDialogInitFS.Filter = "Initialize FileSystem Files|initfs_*|All Files (*.*)|*.*";
            this.openFileDialogInitFS.Title = "Select initfs File to Auto-Open";
            // 
            // Settings
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 191);
            this.Controls.Add(this.groupBoxToolOptions);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBoxRebuildOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.Settings_HelpButtonClicked);
            this.groupBoxRebuildOptions.ResumeLayout(false);
            this.groupBoxRebuildOptions.PerformLayout();
            this.groupBoxToolOptions.ResumeLayout(false);
            this.groupBoxToolOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxRebuildOptions;
        private System.Windows.Forms.CheckBox checkBoxUseOriginalDiceKeys;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.GroupBox groupBoxToolOptions;
        private System.Windows.Forms.CheckBox checkBoxAutoOpenInitFS;
        private System.Windows.Forms.Button buttonAutoOpenInitFSBrowse;
        private System.Windows.Forms.TextBox textBoxAutoLoadInitFSPath;
        private System.Windows.Forms.OpenFileDialog openFileDialogInitFS;
    }
}