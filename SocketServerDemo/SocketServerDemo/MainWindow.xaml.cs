using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace SocketServerDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Socket serverSocketCmd = null;    
        public static Socket serverSocketFile = null;  
        private string TB_IP;
        private string TB_Count;
        public static Thread th = null;

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string strHostName = Dns.GetHostName(); //得到本机的主机名
            //IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
            IPAddress[] ipEntry = Dns.GetHostAddresses(strHostName);
            string strAddr = null;
            foreach (var ip in ipEntry)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    strAddr = ip.ToString() ;
                    break;
                }
            }
            App.IsConnect = false;
            //serverSocketCmd = null;
            //serverSocketFile = null;
            App.LoadDict();
            TB_IP = strAddr;
            TB_Count= "10240";
            ip.Text = strAddr;
            img.Source = CreateQRCode(TB_IP, 200, 200);
            Listen();
        }
        private void Listen()
        {
            IPAddress ip = IPAddress.Any;
            IPEndPoint ipep = new IPEndPoint(ip, Convert.ToInt32(TB_Count));       
            IPEndPoint ipepfile = new IPEndPoint(ip, Convert.ToInt32(TB_Count) + 1);
            if (serverSocketCmd == null)
            {
                serverSocketCmd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocketCmd.Bind(ipep);
                serverSocketCmd.Listen(2);
                serverSocketCmd.BeginAccept(Callback, serverSocketCmd);
            }         
            if (serverSocketFile == null)
            {
                serverSocketFile = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocketFile.Bind(ipepfile);
                serverSocketFile.Listen(2);
                serverSocketFile.BeginAccept(CallbackFile, serverSocketFile);
            }
          
        }
      
        private void CallbackFile(IAsyncResult ar)
        {
            Socket s = ar.AsyncState as Socket;
            App.serverSocketFile= s.EndAccept(ar);
            Dispatcher.Invoke((Action)(() => {
                InfoHandle info = new InfoHandle();
                info.Show();
                this.Close();
            }));
        }

       
        private void Callback(IAsyncResult ar)
        {
           
            Socket s = ar.AsyncState as Socket;
            App.serverSocketCmd = s.EndAccept(ar);
           

        }

        private ImageSource CreateQRCode(string content, int width, int height)
        {
            //包含一些编码，大小等的设置
            EncodingOptions options;
            //用来生成二维码，对应的BarcodeReader用来解码
            BarcodeWriter write = null;

            options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = width,
                Height = height,
                Margin = 0
            };
            write = new BarcodeWriter();
            write.Format = BarcodeFormat.QR_CODE;
            write.Options = options;
            Bitmap bitmap = write.Write(content);
            IntPtr ip = bitmap.GetHbitmap();
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ip);
            return bitmapSource;
        }
    }
}
