using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZookeeperViewer.ViewModel;

namespace ZookeeperViewer.View
{
    /// <summary>
    /// ConnectSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectSettingWindow : Window
    {
        public ConnectSettingWindow()
        {
            InitializeComponent();
            this.DataContext = new ConnectSettingWindowVM();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
