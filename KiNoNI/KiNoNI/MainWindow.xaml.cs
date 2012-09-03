using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CCT.NUI.Core;
using CCT.NUI.Core.OpenNI;
using CCT.NUI.Core.Video;
using CCT.NUI.Visual;
using CCT.NUI.HandTracking;
using CCT.NUI.WPFSamples.PinCode;
using CCT.NUI.KinectSDK;
using CCT.NUI.Core.Clustering;
using System.Diagnostics;
using CCT.NUI.WPFSamples;
namespace KiNoNI
{
    public partial class MainWindow : Window
    {
        private IDataSourceFactory factory;
        private IHandDataSource handDataSource;
        private IClusterDataSource clusterDataSource;
        private IImageDataSource rgbImageDataSource;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Start()
        {
            this.Cursor = Cursors.Wait;
            this.factory = new OpenNIDataSourceFactory("config.xml");
            this.clusterDataSource = this.factory.CreateClusterDataSource(new CCT.NUI.Core.Clustering.ClusterDataSourceSettings { MaximumDepthThreshold = 900 });
            this.handDataSource = new HandDataSource(this.factory.CreateShapeDataSource(this.clusterDataSource, new CCT.NUI.Core.Shape.ShapeDataSourceSettings()));
            this.rgbImageDataSource = this.factory.CreateRGBImageDataSource();
            this.rgbImageDataSource.Start();

            var depthImageSource = this.factory.CreateDepthImageDataSource();
            depthImageSource.NewDataAvailable += new NewDataHandler<ImageSource>(MainWindow_NewDataAvailable);
            depthImageSource.Start();
            handDataSource.Start();
            this.Cursor = Cursors.Arrow;
        }

        void MainWindow_NewDataAvailable(ImageSource data)
        {
            this.videoControl.Dispatcher.Invoke(new Action(() =>
            {
                this.videoControl.ShowImageSource(data);
            }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            new Action(() =>
            {
                this.handDataSource.Stop();
                this.factory.DisposeAll();
            }).BeginInvoke(null, null);
        }
    }

}
