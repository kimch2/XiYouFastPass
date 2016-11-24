using Newtonsoft.Json;
using SocketClientDemo.Common;
using SocketClientDemo.Model;
using SocketClientDemo.UserControls;
using SocketClientDemo.ViewPage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace SocketClientDemo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        object lockObject = new object();
        LoginButtonModel lbtn1;
        LoginButtonModel lbtn2;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
         
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
            App._navigateAboutEvent += NagivateAbout_Me;
            lbtn1 = new LoginButtonModel();
            lbtn2 = new LoginButtonModel();
            lbtn1.ImgSource = "ms-appx:///Image/mycom.png";
            lbtn1.Name = "我的电脑";
            lbtn2.ImgSource = "ms-appx:///Image/controcom.png";
            lbtn2.Name = "遥控电脑";
            LoadPicture();
            



           // filelistview.ItemsSource = App.historyfile ;
         
            //Task t = new Task(UpDownloadUI);

            //t.Start();

        }
        private async void LoadPicture()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
             {
                 mycombtn.DataContext = lbtn1;
                 connectbtn.DataContext = lbtn2;
             }));
           
        }
        private void NagivateAbout_Me(object sender, EventArgs e)
        {
            lock(lockObject)
            {
                if (Frame_Login.CanGoBack)
                {
                    Frame_Login.GoBack();
                }
                if (Frame_Login.Visibility == Visibility.Visible)
                {
                    Frame_Login.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Frame frame = (Window.Current.Content) as Frame;
            if (frame.CanGoBack)
            {
                frame.GoBack();
            }
            else
            {
                App.Current.Exit();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
           
                
            }
        }
     

        private void sidebtn_Click(object sender, RoutedEventArgs e)
        {
            if (splitview.IsPaneOpen == false)
            {
                Frame_Login.Visibility = Visibility.Collapsed;
            }
            splitview.IsPaneOpen = !splitview.IsPaneOpen;
        }

        public  async void mycom_Click(object sender, RoutedEventArgs e)
        {
            String str = sender.ToString();
            if (str.Equals("PPTDO"))
            {
                App.PPTOperation= "PPTDO";
            }
            else
            {
                App.PPTOperation = "PPTSHOW";
            }
            if (App.IsConnect == false)
            {
                await new MessageDialog("请先连接").ShowAsync();
            }
            else
            {

                loadanimation.Begin();
                DiskFolder folder = null;
                if (App.PPTOperation == "PPTDO")
                {
                    folder = new DiskFolder { Name = "我的电脑", Lable = "我的电脑", FullName = "我的电脑"};
                }
                else if(App.PPTOperation=="PPTSHOW")
                {
                    folder = new DiskFolder { Name = "我的电脑", Lable = "我的电脑", FullName = "我的电脑" };
                }
                //类的序列化
                string message = JsonConvert.SerializeObject(folder);
                Command command = new Command { Flag = "2", Msg = message };
                string sendmessage = JsonConvert.SerializeObject(command);
                App.SendData(sendmessage, App.socketCmd);
                if (App.flag == 0)
                {
                    //tDisk = new Task((Action)(() => {
                    App.ReceiveCmd(App.socketCmd);
                    App.flag = 1;
                    //}));
                    //tDisk.Start();
                    loadanimation.Stop();
                    image1.Visibility = Visibility.Collapsed;
                    image2.Visibility = Visibility.Collapsed;
                    image3.Visibility = Visibility.Collapsed;
                }

              
            }
        }
        private void login_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsConnect == false)
            {
                if (Frame_Login.Visibility == Visibility.Collapsed)
                {
                    Frame_Login.Visibility = Visibility.Visible;
                    SuppressNavigationTransitionInfo n = new SuppressNavigationTransitionInfo();
                    Frame_Login.Navigate(typeof(LoginView), null, n);
                }
                else if (Frame_Login.Visibility == Visibility.Visible)
                {
                    Frame_Login.Visibility = Visibility.Collapsed;
                }
            }
           
         

        }

        private async void connectbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsConnect == false)
            {
                await new MessageDialog("请先连接").ShowAsync();
            }
            else
            {
                Frame frame = (Window.Current.Content) as Frame;
                //SuppressNavigationTransitionInfo s = new SuppressNavigationTransitionInfo();
                //CommonNavigationTransitionInfo s = new CommonNavigationTransitionInfo();
                
                //ContinuumNavigationTransitionInfo s = new ContinuumNavigationTransitionInfo();
                //DrillInNavigationTransitionInfo s = new DrillInNavigationTransitionInfo();
                EntranceNavigationTransitionInfo s = new EntranceNavigationTransitionInfo();
                //SlideNavigationTransitionInfo s = new SlideNavigationTransitionInfo();
                frame.Navigate(typeof(ControlComOperation), null, s);
            }
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {           
                Frame_Login.Visibility = Visibility.Visible;
                ContinuumNavigationTransitionInfo n = new ContinuumNavigationTransitionInfo();          
                Frame_Login.Navigate(typeof(LoginView), null, n);     
        }

        private  void uploadtext_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.SendPhoneFile();
        }

        private async void feedback_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var email = new EmailMessage();
            email.Subject = "反馈《西邮快传》";
            email.Body = "fankuixiyoukuaichuan";
            email.To.Add(new EmailRecipient("wangyongzhi@xiyoumobile.com"));
            email.Sender.Address ="wangyongzhi@xiyoumobile.com";
            await EmailManager.ShowComposeNewEmailAsync(email);
            
        }

        private async void about_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.xiyoumobile.com/",UriKind.RelativeOrAbsolute));
        }

        private  void intro_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame frame = (Window.Current.Content) as Frame;
            EntranceNavigationTransitionInfo s = new EntranceNavigationTransitionInfo();
            frame.Navigate(typeof(instruction), null, s);

        }

        private async void scanbtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsConnect == false)
            {
                Frame frame = (Window.Current.Content) as Frame;
                EntranceNavigationTransitionInfo s = new EntranceNavigationTransitionInfo();
                frame.Navigate(typeof(ScanScan), null, s);
            }
            else
            {
                await new MessageDialog("请先断开连接").ShowAsync();
            }
         
        }

      
    }
}
