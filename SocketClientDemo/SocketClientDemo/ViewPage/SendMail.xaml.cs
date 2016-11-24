using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class SendMail : Page
    {
        public SendMail()
        {
            this.InitializeComponent();
        }

        private async void sublimt_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var email = new EmailMessage();
            email.Subject = name.Text+"反馈《西邮快传》";
            email.Body = msg.Text;
            email.To.Add(new EmailRecipient("wangyongzhi@xiyoumobile.com"));

            
            await EmailManager.ShowComposeNewEmailAsync(email);
        }

        private void img_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
