using BinaryFileReader.Helpers;
using BinaryFileReader.Utilities.TreeNodeAdditions;
using Frost4.Frostbite.Utilities.BinaryFile.Surface;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BinaryFileReader.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Flag"/> class.
    /// </summary>
    public static class FlagExtensions
    {
        /// <summary>
        /// Generates <see cref="TreeNode"/> object from <see cref="Flag"/> data.
        /// </summary>
        /// <param name="flag">The <see cref="Flag"/> object to get data from.</param>
        /// <returns>A generated <see cref="TreeNode"/> object with properties from <see cref="Flag"/>.</returns>
        public static TreeNode CreateTreeNode(this Flag flag)
        {
            var flagNode = new TreeNode()
            {
                Tag = new EntryFlagDataTag(flag),
                Text = $"{flag.Name} ",
            };

            // Calculates max length that the flag's data can be for the flagNode text
            // 259 is the max length a TreeNode's text can be
            int maxParsedDataLength = 259 - flagNode.Text.Length;

            switch (flag.Format)
            {
                case FlagFormat.EntryArray:
                    {
                        var data = (List<Entry>)flag.Data;

                        uint entryCount = 0;
                        foreach (Entry entry in data)
                        {
                            TreeNode entryNode = entry.CreateTreeNode();

                            // If subEntry is an EntryList or FlagList then the node's text is set to it's index
                            if (entry.Format == EntryFormat.EntryArray || entry.Format == EntryFormat.FlagList)
                            {
                                entryNode.Text = $"[{entryCount}]";
                            }

                            flagNode.Nodes.Add(entryNode);
                            entryCount++;
                        }

                        // Removes space from end before adding array count, sets that as new flagNode text
                        flagNode.Text = flagNode.Text.Trim() + $"[{data.Count}]";
                        break;
                    }

                case FlagFormat.FlagList:
                    {
                        var data = (List<Flag>)flag.Data;

                        foreach (Flag subFlag in data)
                        {
                            TreeNode subFlagNode = subFlag.CreateTreeNode();
                            flagNode.Nodes.Add(subFlagNode);
                        }

                        // Removes space from end before adding array count, sets that as new flagNode text
                        flagNode.Text = flagNode.Text.Trim() + $"[{data.Count}]";
                        break;
                    }

                case FlagFormat.String:
                    {
                        var data = (string)flag.Data;
                        string parsedData = string.IsNullOrEmpty(data) ? string.Empty : $"\"{data}\"";
                        parsedData = parsedData.FormatForLength(maxParsedDataLength, '\"');

                        flagNode.Text += parsedData;
                        break;
                    }

                case FlagFormat.Id:
                case FlagFormat.Sha1:
                case FlagFormat.ByteArray:
                    {
                        var data = (byte[])flag.Data;
                        string parsedData = BitConverter.ToString(data).Replace("-", "");

                        if (flag.Format != FlagFormat.ByteArray)
                        {
                            // Formats other than ByteArray are better represented all lowercase for the bytes as a string
                            parsedData = parsedData.ToLowerInvariant();
                        }
                        else
                        {
                            // The ByteArray format needs to be optimized, since it could easily exceed 259 (max TreeNode text length)
                            parsedData = parsedData.FormatForLength(maxParsedDataLength);
                        }

                        flagNode.Text += parsedData;
                        break;
                    }

                default:
                    {
                        string data = flag.Data.ToString();
                        flagNode.Text += data;
                        break;
                    }
            }

            return flagNode;
        }
    }
}
