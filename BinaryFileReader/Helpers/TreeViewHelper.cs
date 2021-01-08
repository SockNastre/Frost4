using BinaryFileReader.Extensions;
using Frost4.Frostbite.Utilities.BinaryFile.Surface;
using System.Windows.Forms;

namespace BinaryFileReader.Helpers
{
    /// <summary>
    /// Provides helper methods for the <see cref="TreeView"/> class.
    /// </summary>
    public static class TreeViewHelper
    {
        /// <summary>
        /// Helper method for opening Frostbite binary file data inside of <see cref="TreeView"/>.
        /// </summary>
        /// <param name="treeView">The <see cref="TreeView"/> object to open data in.</param>
        /// <param name="path">The file to open for reading.</param>
        /// <param name="offset">Position to start reading in file.</param>
        public static void OpenFrostbiteBinaryFile(this TreeView treeView, string path, uint offset)
        {
            Entry mainEntry = SurfaceFile.CreateEntry(path, offset);
            TreeNode mainEntryNode = mainEntry.CreateTreeNode();

            // After adding mainEntryNode expands it to reveal data quicker
            treeView.Nodes.Add(mainEntryNode);
            treeView.Nodes[0].Expand();
        }
    }
}
