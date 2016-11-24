using Newtonsoft.Json;
using SocketClientDemo.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Phone.UI.Input;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace SocketClientDemo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Signal : Page
    {
        public Signal()
        {
            this.InitializeComponent();

        }
        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
           
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {

            e.Handled = true;
            Return();
        }
        private void Return()
        {

            Frame frame = (Window.Current.Content) as Frame;
            if (frame.CanGoBack)
            {
                frame.GoBack();
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }
        private void img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Return();
        }


        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            //单指滑动
            if (e.Delta.Expansion == 0)
            {
                double x = e.Delta.Translation.X;
                double y = e.Delta.Translation.Y;
                SendSignal(x, y, "Move1");
            }
            //多指滑动
            else
            {
                //模拟滚轮
                if (e.Delta.Scale > 0.85 && e.Delta.Scale < 1.15 && Math.Abs(e.Delta.Expansion) <= 20)
                {
                    if (e.Delta.Translation.Y > 0)
                    {
                        SendSignal(0, 0, "WheelDown");
                    }
                    else if (e.Delta.Translation.Y < 0)
                    {
                        SendSignal(0, 0, "WheelUp");
                    }
                }
                else if (e.Delta.Scale > 1.5)
                {
                    SendSignal(0, 0, "Big");
                }
                else if (e.Delta.Scale < 0.85)
                {
                    SendSignal(0, 0, "Small");
                }
            }
        }

        private async  void SendSignal(double x, double y, string mode)
        {
         
            string msg = String.Format("{0},{1},{2},",x,y,mode);
            Command command = new Command { Flag = "3", Msg = msg };
            string sendmsg = "XiYou#"+JsonConvert.SerializeObject(command);
            Debug.WriteLine("手势发送的命令:"+sendmsg); 
            byte[] buffer = Encoding.UTF8.GetBytes(sendmsg);
            using (DataWriter writer = new DataWriter(App.socketCmd.OutputStream))
            {
                writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                try
                {
                    writer.WriteBytes(buffer);
                    await writer.StoreAsync();

                    await writer.FlushAsync();                  
                    writer.DetachStream();                    
                    writer.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + "手势操作发生异常");
                    //await new MessageDialog(ex.Message+"手势操作发生异常").ShowAsync();
                }
               
            }
        }
        private void Left_Click(object sender, RoutedEventArgs e)
        {
            Left.IsEnabled = false;
            SendSignal(0, 0, "Left");
            Left.IsEnabled = true;
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            Right.IsEnabled = false;
            SendSignal(0, 0, "Right");
            Right.IsEnabled = true;
        }

       
    }
}
