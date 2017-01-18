using CommandLib;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;
using System;
using WIFIRobotCMDEngineV2;
using System.IO.Ports;
using CleaningRobot.Views;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CleaningRobot.UserControls;
using Tiger.Video.VFW;
using System.Threading.Tasks;
using NativeWifi;

namespace CleaningRobot.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// 开启或关闭摄像头
        /// </summary>
        public bool CameraIsChecked
        {
            get { return cameraIsChecked; }
            set { SetProperty(ref cameraIsChecked, value); }
        }
        private bool cameraIsChecked;

        #region 命令
        public ICommand OpsCameraCommand { get; private set; }//操作 开启、关闭摄像头
        public ICommand ScreenshotCommand { get; private set; }//拍照
        public ICommand VideotapeCommand { get; private set; }//录像
        public ICommand UpCommand { get; private set; }//运动方向控制  上
        public ICommand DownCommand { get; private set; }//运动方向控制  下
        public ICommand LeftCommand { get; private set; }//运动方向控制  左
        public ICommand RightCommand { get; private set; }//运动方向控制  右
        public ICommand TurboCommand { get; private set; }//运动方向控制  加速
        public ICommand DecelerateCommand { get; private set; }//运动方向控制  减速

        public ICommand WiperCommand { get; private set; }//雨刮器开关
        public ICommand WiperTurbeCommand { get; private set; }//雨刮 加速
        public ICommand WiperDecelerateCommand { get; private set; }//雨刮 减速

        public ICommand SprayCommand { get; private set; }//喷水开关
        public ICommand SprayTurbeCommand { get; private set; }//喷水 加速
        public ICommand SprayDecelerateCommand { get; private set; }//喷水 减速

        public ICommand FanCommand { get; private set; }//风机开关
        public ICommand FanTurbeCommand { get; private set; }//风机 加速
        public ICommand FanDecelerateCommand { get; private set; }//风机 减速

        public ICommand CameraLeftCommand { get; private set; }//摄像头 左偏
        public ICommand CameraRightCommand { get; private set; }//摄像头 右偏 
        #endregion

        private string wifiName;
        public string WifiName
        {
            get { return wifiName; }
            set { SetProperty(ref wifiName, value); }
        }

        //方向状态
        public bool Send_status = true;

        public WifiRobotCMDEngine RobotEngine;//实例化引擎
        public WifiRobotCMDEngineV2 RobotEngine2;//实例化引擎
        static IPAddress ips;
        static IPEndPoint ipe;
        static Socket socket = null;

        private int controlType = 3;
        private SerialPort comm = new SerialPort();
        string CameraIp = "";

        static string ImagePath = System.Windows.Forms.Application.StartupPath + "\\Snapshot\\";
        static string VideoPath = System.Windows.Forms.Application.StartupPath + "\\Video\\";

        private DirectionCtrl directionCtrl = new DirectionCtrl();


        private IMotionDetector detector = null;
        private AVIWriter writer = null;
        private System.Timers.Timer timer;
        private const int statLength = 15;
        private int statIndex = 0, statReady = 0;
        private int[] statCount = new int[statLength];

        public double videoRecRate;

        public MainWindowViewModel()
        {
            #region 命令
            this.OpsCameraCommand = new DelegateCommand<object>(OnOpsCamera);
            this.ScreenshotCommand = new DelegateCommand<object>(OnScreenshot);
            this.VideotapeCommand = new DelegateCommand<object>(OnVideotape);
            this.UpCommand = new DelegateCommand<object>(OnUp);
            this.DownCommand = new DelegateCommand<object>(OnDown);
            this.LeftCommand = new DelegateCommand<object>(OnLeft);
            this.RightCommand = new DelegateCommand<object>(OnRight);
            this.TurboCommand = new DelegateCommand<object>(OnTurbe);
            this.DecelerateCommand = new DelegateCommand<object>(OnDecelerate);

            this.CameraLeftCommand = new DelegateCommand<object>(OnCameraLeftFunc);
            this.CameraRightCommand = new DelegateCommand<object>(OnCameraRightFunc);

            this.WiperCommand = new DelegateCommand<object>(OnWiperFunc);
            this.WiperTurbeCommand = new DelegateCommand<object>(OnWiperTurbeFunc);
            this.WiperDecelerateCommand = new DelegateCommand<object>(OnWiperDecelerateFunc);

            this.SprayCommand = new DelegateCommand<object>(OnSprayFunc);
            this.SprayTurbeCommand = new DelegateCommand<object>(OnSprayTurbeFunc);
            this.SprayDecelerateCommand = new DelegateCommand<object>(OnSprayDecelerateFunc);

            this.FanCommand = new DelegateCommand<object>(OnFanFunc);
            this.FanTurbeCommand = new DelegateCommand<object>(OnFanTurboFunc);
            this.FanDecelerateCommand = new DelegateCommand<object>(OnFanDecelerateFunc);
            #endregion

            RobotEngine2 = new WifiRobotCMDEngineV2((Object)MainWindow.statusBar);
            directionCtrl.Init();
            StartWifi();

            timer = new System.Timers.Timer();
            timer.Interval = 1000D;
            timer.SynchronizingObject = MainWindow.CameraWindow;
            this.timer.Elapsed += Timer_Elapsed;
            CameraIp = Utilities.ReadIni("VideoUrl", "videoUrl", "");
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Camera camera = MainWindow.CameraWindow.Camera;
            if (camera != null)
            {
                statCount[statIndex] = camera.FramesReceived;
                if (++statIndex >= statLength)
                {
                    statIndex = 0;
                }
                if (statReady < statLength)
                {
                    statReady++;
                }

                float fps = 0;
                for (int i = 0; i < statReady; i++)
                {
                    fps += statCount[i];
                }
                fps /= statReady;
                statCount[statIndex] = 0;
                videoRecRate = (double)fps;
            }
        }

        private void StartWifi()
        {
            WlanClient client = new WlanClient();
            try
            {
                if (client.Interfaces == null)
                {
                    WifiName = "wifi 未连接";
                }
                else
                {
                    WlanClient.WlanInterface wlan = client.Interfaces[0];

                    string wifi = wlan.CurrentConnection.profileName;
                    Wlan.WlanInterfaceState wifistate = wlan.CurrentConnection.isState;
                    if (Wlan.WlanInterfaceState.Connected == wifistate)
                    {
                        WifiName = wifi;//wifi成功连接

                        string ControlIp = Utilities.ReadIni("ControlUrl", "controlUrl", "");
                        string Port = Utilities.ReadIni("Port", "port", "");
                        controlType = InitWIFISocket(ControlIp, Port) ? 0 : 2;
                        if (0 == controlType)
                        {
                            InitHeartPackage();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        //心跳包
        private void InitHeartPackage()
        {
            Thread HThread = new Thread(() =>
            {
                while (true)
                {
                    RobotEngine2.SendHeartCMD(controlType, comm);
                    Thread.Sleep(10000);
                }
            });
            HThread.IsBackground = true;
            HThread.Start();
        }

        private bool InitWIFISocket(String controlIp, String port)
        {
            try
            {
                ips = IPAddress.Parse(controlIp.ToString());
                ipe = new IPEndPoint(ips, Convert.ToInt32(port.ToString()));
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                RobotEngine2.SOCKET = socket;
                RobotEngine2.IPE = ipe;
                return RobotEngine2.SocketConnect();
            }
            catch (Exception e)
            {
                MessageBox.Show("WIFI初始化失败：" + e.Message, "WIFI初始化失败提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private void OnFanDecelerateFunc(object obj)
        {
            FanCtrl.OnFanDecelerateFunc(obj);
        }

        private void OnFanTurboFunc(object obj)
        {
            FanCtrl.OnFanTurboFunc(obj);
        }

        private void OnFanFunc(object obj)
        {
            FanCtrl.OnFanFunc(obj);
        }

        private void OnSprayDecelerateFunc(object obj)
        {
            SprayCtrl.OnSprayDecelerateFunc(obj);
        }

        private void OnSprayTurbeFunc(object obj)
        {
            SprayCtrl.OnSprayTurbeFunc(obj);
        }

        private void OnSprayFunc(object obj)
        {
            SprayCtrl.OnSprayFunc(obj);
        }

        private void OnWiperDecelerateFunc(object obj)
        {
            WiperCtrl.OnWiperDecelerateFunc(obj);
        }

        private void OnWiperTurbeFunc(object obj)
        {
            WiperCtrl.OnWiperTurbeFunc(obj);
        }

        private void OnWiperFunc(object obj)
        {
            WiperCtrl.OnWiperFunc(obj);
        }

        private void OnCameraRightFunc(object obj)
        {
            CameraCtrl.OnCameraRightFunc(obj);
        }

        private void OnCameraLeftFunc(object obj)
        {
            CameraCtrl.OnCameraLeftFunc(obj);
        }


        /// <summary>
        /// 开关摄像头
        /// </summary>
        /// <param name="obj"></param>
        private void OnOpsCamera(object obj)
        {
            WlanClient client = new WlanClient();
            try
            {
                if (cameraIsChecked)
                {
                    WlanClient.WlanInterface wlan = client.Interfaces[0];
                    if (Wlan.WlanInterfaceState.Connected != wlan.CurrentConnection.isState)
                    {
                        MessageBox.Show("请连接 wifi");
                    }
                    else
                    {
                        //开启摄像头
                        //MjpgFlag = true;//已开启摄像头，等待关闭
                        MJPEGStream mjpegSource = new MJPEGStream();
                        mjpegSource.VideoSource = CameraIp;

                        //打开视频源
                        //MainWindow.CameraWindow.Camera;

                        this.CloseFile();
                        if (detector != null)
                        {
                            detector.MotionLevelCalculation = true;
                        }

                        IVideoSource source = mjpegSource;
                        Camera camera = new Camera(source, detector);
                        camera.Start();
                        MainWindow.CameraWindow.Camera = camera;
                        statIndex = statReady = 0;

                        timer.Start();
                    }
                }
                //关闭摄像头
                else
                {
                    //Task task = Task.Run(() =>
                    //{
                    //    CloseFile();
                    //});
                    //task.Start();
                    Thread thread = new Thread(() =>
                    {
                        CloseFile();
                    });
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("wifi连接出错：【{0}】", ex.Message));
            }
            //CameraCtrl.OnOpsCamera(obj, CameraIsChecked, CameraControl.webCam);
            //if (!CameraIsChecked)
            //{
            //    CameraControl.ImageVideo.Source = new BitmapImage(new Uri("../../Images/000.png", UriKind.Relative));
            //}
        }

        /// <summary>
        /// 开启摄像头前，先关闭资源
        /// </summary>
        void CloseFile()
        {
            Camera camera = MainWindow.CameraWindow.Camera;
            if (camera != null)
            {
                MainWindow.CameraWindow.Camera = null;
                camera.SignalToStop();
                camera.WaitForStop();
                camera = null;

                if (detector != null)
                {
                    detector.Reset();
                }
            }
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
        }



        /// <summary>
        /// 拍照 
        /// 截屏
        /// </summary>
        /// <param name="obj"></param>
        private void OnScreenshot(object obj)
        {
            Func<string> createPictureFile =
                () =>
                {
                    DateTime date = DateTime.Now;
                    String fileName = String.Format("{0}-{1}-{2} {3}-{4}-{5}.bmp",
                        date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                    return fileName;
                };

            RobotEngine2.TakePhoto(
                MainWindow.CameraWindow.Camera.LastFrame,
                ImagePath, createPictureFile());

            //try
            //{
            //    bool isSaved = false;
            //    CameraCtrl.OnScreenshot(obj, CameraControl.ImageVideo, ref isSaved);
            //    if (isSaved)
            //    {
            //        BitmapSource imgSource = (BitmapSource)CameraControl.ImageVideo.Source;
            //        SaveFileDialog sf = new SaveFileDialog();
            //        sf.DefaultExt = ".png";
            //        sf.Filter = "png (.png)|*.png";
            //        if (DialogResult.OK == sf.ShowDialog())
            //        {
            //            PngBitmapEncoder pngE = new PngBitmapEncoder();
            //            pngE.Frames.Add(BitmapFrame.Create(imgSource));
            //            using (Stream stream = File.Create(sf.FileName))
            //            {
            //                pngE.Save(stream);
            //            }
            //        }
            //        //System.Windows.MessageBox.Show("拍照成功");
            //    }
            //    else
            //        System.Windows.MessageBox.Show("拍照失败");
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.MessageBox.Show(ex.Message);
            //}
        }


        static bool saveVideoMonitor = false;
        private void OnVideotape(object obj)
        {            //bool saveOnMotion = false;
            CameraCtrl.OnVideotape(obj);

            Func<string> createVideoFile = () =>
            {
                DateTime date = DateTime.Now;
                String fileName = String.Format("{0}-{1}-{2} {3}-{4}-{5}.avi",
                    date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                return fileName;
            };

            //是否开始录像
            if (!saveVideoMonitor)
            {
                Camera camera = MainWindow.CameraWindow.Camera;
                AviFile.AviManager aviManager = new AviFile.AviManager(VideoPath + createVideoFile(), false);
                double rate = videoRecRate == 0 ? 5.0 : videoRecRate;
                camera.Lock();
                AviFile.VideoStream aviStream = aviManager.AddVideoStream(false, rate, camera.LastFrame);
                camera.Unlock();

                Thread videoRecordThread;
                Action startVideoRecoder = () =>
                {
                    videoRecordThread = new Thread(() =>
                    {
                        Camera _camera = MainWindow.CameraWindow.Camera;
                        while (true)
                        {
                            try
                            {
                                _camera.Lock();
                                aviStream.AddFrame(MainWindow.CameraWindow.Camera.LastFrame);
                                _camera.Unlock();
                                Thread.Sleep(500);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        if (aviManager != null)
                        {
                            aviManager.Close();
                        }
                        videoRecordThread.Abort();
                    });
                    videoRecordThread.IsBackground = true;
                    videoRecordThread.Start();
                };

                saveVideoMonitor = true;
            }

            /*
             录像功能
             */
            //if (!saveOnMotion)
            //{

            //    if (dlg.ShowDialog() == DialogResult.OK)
            //    {
            //        Camera camera = cameraWindow.Camera;
            //        aviManager = new AviManager(VideoPath + CreateVideoFile(), false);
            //        double Rate = dlg.Rate == 0.0 ? videoRecRate : dlg.Rate;
            //        camera.Lock();
            //        aviStream = aviManager.AddVideoStream(false, Rate, camera.LastFrame);
            //        camera.Unlock();

            //        StartVideoRecorder();
            //        saveOnMotion = true;

            //    }
            //}
            //else
            //{
            //    saveOnMotion = false;
            //}
        }

        void OnUp(object obj)
        {
            directionCtrl.OnUp(
                obj,
                RobotEngine2,
                ref Send_status,
                controlType,
                comm);
        }

        void OnDown(object obj)
        {
            directionCtrl.OnDown(
                obj,
                RobotEngine2,
                ref Send_status,
                controlType,
                comm);
        }

        void OnLeft(object obj)
        {
            directionCtrl.OnLeft(
                obj,
                RobotEngine2,
                ref Send_status,
                controlType,
                comm);
        }

        void OnRight(object obj)
        {
            directionCtrl.OnRight(
                obj,
                RobotEngine2,
                ref Send_status,
                controlType,
                comm);
        }

        private void OnDecelerate(object obj)
        {
            directionCtrl.OnDecelerate(obj);
        }

        private void OnTurbe(object obj)
        {
            directionCtrl.OnTurbe(obj);
        }
    }
}
