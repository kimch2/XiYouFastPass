using SocketClientDemo.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientDemo.Model
{
    public class DiskItemModel :DiskFolder,INotifyPropertyChanged
    {
        private string img;
        public string Img
        {
            get
            {
                return img;
            }
            set
            {
                img = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Img"));
                }
            }
        }
        private string downloadpic;
        public string DownLoadPic
        {
            get
            {
                return downloadpic;
            }
            set
            {
                downloadpic = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DownLoadPic"));
                }
            }
        }

          public event PropertyChangedEventHandler PropertyChanged;
    }
}
