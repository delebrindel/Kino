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
using System.Windows.Shapes;
using NAudio;
using NAudio.Wave;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;


namespace KWpf
{
    /// <summary>
    /// Interaction logic for Piano.xaml
    /// 

    /// </summary>
    public partial class Sound : Window
    {  
        public KinectSensor sensor;
        public const float ClickThreshold = .1f; //.2
        private const float SkeletonMaxX = 0.60f; //.6
        private const float SkeletonMaxY = 0.40f;
        public int KMouseOn = 0;
        public double MaxXCord = SystemParameters.PrimaryScreenWidth;
        public double MaxYCord = SystemParameters.PrimaryScreenHeight;
        public double MinYCord = 0;
        public double MinXCord = 0;


        public Sound()
        {
            InitializeComponent();
            StartSounds();
            Expansor.IsExpanded = true;
            kinectSensorChooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser_KinectSensorChanged);
        }

        public void StartSounds()
        {
            String Path = System.IO.Path.GetDirectoryName(
      System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase); 
            //Clap.Source = new Uri(Path, UriKind.Relative);
            //MessageBox.Show(Clap.Source.ToString());
            MessageBox.Show(Path);
        }


        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            StopSounds();
            Clap.Play();
            MessageBox.Show("No!");
        }

        private void Rectangle_MouseEnter2(object sender, MouseEventArgs e)
        {
            StopSounds();
            EDrum.Play();
        }

        public void StopSounds()
        {
            Clap.Stop();
            EDrum.Stop();
        }

        public void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    sensor.Stop();
                    Expansor.IsExpanded = true;
                }
            }
        }

        void kinectSensorChooser_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor old = (KinectSensor)e.OldValue;

            StopKinect(old);

            sensor = (KinectSensor)e.NewValue;

            if (sensor == null)
            {
                Expansor.IsExpanded = true;
                return;
            }
            else
            {
                Expansor.IsExpanded = false;
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
                kinectSensorChooser.AppConflictOccurred();
            }
        }

        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            sensor_DepthFrameReady(e);
            sensor_SkeletonFrameReady(e);
        }

        void sensor_SkeletonFrameReady(AllFramesReadyEventArgs e)
        {
            kinectSensorChooser.Kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;


            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return;
                }

                Skeleton[] allSkeletons = new Skeleton[skeletonFrameData.SkeletonArrayLength];

                skeletonFrameData.CopySkeletonDataTo(allSkeletons);


                foreach (Skeleton sd in allSkeletons)
                {
                        // the first found/tracked skeleton moves the mouse cursor
                        if (sd.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            // make sure both hands are tracked
                            if (sd.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked &&
                                sd.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                            {

                                // get the left and right hand Joints
                                Joint jointRight = sd.Joints[JointType.HandRight];
                                Joint jointLeft = sd.Joints[JointType.HandLeft];

                                // scale those Joints to the primary screen width and height
                                Joint scaledRight = jointRight.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);
                                Joint scaledLeft = jointLeft.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);

                                CoordLeft.Text = jointLeft.Position.X.ToString();
                                CoordRight.Text = jointRight.Position.X.ToString();
                            }
                            return;
                        }
                }
            }


        }
        private void VPai_Ch_SeatedModeChanged(object sender, RoutedEventArgs e)
        {
            if (null != kinectSensorChooser.Kinect)
            {
                if (this.VPia_Ch_Seated.IsChecked.GetValueOrDefault())
                {
                    kinectSensorChooser.Kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    MessageBox.Show("Hola Kinect, Está Trabajando Sentado");
                }
                else
                {
                    kinectSensorChooser.Kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                    MessageBox.Show("Hola Kinect, Está Trabajando Parado");
                }
            }
        }

        void sensor_DepthFrameReady(AllFramesReadyEventArgs e)
        {
            if (Window.GetWindow(this).WindowState != WindowState.Minimized)
            {
                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                    if (depthFrame == null)
                    {
                        return;
                    }

                    video.Source = depthFrame.ToBitmapSource();
                }
            }
        }

        void StopMouse()
        {
            this.StopKinect(this.sensor);
        }
    }
}
