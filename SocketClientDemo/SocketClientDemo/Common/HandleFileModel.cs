using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SocketClientDemo.Common
{
    public class HandleFileModel  :HandleModel
    {

        public long  Size { get; set; }
        
        public string Name { get; set; }
      
    }
}
