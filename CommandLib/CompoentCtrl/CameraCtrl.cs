
using System.Windows;
using System.Windows.Controls;

namespace CommandLib
{
    /// <summary>
    /// 摄像头操作
    /// </summary>
    public class CameraCtrl
    {
        public static void OnOpsCamera(object obj, bool isChecked, WebCam webCan)
        {
            if (isChecked)
                Open(webCan);
            else
                Stop(webCan);
        }

        static string CameraIp = "";
        static void Open(WebCam webCan)
        {
            webCan.Start();
            //MJPEGStream mjpegSource = new MJPEGStream();
            //mjpegSource.VideoSource = CameraIp;
            //OpenVideoSource(mjpegSource);
        }

        // 打开视频源
        private void OpenVideoSource(IVideoSource source)
        {
            //this.Cursor = Cursors.WaitCursor;
            //CloseFile();
            //if (detector != null)
            //{
            //    detector.MotionLevelCalculation = true;
            //}

            //Camera camera = new Camera(source, detector);
            //camera.Start();
            //cameraWindow.Camera = camera;
            //statIndex = statReady = 0;
            //timer.Start();

            //this.Cursor = Cursors.Default;
        }

        static void Stop(WebCam webCan)
        {
            webCan.Stop();
        }

        public static void OnCameraRightFunc(object obj)
        {
            MessageBox.Show("OnCameraRight");
        }

        public static void OnCameraLeftFunc(object obj)
        {
            MessageBox.Show("OnCameraLeft");
        }

        public static void OnScreenshot(object obj, Image ImageVideo, ref bool isSaved)
        {
            string str = ImageVideo.Source.ToString();
            Image im = new Image();
            im.Source = ImageVideo.Source;
            if (ImageVideo.Source != null)
            {
                isSaved = true;
            }
        }

        public static void OnVideotape(object obj)
        {
            MessageBox.Show("OnVideotape");
        }

    }
}
