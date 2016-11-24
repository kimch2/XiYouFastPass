using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.System.Display;
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
using ZXing;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace SocketClientDemo.ViewPage
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ScanScan : Page
    {
        //捕获照片，音频，视频
        private MediaCapture _mediaCapture;
        //全球唯一标识的一个东西
        private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");
        //激活显示请求
        private readonly DisplayRequest _displayRequest = new DisplayRequest();
        //获取手机方向,相机初始化时为横屏,所以视频流也是横屏
        private DisplayOrientations _displayOrientation = DisplayOrientations.Portrait;
        //是否相机初始化成功
        private bool _isInitialized = false;

        //是否视频预览显示到界面上面
        private bool _isPreviewing = false;
        //是否镜像预览，比如前摄像头
        private bool _mirroringPreview = false;
        //是否存在外部设备
        private bool _externalCamera = false;

        //二维码读取
        private ZXing.BarcodeReader barcodeReader;
        //正则表达式，匹配数字
        private readonly static string pattern = @"^\d*$";
        public ScanScan()
        {
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
            await InitializeCameraAsync();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        private async Task InitializeCameraAsync()
        {
            Debug.WriteLine("InitializeCameraAsync");
            //获取相机位置
            var cameraDevice = await FindCamerDeviceByPanel(Windows.Devices.Enumeration.Panel.Back);

            //如果没有摄像设备，则返回并提示没有可用装备
            if (cameraDevice == null)
            {
                Debug.WriteLine("No camera device found!");
                return;
            }
            //初始化视频捕捉器
            if (_mediaCapture == null)
            {
                _mediaCapture = new MediaCapture();
            }
            //在捕获期间发生错误时发生
            _mediaCapture.Failed += _mediaCapture_Failed;
            //获取返回的设备，如果是手机成功返回背部摄像头，如果是电脑就是前面
            var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

            try
            {
                //初始化MediaCapture对象
                await _mediaCapture.InitializeAsync(settings);
                _isInitialized = true;
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("The app was denied access to the camera");
            }
            catch (Exception )
            {

            }
            //如果捕获设备初始化成功
            if (_isInitialized)
            {
                //如果摄像设备位置未知
                if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
                {
                    //如果存在外部设备
                    _externalCamera = true;
                }
                else
                {
                    //不存在外部设备
                    _externalCamera = false;

                    //判断是否当前仅存在前摄像设备
                    _mirroringPreview = cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front;
                }
                //当前摄像设备为前摄像设备
                if (_mirroringPreview)
                {
                    await new MessageDialog("请使用后置摄像机").ShowAsync();
                    return;
                }
                //开始预览
                await StartPreviewAsync();
            }
        }
        /// <summary>
        /// 开始预览
        /// </summary>
        /// <returns></returns>
        private async Task StartPreviewAsync()
        {
            Debug.WriteLine("StartPreviewAsync");

            //激活显示请求
            _displayRequest.RequestActive();
            captureElement.Source = _mediaCapture;
            captureElement.FlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            //开始预览
            try
            {
                await _mediaCapture.StartPreviewAsync();
                //捕捉的内容开始显示在屏幕上了
                _isPreviewing = true;
                //关闭闪光灯，如果不关闭闪关灯，当开始拍照时候就会开启闪光灯
                _mediaCapture.VideoDeviceController.FlashControl.AssistantLightEnabled = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when starting the preview: {0}", ex.ToString());
            }
            //是否开始捕捉内容了
            if (_isPreviewing)
            {
                await SetPreviewRotationAsync();
            }
            tip.Begin();
            PhotoHandle();
        }

        //用于处理所预览的图片
        private bool _cancel = false;
        /// <summary>
        /// 处理所预览的图片
        /// </summary>
        private async void PhotoHandle()
        {
            var focusSettings = new FocusSettings
            {
                Mode = FocusMode.Auto,
                AutoFocusRange = AutoFocusRange.Macro,
                Distance = ManualFocusDistance.Nearest
            };
            Result _result = null;
            try
            {
                while (_result == null && _cancel != true)
                {
                    
                    //对焦
                    var autoFocusSupported = _mediaCapture.VideoDeviceController.FocusControl.SupportedFocusModes.Contains(FocusMode.Auto);
                    if (autoFocusSupported)
                    {
                        _mediaCapture.VideoDeviceController.FocusControl.Configure(focusSettings);
                        //await _mediaCapture.VideoDeviceController.FocusControl.FocusAsync();
                    }
                    var photoStorageFile = await KnownFolders.PicturesLibrary.CreateFileAsync("scan.jpg", CreationCollisionOption.ReplaceExisting);
                    await _mediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), photoStorageFile);

                    //获取文件的流
                    var stream = await photoStorageFile.OpenReadAsync();
                    var writeableBmp = new WriteableBitmap(1, 1);
                    writeableBmp.SetSource(stream);

                    writeableBmp = new WriteableBitmap(writeableBmp.PixelWidth, writeableBmp.PixelHeight);
                    stream.Seek(0);
                    writeableBmp.SetSource(stream);

                    //识别扫描图片
                    status.Text = "开始扫描";
                    _result = await ScanQRCodeAsync(writeableBmp);

                    await photoStorageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    //如果识别成功
                    if (_result != null)
                    {
                        status.Text = "识别成功:" + _result.Text+"开始连接";
                        await StopPreviewAsync();
                        tip.Stop();
                        string result = _result.Text;
                        //二维码解码为纯数字
                        if (IsInteger(result))
                        {
                            string[] info = new string[2];
                            info[0] = "Barcode";
                            info[1] = result;
                            SocketInit(_result.ToString(), "10240");

                            //break;
                            // await new MessageDialog(info[1]).ShowAsync();
                            _result = null;
                        }
                        //如果为网页链接，跳转到网页中
                        else
                        {
                            //await new MessageDialog(_result.ToString()).ShowAsync();
                          
                            SocketInit(_result.ToString(), "10240");

                            //break;
                           // _result = null;
                           
                        }

                    }
                    if (_result == null)
                    {
                        status.Text = "扫码失败，请重新扫描!!!";
                    }
                    if (App.IsConnect == true)
                    {
                        status.Text = "连接成功";
                        break;
                    }


                }
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// 是否为纯数字
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool IsInteger(string result)
        {
            return Regex.IsMatch(result, pattern);
        }
        /// <summary>
        /// 识别图片
        /// </summary>
        /// <param name="writeableBmp"></param>
        /// <returns></returns>
        private async Task<Result> ScanQRCodeAsync(WriteableBitmap writeableBmp)
        {
            Result res = null;
            var options = new ZXing.Common.DecodingOptions() { TryHarder = true };
            this.barcodeReader = new BarcodeReader { Options = options, AutoRotate = true };

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //二维码解码
                res = barcodeReader.Decode(writeableBmp);
            });
            return res;
        }

        /// <summary>
        /// 设置视频预览方向
        /// </summary>
        /// <returns></returns>
        private async Task SetPreviewRotationAsync()
        {
            //如果外部设备存在
            if (_externalCamera)
                return;
            //获取手机当前方向
            int rotationDegrees = ConvertDisplayOrientationToDegrees(_displayOrientation);
            //如果是前置摄像头
            if (_mirroringPreview)
            {
                //如果镜像预览，则对称处理
                rotationDegrees = (rotationDegrees + 270) % 360;

            }
            var props = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            props.Properties.Add(RotationKey, rotationDegrees);
            await _mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
        }
        /// <summary>
        /// 获取手机当前方向
        /// </summary>
        /// <param name="displayOrientations"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        private int ConvertDisplayOrientationToDegrees(DisplayOrientations orientation)
        {
            switch (orientation)
            {
                case DisplayOrientations.Portrait:
                    return 90;
                case DisplayOrientations.LandscapeFlipped:
                    return 180;
                case DisplayOrientations.PortraitFlipped:
                    return 270;
                case DisplayOrientations.Landscape:

                default:
                    return 0;
            }
        }

        private async void _mediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            Debug.WriteLine("MediaCapture_Failed: (0x{0:X}) {1}", errorEventArgs.Code, errorEventArgs.Message);
            await CleanupCameraAsync();
            tip.Stop();
        }
        /// <summary>
        /// 清除相机设置
        /// </summary>
        /// <returns></returns>
        private async Task CleanupCameraAsync()
        {
            //是否相机初始化
            if (_isInitialized)
            {
                //是否视频预览显示到界面上面了
                if (_isPreviewing)
                {
                    //停止预览
                    await StopPreviewAsync();
                }
                _isInitialized = false;
            }
            //捕获照片，视频
            if (_mediaCapture != null)
            {
                _mediaCapture.Failed -= _mediaCapture_Failed;
                _mediaCapture.Dispose();
                _mediaCapture = null;
            }
        }
        /// <summary>
        /// 停止预览
        /// </summary>
        /// <returns></returns>
        private async Task StopPreviewAsync()
        {
            try
            {
                _isPreviewing = false;
                //捕获设备，图片，停止预览
                await _mediaCapture.StopPreviewAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when stopping the preview:{0}", ex.ToString());
            }
            //异步操作
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                captureElement.Source = null;
                _displayRequest.RequestRelease();
            });
        }

        /// <summary>
        /// 获取相机装置
        /// </summary>
        /// <param name="back"></param>
        /// <returns></returns>
        private async Task<DeviceInformation> FindCamerDeviceByPanel(Windows.Devices.Enumeration.Panel back)
        {
            //获取所有视频可以捕获的设备
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            //得到计算机背部的设备
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == back);

            //如果没有计算机背部的设备，则返回第一个发现的设备，比如外部摄像机，或者其他
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Return();
        }
        private   void Return()
        {
            Frame root = (Window.Current.Content) as Frame;
            if (root.CanGoBack)
            {
                root.GoBack();
            }
        }
        private async void SocketInit(string ip, string port)
        {
            App.socketCmd = new StreamSocket();          
            App.socketFile = new StreamSocket();         
            try
            {
                HostName h = new HostName(ip);
                await App.socketCmd.ConnectAsync(h, port);           
                await App.socketFile.ConnectAsync(h, (Convert.ToInt32(port) + 1).ToString());               
                Return();
               
                //await new MessageDialog("连接成功").ShowAsync();

                App.IsConnect = true;
                App.ip = ip;
                App.port = port;
                
                //App._navigateAboutEvent.Invoke(this, new EventArgs());
                //frame.Navigate(typeof(MainViewModel));*/

            }
            catch (Exception )
            {
                await new MessageDialog("建立连接失败").ShowAsync();
                //btn.IsEnabled = true;
            }
        }
    }
}
