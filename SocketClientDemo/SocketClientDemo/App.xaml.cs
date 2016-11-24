using Newtonsoft.Json;
using SocketClientDemo.Common;
using SocketClientDemo.Model;
using SocketClientDemo.UserControls;
using SocketClientDemo.ViewPage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace SocketClientDemo
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
      
        public static Dictionary<string, string> WinCommand = new Dictionary<string, string>();
        public static Dictionary<string, string> Search = new Dictionary<string, string>();
        public static Dictionary<string, string> Web = new Dictionary<string, string>();
        public static Dictionary<string, string> Office = new Dictionary<string, string>();
        //存放磁盘下文件夹和文件信息
        public static List<DiskFolder> folderlist = new List<DiskFolder>();
        //文件下载记录设置
        //正在下载的文件
        public static List<FileDownLoadModel> downloadfile = new List<FileDownLoadModel>();
        //下载的历史文件
        public static List<FileDownLoadModel> historyfile = new List<FileDownLoadModel>();
        //已下载进度
        public static int predownload = 0;
        public static int alreadydownload = 0;
        //总进度
        public static int totaldownload = 0;   
         
        //cmdsocket，传输命令，磁盘，手势，ppt操作
        public static StreamSocket socketCmd =null;
        //filesocket，传输文件信息
        public static StreamSocket socketFile = null;       
        public static int flag = 0;
        //是否连接
        public static bool IsConnect = false;
        //保存ip
        public static string ip="";
        //保存端口
        public static string port="";
        //PPT操作
        public static string PPTOperation = "";
        //存截屏的图片流
        public static byte[] screenbuffer = null;
        //下载的文件是文本文件
        public static string txtstring = "";


        //通知导航事件
        public static EventHandler<EventArgs> _navigateAboutEvent;
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            LoadDict();
        }
        //加载xml文档
        private async void LoadDict()
        {
            StorageFile storageFile = null;
            StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            foreach (StorageFolder f in await folder.GetFoldersAsync())
            {
                if (f.Name == "Common")
                {
                    foreach (StorageFile file in await f.GetFilesAsync())
                    {
                        if (file.Name == "Dict.xml")
                        {
                            storageFile = file;
                        }
                    }
                    try
                    {
                        Stream stream = await storageFile.OpenStreamForReadAsync();
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.IgnoreWhitespace = true;
                        settings.IgnoreComments = true;
                        settings.Async = true;
                        using (XmlReader reader = XmlReader.Create(stream, settings))
                        {
                            
                            while (await reader.ReadAsync())
                            {
                                if (reader.Name == "Cmd")
                                {
                                    WinCommand.Add(reader.GetAttribute("Text"), reader.GetAttribute("Operation"));
                                }
                                else if (reader.Name == "OfficeProgram")
                                {
                                    Office.Add(reader.GetAttribute("Text"), reader.GetAttribute("Operation"));
                                }
                                else if (reader.Name == "Web")
                                {
                                    Web.Add(reader.GetAttribute("Text"), reader.GetAttribute("Operation"));
                                }
                                else if (reader.Name == "Search")
                                {
                                    Search.Add(reader.GetAttribute("Name"), reader.GetAttribute("Command"));
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }

            }
        }


        //参数为文档中的Operation
        public static  void SendCmd(string  str)
        {
            HandleModel handle = new HandleModel();
            handle=StringHandle.MessageHandle(handle, str);
            string msg = JsonConvert.SerializeObject(handle);
            Command command = new Command { Flag = "1", Msg = msg };

            //所发送的Msg，{"FullName":"我的电脑","FileTyp":null,"Name":"我的电脑","Lable":"我的电脑"}"}
            string message = JsonConvert.SerializeObject(command);

            App.SendData(message, App.socketCmd);


        }     
        /// <summary>
        /// 通过指定socket通道发送字符串信息
        /// </summary>
        /// <param name="message"> {"FullName":"我的电脑","FileTyp":null,"Name":"我的电脑","Lable":"我的电脑"}"}</param>
        /// <param name="socket"> App.SocketCmd</param>
        public async   static void  SendData(string message,StreamSocket socket)
        {            
            string sendMessage = string.Format("XiYou#{0}",message);
            Debug.WriteLine("SocketCmd发送的命令:" + sendMessage);
            byte[] buffer = Encoding.UTF8.GetBytes(sendMessage);
            //Encoding.UTF8.GetBytes(sendMessage, 0, sendMessage.Length,buffer, 0);
            using (DataWriter writer = new DataWriter(socket.OutputStream))
            {
                writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                try
                {
                    
                    writer.WriteBytes(buffer);
                    var a=await writer.StoreAsync();
                    await writer.FlushAsync();
                    writer.DetachStream();
                    writer.Dispose();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    //await new MessageDialog(e.Message).ShowAsync();
                    //writer.Dispose();
                    return;
                }
            }
        }
        public async static void SendPhoneFile()
        {
            FileOpenPicker openpicker = new FileOpenPicker();
            openpicker.ViewMode = PickerViewMode.Thumbnail;
            openpicker.SuggestedStartLocation = PickerLocationId.ComputerFolder ;
            //图片
            openpicker.FileTypeFilter.Add(".png");
            openpicker.FileTypeFilter.Add(".jpg");
            openpicker.FileTypeFilter.Add(".jpeg");
            //文本文件
            openpicker.FileTypeFilter.Add(".txt");
            openpicker.FileTypeFilter.Add(".pptx");
            openpicker.FileTypeFilter.Add(".docx");
            openpicker.FileTypeFilter.Add(".xlsx");
            //音频
            openpicker.FileTypeFilter.Add(".3gp");
            openpicker.FileTypeFilter.Add(".mp4");
            openpicker.FileTypeFilter.Add(".mp3");           
            //视频
            openpicker.FileTypeFilter.Add(".wma");
            openpicker.FileTypeFilter.Add(".avi");
            openpicker.FileTypeFilter.Add(".mov");
            openpicker.FileTypeFilter.Add(".amr");
            //
            var sendfile = await openpicker.PickSingleFileAsync();
            if (sendfile != null)
            {
                FileModel filemodel = new FileModel(sendfile, sendfile.Name);
                App.SendFile(filemodel, App.socketFile);
            }
        }
        //通过socketFile通道上传文件信息
        internal async static void SendFile(FileModel fileModel, StreamSocket socketFile)
        {
            //FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            //file.OpenStreamForReadAsync();
            Stream filestream = null;
            HandleFileModel handlefilemodel = new HandleFileModel();
            handlefilemodel.Typ = "FileUpload";
            handlefilemodel.Name=fileModel.fileName;
            handlefilemodel.Msg = null;
            filestream = await fileModel.file.OpenStreamForReadAsync();
           
            handlefilemodel.Size = filestream.Length;
            //如果文件内容大小大于100M
            if (handlefilemodel.Size > 100*1024*1024)
            {
                await new MessageDialog("文件过大,无法上传").ShowAsync();
                return;
            }
            int len = (int)handlefilemodel.Size / (1024 * 1024 * 1);
            int BUFFERSIZE = SetBUFFERSIZE(len);

            string sendmessage = JsonConvert.SerializeObject(handlefilemodel);
            Debug.WriteLine("SocketFile发送的命令:" + sendmessage);
            byte[] buffer = null;
            buffer = new byte[sendmessage.Length];
           
            buffer=Encoding.UTF8.GetBytes(sendmessage);

            
            if (App.IsConnect == true)
            {
                using (DataWriter writer = new DataWriter(socketFile.OutputStream))
                {
                    
                    writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    try
                    {
                        writer.WriteBytes(buffer);
                        await writer.StoreAsync();

                     
                        byte[] buffer1=new byte[BUFFERSIZE];
                        byte[] sendbuffer;
                        //从文件中以流的方式读取，，返回值为实际读取字节数
                        //当读到文件流末尾时返回0
                        int length = filestream.Read(buffer1, 0, BUFFERSIZE);
                        sendbuffer = new byte[length];
                        sendbuffer = GetSendBuffer(sendbuffer, buffer1);
                        //length为每次实际读取的字节数，当为0
                        //时就是文件流的末尾
                        while (length>0)
                        {
                            
                            writer.WriteBytes(sendbuffer);
                            //线程停止10毫秒
                            try
                            {
                                await Task.Delay(10);
                            }
                            catch (Exception)
                            {
                               
                            }
                            await writer.StoreAsync();                         
                        
                            //从文件中以流的方式读取，，返回值为实际读取字节数
                            //当读到文件流末尾时返回0
                            length = filestream.Read(buffer1, 0, BUFFERSIZE);
                            sendbuffer = new byte[length];
                            sendbuffer = GetSendBuffer(sendbuffer, buffer1);
                        }
                        //writer.WriteBytes(buffer);

                        await filestream.FlushAsync();
                        filestream.Dispose();
                      
                        await writer.FlushAsync();
                        writer.DetachStream();
                        writer.Dispose();
                        await new MessageDialog("文件上传完毕").ShowAsync();
                    }
                
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                       // await new MessageDialog(e.Message).ShowAsync();
                        //writer.Dispose();
                        return;
                    }

                }
            }
            else
            {
                await new MessageDialog("请先连接").ShowAsync();
            }
          
        }


        private static byte[] GetSendBuffer(byte[] sendbuffer,byte[] buffer)
        {
          
            for (int i = 0; i < sendbuffer.Length; i++)
            {
                sendbuffer[i] = buffer[i];
            }
            return sendbuffer;
        }
        //通过socketFile通道下载文件信息
        internal async static void DownloadFile(string path, StreamSocket socket)
        {
       
            HandleFileModel handlefileModel = new HandleFileModel();
            handlefileModel.Msg = path;
            handlefileModel.Typ = "FileDownload";
            handlefileModel.Name ="";
            handlefileModel.Size = 0;
     
            string sendmessage = JsonConvert.SerializeObject(handlefileModel);
            byte[] buffer = new byte[sendmessage.Length];
            buffer=Encoding.UTF8.GetBytes(sendmessage);
            //Encoding.UTF8.GetBytes(sendmessage,0,sendmessage.Length,buffer,0);
           
            using (DataWriter writer = new DataWriter(socket.OutputStream))
            {
                try
                {
                    writer.WriteBytes(buffer);
                    await writer.StoreAsync();      //提交数据
                    await writer.FlushAsync();       //刷新数据
                    writer.DetachStream();              //分离流
                    writer.Dispose();                   //关闭
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    //await new MessageDialog(e.Message).ShowAsync();
                    //socket.Dispose();
                    return;
                }
            }                  
        }
           
        //接收socketDisk通道的信息
        public async static void ReceiveCmd(object obj)
        {
            StreamSocket s = obj as StreamSocket;
            byte[] temp=null;
            byte[] buffer = null;
            byte[] bufferlength = null;
            byte[] recvbuffer = null;
            int length;
            while (true)
            {
                recvbuffer = new byte[0];              
                string info = null;
                DataReader reader = new DataReader(s.InputStream);
                reader.InputStreamOptions = InputStreamOptions.Partial;
                reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                try
                {
                    bufferlength = new byte[sizeof(Int32)];
                    await reader.LoadAsync(sizeof(Int32));
                    reader.ReadBytes(bufferlength);
                    //reader.DetachStream();
                    length = BitConverter.ToInt32(bufferlength,0);
                    int notRcvLength = length;
                    //作为接收缓存区,每次最多接收length的字节，最少大于0
                    buffer = new byte[length];
                    while (recvbuffer.Length < length || notRcvLength != 0)
                    {
                        uint count = await reader.LoadAsync((uint)length);
                        notRcvLength -= (int)count;
                        temp = new byte[count];
                        reader.ReadBytes(temp);
                        recvbuffer = recvbuffer.Concat(temp).ToArray();
                    }
    
                    reader.DetachStream();
                    //reader.Dispose();
                                      
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    //await new MessageDialog(e.Message).ShowAsync();
                    return;
                }
                info = Encoding.UTF8.GetString(recvbuffer);
                Debug.WriteLine("SocketCmd接收的信息:"+info);
                //接收到的是磁盘信息
                if (info != "" && info.Contains("XiYou#"))
                {
                    string[] sendinfo = Regex.Split(info, "#");
                    //MainViewModel.m.rcvMsg = sendinfo[1];
                    //字符串反序列化
                    folderlist = JsonConvert.DeserializeObject<List<DiskFolder>>(sendinfo[1]);
                    if (folderlist.Count > 0)
                    {
                        if (folderlist[0].Lable == "本地磁盘")
                        {
                            Frame frame = (Window.Current.Content) as Frame;
                            EntranceNavigationTransitionInfo s1 = new EntranceNavigationTransitionInfo();
                            frame.Navigate(typeof(ComOperation), folderlist, s1);
                        }
                        else if (folderlist[0].Lable == "PPTSHOW")
                        {
                            Frame frame = (Window.Current.Content) as Frame;
                            SuppressNavigationTransitionInfo s1 = new SuppressNavigationTransitionInfo();
                            frame.Navigate(typeof(DiskShow), folderlist, s1);
                        }
                        else if (folderlist[0].Lable == "PPTDO")
                        {
                            Frame frame = (Window.Current.Content) as Frame;
                            SuppressNavigationTransitionInfo s1 = new SuppressNavigationTransitionInfo();
                            frame.Navigate(typeof(DiskPPTShow), folderlist, s1);
                        }
                    }
                    else
                    {
                        if (App.PPTOperation == "PPTSHOW")
                        {
                            DiskFolder diskfolder = new DiskFolder();
                            diskfolder.FullName = "";
                            diskfolder.Name = "文件夹为空";
                            diskfolder.Lable = "";
                            diskfolder.FileTyp = "";
                            folderlist.Add(diskfolder);

                            Frame frame = (Window.Current.Content) as Frame;
                            SuppressNavigationTransitionInfo s1 = new SuppressNavigationTransitionInfo();
                            frame.Navigate(typeof(DiskShow), folderlist, s1);
                        }
                        else if (App.PPTOperation == "PPTDO")
                        {
                            DiskFolder diskfolder = new DiskFolder();
                            diskfolder.FullName = "";
                            diskfolder.Name = "文件夹为空";
                            diskfolder.Lable = "";
                            diskfolder.FileTyp = "";
                           
                            folderlist.Add(diskfolder);
                            Frame frame = (Window.Current.Content) as Frame;
                            SuppressNavigationTransitionInfo s1 = new SuppressNavigationTransitionInfo();
                            frame.Navigate(typeof(DiskPPTShow), folderlist, s1);
                        }

                    }

                }
                else
                {
                    byte[] writebuffer = new byte[length];
                    App.screenbuffer = GetSendBuffer(writebuffer, recvbuffer);
                  
               


                }
               
            }
           
        }
        //下载文件，接收文件信息

          
        public async static void ReceiveFile(DiskItemModel diskitem,object  obj)
        {
            StreamSocket stream=obj  as StreamSocket;
            int BUFFERSZEINT;
             
            byte[] bufferlength = new byte[sizeof(Int32)];
            byte[] filebuffer ;
            byte[] temp=null;
            byte[] recvfilebuffer = new byte[0];
            DataReader reader = new DataReader(stream.InputStream);
            reader.InputStreamOptions = InputStreamOptions.Partial;     //异步读取
            try
            {                                                        
                uint count=await reader.LoadAsync(sizeof(Int32));      //从流中加载一个long长度的数据                 
                reader.ReadBytes(bufferlength);      //获取所加载的long长度的数据
                                                        //await Task.Delay(1000);              
               
                //文件长度
                int fileLength = BitConverter.ToInt32(bufferlength,0);
                App.totaldownload = fileLength;
                Debug.WriteLine("接收到的文件大小: "+fileLength);
                //文件接收剩余长度
                int notRecFileLength =fileLength;
                    
                int len = fileLength / (1024 * 1024 * 1);
                BUFFERSZEINT = SetBUFFERSIZE(len);
                filebuffer = new byte[BUFFERSZEINT];
                    
                //在非ui线程中一定不要进行ui操作，容易发生线程冲突，从而形成Access is denied ,未捕获的异常
                //await new MessageDialog("文件开始下载,请等待").ShowAsync();
                //当接收文件的长度小于文件长度
                while (recvfilebuffer.Length< fileLength)
                {
                    //最后一次少于最多接满缓冲区
                    if (notRecFileLength < BUFFERSZEINT)
                    {                         
                        count = await reader.LoadAsync((uint)notRecFileLength);
                        App.predownload = App.alreadydownload;
                        App.alreadydownload += (int)count;
                        notRecFileLength -= (int)count;
                     
                        temp = new byte[count];
                    }
                    //前几次，每次缓冲区都为最大
                    else
                    {
                        //filebuffer = new byte[BUFFERSZEINT];
                        //实际接收的流长度为count
                        count = await reader.LoadAsync((uint)BUFFERSZEINT);
                        App.predownload = App.alreadydownload;
                        App.alreadydownload += (int)count;
                    
                        //未接收文件的长度
                        notRecFileLength -= (int)count;
                        temp = new byte[count];                         
                    }
                        
                    reader.ReadBytes(temp);
                    //alreadydownload +=(int)count;

                    //fileinfo.Count = alreadydownload;
                    //获取所加载的数据以字节数组的形式
                    //将已接收的文件流存起来
                    recvfilebuffer = recvfilebuffer.Concat(temp).ToArray();
                    //下载完毕
                    if (recvfilebuffer.Length >= fileLength)
                    {
                            //GetFile(diskitem.FullName,fileLength,"下载成功");                        
                            App.downloadfile.RemoveAt(0);             
                    }                                                                                               
                }
                reader.DetachStream();
                  
                   
                  
                byte[] writebuffer = new byte[fileLength];
                writebuffer = GetSendBuffer(writebuffer, recvfilebuffer);
                if (!diskitem.Name.EndsWith(".jpg") && !diskitem.Name.EndsWith(".jpeg") && !diskitem.Name.EndsWith(".png") &&
                   !diskitem.Name.EndsWith(".3gp") && !diskitem.Name.EndsWith(".mp4") && !diskitem.Name.EndsWith(".mp3") &&
                   !diskitem.Name.EndsWith(".wma") && !diskitem.Name.EndsWith(".avi") && !diskitem.Name.EndsWith(".mov") &&
                   !diskitem.Name.EndsWith(".amr"))
                {

                    App.txtstring = Encoding.UTF8.GetString(writebuffer);

                }
                else
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
                                file = await SDfolder.CreateFileAsync(diskitem.Name, CreationCollisionOption.ReplaceExisting);
                                File.WriteAllBytes(file.Path, writebuffer);

                                //Task t = Task.Factory.StartNew( new  Action(async ()=> {                            
                                //    await new MessageDialog("文件下载完成").ShowAsync();
                                //}));

                                flag = 1;
                                break;
                            }
                        }
                        if (flag == 0)
                        {
                            file = await f.CreateFileAsync(diskitem.Name, CreationCollisionOption.ReplaceExisting);
                            File.WriteAllBytes(file.Path, writebuffer);
                            break;
                        }
                        //在download文件夹中保存成功，下载下一个文件。          
                        else if (flag == 1)
                        {
                            //待下载队列不为空，下载
                            if (App.downloadfile.Count > 0)
                            {
                                DownloadNextFile();
                            }
                            break;
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                //await new MessageDialog(e.Message).ShowAsync();
                reader.DetachStream();
                App.downloadfile.RemoveAt(0);
                if (App.downloadfile.Count > 0)
                {
                    DownloadNextFile();
                }
                //stream.Dispose();
                return ;
            }
         
        }
        private static void DownloadNextFile( )
        {
            App.DownloadFile(App.downloadfile[0].FullName, App.socketFile);

            item = new DiskItemModel();
            item.Img = App.downloadfile[0].Img;
            item.FullName = App.downloadfile[0].FullName;
            item.Name = App.downloadfile[0].Name;

            Task t = Task.Factory.StartNew(() =>
            {
                App.ReceiveFile(item, App.socketFile);
            });
          
        }
        private static DiskItemModel item;
     
        private static int SetBUFFERSIZE(int len)
        {
            return ((len / 10) + 1) * 2 * 1024 * 1024;

        }        
        private static void GetFile(string fullName,int length,string status)
        {
            for(int i=0;i<App.historyfile.Count;i++)
            {
                if (App.historyfile[i].FullName.Equals(fullName))
                {
                    App.historyfile[i].Size = length;
                    App.historyfile[i].Status = status;
                    break;
                }
            }
          
        }
    



        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // 当导航堆栈尚未还原时，导航到第一页，
                // 并通过将所需信息作为导航参数传入来配置
                // 参数
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // 确保当前窗口处于活动状态
            Window.Current.Activate();
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
