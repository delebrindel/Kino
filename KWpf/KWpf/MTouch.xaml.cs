using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KWpf
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MTouch : Window
    {
        public KinectSensor sensor;
        private const float SkeletonMaxX = 0.60f; //.6
        private const float SkeletonMaxY = 0.40f;
        public MTouch()
        {
            InitializeComponent();
            Expansor.IsExpanded = true;
            SensorChooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser_KinectSensorChanged);
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
                SensorChooser.AppConflictOccurred();
            }
        }


        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            sensor_DepthFrameReady(e);
            sensor_SkeletonFrameReady(e);
        }



        void sensor_SkeletonFrameReady(AllFramesReadyEventArgs e)
        {
            SensorChooser.Kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;


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
                            getCoords(jointRight, jointLeft);
                            return;
                        }
                    }
                }
            }


        }

        private void getCoords(Joint RightHand, Joint LeftHand)
        {
            L_LHandX.Text = LeftHand.Position.X.ToString();
            L_LHandY.Text = LeftHand.Position.Y.ToString();
            L_LHandZ.Text = LeftHand.Position.Z.ToString();
            L_RHandX.Text = RightHand.Position.X.ToString();
            L_RHandY.Text = RightHand.Position.Y.ToString();
            L_RHandZ.Text = RightHand.Position.Z.ToString();

        }


        private void VPai_Ch_SeatedModeChanged(object sender, RoutedEventArgs e)
        {
            if (null != SensorChooser.Kinect)
            {
                if (this.VPai_Ch_Seated.IsChecked.GetValueOrDefault())
                {
                    SensorChooser.Kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    MessageBox.Show("Hola Kinect, Está Trabajando Sentado");
                }
                else
                {
                    SensorChooser.Kinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
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
    }
}
