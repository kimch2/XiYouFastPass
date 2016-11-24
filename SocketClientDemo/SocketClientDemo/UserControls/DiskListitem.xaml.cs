using SocketClientDemo.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SocketClientDemo.UserControls
{
    public sealed partial class DiskListitem : UserControl
    {
        private static DiskItemModel diskitem=new DiskItemModel();
        public static FileDownLoadModel filemodel = null;

        public DiskListitem()
        {
            this.InitializeComponent();
        }

        private  async void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            
            diskitem = btn.DataContext as DiskItemModel;
            if (diskitem.FileTyp == "Archive")
            {
                string downfile = diskitem.FullName;
                btn.IsEnabled = false;
                //await new MessageDialog("选择下载或取消").ShowAsync();
                filemodel = new FileDownLoadModel();
                filemodel.Img = diskitem.Img;
                filemodel.FullName = diskitem.FullName;
                filemodel.Name = diskitem.Name;
                //filemodel.Size =100;
             
                //filemodel.Count = 0;
               
                //看是否有要 下载的文件没
                if (App.downloadfile.Count == 0)
                {
                    filemodel.Status = "正在下载";
                    //向待下载队列添加文件
                    App.downloadfile.Add(filemodel);
                    //向历史队列添加文件
                    //也是listview的绑定对象，只增加，不减少
                    App.historyfile.Add(filemodel);
                    App.DownloadFile(downfile, App.socketFile);
                    await new MessageDialog("准备下载").ShowAsync();
                    btn.IsEnabled = true;
                    //App.ReceiveFile(diskitem, App.socketFile);
                    Task t = Task.Factory.StartNew(() =>
                    {
                        App.ReceiveFile(diskitem, App.socketFile);
                    });
                
                    //Task t = new Task(DoReciveFile);
                    //t.Start();

                    //await new MessageDialog("文件下载完毕").ShowAsync();
                }
                else
                {
                    filemodel.Status = "等待下载";
                    //向待下载队列添加文件
                    App.downloadfile.Add(filemodel);
                    //向历史队列添加文件
                    App.historyfile.Add(filemodel);
                    await new MessageDialog("已加入下载队列").ShowAsync();
                    btn.IsEnabled = true;
                }
            }
           
            
        }
      
    }
}
