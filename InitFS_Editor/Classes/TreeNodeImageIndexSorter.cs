using System.Collections;
using System.Windows.Forms;

namespace InitFS_Editor.Classes
{
    public class TreeNodeImageIndexSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            var nodeX = x as TreeNode;
            var nodeY = y as TreeNode;

            if (nodeY.ImageIndex != nodeX.ImageIndex)
            {
                return nodeY.ImageIndex - nodeX.ImageIndex;
            }

            return 1;
        }
    }
}