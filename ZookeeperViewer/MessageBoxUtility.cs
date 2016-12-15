using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZookeeperViewer
{
    public static class MessageBoxUtility
    {
        public static void ShowError(string text)
        {
            MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowQuery(string text)
        {
            return MessageBox.Show(text, "Query", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        public static void ShowInformation(string text)
        {
            MessageBox.Show(text, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
