using System;
using System.Windows;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;
using Nui = Microsoft.Kinect;
using System.Windows.Media.Imaging;
using System.Windows.Ink;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Threading;
using System.IO;
using System.Text;
using System.Windows.Media.Animation;
using System.Media;
using System.Drawing.Imaging;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.Serialization;


namespace KinoControlLib
{

    /// <summary>
    /// Interaction logic for Piano.xaml
    /// </summary>
    public partial class KinoMouse : UserControl
    {
        public KinectSensor sensor;
        public const float ClickThreshold = .1f; //.2
        private const float SkeletonMaxX = 0.60f; //.6
        private const float SkeletonMaxY = 0.40f;
        SoundPlayer player = new SoundPlayer();
        public int on = 0;
        public double MaxXCord=SystemParameters.PrimaryScreenWidth;
        public double MaxYCord=SystemParameters.PrimaryScreenHeight;
        public double MinYCord=0;
        public double MinXCord = 0;
        public KinoMouse()
        {
            //String ruta=Directory.GetCurrentDirectory()+@"\Hand.cur";
            //Cursor miCursor = new Cursor(ruta);
            //Mouse.OverrideCursor=miCursor;
        }
        
        public void StartKinoMouse(){
            InitializeComponent();
            Expansor.IsExpanded = true;
            kinectSensorChooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser_KinectSensorChanged);
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
                            int cursorX, cursorY;

                            // get the left and right hand Joints
                            Joint jointRight = sd.Joints[JointType.HandRight];
                            Joint jointLeft = sd.Joints[JointType.HandLeft];

                            // scale those Joints to the primary screen width and height
                            Joint scaledRight = jointRight.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);
                            Joint scaledLeft = jointLeft.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);

                            // figure out the cursor position based on left/right handedness
                            if (LeftHand.IsChecked.GetValueOrDefault())
                            {
                                cursorX = (int)scaledLeft.Position.X;
                                cursorY = (int)scaledLeft.Position.Y;
                                if (cursorY > MaxYCord)
                                    cursorY = (int)MaxYCord;
                                if (cursorY < MinYCord)
                                    cursorY = (int)MinYCord;
                                if (cursorX > MaxXCord)
                                    cursorX = (int)MaxXCord;
                                if (cursorX < MinXCord)
                                    cursorX = (int)MinXCord;
                            }

                            else
                            {
                                cursorX = (int)scaledRight.Position.X;
                                cursorY = (int)scaledRight.Position.Y;
                                if (cursorY > MaxYCord)
                                    cursorY = (int)MaxYCord;
                                if (cursorY < MinYCord)
                                    cursorY = (int)MinYCord;
                                if (cursorX > MaxXCord)
                                    cursorX = (int)MaxXCord;
                                if (cursorX < MinXCord)
                                    cursorX = (int)MinXCord;
                            }

                            bool leftClick;

                            // figure out whether the mouse button is down based on where the opposite hand is
                            if ((LeftHand.IsChecked.GetValueOrDefault() && jointRight.Position.Y > ClickThreshold) ||
                                    (!LeftHand.IsChecked.GetValueOrDefault() && jointLeft.Position.Y > ClickThreshold))
                                leftClick = true;
                            else
                                leftClick = false;
                            NativeMethods.SendMouseInput(cursorX, cursorY, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, leftClick);

                            return;
                        }
                    }
                }
            }


        }


        private void VPai_Ch_SeatedModeChanged(object sender, RoutedEventArgs e)
        {
            if (null != kinectSensorChooser.Kinect)
            {
                if (this.VPai_Ch_Seated.IsChecked.GetValueOrDefault())
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

        public void load()
        {
            MessageBox.Show(Window.GetWindow(this).ToString());
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
