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
using Windows.UI.Core;
using Windows.UI.Popups;
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
    public sealed partial class DiskShow : Page
    {
        List<DiskFolder> diskfolderitem;

        List<DiskItemModel> diskitemmodel = new List<DiskItemModel>();
        public DiskShow()
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


            diskfolderitem = e.Parameter as List<DiskFolder>;
            if (diskfolderitem.Count > 0)
            {
                pathname.DataContext = new DiskFolder() { FullName = handle_PathName(diskfolderitem[0].FullName) };
                for (int i = 0; i < diskfolderitem.Count; i++)
                {
                    if (diskfolderitem[i].FileTyp == "Directory")
                    {

                        diskitemmodel.Add(new DiskItemModel { Img = "ms-appx:///Image/folder.png", Name = diskfolderitem[i].Name, FullName = diskfolderitem[i].FullName, DownLoadPic = null, FileTyp = diskfolderitem[i].FileTyp });
                    }
                    else if (diskfolderitem[i].FileTyp == "Archive")
                    {
                        diskitemmodel.Add(new DiskItemModel { Img = "ms-appx:///Image/file.png", Name = diskfolderitem[i].Name, FullName = diskfolderitem[i].FullName, DownLoadPic = "ms-appx:///Image/download.png", FileTyp = diskfolderitem[i].FileTyp });
                    }
                    else
                    {
                        diskitemmodel.Add(new DiskItemModel { Img = "ms-appx:///Image/folder.png", Name = diskfolderitem[i].Name, FullName = diskfolderitem[i].FullName, DownLoadPic = null, FileTyp = diskfolderitem[i].FileTyp });
                    }
                }
            }

            disklistview.ItemsSource = diskitemmodel;
            Task t = Task.Factory.StartNew(new Action(async () =>
            {

                while (true)
                {
                    while (App.alreadydownload <= App.totaldownload)
                    {
                        if (DiskListitem.filemodel != null)
                        {
                            if (App.alreadydownload == 0)
                            {
                                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                                {
                                    loadanimation.Begin();
                                    btn.IsEnabled = false;
                                }));
                            }

                            if (App.alreadydownload != App.predownload)
                            {
                                FileDownLoadModel filemodel = new FileDownLoadModel();
                                filemodel.Name = DiskListitem.filemodel.Name;
                                filemodel.FullName = DiskListitem.filemodel.FullName;
                                filemodel.Img = "ms-appx:///Image/file1.png";
                                filemodel.Size = App.totaldownload;
                                filemodel.Count = App.alreadydownload;
                                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                                {

                                    grid.DataContext = filemodel;
                                }));
                                await Task.Delay(30);
                            }
                        }
                        if (App.alreadydownload == App.totaldownload && App.totaldownload != 0)
                        {

                            await new MessageDialog("下载完毕").ShowAsync();
                            App.alreadydownload = 0;
                            //App.totaldownload = 0;
                            //App.predownload = 0;
                            DiskListitem.filemodel = null;
                            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                            {
                                canelanimation.Begin();
                                btn.IsEnabled = true;
                            }));


                        }

                    }
                }
            }));
                 
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

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Return();
        }
        private string handle_PathName(string path)
        {
            int i;
            for(i=path.Length-1;i>=0;i--)
            {
                if(path[i]=='\\')
                {
                    break;
                }
            }
            path = path.Substring(0,i+1);
            return path;
        }

     

        private void disklistview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            disklistview.IsEnabled = false;
             DiskItemModel diskitem = disklistview.SelectedItem as DiskItemModel;
            string fullname = pathname.Text + diskitem.Name;
            DiskFolder folder = new DiskFolder { Name = null, Lable ="PPTSHOW", FullName = fullname };                           
            string message = JsonConvert.SerializeObject(folder);
            Command command = new Command { Flag = "2", Msg = message };
            string sendmessage= JsonConvert.SerializeObject(command);
            App.SendData(sendmessage, App.socketCmd);       
            disklistview.IsEnabled = true;          
        }   
    }
}
