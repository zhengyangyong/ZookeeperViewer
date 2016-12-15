using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ZookeeperViewer.Component;
using ZookeeperViewer.Model;
using ZookeeperViewer.View;
using ZooKeeperNet;
using Org.Apache.Zookeeper.Data;
using ZookeeperViewer.Enum;
using System.IO;
using System.Windows.Data;
using Prism.Commands;

namespace ZookeeperViewer.ViewModel
{
    public partial class MainWindowVM:NotifyObject
    {
        public ObservableCollection<ZookeeperTreeNodeModel> TreeViewDataContext { get; set; }
        private object _treeViewDataContextLock = new object();
        public ObservableCollection<ZookeeperStatModel> ListViewDataContext { get; set; }
        private object _listViewDataContextLock = new object();
        public ObservableCollection<string> Logs { get; set; }
        private object _logsLock = new object();

        private ZookeeperTreeNodeModel _selectedZookeeperTreeNodeModel = null;
        public ZookeeperTreeNodeModel SelectedZookeeperTreeNodeModel
        {
            get { return _selectedZookeeperTreeNodeModel; }
            set
            {
                _selectedZookeeperTreeNodeModel = value;
                this.RaisePropertyChanged("SelectedZookeeperTreeNodeModel");
                this.GetZookeeperNodeStatAndData();
            }
        }
        public ZookeeperStatModel SelectedZookeeperStatModel { get; set; }

        private string _selectedEncoding = "UTF8";
        public string SelectedEncoding
        {
            get
            {
                return _selectedEncoding;
            }
            set
            {
                _selectedEncoding = value;
                this.SyncDataText();
            }
        }

        private byte[] _data = null;
        public byte[] Data
        {
            get { return _data; }
            set
            {
                _data = value;
                this.SyncDataText();
            }
        }

        private string _dataText = null;
        public string DataText
        {
            get { return _dataText; }
            set
            {
                _dataText = value;
                SaveModify.RaiseCanExecuteChanged();
                this.RaisePropertyChanged("DataText");
            }
        }

        public IWatcher Watcher { get; private set; }

        public MainWindowVM()
        {
            TreeViewDataContext = new ObservableCollection<ZookeeperTreeNodeModel>();
            ListViewDataContext = new ObservableCollection<ZookeeperStatModel>();
            Logs = new ObservableCollection<string>();

            BindingOperations.EnableCollectionSynchronization(TreeViewDataContext, _treeViewDataContextLock);
            BindingOperations.EnableCollectionSynchronization(ListViewDataContext, _listViewDataContextLock);
            BindingOperations.EnableCollectionSynchronization(Logs, _logsLock);

            Connect = new DelegateCommand(DoConnect, DoCanConnect);
            Disconnect = new DelegateCommand(DoDisconnect, DoCanDisconnect);
            Refresh = new DelegateCommand(DoRefresh, DoCanRefresh);
            Clear = new DelegateCommand(DoClear, DoCanClear);

            SaveModify = new DelegateCommand(DoSaveModify, DoCanSaveModify);

            AddNode = new DelegateCommand(DoAddNode, DoCanAddNode);
            DeleteNode = new DelegateCommand(DoDeleteNode, DoCanDeleteNode);
        }

        public void RaiseToolBarCanExecuteChanged()
        {
            this.Connect.RaiseCanExecuteChanged();
            this.Disconnect.RaiseCanExecuteChanged();
            this.Refresh.RaiseCanExecuteChanged();
        }

        public void RaiseTreeViewContextMenuCanExecuteChanged()
        {
            this.AddNode.RaiseCanExecuteChanged();
            this.DeleteNode.RaiseCanExecuteChanged();
        }

        private ZookeeperTreeNodeModel SearchNodeViaPath(ZookeeperTreeNodeModel root,string queryPath)
        {
            if (root.QueryPath == queryPath) return root;
            foreach (var node in root.Childs)
            {
                if (node.QueryPath == queryPath) return node;
                else if (queryPath.Contains(node.QueryPath))
                {
                    return SearchNodeViaPath(node, queryPath);
                }
            }
            return null;
        }

        private void SyncDataText()
        {
            if (_data != null && !string.IsNullOrWhiteSpace(_selectedEncoding))
            {
                if (_selectedEncoding == "HEX")
                {
                    DataText = BitConverter.ToString(_data).ToUpper();
                }
                else
                {
                    DataText = ConvertEncoding(_selectedEncoding).GetString(_data);
                }
            }
            else DataText = null;
        }

        public void AddLog(LogType type, string log)
        {
            this.Logs.Add(string.Format("[{0}]{1}:{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), type.ToString(), log));
        }

        public void ChangeListView(Stat stat)
        {
            ListViewDataContext.Clear();
            ListViewDataContext.Add(new ZookeeperStatModel("Aversion", stat.Aversion.ToString()));
            ListViewDataContext.Add(new ZookeeperStatModel("Ctime", stat.Ctime == 0 ? null : ConvertTimeTickToLocalDateTime(stat.Ctime).ToString("yyyy-MM-dd HH:mm:ss")));
            ListViewDataContext.Add(new ZookeeperStatModel("Cversion", stat.Cversion.ToString()));
            ListViewDataContext.Add(new ZookeeperStatModel("Czxid", stat.Czxid.ToString()));
            ListViewDataContext.Add(new ZookeeperStatModel("DataLength", stat.DataLength.ToString()));
            ListViewDataContext.Add(new ZookeeperStatModel("EphemeralOwner", stat.EphemeralOwner.ToString()));
            ListViewDataContext.Add(new ZookeeperStatModel("Mtime", stat.Mtime == 0 ? null : ConvertTimeTickToLocalDateTime(stat.Mtime).ToString("yyyy-MM-dd HH:mm:ss")));
            ListViewDataContext.Add(new ZookeeperStatModel("Mzxid", stat.Mzxid.ToString()));
            ListViewDataContext.Add(new ZookeeperStatModel("NumChildren", stat.NumChildren.ToString()));
            ListViewDataContext.Add(new ZookeeperStatModel("Pzxid", stat.Pzxid.ToString()));
            ListViewDataContext.Add(new ZookeeperStatModel("Version", stat.Version.ToString()));
        }

        private DateTime ConvertTimeTickToLocalDateTime(long tick)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1).AddMilliseconds(tick));
        }

        private Encoding ConvertEncoding(string name)
        {
            switch (name)
            {
                case "ASCII":
                    return Encoding.ASCII;
                case "UTF8":
                    return Encoding.UTF8;
                case "Unicode":
                    return Encoding.Unicode;
                case "GB2312":
                    return Encoding.GetEncoding("gb2312");
                case "GBK":
                    return Encoding.GetEncoding("gbk");
                case "BigEndianUnicode":
                    return Encoding.BigEndianUnicode;
                case "UTF32":
                    return Encoding.UTF32;
                case "UTF7":
                    return Encoding.UTF7;
            }
            return Encoding.Default;
        }
    }
}
