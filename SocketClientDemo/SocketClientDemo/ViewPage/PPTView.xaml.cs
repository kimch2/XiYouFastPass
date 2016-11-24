using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class PPTView : Page
    {
        public PPTView()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
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

        private void previous_Tapped(object sender, TappedRoutedEventArgs e)
        {
            previous.IsEnabled = false;
            App.SendCmd("上一页ppt");
            previous.IsEnabled = true;
        }

        private void next_Tapped(object sender, TappedRoutedEventArgs e)
        {
            next.IsEnabled = false;
            App.SendCmd("下一页ppt");
            next.IsEnabled = true;
        }

        private void first_Tapped(object sender, TappedRoutedEventArgs e)
        {
            first.IsEnabled = false;
            App.SendCmd("第一页");
            first.IsEnabled = true;
        }

        private void last_Tapped(object sender, TappedRoutedEventArgs e)
        {
            last.IsEnabled = false;
            App.SendCmd("最后一页");
            last.IsEnabled = true
                ;
        }

        private void play_Tapped(object sender, TappedRoutedEventArgs e)
        {
            play.IsEnabled = false;
            App.SendCmd("播放ppt");
            play.IsEnabled = true;
        }

        private void close_Tapped(object sender, TappedRoutedEventArgs e)
        {
            close.IsEnabled = false;
            App.SendCmd("关闭ppt");
            close.IsEnabled = true;
        }
    }
}
