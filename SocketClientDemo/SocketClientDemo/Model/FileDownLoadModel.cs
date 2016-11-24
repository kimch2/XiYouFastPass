using SocketClientDemo.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientDemo.Model
{
    public class FileDownLoadModel : DiskFolder, INotifyPropertyChanged
    {
        private int size;
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Size"));
                }
            }
        }
        private int count;
        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Count"));
                }
            }
        }
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
        private string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }
        public event PropertyChangedEventHandler  PropertyChanged;
    }
}
