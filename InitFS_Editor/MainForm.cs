using InitFS_Editor.Classes;
using InitFS_Editor.FrostbiteUtilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;

namespace InitFS_Editor
{
    public partial class MainForm : Form
    {
        private Ini SettingsIni;
        private readonly OpenFolderDialog OpenFolderDialog = new OpenFolderDialog();
        private List<InitFSFile> VirtualFileList = new List<InitFSFile>(); // Stores initfs files
        private int SelectedVirtualFileListIndex = -1;

        private string InitFSSavePath = string.Empty;
        private bool IsInitFSWin32 = false;
        private byte[] InitFSKeys = new byte[552];

        public MainForm()
        {
            InitializeComponent();

            string iniPath = AppDomain.CurrentDomain.BaseDirectory + "\\InitFS Editor.ini";
            Ini settingsIni;

            // All code following is for managing the ini configuration file used by the tool
            if (File.Exists(iniPath))
            {
                settingsIni = new Ini(iniPath);

                if (IsIniValid(settingsIni))
                {
                    SettingsIni = settingsIni;

                    if (bool.Parse(SettingsIni.Data["Tool"]["IsUsingListView"]) == false)
                    {
                        toolStripButtonTreeView.Checked = true;
                    }

                    string autoLoadPath = SettingsIni.Data["Tool"]["AutoLoadInitFSPath"];
                    if (bool.Parse(SettingsIni.Data["Tool"]["AutoLoadInitFS"]) == true && !string.IsNullOrEmpty(autoLoadPath))
                    {
                        this.OpenInitFS(autoLoadPath);
                    }

                    return;
                }
                else
                {
                    File.Delete(iniPath);
                }
            }

            // When all else fails, code comes here and just remakes the ini file
            settingsIni = new Ini();
            settingsIni.Data.Add("Rebuild Options", new Dictionary<string, string> { { "IsUsingOriginalDICEKeys", "False" } });
            settingsIni.Data.Add("Tool", new Dictionary<string, string> { { "IsUsingListView", "True" }, { "AutoLoadInitFS", "False" }, { "AutoLoadInitFSPath", string.Empty } });
            settingsIni.WriteToFile(iniPath);

            SettingsIni = settingsIni;
        }


        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            
            if (files.Length == 1)
            {
                newToolStripMenuItem.PerformClick();
                this.OpenInitFS(files[0]);
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Makes sure to show user that more than one file cannot be dropped
                e.Effect = files.Length == 1 ? DragDropEffects.Copy : DragDropEffects.None;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Saves ini file
            string iniPath = AppDomain.CurrentDomain.BaseDirectory + "\\InitFS Editor.ini";
            SettingsIni.Data["Tool"]["IsUsingListView"] = toolStripButtonListView.Checked.ToString();
            SettingsIni.WriteToFile(iniPath);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VirtualFileList.Clear();
            SelectedVirtualFileListIndex = -1;
            InitFSSavePath = string.Empty;
            IsInitFSWin32 = false;

            saveToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            exportAllToolStripMenuItem.Enabled = false;
            propertyGridFileFlags.Enabled = false;

            listViewVFS.Items.Clear();
            treeViewVFS.Nodes.Clear();
            propertyGridFileFlags.SelectedObject = null;

            // Does this to fix bug with sorting where sorting changes after opening another initfs
            treeViewVFS.TreeViewNodeSorter = null;
            treeViewVFS.Sort();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogInitFS.ShowDialog() == DialogResult.OK)
            {
                newToolStripMenuItem.PerformClick();
                this.OpenInitFS(openFileDialogInitFS.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var entryList = new List<Entry>();
            foreach (InitFSFile file in VirtualFileList)
            {
                var fileFlagList = new List<Flag>();

                // Checks if "fs" flag is used
                if (!string.IsNullOrEmpty(file.FileSystem))
                {
                    var fs = new Flag
                    {
                        Type = FlagType.Str,
                        Name = "fs",
                        Data = file.FileSystem
                    };

                    fileFlagList.Add(fs);
                }

                var name = new Flag
                {
                    Type = FlagType.Str,
                    Name = "name",
                    Data = file.Name
                };

                var payload = new Flag
                {
                    Type = FlagType.VariableBytes,
                    Name = "payload",
                    Data = file.IsPayloadModified ? file.ModifiedPayload : file.OrgPayload
                };

                fileFlagList.Add(name);
                fileFlagList.Add(payload);

                var fileFlag = new Flag
                {
                    Type = FlagType.FlagList,
                    Name = "$file",
                    Data = fileFlagList
                };

                var entry = new Entry
                {
                    Type = EntryType.FlagList,
                    Data = new List<Flag> { fileFlag }
                };

                entryList.Add(entry);
            }

            var mainEntry = new Entry
            {
                Type = EntryType.EntryList,
                Data = entryList
            };

            if (IsInitFSWin32)
            {
                // Relies on the setting here, which is a boolean for whether or not to use original keys
                bool useKeys = bool.Parse(SettingsIni.Data["Rebuild Options"]["IsUsingOriginalDICEKeys"]);

                mainEntry.WriteToFile(InitFSSavePath, 0, true, useKeys ? InitFSKeys : null);
            }
            else
            {
                mainEntry.WriteToFile(InitFSSavePath);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialogInitFS.FileName = Path.GetFileName(InitFSSavePath);
            if (saveFileDialogInitFS.ShowDialog() == DialogResult.OK)
            {
                InitFSSavePath = saveFileDialogInitFS.FileName;
                saveToolStripMenuItem.PerformClick();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenFolderDialog.ShowDialog(this) == DialogResult.OK)
            {
                foreach (InitFSFile file in VirtualFileList)
                {
                    string extractPath = OpenFolderDialog.Folder + '\\' + file.Name;

                    Directory.CreateDirectory(Path.GetDirectoryName(extractPath));
                    file.ExportPayload(extractPath);
                }
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!toolStripButtonListView.Checked)
                return;

            foreach (ListViewItem item in listViewVFS.Items)
            {
                item.Selected = true;
                propertyGridFileFlags.SelectedObject = null; // Does this to avoid last item showing it's properties here
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settingsForm = new Settings(SettingsIni);
            settingsForm.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Version: 1.0.0.0\nCopyright ©  2020 SockNastre", "About InitFS Editor", MessageBoxButtons.OK);
        }

        private void toolStripButtonListView_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButtonListView.Checked)
            {
                toolStripButtonTreeView.Checked = false;

                listViewVFS.Enabled = true;
                listViewVFS.Visible = true;
                treeViewVFS.Enabled = false;
                treeViewVFS.Visible = false;
            }
            else if (!toolStripButtonListView.Checked && !toolStripButtonTreeView.Checked)
            {
                // This makes it unable to have both buttons unchecked
                toolStripButtonListView.Checked = true;
            }
        }

        private void toolStripButtonTreeView_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButtonTreeView.Checked)
            {
                toolStripButtonListView.Checked = false;

                treeViewVFS.Enabled = true;
                treeViewVFS.Visible = true;
                listViewVFS.Enabled = false;
                listViewVFS.Visible = false;
            }
            else if (!toolStripButtonTreeView.Checked && !toolStripButtonListView.Checked)
            {
                // This makes it unable to have both buttons unchecked
                toolStripButtonTreeView.Checked = true;
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenFolderDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (toolStripButtonListView.Checked && listViewVFS.SelectedItems.Count > 1)
                {
                    foreach(ListViewItem item in listViewVFS.SelectedItems)
                    {
                        this.ExportFile(item, OpenFolderDialog.Folder + '\\' + item.Text);
                    }

                    return;
                }

                if (SelectedVirtualFileListIndex == -1)
                    return;

                // Activates for a single file export in ListView or TreeView
                InitFSFile file = VirtualFileList[SelectedVirtualFileListIndex];
                file.ExportPayload(OpenFolderDialog.Folder + '\\' + Path.GetFileName(file.Name));
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedVirtualFileListIndex == -1)
                return;

