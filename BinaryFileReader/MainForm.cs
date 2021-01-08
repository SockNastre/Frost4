using BinaryFileReader.Helpers;
using BinaryFileReader.Utilities.TreeNodeAdditions;
using Frost4.Frostbite.Utilities.BinaryFile.Surface;
using System;
using System.IO;
using System.Windows.Forms;

namespace BinaryFileReader
{
    /// <summary>
    /// Main form with controls for reading Frostbite binary file.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Text of the current <see cref="MainForm"/> form.
        /// </summary>
        private readonly string formText;

        /// <summary>
        /// The current <see cref="TreeNode.Tag"/> that is in focus.
        /// </summary>
        private EntryFlagDataTag selectedNodeTag;

        /// <summary>
        /// Initializes a new instance of <see cref="MainForm"/> with a null-accepting file auto-load path.
        /// </summary>
        /// <param name="filePath">File path to load in tool on initialization.</param>
        public MainForm(string filePath)
        {
            InitializeComponent();
            formText = this.Text;

            // Opens binary file from filePath if it's not set to null
            if (!string.IsNullOrEmpty(filePath))
            {
                this.OpenBinaryFile(filePath);
            }
        }

        /// <summary>
        /// Displays <see cref="openFileDialogBinaryFile"/> and opens binary file from that.
        /// </summary>
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialogBinaryFile.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialogBinaryFile.FileName;
                this.OpenBinaryFile(filePath);
            }
        }

        /// <summary>
        /// Changes <see cref="textBoxOffset"/> text depending on if this checkbox is checked or not.
        /// </summary>
        private void checkBoxHeader_CheckedChanged(object sender, EventArgs e)
        {
            textBoxOffset.Text = checkBoxHeader.Checked ? "22C" : "0";
        }

        /// <summary>
        /// Copies <see cref="selectedNodeTag"/> name property to clipboard.
        /// </summary>
        private void copyNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Trims off quotation marks from name string
            Clipboard.SetText(selectedNodeTag.Name.Trim('\"'));
        }

        /// <summary>
        /// Copies <see cref="selectedNodeTag"/> data property to clipboard.
        /// </summary>
        private void copyDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(selectedNodeTag.Data);
        }

        /// <summary>
        /// Shows <see cref="selectedNodeTag"/> metadata as formatted string in <see cref="MessageBox"/>.
        /// </summary>
        private void giveMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(selectedNodeTag.GiveMetadata);
        }

        /// <summary>
        /// Displays <see cref="contextMenuStripNodeOptions"/> when <see cref="TreeNode"/> from <see cref="treeViewBinaryFile"/> is right-clicked.
        /// </summary>
        private void treeViewBinaryFile_MouseUp(object sender, MouseEventArgs e)
        {
            // To proceed there must be a selected node
            if (treeViewBinaryFile.SelectedNode == null)
            {
                return;
            }

            if (e.Button == MouseButtons.Right && treeViewBinaryFile.SelectedNode.Bounds.Contains(e.Location))
            {
                // Gets right-clicked node, and data from tag is stored in field
                TreeNode selectedNode = treeViewBinaryFile.GetNodeAt(e.X, e.Y);
                selectedNodeTag = (EntryFlagDataTag)selectedNode.Tag;

                // Checks that data inside tag isn't null, null data cannot be copied
                copyDataToolStripMenuItem.Enabled = !string.IsNullOrEmpty(selectedNodeTag.Data);

                // Entry nodes don't have actual names, so they cannot be copied
                copyNameToolStripMenuItem.Enabled = selectedNodeTag.IsFlag;

                contextMenuStripNodeOptions.Show(treeViewBinaryFile, e.Location);
            }
        }

        /// <summary>
        /// Allows both left and right-clicking a node to select it. <see href="https://stackoverflow.com/a/4784415/10216412">SOURCE</see>
        /// </summary>
        private void treeViewBinaryFile_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeViewBinaryFile.SelectedNode = e.Node;
        }

        /// <summary>
        /// Opens Frostbite binary file in <see cref="treeViewBinaryFile"/> based on given file path and hexadecimal offset in <see cref="textBoxOffset"/>.
        /// </summary>
        /// <param name="filePath">File path to load in tool.</param>
        private void OpenBinaryFile(string filePath)
        {
            uint offset = Convert.ToUInt32(textBoxOffset.Text, 16);
            treeViewBinaryFile.Nodes.Clear(); // Clears any potentially previously-loaded nodes

            // Tool tries its best not to overwrite user's custom-inputted offset
            // So it checks that the offset is 0 or 556 (header size) before doing header checks and changing offset
            if (offset == 0 || offset == 556)
            {
                if (SurfaceFile.IsHeaderPresent(filePath))
                {
                    checkBoxHeader.Checked = true;
                    offset = 556;
                }
                else
                {
                    checkBoxHeader.Checked = false;
                    offset = 0;
                }
            }

            treeViewBinaryFile.OpenFrostbiteBinaryFile(filePath, offset);
            this.Text = $"{this.formText} - {Path.GetFileName(filePath)}";
        }
    }
}
