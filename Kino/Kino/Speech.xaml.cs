using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect; 

namespace Kino
{
    /// <summary>
    /// Interaction logic for Speech.xaml
    /// </summary>
    public partial class Speech : Window
    {
        private Stream AudioStream;

        public Speech()
        {
            InitializeComponent();
        }

        private void Speech_Loaded(object sender, RoutedEventArgs e)
        {
            V_Speech_Chooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser_KinectSensorChanged);

        }
        #region mao
        void kinectSensorChooser_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor old = (KinectSensor)e.OldValue;

            StopKinect(old);

            KinectSensor sensor = (KinectSensor)e.NewValue;

            if (sensor == null)
            {
                return;
            }

            TransformSmoothParameters parameters = new TransformSmoothParameters();
            parameters.Smoothing = 0.7f;
            parameters.Correction = 0.3f;
            parameters.Prediction = 0.4f;
            parameters.JitterRadius = 1.0f;
            parameters.MaxDeviationRadius = 0.5f;

            sensor.SkeletonStream.Enable(parameters);
            //sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            //sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            sensor.DepthStream.Enable(DepthImageFormat.Resolution80x60Fps30);

            sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
            try
            {
                sensor.Start();
            }
            catch (System.IO.IOException)
            {
                //another app is using Kinect
                V_Speech_Chooser.AppConflictOccurred();
            }

        }

        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            sensor_DepthFrameReady(e);
        }

        void audio_StreamReady(AllFramesReadyEventArgs e)
        {
            this.AudioStream = V_Speech_Chooser.Kinect.AudioSource.Start();
        }


        void sensor_DepthFrameReady(AllFramesReadyEventArgs e)
        {
            // if the window is displayed, show the depth buffer image
            if (this.WindowState == WindowState.Normal)
            {
                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                    if (depthFrame == null)
                    {
                        return;
                    }

                }

            }
        }
        #endregion mao


        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    sensor.Stop();
                }
            }
        }

        private void Speech_Closed(object sender, EventArgs e)
        {
            MainWindow VP = new MainWindow();
            VP.Show();
        }


    }
}
