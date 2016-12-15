using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ZookeeperViewer.Extension
{
    public static class TreeViewExtension
    {
        public static void ExpandAll(this System.Windows.Controls.TreeView treeView)
        {
            ExpandAllItems(treeView);
        }

        private static void ExpandAllItems(ItemsControl control)
        {
            if (control == null)
            {
                return;
            }

            foreach (Object item in control.Items)
            {
                System.Windows.Controls.TreeViewItem treeItem = control.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;


                if (treeItem == null || !treeItem.HasItems)
                {
                    continue;
                }

                treeItem.IsExpanded = true;
                ExpandAllItems(treeItem as ItemsControl);
            }
        }
    }
}
