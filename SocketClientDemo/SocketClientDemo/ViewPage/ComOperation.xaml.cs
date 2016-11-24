using Newtonsoft.Json;
using SocketClientDemo.Common;
using SocketClientDemo.Model;
using SocketClientDemo.UserControls;
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
    public sealed partial class ComOperation : Page
    {
        public static List<DiskFolder> diskfolder;

        List<DiskButton> btnName =new List<DiskButton>();
        List<DiskButton> btnAll;

        List<DiskBtnModel> diskbtnModel = new List<DiskBtnModel>();

        List<string> color = new List<string> { "#2198DC", "#1D98D9", "#64C418", "#FEAF14", "#F740D1", "#2099DE" , "#2198DC", "#1D98D9"};
    
        public ComOperation()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int i;
            base.OnNavigatedTo(e);
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
            btnAll = new List<DiskButton> { one, two, three, four,five,six,seven,eight};
          
            diskfolder = e.Parameter as List<DiskFolder>;
            for (i = 0; i <= diskfolder.Count; i++)
            {
                DiskButton diskbtn = btnAll[i];
                diskbtn.IsEnabled = true;
                diskbtn.Visibility = Visibility.Visible;
                btnName.Add(diskbtn);
            }
           
            diskbtnModel.Add(new DiskBtnModel { Name="Desktop",Lable="桌面文件",Color=color[0]});
            for ( i = 0; i < diskfolder.Count; i = i + 1)
            {
                diskbtnModel.Add(new DiskBtnModel { Name = diskfolder[i].Name, Lable = diskfolder[i].Lable, Color = color[i+1] });
                btnName[i].DataContext = diskbtnModel[i];

            }
            btnName[i].DataContext = diskbtnModel[i];
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
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
        private void img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Return();
        }

        private void one_readDisk(object sender, TappedRoutedEventArgs e)
        {
            one.IsEnabled = false;
            int i = 0;
            Send(i);                     
        }

        private void two_readDisk(object sender, TappedRoutedEventArgs e)
        {
            two.IsEnabled = false;
            int i = 1;
            Send(i);
        
        }

        private void three_readDisk(object sender, TappedRoutedEventArgs e)
        {
            three.IsEnabled = false;
            int i = 2;
            Send(i);
           
        }

        private void four_readDisk(object sender, TappedRoutedEventArgs e)
        {
            four.IsEnabled = false;
            int i = 3;
            Send(i);
          
        }

        private void five_readDisk(object sender, TappedRoutedEventArgs e)
        {
            five.IsEnabled = false;
            int i = 4;
            Send(i);
           
        }

        private void six_readDisk(object sender, TappedRoutedEventArgs e)
        {
            six.IsEnabled = false;
            int i = 5;
            Send(i);
           
        }
      
        private void seven_readDisk(object sender, TappedRoutedEventArgs e)
        {
            seven.IsEnabled = false;
            int i = 6;
            Send(i);
        }
        private void eight_readDisk(object sender, TappedRoutedEventArgs e)
        {
            eight.IsEnabled = false;
            int i = 7;
            Send(i);
        }
        private void Send(int i)
        {
            DiskFolder folder = null;
            if (App.PPTOperation == "PPTSHOW")
            {
                folder = new DiskFolder { Name = null, Lable ="PPTSHOW", FullName = diskbtnModel[i].Name };
            }
            else if (App.PPTOperation == "PPTDO")
            {
                folder = new DiskFolder { Name = null, Lable = "PPTDO", FullName = diskbtnModel[i].Name };
            }
           
            string message = JsonConvert.SerializeObject(folder);
            Command command = new Command { Flag = "2", Msg = message };
            string  sendmessage= JsonConvert.SerializeObject(command);
            App.SendData(sendmessage, App.socketCmd);
            
        }

    }
}
