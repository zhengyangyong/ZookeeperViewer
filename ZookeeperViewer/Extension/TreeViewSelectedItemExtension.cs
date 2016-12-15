using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ZookeeperViewer.Extension
{
    public class TreeViewSelectedItemExtension
    {
        public static object GetTreeViewSelectedItem(DependencyObject obj)
        {
            return (object)obj.GetValue(TreeViewSelectedItemProperty);
        }

        public static void SetTreeViewSelectedItem(DependencyObject obj, object value)
        {
            obj.SetValue(TreeViewSelectedItemProperty, value);
        }

        public static readonly DependencyProperty TreeViewSelectedItemProperty =
            DependencyProperty.RegisterAttached("TreeViewSelectedItem", typeof(object), typeof(TreeViewSelectedItemExtension), new PropertyMetadata(new object(), TreeViewSelectedItemChanged));

        static void TreeViewSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TreeView treeView = sender as TreeView;
            if (treeView == null)
            {
                return;
            }

            treeView.SelectedItemChanged -= new RoutedPropertyChangedEventHandler<object>(treeView_SelectedItemChanged);
            treeView.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(treeView_SelectedItemChanged);

            TreeViewItem thisItem = treeView.ItemContainerGenerator.ContainerFromItem(e.NewValue) as TreeViewItem;
            if (thisItem != null)
            {
                thisItem.IsSelected = true;
                return;
            }

            for (int i = 0; i < treeView.Items.Count; i++)
                SelectItem(e.NewValue, treeView.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem);

        }

        static void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView treeView = sender as TreeView;
            SetTreeViewSelectedItem(treeView, e.NewValue);
        }

        private static bool SelectItem(object o, TreeViewItem parentItem)
        {
            if (parentItem == null)
                return false;

            bool isExpanded = parentItem.IsExpanded;
            if (!isExpanded)
            {
                parentItem.IsExpanded = true;
                parentItem.UpdateLayout();
            }

            TreeViewItem item = parentItem.ItemContainerGenerator.ContainerFromItem(o) as TreeViewItem;
            if (item != null)
            {
                item.IsSelected = true;
                return true;
            }

            bool wasFound = false;
            for (int i = 0; i < parentItem.Items.Count; i++)
            {
                TreeViewItem itm = parentItem.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
                var found = SelectItem(o, itm);
                if (!found)
                    itm.IsExpanded = false;
                else
                    wasFound = true;
            }

            return wasFound;
        }
    } 
}
