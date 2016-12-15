using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZooKeeperNet;
using ZookeeperViewer.ViewModel;

namespace ZookeeperViewer.Component
{
    public class ZookeeperWatcher:IWatcher
    {
        private MainWindowVM _vm = null;

        public ZookeeperWatcher(MainWindowVM vm)
        {
            _vm = vm;
        }

        public void Process(WatchedEvent @event)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _vm.ProcessWatchedEvent(@event);
            });
        }
    }
}
