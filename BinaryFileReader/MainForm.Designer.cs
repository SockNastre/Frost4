
namespace BinaryFileReader
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitterOptions = new System.Windows.Forms.Splitter();
            this.labelOffset = new System.Windows.Forms.Label();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.treeViewBinaryFile = new System.Windows.Forms.TreeView();
            this.openFileDialogBinaryFile = new System.Windows.Forms.OpenFileDialog();
            this.checkBoxHeader = new System.Windows.Forms.CheckBox();
            this.contextMenuStripNodeOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorNodeOptions1 = new System.Windows.Forms.ToolStripSeparator();
            this.giveMetadataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxOffset = new System.Windows.Forms.TextBox();
            this.contextMenuStripNodeOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitterOptions
            // 
            this.splitterOptions.Cursor = System.Windows.Forms.Cursors.Default;
            this.splitterOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitterOptions.Location = new System.Drawing.Point(0, 0);
            this.splitterOptions.Name = "splitterOptions";
            this.splitterOptions.Size = new System.Drawing.Size(734, 45);
            this.splitterOptions.TabIndex = 4;
            this.splitterOptions.TabStop = false;
            // 
            // labelOffset
            // 
            this.labelOffset.AutoSize = true;
            this.labelOffset.Location = new System.Drawing.Point(103, 17);
            this.labelOffset.Name = "labelOffset";
            this.labelOffset.Size = new System.Drawing.Size(52, 13);
            this.labelOffset.TabIndex = 7;
            this.labelOffset.Text = "Offset: 0x";
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(12, 12);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(75, 23);
            this.buttonOpen.TabIndex = 5;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // treeViewBinaryFile
            // 
            this.treeViewBinaryFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewBinaryFile.Location = new System.Drawing.Point(0, 45);
            this.treeViewBinaryFile.Name = "treeViewBinaryFile";
            this.treeViewBinaryFile.Size = new System.Drawing.Size(734, 376);
            this.treeViewBinaryFile.TabIndex = 9;
            this.treeViewBinaryFile.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewBinaryFile_NodeMouseClick);
            this.treeViewBinaryFile.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeViewBinaryFile_MouseUp);
            // 
            // openFileDialogBinaryFile
            // 
            this.openFileDialogBinaryFile.Title = "Select Frostbite Binary File";
            // 
            // checkBoxHeader
            // 
            this.checkBoxHeader.AutoSize = true;
            this.checkBoxHeader.Location = new System.Drawing.Point(261, 16);
            this.checkBoxHeader.Name = "checkBoxHeader";
            this.checkBoxHeader.Size = new System.Drawing.Size(61, 17);
            this.checkBoxHeader.TabIndex = 10;
            this.checkBoxHeader.Text = "Header";
            this.checkBoxHeader.UseVisualStyleBackColor = true;
            this.checkBoxHeader.CheckedChanged += new System.EventHandler(this.checkBoxHeader_CheckedChanged);
            // 
            // contextMenuStripNodeOptions
            // 
            this.contextMenuStripNodeOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyNameToolStripMenuItem,
            this.copyDataToolStripMenuItem,
            this.toolStripSeparatorNodeOptions1,
            this.giveMetadataToolStripMenuItem});
            this.contextMenuStripNodeOptions.Name = "contextMenuStripNodeOptions";
            this.contextMenuStripNodeOptions.Size = new System.Drawing.Size(151, 76);
            // 
            // copyNameToolStripMenuItem
            // 
            this.copyNameToolStripMenuItem.Name = "copyNameToolStripMenuItem";
            this.copyNameToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.copyNameToolStripMenuItem.Text = "Copy Name";
            this.copyNameToolStripMenuItem.Click += new System.EventHandler(this.copyNameToolStripMenuItem_Click);
            // 
            // copyDataToolStripMenuItem
            // 
            this.copyDataToolStripMenuItem.Name = "copyDataToolStripMenuItem";
            this.copyDataToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.copyDataToolStripMenuItem.Text = "Copy Data";
            this.copyDataToolStripMenuItem.Click += new System.EventHandler(this.copyDataToolStripMenuItem_Click);
            // 
            // toolStripSeparatorNodeOptions1
            // 
            this.toolStripSeparatorNodeOptions1.Name = "toolStripSeparatorNodeOptions1";
            this.toolStripSeparatorNodeOptions1.Size = new System.Drawing.Size(147, 6);
            // 
            // giveMetadataToolStripMenuItem
            // 
            this.giveMetadataToolStripMenuItem.Name = "giveMetadataToolStripMenuItem";
            this.giveMetadataToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.giveMetadataToolStripMenuItem.Text = "Give Metadata";
            this.giveMetadataToolStripMenuItem.Click += new System.EventHandler(this.giveMetadataToolStripMenuItem_Click);
            // 
            // textBoxOffset
            // 
            this.textBoxOffset.Location = new System.Drawing.Point(155, 14);
            this.textBoxOffset.Name = "textBoxOffset";
            this.textBoxOffset.Size = new System.Drawing.Size(100, 20);
            this.textBoxOffset.TabIndex = 8;
            this.textBoxOffset.Text = "0";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 421);
            this.Controls.Add(this.checkBoxHeader);
            this.Controls.Add(this.treeViewBinaryFile);
            this.Controls.Add(this.textBoxOffset);
            this.Controls.Add(this.labelOffset);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.splitterOptions);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(498, 225);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Frost4 | Binary File Reader";
            this.contextMenuStripNodeOptions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Splitter splitterOptions;
        private System.Windows.Forms.Label labelOffset;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.TreeView treeViewBinaryFile;
        private System.Windows.Forms.OpenFileDialog openFileDialogBinaryFile;
        private System.Windows.Forms.CheckBox checkBoxHeader;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNodeOptions;
        private System.Windows.Forms.ToolStripMenuItem copyDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorNodeOptions1;
        private System.Windows.Forms.ToolStripMenuItem giveMetadataToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxOffset;
    }
}

