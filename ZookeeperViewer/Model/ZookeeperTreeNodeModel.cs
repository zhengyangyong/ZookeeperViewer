using Org.Apache.Zookeeper.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZookeeperViewer.Component;

namespace ZookeeperViewer.Model
{
    public class ZookeeperTreeNodeModel : NotifyObject
    {
        private string _displayName = null;
        public string DisplayName
        {
            get { return this._displayName; }
            set
            {
                this._displayName = value;
                this.RaisePropertyChanged("DisplayName");
            }
        }

        public string QueryPath { get; set; }
        public string JoinPath { get; set; }

        public ZookeeperTreeNodeModel Root { get; set; }

        public Stat Stat { get; set; }

        public ZookeeperTreeNodeModel(string displayName, string queryPath, ZookeeperTreeNodeModel root)
            : this(displayName, queryPath, root, null)
        {
        }

        public ZookeeperTreeNodeModel(string displayName, string queryPath, ZookeeperTreeNodeModel root, Stat stat)
            : this()
        {
            this.QueryPath = queryPath;
            this.JoinPath = queryPath == "/" ? string.Empty : queryPath;
            this.Root = root;
            this.Stat = stat;
            this.DisplayName = displayName;
        }

        public ZookeeperTreeNodeModel()
        {
            Childs = new ObservableCollection<ZookeeperTreeNodeModel>();
        }

        public ObservableCollection<ZookeeperTreeNodeModel> Childs { get; set; }
    }
}