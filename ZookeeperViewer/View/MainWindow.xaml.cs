using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZookeeperViewer.ViewModel;

namespace ZookeeperViewer.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowVM();
        }

        private void MainTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (this.DataContext as MainWindowVM).GetZookeeperNodeStatAndData();
        }

        private void MainTree_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            (this.DataContext as MainWindowVM).RaiseTreeViewContextMenuCanExecuteChanged();
        }
    }
}