            if (openFileDialogFile.ShowDialog() == DialogResult.OK)
            {
                InitFSFile file = VirtualFileList[SelectedVirtualFileListIndex];
                file.IsPayloadModified = true;
                file.ModifiedPayload = File.ReadAllBytes(openFileDialogFile.FileName);

                propertyGridFileFlags.Refresh();
            }
        }


        private void revertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (toolStripButtonListView.Checked && listViewVFS.SelectedItems.Count > 1)
            {
                foreach (ListViewItem item in listViewVFS.SelectedItems)
                {
                    InitFSFile itemFile = VirtualFileList[(int)item.Tag];
                    itemFile.ResetPayload();
                }

                return;
            }

            if (SelectedVirtualFileListIndex == -1)
                return;

            // Activates for a single file export in ListView or TreeView
            InitFSFile file = VirtualFileList[SelectedVirtualFileListIndex];
            file.ResetPayload();

            propertyGridFileFlags.Refresh();
        }

        private void treeViewVFS_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = treeViewVFS.SelectedNode;

            if (node != null)
            {
                if (node.ImageIndex != 1)
                {
                    propertyGridFileFlags.SelectedObject = VirtualFileList[(int)node.Tag];
                    SelectedVirtualFileListIndex = (int)node.Tag;
                }
            }
        }

        private void treeViewVFS_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var node = treeViewVFS.SelectedNode;

            if (node != null)
            {
                if (node.ImageIndex != 1)
                {
                    var obj = new DataObject();
                    var sc = new StringCollection();

                    string tempDir = Path.GetTempPath() + InitFSSavePath.GetHashCode().ToString() + '\\';
                    Directory.CreateDirectory(tempDir);

                    string dest = tempDir + node.Text;
                    this.ExportFile(node, dest);
                    sc.Add(dest);

                    obj.SetFileDropList(sc);
                    listViewVFS.DoDragDrop(obj, DragDropEffects.Move);
                    Directory.Delete(tempDir, true);
                }
            }
        }

        private void treeViewVFS_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && treeViewVFS.SelectedNode.Bounds.Contains(e.Location))
            {
                if (treeViewVFS.SelectedNode.ImageIndex == 1)
                    return;

                SelectedVirtualFileListIndex = (int)treeViewVFS.SelectedNode.Tag;
                contextMenuStripFileOptions.Show(Cursor.Position);
            }
        }

        private void listViewVFS_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (listViewVFS.SelectedItems.Count < 1)
                return;

            var obj = new DataObject();
            var sc = new StringCollection();

            string tempDir = Path.GetTempPath() + InitFSSavePath.GetHashCode().ToString() + '\\';
            Directory.CreateDirectory(tempDir);

            foreach (ListViewItem item in listViewVFS.SelectedItems)
            {
                string dest = tempDir + item.Text;
                this.ExportFile(item, dest);

                sc.Add(dest);
            }

            obj.SetFileDropList(sc);
            listViewVFS.DoDragDrop(obj, DragDropEffects.Move);
            Directory.Delete(tempDir, true);
        }

        private void listViewVFS_MouseClick(object sender, MouseEventArgs e)
        {
            if (listViewVFS.FocusedItem == null)
                return;

            if (e.Button == MouseButtons.Right && listViewVFS.FocusedItem.Bounds.Contains(e.Location))
            {
                SelectedVirtualFileListIndex = (int)listViewVFS.FocusedItem.Tag;

                if (listViewVFS.SelectedItems.Count > 1)
                {
                    SelectedVirtualFileListIndex = -1;
                    importToolStripMenuItem.Enabled = false;
                }
                else
                {
                    importToolStripMenuItem.Enabled = true;
                }

                contextMenuStripFileOptions.Show(Cursor.Position);
            }
        }

        private void listViewVFS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewVFS.SelectedItems.Count == 1 && listViewVFS.FocusedItem != null)
            {
                int index = (int)listViewVFS.FocusedItem.Tag;

                propertyGridFileFlags.SelectedObject = VirtualFileList[index];
                SelectedVirtualFileListIndex = index;
            }
            else
            {
                propertyGridFileFlags.SelectedObject = null;
                SelectedVirtualFileListIndex = -1;
            }
        }

        private void ExportFile(ListViewItem item, string path)
        {
            VirtualFileList[(int)item.Tag].ExportPayload(path);
        }

        private void ExportFile(TreeNode node, string path)
        {
            VirtualFileList[(int)node.Tag].ExportPayload(path);
        }

        private bool IsIniValid(Ini ini)
        {
            try
            {
                string val = ini.Data["Rebuild Options"]["IsUsingOriginalDICEKeys"];
                bool.Parse(val);

                val = ini.Data["Tool"]["IsUsingListView"];
                bool.Parse(val);

                val = ini.Data["Tool"]["AutoLoadInitFS"];
                bool.Parse(val);

                val = ini.Data["Tool"]["AutoLoadInitFSPath"];

                // If the above code works without exceptions, the ini is valid for this tool
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void OpenInitFS(string savePath)
        {
            InitFSSavePath = savePath;

            using (var reader = new BinaryReader(File.Open(InitFSSavePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                if (reader.ReadInt32() == 0x01CED100) // PC, aka "Win32"
                {
                    IsInitFSWin32 = true;
                    InitFSKeys = reader.ReadBytes(552);
                }
                else // PS4, possibly another platform
                {
                    reader.BaseStream.Position -= 4;
                }

                var initFS = new Entry(reader);

                if (initFS.Type == EntryType.EntryList)
                {
                    VirtualFileList = initFS.GetFilesInVirtualList();
                    initFS.SetIntoListView(listViewVFS);
                    initFS.SetIntoTreeView(treeViewVFS);

                    saveToolStripMenuItem.Enabled = true;
                    saveAsToolStripMenuItem.Enabled = true;
                    exportAllToolStripMenuItem.Enabled = true;
                }
            }
        }
    }
}