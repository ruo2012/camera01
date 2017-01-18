using CommandLib;
using Microsoft.Practices.Prism.Mvvm;
using System.Windows;
using System.Windows.Controls;

namespace CleaningRobot.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        public MainWindow()
        {
            InitializeComponent();
            statusBar = new System.Windows.Forms.StatusBar();
        }

        //public static WebCam webCam;
        //public static Image ImageVideo;
        public static System.Windows.Forms.StatusBar statusBar;
        public static CameraView.CameraWindow CameraWindow;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //webCam = new WebCam();
            //webCam.InitializeWebCam(ref imgVideo);
            //ImageVideo = imgVideo;
            CameraWindow = cameraWindow;
        }
    }
}
