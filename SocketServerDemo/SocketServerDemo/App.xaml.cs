﻿using SocketServerDemo.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Xml;
using SocketServerDemo.Model;

namespace SocketServerDemo
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static Socket serverSocketCmd = null;
        //发送文件的Socket
        public static Socket serverSocketFile = null;
    
        //储存磁盘的名字
        private static List<DiskFolder> folderlist;
        //Json序列化得到的字符串
        private  static string message=null;

        private static string sendMessage = null;

        //private static Thread threadSendDisk = null;
        public static Dictionary<string, string> dicCmd = new Dictionary<string, string>();
        public static Dictionary<string, string> dicSearch = new Dictionary<string, string>();
        public static Dictionary<string, string> dicOffice = new Dictionary<string, string>();
        public static Dictionary<string, string> dicWeb = new Dictionary<string, string>();

        public static bool IsConnect = false;
        
        //加载命令xml文件
        public static void LoadDict()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;//忽略文档里面的注释          
            XmlReader reader = XmlReader.Create(@"..\..\Common\Dict.xml", settings);
            xmlDoc.Load(reader);         
            XmlNode xn = xmlDoc.SelectSingleNode("Dictionary");
            XmlNodeList xnl = xn.ChildNodes;

            foreach (XmlNode xnf in xnl)
            {
                XmlElement xe = xnf as XmlElement;
                if (xe != null)
                {
                    //App.dicCmd[xe.GetAttribute("Operation")] == null,避免键重复
                    if (xe.Name == "Cmd"&& IsExistence(xe.GetAttribute("Operation"),0)==false)
                    {
                        string s = xe.GetAttribute("Operation");//显示属性值
                        string t = xe.GetAttribute("Command");
                        if(!dicCmd.ContainsKey(s))
                            dicCmd.Add(s, t);
                    }
                    else if(xe.Name=="OfficeProgram"&& IsExistence(xe.GetAttribute("Operation"), 1) == false)
                    {
                        string s = xe.GetAttribute("Operation");//显示属性值
                        string t = xe.GetAttribute("Command");
                        if(!dicOffice.ContainsKey(s))
                            dicOffice.Add(s, t);                       
                    }
                    else if (xe.Name == "Web"&& IsExistence(xe.GetAttribute("Operation"), 2) == false)
                    {
                        string s = xe.GetAttribute("Operation");//显示属性值
                        string t = xe.GetAttribute("Command");
                        if(!dicWeb.ContainsKey(s))
                             dicWeb.Add(s, t);
                    }
                    else if (xe.Name == "Search" && IsExistence(xe.GetAttribute("Operation"), 3) == false)
                    {
                        string s = xe.GetAttribute("Name");//显示属性值
                        string t = xe.GetAttribute("Command");
                        if(!dicSearch.ContainsKey(s))
                            dicSearch.Add(s, t);
                    }
                }
                
            }
        }

        //判断节点是否已存入字典
        public static Boolean IsExistence(string str,int flag)
        {
            if (flag == 0)
            {
                foreach (var key in App.dicCmd.Keys)
                {
                    if (key == str)
                    {
                        return true;
                    }
                }
            }
            if (flag == 1)
            {
                foreach (var key in App.dicOffice.Keys)
                {
                    if (key == str)
                    {
                        return true;
                    }
                }
            }
            if (flag == 2)
            {
                foreach (var key in App.dicWeb.Keys)
                {
                    if (key == str)
                    {
                        return true;
                    }
                }
            }
            if (flag == 3)
            {
                foreach (var key in App.dicSearch.Keys)
                {
                    if (key == str)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //发送本地磁盘
        public static void SendDiskHandle()
        {
            folderlist = new List<DiskFolder>();
            //获取本地磁盘
            foreach (var item in System.IO.DriveInfo.GetDrives())
            {
                //|| System.IO.DriveType.Removable.Equals(item.DriveType)
                //获取U盘名字有问题
                if (System.IO.DriveType.Fixed.Equals(item.DriveType) || System.IO.DriveType.Removable.Equals(item.DriveType))
                {
                    DiskFolder disk = new DiskFolder { Name = item.Name, Lable = item.VolumeLabel, FullName = null, FileTyp="disk"};
                    folderlist.Add(disk);
                }
            }
            //集合序列化为字符串
            message = JsonConvert.SerializeObject(folderlist);
            //发送的字符串
            sendMessage = string.Format("XiYou#{0}", message);
            
      
            //发送
            SendDiskSocket(App.serverSocketCmd);
            //threadSendDisk = new Thread(new ParameterizedThreadStart(Send));
            //if (serverSocketDisk != null)
            //    threadSendDisk.Start(App.serverSocketDisk);
        }

     
        //发送磁盘文件夹
        public static void SendPathHandle(string path,string flag)
        {
            folderlist = new List<DiskFolder>();
            DirectoryInfo info = new DirectoryInfo(path);
            //非磁盘， 非文件夹，非桌面
            //即文件
            if (info.Attributes != System.IO.FileAttributes.Directory&&info.Name.Length!=3&&info.Name!="Desktop")
            {
                Process.Start(path);
            }
            else
            {
             
                FileSystemInfo[] files = info.GetFileSystemInfos();
                foreach (var file in files)
                {
                    if (file.Attributes == FileAttributes.Directory && file.Attributes != FileAttributes.Hidden)
                    {
                        DiskFolder folder = new DiskFolder { FullName = file.FullName, Name = file.Name, Lable = flag, FileTyp = file.Attributes.ToString() };
                        folderlist.Add(folder);
                    }

                }
                if (flag == "PPTSHOW")
                {
                    foreach (var file in files)
                    {
                        if (file.Attributes == FileAttributes.Archive && file.Attributes != FileAttributes.Hidden)
                        {
                            DiskFolder folder = new DiskFolder { FullName = file.FullName, Name = file.Name, Lable = flag, FileTyp = file.Attributes.ToString() };
                            folderlist.Add(folder);
                        }
                    }
                }
                else if(flag=="PPTDO")
                {
                    foreach (var file in files)
                    {
                        if (file.Attributes == FileAttributes.Archive && file.Attributes !=FileAttributes.Hidden&&(file.Extension==".pptx"|| file.Extension == ".ppt"))
                        {
                            DiskFolder folder = new DiskFolder { FullName = file.FullName, Name = file.Name, Lable = flag, FileTyp = file.Attributes.ToString() };
                            folderlist.Add(folder);
                        }
                    }
                }
              
                message = JsonConvert.SerializeObject(folderlist);
                sendMessage = string.Format("XiYou#{0}", message);
               
               
                //发送
                SendDiskSocket(App.serverSocketCmd);
            }
        }

      

        //通过socketcmd发送截图图片
        public static void SendScreen(byte[] buffer,object obj)
        {
            Socket s = obj as Socket;
          
            byte[] filelengthbuffer = null;             
            //文件大小
            string filelength = buffer.Length.ToString();
            filelengthbuffer = new byte[sizeof(Int32)];
            filelengthbuffer = BitConverter.GetBytes(Convert.ToInt32(filelength));
            //Encoding.UTF8.GetBytes(filelength,0,filelength.Length,filelengthbuffer,0);
       
            try
            {
                //发送文件大小
                s.Send(filelengthbuffer, sizeof(Int32), SocketFlags.None);
                Debug.WriteLine("SocketCmd发送截屏大小:" + filelength);
                Thread.Sleep(10);                           
                //发送
                int sendcount = s.Send(buffer, 0, buffer.Length, SocketFlags.None);                    
                //s.Send(filebuffer,(int)handlefileModel.Size,Convert.ToInt32(filelength),SocketFlags.None);
            }
            catch (ArgumentNullException)
            {             
                return;
            }
            catch (SocketException)
            { 
                return;
            }
            catch (ObjectDisposedException)
            {
                return;
            }
        }
        //发送盘符的Socket通道
        private  static void SendDiskSocket(object obj)
        {
            Socket s = obj as Socket;
            //将发送消息转化为字节数组
            byte[] buffer = Encoding.UTF8.GetBytes(sendMessage);

            byte[] bufferLength = new byte[sizeof(Int32)];
            int len = buffer.Length;
            bufferLength = BitConverter.GetBytes(len);
            // Encoding.UTF8.GetBytes(len.ToString(),0,len.ToString().Length,bufferLength,0);
            //Encoding.UTF8.GetBytes(len, 0, len.Length, bufferLength, 0);
            //Encoding.UTF8.GetBytes(sendMessage, 0, sendMessage.Length, buffer, 0);        
            //Encoding.ASCII.GetBytes(sendMessage, 0, sendMessage.Length, buffer, 0);
            //Encoding.ASCII.GetBytes(len, 0, len.Length, bufferLength, 0);
            try
            {        
                s.Send(bufferLength, sizeof(Int32), SocketFlags.None);
                //s.Send(buffer,sendMessage.Length+sizeof(uint),SocketFlags.None);
                Thread.Sleep(10);
                s.Send(buffer, buffer.Length, SocketFlags.None);
                Thread.Sleep(10);
                Debug.WriteLine("SocketCmd发送磁盘信息:" + sendMessage);

            }
            catch (ArgumentException )
            {
                //MessageBox.Show("发送的东西为空");
                // s.Dispose();
                //InfoHandle.threadDisk.Abort();
                return;
            }
            catch (SocketException )
            {
                //MessageBox.Show("访问套接字失败");
                return;
            }
            catch (ObjectDisposedException )
            {
                //MessageBox.Show("Socet已断开");
                return;
            }
        }
        //向客户端发送要下载文件信息
        public static void SendFileSocket(HandleFileModel handlefileModel,object obj)
        {         
            Socket s=obj as Socket;
            byte[] buffer = null;
            byte[] filelengthbuffer=null;
            FileStream fstream;
        
          
            try
            {
                fstream = new FileStream(handlefileModel.Msg, FileMode.Open);
            }
            catch (Exception)
            {
                //MessageBox.Show("文件获取失败");
                return;
            }



            //100M文件
            if (fstream.Length > 100*1024*1024)
            {
                MessageBox.Show("文件过大，无法下载");
                return;
            }
           
            //文件大小
            string filelength = fstream.Length.ToString();
            filelengthbuffer = new byte[sizeof(Int32)];
            filelengthbuffer = BitConverter.GetBytes(Convert.ToInt32(fstream.Length));
            //Encoding.UTF8.GetBytes(filelength,0,filelength.Length,filelengthbuffer,0);

            int len = (int)fstream.Length / (1024 * 1024 * 1);
            int BUFFERSIZE = SetBUFFERSIZE(len);
            //读流的缓冲区
            buffer = new byte[BUFFERSIZE];
            try
            {
                //发送文件大小
                s.Send(filelengthbuffer,sizeof(Int32), SocketFlags.None);
                Debug.WriteLine("SocketFile发送文件大小:"+ filelength);
                Thread.Sleep(10);

             
                byte[] sendfilebuffer;
                //实际读取的字节数,count
                int count = fstream.Read(buffer, 0, BUFFERSIZE);

                //发送的数据
                sendfilebuffer = new byte[count];
                sendfilebuffer = GetSocketBuffer(sendfilebuffer,buffer);
                //Thread.Sleep(1000);
                //每次向BUFFERSIZE大小的缓冲区读字节，读多少发多少，直到文件读完为止
                while (count > 0)
                {
                    //发送
                    s.Send(sendfilebuffer, 0, sendfilebuffer.Length,  SocketFlags.None);
                    Thread.Sleep(10);

                    count = fstream.Read(buffer, 0, BUFFERSIZE); 
                    
                    //发送的数据             
                    sendfilebuffer = new byte[count];
                    sendfilebuffer = GetSocketBuffer(sendfilebuffer, buffer);
                }
                Debug.WriteLine("SocketFile发送文件完毕！");
                fstream.Flush();
                fstream.Dispose();
                fstream.Close();
           
               
         
                //s.Send(filebuffer,(int)handlefileModel.Size,Convert.ToInt32(filelength),SocketFlags.None);
               
            }
            catch (ArgumentNullException )
            {            
                return;
            }
            catch (SocketException )
            {              
                return;
            }
            catch (ObjectDisposedException )
            {              
                return;
            }
          
        }

        public static void ReceiveCmd(object obj)
        {

            Socket s = obj as Socket;
            byte[] buffer = null;
            while (true)
            {
                try
                {
                    buffer = new byte[200];
                    s.Receive(buffer);
                }
                catch (ArgumentNullException)
                {
                    //MessageBox.Show("发送的东西为空");
                    // s.Dispose();
                    //InfoHandle.threadDisk.Abort();
                    return;
                }
                catch (SocketException)
                {
                    //MessageBox.Show("访问套接字失败");
                    return;
                }
                catch (ObjectDisposedException)
                {
                    Debug.WriteLine("连接已断开");
                    //MessageBox.Show("连接已断开");
                    //DisConnect();
                    //Cmd.OnStartUp();
                    return;
                }
                string info = Encoding.UTF8.GetString(buffer);
                info = Msg.HandleRcvMsg(info);
                //考虑是否有#
                try
                {
                    if (info.Contains("XiYou#"))
                    {
                        HandleRcvMsg(ref info);
                        Debug.WriteLine("SocketCmd接收的信息："+info);                
                        Command command = JsonConvert.DeserializeObject<Command>(info);
                        if (command.Flag == "1")
                        {
                            HandleModel handle = JsonConvert.DeserializeObject<HandleModel>(command.Msg);
                            Cmd.Command(handle);
                        }
                        else if (command.Flag == "2")
                        {

                            //json反序列为类
                            DiskFolder folder = JsonConvert.DeserializeObject<DiskFolder>(command.Msg);
                            DiskHandle(folder);
                        }
                        else if (command.Flag == "3")
                        {
                            string[] offset = Regex.Split(command.Msg, ",");
                            if (offset.Length >= 2 && offset.Length < 5)
                            {
                                var sgl1 = new Signal1();
                                sgl1.x = Convert.ToDouble(offset[0]);
                                sgl1.y = Convert.ToDouble(offset[1]);
                                sgl1.mode = Msg.HandleRcvMsg(offset[2]);
                                Signal1.DoMouseEvent(sgl1);
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
        public static  void HandleRcvMsg(ref string info)
        {
            int flag = 0;
            for (int i = 0; i < info.Length; i++)
            {
                if (info[i] == 'X')
                {
                    info = info.Substring(i);
                    flag = 1;
                    break;
                }
            }
            if (flag == 0)
            {
                info = "";
            }
            else
            {
                int count = 0,leftbarcket=0,rightbarcket=0;
                for (int i = 0; i < info.Length; i++)
                {
                    if (info[i]=='{')
                    {
                        count++;
                        leftbarcket++;
                    }
                    if (info[i] == '}')
                    {
                        rightbarcket++;
                        count--;
                        if (leftbarcket == rightbarcket && count == 0)
                        {
                            info = info.Substring(6, i + 1-6);
                            break;
                        }
                    }                                   
                }
            }
         
        }
        //接收文件上传命令
        internal static void ReceiveFileSocket(object obj)
        {

            Socket s = obj as Socket;
            //用于接收文件信息，得到文件长度
            int BUFFERSIZE = 10*1024;
            byte[] buffer = new byte[BUFFERSIZE];
            byte[] recvfilebuffer = new byte[0];
            byte[] temp ;
            int filelength=0;
            string filename="";
            int count = 0;
            HandleFileModel handlefileModel = null;
            while (true)
            {              
                try
                {
                    //请求接收BUFFERSIZE个字节的数据，实际接收count字节
                    count = s.Receive(buffer ,SocketFlags.None);
                    temp = new byte[count];
                    temp = GetSocketBuffer(temp,buffer);
                    //recvfile累加
                    recvfilebuffer = recvfilebuffer.Concat(temp).ToArray();                                               
                }
                catch (ArgumentNullException )
                {
                    //MessageBox.Show("发送的东西为空");
                    // s.Dispose();
                    //InfoHandle.threadDisk.Abort();
                    return;
                }
                catch (SocketException )
                {
                    //MessageBox.Show("访问套接字失败");
                    return;
                }
                catch (ObjectDisposedException )
                {
                    //MessageBox.Show("Socet已断开");
                    return;
                }
                try
                {
                    string message = Encoding.UTF8.GetString(temp);
                    
                    message = Msg.HandleRcvMsg(message);
                    Debug.WriteLine("SocketFile接收的信息"+message);

                    if (message.Contains("Size") && message.Contains("Name"))
                    {
                        string[] info = message.Split('}');
                        message = info[0] + "}";
                      
                        handlefileModel = JsonConvert.DeserializeObject<HandleFileModel>(message);
                        //文件下载时handlefileModel.Size为0，主要用于文件上传
                        filelength = (int)handlefileModel.Size;
                        Debug.WriteLine("SocketFile接收的文件大小" + filelength);
                        filename = handlefileModel.Name;
                        // temp = new byte[10];
                        recvfilebuffer = new byte[0];

                        //根据文件大小，设置缓冲区大小
                        int len = (int)handlefileModel.Size / (1024 * 1024 * 1);
                        BUFFERSIZE = SetBUFFERSIZE(len);
                        buffer = new byte[BUFFERSIZE];
                    }     
                    //如果接收的数据到达文件长度         
                    else if(recvfilebuffer.Length >= filelength)
                    {
                        if (filelength >0)
                        {
                            //temp=temp.Concat(buffer).ToArray();
                            handlefileModel = null;
                            string savepath = "C:\\Users\\" + Environment.UserName + "\\Desktop" + '\\' + filename;

                            //filebuffer.CopyTo(temp,10);
                            File.WriteAllBytes(savepath, recvfilebuffer);
                         
                        }
                       
                        
                    }       
                    /*     
                    else if(handlefileModel!=null)
                    {
                        handlefileModel.Typ = null;
                    }*/
                   
                 
                }
                catch (Exception )
                {
                   // MessageBox.Show("文件传输错误");
                }
                
                if (handlefileModel != null)
                {
                    /*if (handlefileModel.Typ == "FileUpload")
                    {
                        byte[] filebuffer = new byte[handlefileModel.Size];
                        try
                        {
                            InfoHandle.m.rcvMsg = "文件接收中";
                            s.Receive(filebuffer);
                        }
                        catch (ArgumentNullException ex)
                        {
                            //MessageBox.Show("发送的东西为空");
                            // s.Dispose();
                            //InfoHandle.threadDisk.Abort();
                            return;
                        }
                        catch (SocketException ex)
                        {
                            //MessageBox.Show("访问套接字失败");
                            return;
                        }
                        catch (ObjectDisposedException ex)
                        {
                            //MessageBox.Show("Socet已断开");
                            return;
                        }
                       


                    }
                    */
                    if (handlefileModel.Typ == "FileDownload")
                    {

                        SendFileSocket(handlefileModel, App.serverSocketFile);
                    }
                }
            
           
            }

        }

        private static byte[] GetSocketBuffer(byte[] socketbuffer,byte[] buffer)
        {
            for (int i = 0; i < socketbuffer.Length; i++)
            {
                socketbuffer[i] = buffer[i];
            }
            return socketbuffer;
        }
        private static  int SetBUFFERSIZE(int len)
        {
            return ((len / 10) + 1) * 2 * 1024 * 1024;
           
        }
   
        private static void DiskHandle(DiskFolder folder)
        {
            if (folder != null)
            {
                if (folder.Name == "我的电脑" && folder.Lable == "我的电脑" && folder.FullName == "我的电脑")
                {
                    SendDiskHandle();
                }
                else 
                {
                 
                    string path = "";
                    if (folder.FullName == "Desktop")
                    {
                        path = "C:\\Users\\" + Environment.UserName + "\\Desktop";
                    }
                    else
                    {
                        path = folder.FullName;
                    }
                    ////显示文件
                    //if (folder.Operation == "Show")
                    //{
                        if (folder.Lable == "PPTSHOW")
                        {
                            SendPathHandle(path, "PPTSHOW");
                        }
                        else if (folder.Lable == "PPTDO")
                        {
                            SendPathHandle(path, "PPTDO");
                        }
                    //}
                    ////文件重命名
                    //else if (folder.Operation == "ReName")
                    //{
                    //    File.Move(path, "newfolder.txt");
                    //}
                    ////移动文件
                    //else if (folder.Operation == "Move")
                    //{
                    //    File.Move(path, "D://newfolder.txt");
                    //}
                    ////文件删除
                    //else if (folder.Operation == "Delete")
                    //{
                    //    File.Delete(path);
                    //}
                }           
            }          
      
        }      
    }
}
