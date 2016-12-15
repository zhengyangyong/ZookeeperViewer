using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZookeeperViewer.Model
{
    public class ZookeeperStatModel
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public ZookeeperStatModel(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
