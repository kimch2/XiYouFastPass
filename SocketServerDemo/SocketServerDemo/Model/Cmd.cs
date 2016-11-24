using SocketServerDemo.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

using SocketServerDemo.Model;
using System.Windows.Threading;
using System.Windows;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Media;
using System.Drawing.Imaging;

namespace SocketServerDemo.Model
{
    public class Cmd
    {
        public static Stream stream;
        
        [DllImport("User32.dll")]
        //键盘事件
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);  //导入模拟键盘的方法
          
        static Dictionary<IntPtr, string> windows = null;
        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
      
        [DllImport("user32.dll")]
        //枚举桌面所有窗口

        static extern int EnumWindows(EnumWindowsProc hWnd, IntPtr lParam);
      

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        //得到所有窗口的名字

        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpText, int nCount);

        
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        //关闭句柄为hWnd的窗口
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
 
        //关闭窗口命令
        const int WM_CLOSE = 0x0010;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]

        private static extern System.IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);

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
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        public static void  SetVolUp()
        {
            p = Process.GetCurrentProcess();
            for (int i = 0; i < 5; i++)
            {
                SendMessageW(p.Handle, WM_APPCOMMAND, p.Handle, (IntPtr)APPCOMMAND_VOLUME_UP);
            }
        }
        public static void SetVolDown()
        {
            p = Process.GetCurrentProcess();
            for (int i = 0; i < 2; i++)
            {
                SendMessageW(p.Handle, WM_APPCOMMAND, p.Handle, (IntPtr)APPCOMMAND_VOLUME_DOWN);
            }
        }
        private static Process p;
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0x0a0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x090000;
        private const int WM_APPCOMMAND = 0x319;
        public static void SetVolMute()
        {
            p = Process.GetCurrentProcess();

            SendMessageW(p.Handle, WM_APPCOMMAND, p.Handle, (IntPtr)APPCOMMAND_VOLUME_MUTE);

        }

      

        public static void Command(HandleModel handle)
        {
           
            if (handle.Typ == "Cmd")
            {
                //dicCmd命令
                foreach (KeyValuePair<string, string> s in App.dicCmd)
                {
                    if (s.Key.Equals(handle.Msg))
                    {
                        if (handle.Msg.Equals("DisConnect"))
                        {
                            InfoHandle.DisConnectClient();

                            OnStartUp();
                        }
                        else if (handle.Msg.Equals("Win_Desktop"))
                        {
                            string t = "C:\\Users\\" + Environment.UserName + "\\Desktop";
                            Process.Start(@t);
                        }
                        else if (handle.Msg.Equals("Win_VolumeUp"))
                        {
                            SetVolUp();
                        }
                        else if (handle.Msg.Equals("Win_VolumeDown"))
                        {
                            SetVolDown();

                        }
                        else if (handle.Msg.Equals("Win_VolumeMute"))
                        {
                            SetVolMute();
                        }

                        //请求截屏
                        else if (handle.Msg.Equals("Win_Screenshot"))
                        {

                            Screen scr = Screen.PrimaryScreen;
                            int iWidth = scr.Bounds.Width;
                            int iHeight = scr.Bounds.Height;
                            System.Drawing.Image myImage = new Bitmap(iWidth, iHeight);
                            Graphics g = Graphics.FromImage(myImage);
                            g.CopyFromScreen(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Size(iWidth, iHeight));

                            //保存文件流到stream
                            stream = new MemoryStream();
                            myImage.Save(stream, ImageFormat.Png);

                            byte[] filebuffer = new byte[stream.Length];
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.Read(filebuffer, 0, filebuffer.Length);
                            // 设置当前流的位置为流的开始


                            //ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                            //imgscreen.Source = (ImageSource)imageSourceConverter.ConvertFrom(stream);

                            //向客户端发送截图的文件
                            App.SendScreen(filebuffer, App.serverSocketCmd);
                            //系统截屏
                            /*
                            keybd_event(0x2C, 0, 0, 0);
                            keybd_event(0x2C, 0, 0x2, 0);
                            System.Diagnostics.Process.Start(@"C:\\WINDOWS\\system32\\mspaint.exe");//打开文件和文件夹
                            Thread.Sleep(1000);
                            keybd_event(17, 0, 0, 0);
                            keybd_event(86, 0, 0, 0);
                            keybd_event(86, 0, 0x2, 0);
                            keybd_event(17, 0, 0x2, 0);*/

                        }

                        else if (handle.Msg.Equals("Win_Logoff"))
                        {
                            Process.Start("shutdown.exe", s.Value);
                        }
                        else if (handle.Msg.Equals("Win_Sleep"))
                        {
                            Process.Start("shutdown.exe", s.Value);
                        }
                        else if (handle.Msg.Equals("Win_Reset"))
                        {
                            Process.Start("shutdown.exe", s.Value);
                        }
                        else if (handle.Msg.Equals("Win_Shutdown_Immediately"))
                        {
                            Process.Start("shutdown.exe", s.Value);
                        }
                        else if (handle.Msg.Equals("Win_Shutdown_HalfAfterHour"))
                        {
                            Process.Start("shutdown.exe", s.Value);
                        }
                        else if (handle.Msg.Equals("Win_Shutdown_AfterOneHour"))
                        {
                            Process.Start("shutdown.exe", s.Value);
                        }
                        else if (handle.Msg.Equals("Win_Shutdown_Cancel"))
                        {
                            Process.Start("shutdown.exe", s.Value);
                        }
                        else if (handle.Msg.Contains("Close"))
                        {
                            string suffix = "";
                            int flag = 0;
                            windows = new Dictionary<IntPtr, string>();
                            EnumWindows(PrintWindow, IntPtr.Zero);
                            if (handle.Msg.Equals("Close_mspaint"))
                            {
                                suffix = "画图";
                            }
                            else if (handle.Msg.Equals("Close_calc"))
                            {
                                suffix = "计算器";
                                flag = 1;
                            }
                            else if (handle.Msg.Equals("Close_notepad"))
                            {
                                suffix = "记事本";
                            }
                            else if (handle.Msg.Equals("Close_cmd"))
                            {
                                suffix = @"\system32\cmd.exe";
                            }
                            else if (handle.Msg.Equals("Close_write"))
                            {
                                suffix = "写字板";
                            }
                            else if (handle.Msg.Equals("Close_mstsc"))
                            {
                                suffix = "远程桌面连接";
                                flag = 1;
                            }
                            else if (handle.Msg.Equals("Close_taskmgr"))
                            {
                                suffix = "任务管理器";
                                flag = 1;
                            }
                            else if (handle.Msg.Equals("Close_explorer"))
                            {
                                suffix = "文件资源管理器";
                                flag = 1;
                            }
                            else if (handle.Msg.Equals("Close_devmgmt.msc"))
                            {
                                suffix = "设备管理器";
                                flag = 1;
                            }
                            else if (handle.Msg.Equals("Close_iexplore.exe"))
                            {
                                suffix = "Internet Explorer";
                            }
                            else if (handle.Msg.Equals("Close_control"))
                            {
                                suffix = "控制面板";
                                flag = 1;
                            }
                            else if (handle.Msg.Equals("Close_narrator"))
                            {
                                suffix = "\"讲述人\"设置";
                                flag = 1;
                            }
                            else if (handle.Msg.Equals("Close_winword"))
                            {
                                suffix = "Word";
                            }
                            else if (handle.Msg.Equals("Close_powerpnt"))
                            {
                                suffix = "PowerPoint";
                            }
                            else if (handle.Msg.Equals("Close_exce"))
                            {
                                suffix = "Excel";
                            }
                            if (flag == 0)
                            {
                                CloseWindow(suffix, suffix.Length);
                            }
                            else if (flag == 1)
                            {
                                CloseWindow(suffix);
                            }
                        }
                        else if (handle.Msg.Contains("Video"))
                        {
                            if (handle.Msg.Equals("Video_Stop"))
                            {
                                keybd_event(0x11, 0, 0, 0);
                                keybd_event(83, 0, 0, 0);
                                keybd_event(83, 0, 0x2, 0);
                                keybd_event(0x11, 0, 0x2, 0);
                            }
                            else if (handle.Msg.Equals("Video_Pause"))
                            {
                                keybd_event(32, 0, 0, 0);
                                keybd_event(32, 0, 0x2, 0);
                            }
                            else if (handle.Msg.Equals("Video_Play"))
                            {
                                keybd_event(32, 0, 0, 0);
                                keybd_event(32, 0, 0x2, 0);
                            }

                            else if (handle.Msg.Equals("Video_Forward"))
                            {

                                keybd_event(39, 0, 0, 0);
                                keybd_event(39, 0, 0x2, 0);

                            }
                            else if (handle.Msg.Equals("Video_Backward"))
                            {
                                keybd_event(37, 0, 0, 0);
                                keybd_event(37, 0, 0x2, 0);
                            }
                            else if (handle.Msg.Equals("Video_Brightincrease"))
                            {
                                keybd_event(107, 0, 0, 0);
                                keybd_event(107, 0, 0x2, 0);
                                keybd_event(107, 0, 0, 0);
                                keybd_event(107, 0, 0x2, 0);
                                keybd_event(107, 0, 0, 0);
                                keybd_event(107, 0, 0x2, 0);
                                keybd_event(107, 0, 0, 0);
                                keybd_event(107, 0, 0x2, 0);
                                keybd_event(107, 0, 0, 0);
                                keybd_event(107, 0, 0x2, 0);
                            }
                            else if (handle.Msg.Equals("Video_Brightreduce"))
                            {
                                keybd_event(109, 0, 0, 0);
                                keybd_event(109, 0, 0x2, 0);
                                keybd_event(109, 0, 0, 0);
                                keybd_event(109, 0, 0x2, 0);
                                keybd_event(109, 0, 0, 0);
                                keybd_event(109, 0, 0x2, 0);
                            }
                            else if (handle.Msg.Equals("Video_PageDown"))
                            {
                                keybd_event(34, 0, 0, 0);
                                keybd_event(34, 0, 0x2, 0);
                            }
                            else if (handle.Msg.Equals("Video_PageUp"))
                            {
                                keybd_event(33, 0, 0, 0);
                                keybd_event(33, 0, 0x2, 0);
                            }
                            else if (handle.Msg.Equals("Video_AllScreen"))
                            {
                                keybd_event(111, 0, 0, 0);
                                keybd_event(111, 0, 0x2, 0);
                            }
                            else if (handle.Msg.Equals("Video_Volumnreduce"))
                            {
                                keybd_event(40, 0, 0, 0);
                                keybd_event(40, 0, 0x2, 0);
                                keybd_event(40, 0, 0, 0);
                                keybd_event(40, 0, 0x2, 0);
                                keybd_event(40, 0, 0, 0);
                                keybd_event(40, 0, 0x2, 0);
                            }
                            else if (handle.Msg.Equals("Video_Volumnincrease"))
                            {
                                keybd_event(38, 0, 0, 0);
                                keybd_event(38, 0, 0x2, 0);
                                keybd_event(38, 0, 0, 0);
                                keybd_event(38, 0, 0x2, 0);
                                keybd_event(38, 0, 0, 0);
                                keybd_event(38, 0, 0x2, 0);
                            }
                        }
                        else
                        {
                            string[] com = s.Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            if (com.Length == 1)
                            {
                                Process.Start(@s.Value);
                            }
                        }
                    }
                }
            }
            else if (handle.Typ == "Office")
            {
                //dicOffice命令
                foreach (KeyValuePair<string, string> s in App.dicOffice)
                {
                    if (s.Key.Equals(handle.Msg))
                    {
                        if (s.Value == "DownPPt")
                        {
                            PPT.Down();
                        }
                        else if (s.Value == "UpPPt")
                        {
                            PPT.Up();
                        }
                        else if (s.Value == "FirstPPt")
                        {
                            PPT.First();
                        }
                        else if (s.Value == "LastPPt")
                        {
                            PPT.Last();
                        }
                        else if (s.Value == "ClosePPt")
                        {
                            PPT.Close();
                        }
                        else if (s.Value == "Play_powerpnt")
                        {
                            PPT.Play();
                        }
                        else
                        {
                            PPT.Open(s.Value);
                        }
                    }
                }
            }
            else if (handle.Typ == "Web")
            {
                //dicWeb命令
                foreach (KeyValuePair<string, string> s in App.dicWeb)
                {
                    if (s.Key.Equals(handle.Msg))
                    {
                        System.Diagnostics.Process.Start(s.Value);
                    }
                }
            }
            else if (handle.Typ == "Search")
            {
                System.Diagnostics.Process.Start(handle.Msg);
            }
                      
        }

        
        private void SendMail(string    name ,string msg)
        {
            MailMessage mailmsg = new MailMessage();
            mailmsg.To.Add("wangyongzhi@xiyoumobile.com");
            mailmsg.CC.Add("tangnian@xiyoumobile.com");
            mailmsg.CC.Add("jiayudong@xiyoumobile.com");

            mailmsg.From = new MailAddress("angel", name, Encoding.UTF8);
            mailmsg.Subject =name;
            mailmsg.SubjectEncoding = Encoding.UTF8;
            mailmsg.Body = msg;
            mailmsg.BodyEncoding = Encoding.UTF8;
            mailmsg.IsBodyHtml = false;
            mailmsg.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential("wangyongzhi@xiyoumobile.com", "Wangyongzhi61011");
            client.Port = 465;
            client.Host = "smtp.exmail.qq.com";
            client.EnableSsl = true;
            object userState = mailmsg;
            try
            {
                client.SendAsync(mailmsg, userState);
                System.Windows.MessageBox.Show("发送成功");
            }
            catch (SmtpException )
            {
                System.Windows.MessageBox.Show("发送邮件出错");
            }


        }

        public   static  void OnStartUp()
        {       
            
            string path = Environment.CurrentDirectory + "\\SocketServerDemo.exe";
            /*path=AppDomain.CurrentDomain.BaseDirectory;
           path=Directory.GetCurrentDirectory();
           path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;*/


            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                //MainWindow mainwindow = new MainWindow();
                //mainwindow.Show();
                System.Diagnostics.Process.Start(path);
                System.Windows.Application.Current.Shutdown();

                System.Reflection.Assembly.GetEntryAssembly();
                string startpath = System.IO.Directory.GetCurrentDirectory();
             

            }));

        }

        private static void CloseWindow(string suffix,int length)
        {
            foreach (var window in windows)
            {
                if (window.Value.EndsWith(suffix)) 
                {
                    SendMessage(window.Key, WM_CLOSE, 0, 0);
                    break;
                }
            }
        }
        private static void CloseWindow(string name)
        {
            foreach (var window in windows)
            {          
                if (window.Value== name)
                {
                    SendMessage(window.Key, WM_CLOSE, 0, 0);
                    break;
                }
            }
        }
    }
}
