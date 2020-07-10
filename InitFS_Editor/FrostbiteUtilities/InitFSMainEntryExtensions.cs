using InitFS_Editor.Classes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace InitFS_Editor.FrostbiteUtilities
{
    public static class InitFSMainEntryExtensions
    {
        public static List<InitFSFile> GetFilesInVirtualList(this Entry initFS)
        {
             var virtualList = new List<InitFSFile>();

            foreach (Entry entry in (List<Entry>)initFS.Data)
            {
                var flagList = (List<Flag>)entry.Data;
                var subFlagList = (List<Flag>)flagList[0].Data;

                virtualList.Add(new InitFSFile(subFlagList));
            }

            return virtualList;
        }

        public static void SetIntoListView(this Entry initFSMainEntry, ListView listView)
        {
            listView.BeginUpdate();

            int count = 0;
            foreach (Entry entry in (List<Entry>)initFSMainEntry.Data)
            {
                var flagList = (List<Flag>)entry.Data;
                var subFlagList = (List<Flag>)flagList[0].Data;

                var item = new ListViewItem
                {
                    ImageIndex = 0,
                    Tag = count,
                    Text = Path.GetFileName((string)subFlagList[subFlagList.Count == 2 ? 0 : 1].Data)
                };

                listView.Items.Add(item);
                count++;
            }

            listView.EndUpdate();
        }

        public static void SetIntoTreeView(this Entry initFSMainEntry, TreeView treeView)
        {
            treeView.BeginUpdate();

            int count = 0;
            foreach (Entry entry in (List<Entry>)initFSMainEntry.Data)
            {
                var flagList = (List<Flag>)entry.Data;
                var subFlagList = (List<Flag>)flagList[0].Data;
                var nameFlagData = (string)subFlagList[subFlagList.Count == 2 ? 0 : 1].Data;

                string[] dir = Path.GetDirectoryName(nameFlagData).Split('\\');
                string name = Path.GetFileName(nameFlagData);

                TreeNode folderNode = null; // Gets changed depending on what folder in loop below
                bool isRootRead = false;
                var fileNode = new TreeNode(name) { Tag = count };

                for (uint i = 0; i < dir.Count(); i++)
                {
                    string folder = dir[i];

                    // Fixes issue with blank folder names
                    if (!string.IsNullOrEmpty(folder))
                    {
                        if (isRootRead)
                        {
                            folderNode = folderNode.Nodes.ContainsKey(folder) ? folderNode.Nodes[folder] : folderNode.Nodes.Add(folder, folder, 1, 1);
                        }
                        else
                        {
                            if (!treeView.Nodes.ContainsKey(folder))
                            {
                                treeView.Nodes.Add(folder, folder, 1, 1);
                            }

                            folderNode = treeView.Nodes.Find(folder, false).First();
                            isRootRead = true;
                        }

                        if (i + 1 == dir.Count())
                        {
                            folderNode.Nodes.Add(fileNode);
                        }
                    }
                }

                if (string.IsNullOrEmpty(dir.Last()))
                    treeView.Nodes.Add(fileNode);

                count++;
            }

            treeView.TreeViewNodeSorter = new TreeNodeImageIndexSorter();
            treeView.EndUpdate();
        }
    }
}