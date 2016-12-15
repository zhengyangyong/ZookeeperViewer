using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZookeeperViewer.Component
{
    public class NotifyObject : INotifyPropertyChanged
    {
        public void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public void RaisePropertyChanged(IEnumerable<string> names)
        {
            if (PropertyChanged != null && names != null)
            {
                foreach (var name in names)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
