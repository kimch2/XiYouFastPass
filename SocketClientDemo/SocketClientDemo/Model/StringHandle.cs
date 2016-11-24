using SocketClientDemo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace SocketClientDemo.Model
{
    class StringHandle
    {
       
        public static  HandleModel MessageHandle(HandleModel handle, string str)
        {

            if (App.WinCommand.ContainsKey(str))
            {
                handle.Msg = App.WinCommand[str];
                handle.Typ = "Cmd";
                return handle;
            }
            else if (App.Office.ContainsKey(str))
            {
                handle.Msg = App.Office[str];
                handle.Typ = "Office";
                return handle;
            }
            else if (App.Web.ContainsKey(str))
            {
                handle.Msg = App.Web[str];
                handle.Typ = "Web";
                return handle;
            }
            else if (App.Search.ContainsKey(str))
            {
                handle.Msg = App.Search[str];
                handle.Typ = "Search";
                return handle;
            }
            else
            {
                handle.Msg = "";
                handle.Typ = "";
                return handle;
            }
        }
    }
}
