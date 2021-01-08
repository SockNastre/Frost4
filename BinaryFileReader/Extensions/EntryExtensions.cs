using BinaryFileReader.Helpers;
using BinaryFileReader.Utilities.TreeNodeAdditions;
using Frost4.Frostbite.Utilities.BinaryFile.Surface;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BinaryFileReader.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Entry"/> class.
    /// </summary>
    public static class EntryExtensions
    {
        /// <summary>
        /// Generates <see cref="TreeNode"/> object from <see cref="Entry"/> data.
        /// </summary>
        /// <param name="entry">The <see cref="Entry"/> object to get data from.</param>
        /// <returns>A generated <see cref="TreeNode"/> object with properties from <see cref="Entry"/>.</returns>
        public static TreeNode CreateTreeNode(this Entry entry)
        {
            var entryNode = new TreeNode()
            {
                Tag = new EntryFlagDataTag(entry),
            };

            switch (entry.Format)
            {
                case EntryFormat.EntryArray:
                    {
                        var data = (List<Entry>)entry.Data;

                        uint subEntryCount = 0;
                        foreach (Entry subEntry in data)
                        {
                            TreeNode subEntryNode = subEntry.CreateTreeNode();

                            // If subEntry is an EntryList or FlagList then the node's text is set to it's index
                            if (subEntry.Format == EntryFormat.EntryArray || subEntry.Format == EntryFormat.FlagList)
                            {
                                subEntryNode.Text = $"[{subEntryCount}]";
                            }

                            entryNode.Nodes.Add(subEntryNode);
                            subEntryCount++;
                        }

                        entryNode.Text = $"[{data.Count}]";
                        break;
                    }

                case EntryFormat.FlagList:
                    {
                        var data = (List<Flag>)entry.Data;

                        foreach (Flag flag in data)
                        {
                            TreeNode flagNode = flag.CreateTreeNode();
                            entryNode.Nodes.Add(flagNode);
                        }

                        entryNode.Text = $"[{data.Count}]";
                        break;
                    }

                case EntryFormat.String:
                    {
                        var data = (string)entry.Data;
                        string parsedData = string.IsNullOrEmpty(data) ? "NULL" : $"\"{data}\"";
                        parsedData = parsedData.FormatForLength(259, '\"'); // 259 is max TreeNode text length

                        entryNode.Text = parsedData;
                        break;
                    }

                case EntryFormat.ChunkId:
                    {
                        var data = (byte[])entry.Data;
                        string parsedData = BitConverter.ToString(data)
                            .Replace("-", "")
                            .ToLowerInvariant();
                        entryNode.Text = parsedData;

                        break;
                    }
            }

            return entryNode;
        }
    }
}
