using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace SocketClientDemo.ViewPage
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ScreenShow : Page
    {
        private static BitmapImage bitmap = null;
        byte[] writebuffer = new byte[0];
    
        public ScreenShow()
        {
            this.InitializeComponent();
            


        }

        protected override  void OnNavigatedTo(NavigationEventArgs e)
        {

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
         
            SetScreenImg();

        }

        private async void SetScreenImg()
        {
            if (App.flag == 0)
            {
                //tDisk = new Task((Action)(() => {
                App.ReceiveCmd(App.socketCmd);
                App.flag = 1;
            }
            App.SendCmd("截屏");
            StorageFolder folder = null;
            StorageFile file = null;
            folder = KnownFolders.PicturesLibrary;
            file = await folder.CreateFileAsync("screen.png", CreationCollisionOption.ReplaceExisting);
            writebuffer = new byte[0];
            Task t = Task.Factory.StartNew(new Action(async() =>
            {
                while (App.screenbuffer == null)
                {
                    ;
                }
                File.WriteAllBytes(file.Path, App.screenbuffer);               
                writebuffer = writebuffer.Concat(App.screenbuffer).ToArray();

                IRandomAccessStream inputstream = await file.OpenReadAsync();
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                {
                    bitmap = new BitmapImage();
                    bitmap.SetSource(inputstream);
                    screenimg.Source = bitmap;
                    App.screenbuffer = null;
                }));
               

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
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }
        private void img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Return();
        }

        private async void save_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MessageDialog dlg = new MessageDialog("确定要保存图片?","提示");
            UICommand cmdOK = new UICommand("确定",new UICommandInvokedHandler(OnCommandAct),1);
            UICommand cmdCanel = new UICommand("取消", new UICommandInvokedHandler(OnCommandAct),2);
            dlg.Commands.Add(cmdOK);
            dlg.Commands.Add(cmdCanel);
            await dlg.ShowAsync();
        }

        private async void OnCommandAct(IUICommand command)
        {
            int cmdId = (int)command.Id;
            if (cmdId == 1)
            {
                StorageFile file = null;
                StorageFolder storagefolder = KnownFolders.RemovableDevices;
                int flag = 0;
                foreach (var f in await storagefolder.GetFoldersAsync())
                {
                    foreach (var SDfolder in await f.GetFoldersAsync())
                    {
                        if (SDfolder.Name == "Downloads")
                        {
                            file = await SDfolder.CreateFileAsync("screenimg.jpg", CreationCollisionOption.ReplaceExisting);
                            File.WriteAllBytes(file.Path, writebuffer);
                                                 
                            await new MessageDialog("图片已保存").ShowAsync();                  
                            flag = 1;
                            break;
                        }
                    }
                    if (flag == 0)
                    {
                        file = await f.CreateFileAsync("screenimg.jpg", CreationCollisionOption.ReplaceExisting);
                        File.WriteAllBytes(file.Path, writebuffer);
                        break;
                    }
                }
            }
        }
    }
}
