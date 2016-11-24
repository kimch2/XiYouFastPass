using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SocketClientDemo.Model
{
    public class FileModel
    {
        public FileModel(StorageFile file, string fileName)
        {
            this.file = file;
            this.fileName = fileName;
        }
        public StorageFile file { get; set; }

        public string fileName { get; set; }
    }
}
