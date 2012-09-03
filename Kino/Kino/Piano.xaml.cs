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
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Threading;
using System.IO;
using System.Text;
using System.Windows.Media.Animation;
using System.Media;
using System.Drawing.Imaging;
using System.Drawing;
using System.ComponentModel;


namespace Kino
{
    /// <summary>
    /// Interaction logic for Piano.xaml
    /// </summary>
    public partial class Piano : Window
    {
        private const float ClickThreshold = 6f; //.2
        private const float SkeletonMaxX = 0.60f; //.6
        private const float SkeletonMaxY = 0.40f;
        SoundPlayer player = new SoundPlayer();
        public Piano()
        {
            InitializeComponent();
            Expansor.IsExpanded = true;
            this.Topmost = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            {
                this.Show();
                this.Focus();
            }

            player.LoadCompleted += new AsyncCompletedEventHandler(wavPlayer_LoadCompleted);
            player.LoadAsync();
        }

        private void wavPlayer_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ((System.Media.SoundPlayer)sender).Play();
        }

        private void ButtonHandler(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            String nombreboton = b.Name;
            switch (nombreboton)
            {
                case "VPi_B_C": player.SoundLocation = @"..\..\Sounds\DNotes\C.wav";
                                player.Play();
                                break;
                case "VPi_B_CS": player.SoundLocation = @"..\..\Sounds\DNotes\CS.wav";
                                player.Play();
                                break;
                case "VPi_B_D": player.SoundLocation = @"..\..\Sounds\DNotes\D.wav";
                                player.Play();
                                break;
                case "VPi_B_DS": player.SoundLocation = @"..\..\Sounds\DNotes\DS.wav";
                                player.Play();
                                break;
                case "VPi_B_E": player.SoundLocation = @"..\..\Sounds\DNotes\E.wav";
                                player.Play();
                                break;
                case "VPi_B_F": player.SoundLocation = @"..\..\Sounds\DNotes\F.wav";
                                player.Play();
                                break;
                case "VPi_B_FS": player.SoundLocation = @"..\..\Sounds\DNotes\FS.wav";
                                player.Play();
                                break;
                case "VPi_B_G": player.SoundLocation = @"..\..\Sounds\DNotes\G.wav";
                                player.Play();
                                break;
                case "VPi_B_GS": player.SoundLocation = @"..\..\Sounds\DNotes\GS.wav";
                                player.Play();
                                break;
                case "VPi_B_A": player.SoundLocation = @"..\..\Sounds\DNotes\A.wav";
                                player.Play();
                                break;
                case "VPi_B_AS": player.SoundLocation = @"..\..\Sounds\DNotes\AS.wav";
                                player.Play();
                                break;
                case "VPi_B_B": player.SoundLocation = @"..\..\Sounds\DNotes\B.wav";
                                player.Play();
                                break;
            }

        }

        private void Piano_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser_KinectSensorChanged);
        }

        private void StopKinect(KinectSensor sensor)
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

            KinectSensor sensor = (KinectSensor)e.NewValue;

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
                                if (cursorY > 500)
                                    cursorY = 500;
                                if (cursorY < 250)
                                    cursorY = 250;
                            }

                            else
                            {
                                cursorX = (int)scaledRight.Position.X;
                                cursorY = (int)scaledRight.Position.Y;
                                if (cursorY > 500)
                                    cursorY = 500;
                                if (cursorY < 250)
                                    cursorY = 250;
                            }
                            CoordY.Text = cursorY.ToString();

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

                    video.Source = depthFrame.ToBitmapSource();
                }

            }
        }



        private void WindowClosed(object sender, EventArgs e)
        {

            if (null != kinectSensorChooser.Kinect)
            {
                kinectSensorChooser.Kinect.Stop();
            }
            MainWindow VP = new MainWindow();
            VP.Show();
        }
    }
}
