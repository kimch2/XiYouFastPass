using Newtonsoft.Json;
using SocketServerDemo.Common;
using SocketServerDemo.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SocketServerDemo
{
    /// <summary>
    /// InfoHandle.xaml 的交互逻辑
    /// </summary>
    public partial class InfoHandle : Window
    {
        public static Thread threadCmd = null;    
        public static Thread threadFile = null;      
   
        public InfoHandle()
        {
            InitializeComponent();
            App.IsConnect = true;
            threadCmd = new Thread(new ParameterizedThreadStart(App.ReceiveCmd));
            threadFile = new Thread(new ParameterizedThreadStart(App.ReceiveFileSocket));
            if(App.serverSocketCmd!=null)
                threadCmd.Start(App.serverSocketCmd);          
            if (App.serverSocketFile != null)
                threadFile.Start(App.serverSocketFile);
           /* Task t = Task.Factory.StartNew(new Action(()=>
            {
                while (App.IsConnect == true)
                {
                    if (App.IsConnect == false)
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            MainWindow mainwindow = new MainWindow();
                            mainwindow.Show();
                            this.Close();
                        }));
                    }                
                }           
            }));  */       
        }
       
      

        public   void Button_Click(object sender, RoutedEventArgs e)
        {
           
            if (btn.Content.ToString() == "断开连接")
            {              
                DisConnectServer();
            }
            Dispatcher.Invoke((Action)(() =>
            {
                MainWindow mainwindow = new MainWindow();
                mainwindow.Show();
                this.Close();
            }));


        }
        public static void DisConnect()
        {

            if (App.serverSocketCmd != null)
            {
                App.serverSocketCmd.Dispose();
                App.serverSocketCmd.Close();             
            }
            if (App.serverSocketFile != null)
            {
                App.serverSocketFile.Dispose();
                App.serverSocketFile.Close();              
            }
            if (MainWindow.serverSocketCmd != null)
            {
                MainWindow.serverSocketCmd.Dispose();
                MainWindow.serverSocketCmd.Close();
                MainWindow.serverSocketCmd = null;    
            }
            if (MainWindow.serverSocketFile != null)
            {
                MainWindow.serverSocketFile.Dispose();
                MainWindow.serverSocketFile.Close();
                MainWindow.serverSocketFile = null;
            }
            App.IsConnect = false;
        }
        public static void DisConnectClient()
        {
            DisConnect();
        }
        public static void DisConnectServer()
        {
            DisConnect();                     
        }

        
    }
}
