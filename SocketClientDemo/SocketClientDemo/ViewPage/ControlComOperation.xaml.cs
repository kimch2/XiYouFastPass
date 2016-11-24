using SocketClientDemo.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace SocketClientDemo.ViewPage
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ControlComOperation : Page
    {
        List<ControlBtnModel> controlbtn = new List<ControlBtnModel>();

        public ControlComOperation()
        {
            this.InitializeComponent();
        }     
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
            controlbtn.Add(new ControlBtnModel {Source="ms-appx:///Image/uploadc.png" ,Name="上传文件"});
            controlbtn.Add(new ControlBtnModel { Source = "ms-appx:///Image/gradscreen.png", Name = "抓屏幕" });
            controlbtn.Add(new ControlBtnModel { Source = "ms-appx:///Image/shutdown.png", Name = "遥控关机" });
            controlbtn.Add(new ControlBtnModel { Source = "ms-appx:///Image/signal.png", Name = "遥控器" });
            controlbtn.Add(new ControlBtnModel { Source = "ms-appx:///Image/ppt.png", Name = "遥控PPT" });
            controlbtn.Add(new ControlBtnModel { Source = "ms-appx:///Image/volume.png", Name = "语音操作" });
            uploadfile.DataContext = controlbtn[0];
            gradscreen.DataContext = controlbtn[1];
            shutdown.DataContext = controlbtn[2];
            signal.DataContext = controlbtn[3];
            ppt.DataContext = controlbtn[4];
            voice.DataContext = controlbtn[5];
            shutdown.IsEnabled = true;
            signal.IsEnabled = true;
            ppt.IsEnabled = true ;
            voice.IsEnabled = true ;
            gradscreen.IsEnabled = true;
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
            base.OnNavigatedFrom(e);
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }

        private void img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Return();
        }

        private void uploadfile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            uploadfile.IsEnabled = false;
            App.SendPhoneFile();

            uploadfile.IsEnabled = true;
        }

        private void gradscreen_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gradscreen.IsEnabled = false;
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(ScreenShow));
           
     

        }

        private void shutdown_Tapped(object sender, TappedRoutedEventArgs e)
        {
            shutdown.IsEnabled = false;
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(ShutdownView));
           // shutdown.IsEnabled = true;
        }

        private void signalctr_Tapped(object sender, TappedRoutedEventArgs e)
        {
            signal.IsEnabled = false;
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(Signal));
        }

        private void ppt_Tapped(object sender, TappedRoutedEventArgs e)
        {
          
            ppt.IsEnabled = false;
            /*Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(PPTMainView));*/
        
            new MainPage().mycom_Click("PPTDO", null);
        }
      
        private void voice_Tapped(object sender, TappedRoutedEventArgs e)
        {          
            voice.IsEnabled = false;
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(VoiceDemo));
        }
    }
}
