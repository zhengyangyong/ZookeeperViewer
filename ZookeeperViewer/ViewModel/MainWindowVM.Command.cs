using log4net;
using Org.Apache.Zookeeper.Data;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;
using ZookeeperViewer.Component;
using ZookeeperViewer.Enum;
using ZookeeperViewer.Model;
using ZookeeperViewer.View;

namespace ZookeeperViewer.ViewModel
{
    public partial class MainWindowVM
    {
        private static ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DelegateCommand Connect { get; set; }
        public DelegateCommand Disconnect { get; set; }
        public DelegateCommand Refresh { get; set; }
        public DelegateCommand Clear { get; set; }
        public DelegateCommand SaveModify { get; set; }

        public DelegateCommand AddNode { get; set; }
        public DelegateCommand DeleteNode { get; set; }


        private void DoConnect()
        {
            ConnectSettingWindow win = new ConnectSettingWindow();
            bool? result = win.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var vm = win.DataContext as ConnectSettingWindowVM;
                try
                {
                    this.Watcher = new ZookeeperWatcher(this);
                    _zk = new ZooKeeper(vm.ConnectionString, TimeSpan.FromMilliseconds(double.Parse(vm.Timeout)), this.Watcher);
                    this.GetZookeeperNodes();
                }
                catch (Exception ex)
                {
                    _log.Fatal(ex.ToString());
                    this.DoDisconnect();
                    MessageBoxUtility.ShowError("Connect Failed : " + ex.Message);
                }
                finally
                {
                    this.RaiseToolBarCanExecuteChanged();
                }
            }
        }

        private bool DoCanConnect()
        {
            return _zk == null;
        }

        private void DoDisconnect()
        {
            try
            {
                //if (_zk != null) _zk.Dispose();
                _zk = null;
                ListViewDataContext.Clear();
                TreeViewDataContext.Clear();
                Data = null;
            }
            catch (Exception ex)
            {
                this.AddLog(LogType.Fatal, ex.Message);
            }
            finally
            {
                this.RaiseToolBarCanExecuteChanged();
            }
        }

        private bool DoCanDisconnect()
        {
            return _zk != null;
        }

        private void DoRefresh()
        {
            try
            {
                this.GetZookeeperNodes();
            }
            catch (Exception ex)
            {
                this.AddLog(LogType.Fatal, ex.Message);
            }
            finally
            {
                this.RaiseToolBarCanExecuteChanged();
            }
        }

        private bool DoCanRefresh()
        {
            return _zk != null;
        }

        private void DoClear()
        {
            this.Logs.Clear();
        }

        private bool DoCanClear()
        {
            return true;
        }

        private void DoSaveModify()
        {
            byte[] data = null;
            if (_selectedEncoding == "HEX")
            {
                MemoryStream ms = new MemoryStream();
                foreach (string hex in DataText.Split('-'))
                {
                    byte b = 0;
                    if (byte.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out b))
                    {
                        ms.WriteByte(b);
                    }
                    else
                    {
                        this.AddLog(LogType.Error, "hex data format error");
                        return;
                    }
                }
                data = ms.ToArray();
            }
            else
            {
                data = ConvertEncoding(_selectedEncoding).GetBytes(DataText);
            }

            Stat stat = this.SetZookeeperNodeData(SelectedZookeeperTreeNodeModel.QueryPath, data, SelectedZookeeperTreeNodeModel.Stat.Cversion);
            if (stat != null)
            {
                this.SelectedZookeeperTreeNodeModel.Stat = stat;
                this.ChangeListView(stat);
            }
            else
            {
                this.SyncDataText();
                this.AddLog(LogType.Error, "version unmatch,set data failed");
            }
        }

        private bool DoCanSaveModify()
        {
            return SelectedZookeeperTreeNodeModel != null && !string.IsNullOrWhiteSpace(this.DataText);
        }

        private void DoAddNode()
        {
            AddNodeWindow win = new AddNodeWindow(this.SelectedZookeeperTreeNodeModel.QueryPath);
            bool? result = win.ShowDialog();
            if (result.HasValue && result.Value)
            {
                AddNodeWindowVM vm = win.DataContext as AddNodeWindowVM;
                if (this.CreateZookeeperNode(string.Concat(this.SelectedZookeeperTreeNodeModel.JoinPath, "/", vm.NodeName), vm.SelectedACLMode, vm.SelectedCreateMode))
                {
                    this.SelectedZookeeperTreeNodeModel.Childs.Add(new ZookeeperTreeNodeModel(vm.NodeName, string.Concat(this.SelectedZookeeperTreeNodeModel.JoinPath, "/", vm.NodeName), this.SelectedZookeeperTreeNodeModel));
                }
                else
                {
                    this.AddLog(LogType.Error, "add node failed");
                }
            }
        }

        private bool DoCanAddNode()
        {
            return this.SelectedZookeeperTreeNodeModel != null;
        }

        private void DoDeleteNode()
        {
            try
            {
                this.DeleteZookeeperNode(this.SelectedZookeeperTreeNodeModel.QueryPath, this.SelectedZookeeperTreeNodeModel.Stat.Version);
            }
            catch
            {
                this.AddLog(LogType.Error, "delete node failed");
            }
        }

        private bool DoCanDeleteNode()
        {
            return SelectedZookeeperTreeNodeModel != null && SelectedZookeeperTreeNodeModel.QueryPath != "/";
        }
    }
}