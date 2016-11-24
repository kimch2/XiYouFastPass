using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace SocketClientDemo.ViewPage
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoginView : Page
    {
        public LoginView()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
            if (App.IsConnect == true)
            {
                btn.Content = "断开连接";
                TB_IPText.Text = App.ip;
                TB_CountText.Text = App.port;
            }
            else
            {
                var hosts = NetworkInformation.GetHostNames();
                HostName svName = hosts.FirstOrDefault(h => h.Type == HostNameType.Ipv4 && h.IPInformation.NetworkAdapter.IanaInterfaceType == 71);
                if (svName != null)
                {
                    TB_IPText.Text = svName.DisplayName;
                    TB_CountText.Text = "10240";
                }
            }

        }

   

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            App._navigateAboutEvent.Invoke(this, new EventArgs());
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }

        private void OnConnect(object sender, RoutedEventArgs e)
        {
            btn.IsEnabled = false;
            if (App.IsConnect == false && btn.Content.ToString() == "连接")
            {
                SocketInit();
            }
            else if (App.IsConnect == true && btn.Content.ToString()=="断开连接")
            {
                App.SendCmd("断开连接");           
                App.IsConnect = false;
                App.flag = 0;
                btn.Content = "连接";
                btn.IsEnabled = true;
                App.socketCmd.Dispose();
                App.socketFile.Dispose();       
            }                    
        }

        private async  void SocketInit()
        {
            App.socketCmd = new StreamSocket();         
            App.socketFile = new StreamSocket();        
            try
            {
                HostName h = new HostName(TB_IPText.Text);
                await App.socketCmd.ConnectAsync(h, TB_CountText.Text);           
                await App.socketFile.ConnectAsync(h, (Convert.ToInt32(TB_CountText.Text) + 1).ToString());               
                //await new MessageDialog("连接成功").ShowAsync();
                btn.Content = "断开连接";
                App.IsConnect = true;
                btn.IsEnabled = true;
                App.ip = TB_IPText.Text;
                App.port = TB_CountText.Text;
                App._navigateAboutEvent.Invoke(this, new EventArgs());
                //frame.Navigate(typeof(MainViewModel));*/

            }
            catch (Exception )
            {
                await new MessageDialog("建立连接失败").ShowAsync();
                btn.IsEnabled = true;
            }
        }

        private void Close_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App._navigateAboutEvent.Invoke(this, new EventArgs());

            //SuppressNavigationTransitionInfo n = new SuppressNavigationTransitionInfo();
            //frame.Navigate(typeof(MainPage), null, n);

        }
    }
}
