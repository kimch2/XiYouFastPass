
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientDemo.Common
{
    public class Msg:INotifyPropertyChanged
    {
        private string rcvmsg;
        public string rcvMsg
        {
            get
            {
                return rcvmsg;
            }
            set
            {
                rcvmsg = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("rcvMsg"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
