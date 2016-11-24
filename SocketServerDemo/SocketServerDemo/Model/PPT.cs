using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SocketServerDemo.Model
{
    class PPT
    {
 
        [DllImport("User32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);  //导入模拟键盘的方法

        static Dictionary<IntPtr, string> windows = null;
        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        //枚举桌面所有窗口
        [DllImport("user32.dll")]

        static extern int EnumWindows(EnumWindowsProc hWnd, IntPtr lParam);
        //得到所有窗口的名字

        [DllImport("user32.dll", CharSet = CharSet.Auto)]

        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpText, int nCount);

        //关闭句柄为hWnd的窗口
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        //关闭窗口命令
        const int WM_CLOSE = 0x0010;

        static bool PrintWindow(IntPtr hWnd, IntPtr lParam)

        {


            //分配空间

            var sb = new StringBuilder(50);

            GetWindowText(hWnd, sb, sb.Capacity);


            //注意某些窗口没有标题

            if (sb.ToString() != String.Empty)

            {
                if (!windows.Values.Contains(sb.ToString()))
                {
                    windows.Add(hWnd, sb.ToString());
                }
            }

            //回调函数有返回值

            return true;

        }
        public static void Open(string str)      //打开Office
        {
            System.Diagnostics.Process.Start(str);
        }
        public static void Down()//下一页
        {
            keybd_event(0x28, 0, 0, 0);
            keybd_event(0x28, 0, 2, 0);
        }
        public static void Up()//上一页
        {
            keybd_event(0x26, 0, 0, 0);
            keybd_event(0x26, 0, 2, 0);
        }
        public static void Last()//最后一页
        {
            keybd_event(0x23, 0, 0, 0);
            keybd_event(0x23, 0, 2, 0);
        }
        public static void First()//最后一页
        {

            keybd_event(0x24, 0, 0, 0);
            keybd_event(0x24, 0, 2, 0);
        }
        public static void Play()   //放映
        {
             //F5
             keybd_event(116, 0, 0, 0);
             keybd_event(116, 0, 0x2, 0);
            
        }
        public static void Close()  //关闭
        {
            windows = new Dictionary<IntPtr, string>();
            EnumWindows(PrintWindow, IntPtr.Zero);
            foreach (var window in windows)
            {
                
                if (window.Value.EndsWith("PowerPoint"))
                {
                    SendMessage(window.Key, WM_CLOSE, 0, 0);
                    break;
                }
            }
        }

    }
}
