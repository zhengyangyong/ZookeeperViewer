using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZookeeperViewer.Component;
using ZookeeperViewer.Model;

namespace ZookeeperViewer.ViewModel
{
    public class ConnectSettingWindowVM : NotifyObject
    {
        private string _connectionString = null;
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                _connectionString = value;
                OK.RaiseCanExecuteChanged();
                this.RaisePropertyChanged("ConnectionString");
            }
        }

        private string _timeout = "15000";
        public string Timeout
        {
            get { return _timeout; }
            set
            {
                _timeout = value;
                OK.RaiseCanExecuteChanged();
                this.RaisePropertyChanged("Timeout");
            }
        }

        public DelegateCommand OK { get; set; }

        public ConnectSettingWindowVM()
        {
            OK = new DelegateCommand(DoOK, DoCanOK);
        }

        private void DoOK() { }

        private bool DoCanOK()
        {
            return true;
        }
    }
}
