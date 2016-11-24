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
    public sealed partial class ShutdownView : Page
    {
        public ShutdownView()
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
        private void shutdown_Tapped(object sender, TappedRoutedEventArgs e)
        {
            loadanimation.Begin();
        }

        private void logoff_Tapped(object sender, TappedRoutedEventArgs e)
        {
            logoff.IsEnabled = false;
            App.SendCmd("重启");
            logoff.IsEnabled = true;
        }

        private void immediately_Click(object sender, RoutedEventArgs e)
        {
            immediately.IsEnabled = false;
            canelanimation.Begin();
            App.SendCmd("立即关机");
            immediately.IsEnabled = true;
        }

        private void afterhalfhour_Click(object sender, RoutedEventArgs e)
        {
            afterhalfhour.IsEnabled = false;
            canelanimation.Begin();
            App.SendCmd("半小时后关机");
            afterhalfhour.IsEnabled = true;
        }

        private void afteronehour_Click(object sender, RoutedEventArgs e)
        {
            afteronehour.IsEnabled = false;
            canelanimation.Begin();
            App.SendCmd("一小时后关机");
            afteronehour.IsEnabled = true;
        }

        private void canelClose_Click(object sender, RoutedEventArgs e)
        {
            canelClose.IsEnabled = false;
            canelanimation.Begin();
            App.SendCmd("取消关机");
            canelClose.IsEnabled = true;
        }

        private void canel_Click(object sender, RoutedEventArgs e)
        {
            canel.IsEnabled = false;
            canelanimation.Begin();
        }
    }
}
