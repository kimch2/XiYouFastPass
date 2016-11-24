using Newtonsoft.Json;
using SocketClientDemo.Common;
using SocketClientDemo.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace SocketClientDemo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VoiceDemo : Page
    {
        private string info;

        public VoiceDemo()
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
            voice.IsEnabled = true;
            Return();
         }
        private void Return()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (webView.Visibility == Visibility.Collapsed)
            {
                if (rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                }
            }
            else if (webView.Visibility == Visibility.Visible)
            {
                if (webView.CanGoBack)
                {
                    webView.GoBack();
                }
                else
                {
                    webView.Visibility = Visibility.Collapsed;
                    voice.IsEnabled = true;
                }
             
            }
            voice.IsEnabled = true;
        }
        private void img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Return();
        }

        private async void Voice(object sender, RoutedEventArgs e)
        {
            voice.IsEnabled = false;
            using (SpeechRecognizer speechRecognizer = new SpeechRecognizer())
            {
                try
                {
                    //编译所有语法协定
                    SpeechRecognitionCompilationResult compilationResult = await speechRecognizer.CompileConstraintsAsync();
                    if (compilationResult.Status == SpeechRecognitionResultStatus.Success)
                    {
                        //开始语音识别
                        var recogResult = await speechRecognizer.RecognizeWithUIAsync();

                        if (recogResult.Status == SpeechRecognitionResultStatus.Success)
                        {
                            info = recogResult.Text;
                            HandleModel handle = new HandleModel() ;
                            handle=StringHandle.MessageHandle(handle, info);
                            string msg = JsonConvert.SerializeObject(handle);

                            Command command = new Command { Flag = "1", Msg = msg };
                            if (handle.Typ!="")
                            {
                                string sendMsg = JsonConvert.SerializeObject(command);
                                Debug.WriteLine("语音发送的命令:" + sendMsg);
                                TextToSpeech("正在为你" + info);
                                App.SendData(sendMsg, App.socketCmd);
                                voice.IsEnabled = true;
                            }
                            else
                            {
                                handle.Typ = "Search";
                                //handle.Msg = speechContent.Text;

                                string uriMsg = string.Format("{0}{1}", App.Search["百度"], System.Net.WebUtility.UrlEncode(info));
                                Uri uri = new Uri(uriMsg);
                                handle.Msg = uriMsg;
                                string msg1 = JsonConvert.SerializeObject(handle);
                                command.Msg = msg1;
                                string sendMsg = JsonConvert.SerializeObject(command);
                                Debug.WriteLine("语音发送的命令:" + sendMsg);
                                webView.Visibility = Visibility.Visible;
                                webView.Navigate(uri);
                                TextToSpeech("指令无效即将使用百度搜索");

                                App.SendData(sendMsg, App.socketCmd);
                                voice.IsEnabled = true;

                            }

                        }
                        //语音识别失败
                        if (recogResult.Status != 0)
                        {
                            voice.IsEnabled = true;
                        }
                     
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("语音异常:"+ex.Message);
                    //await new MessageDialog("语音异常:" + ex.Message).ShowAsync();
                }
            }
        }


        public async   void TextToSpeech(string  str)
        {
            SpeechSynthesizer synthesizer = new SpeechSynthesizer();
            SpeechSynthesisStream stream = await synthesizer.SynthesizeTextToStreamAsync(str);
            media1.SetSource(stream, stream.ContentType);
                        
        }

       
    }
}
