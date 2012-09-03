#region Using
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
using System.Media;
#endregion Using

namespace Kino
{   /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "In a full-fledged application, the SpeechRecognitionEngine object should be properly disposed. For the sake of simplicity, we're omitting that code in this sample.")]
    public partial class GesturesWindow : Window
    {
        private const float ClickThreshold = 0.1f; //.2
        private const float SkeletonMaxX = 0.60f; //.6
        private const float SkeletonMaxY = 0.40f;
        DrawingAttributes inkAttributes = new DrawingAttributes();
        SoundPlayer player = new SoundPlayer();


        public GesturesWindow()
        {
            InitializeComponent();
            this.Topmost = true;
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                this.Focus();
            };
        }




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser_KinectSensorChanged);
            if (kinectSensorChooser.Kinect == null)
                VG_Ex_Sensor.IsExpanded = true;
        }

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

        void kinectSensorChooser_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VG_Ex_Sensor.IsExpanded = true;
            KinectSensor old = (KinectSensor)e.OldValue;

            StopKinect(old);

            KinectSensor sensor = (KinectSensor)e.NewValue;

            if (sensor == null)
            {
                VG_Ex_Sensor.IsExpanded = true;
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
            sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

            sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
            try
            {
                sensor.Start();
                VG_Ex_Sensor.IsExpanded = false;
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



        private void WindowClosed(object sender, EventArgs e)
        {

            if (null != kinectSensorChooser.Kinect)
            {
                kinectSensorChooser.Kinect.Stop();
            }
            MainWindow VP = new MainWindow();
            VP.Show();
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

        void sensor_SkeletonFrameReady(AllFramesReadyEventArgs e)
        {
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
                            Joint jointHead = sd.Joints[JointType.Head];
                            Joint jointSpine = sd.Joints[JointType.Spine];
                            Joint jointRShoulder = sd.Joints[JointType.ShoulderRight];
                            Joint jointLShoulder = sd.Joints[JointType.ShoulderLeft];
                            Joint jointRHip = sd.Joints[JointType.HipRight];
                            Joint jointLHip = sd.Joints[JointType.HipLeft];
                            Joint jointRLeg = sd.Joints[JointType.KneeRight];
                            Joint jointLLeg = sd.Joints[JointType.KneeLeft];

                            // scale those Joints to the primary screen width and height
                            Joint scaledRight = jointRight.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);
                            Joint scaledLeft = jointLeft.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);

                            // figure out the cursor position based on left/right handedness
                            if (LeftHand.IsChecked.GetValueOrDefault())
                            {
                                cursorX = (int)scaledLeft.Position.X;
                                cursorY = (int)scaledLeft.Position.Y;
                            }
                            else
                            {
                                cursorX = (int)scaledRight.Position.X;
                                cursorY = (int)scaledRight.Position.Y;
                            }

                            /*
                                1 = "Cruce De Manos";
                                2 = "Manos Abiertas";
                                3 = "Mano Izquierda Sobre La Cabeza";
                                4 = "Mano Derecha Sobre La Cabeza";
                                5 = "Ambas Manos Sobre La Cabeza";
                                6 = "Brazos Extendidos";
                                7 = "Mano Derecha Alzada";
                                8 = "Mano Izquierda Alzada";
                             */
                            if (jointLeft.Position.X > jointRight.Position.X)
                            {
                                GestureManager(1);
                            }
                            if (jointLeft.Position.X < jointRight.Position.X)
                            {
                                GestureManager(2);
                                if (jointLShoulder.Position.Y < jointLeft.Position.Y)
                                {
                                    GestureManager(8);
                                    if (jointHead.Position.Y < jointLeft.Position.Y)
                                    {
                                        GestureManager(3);
                                    }
                                    if (jointRShoulder.Position.Y < jointRight.Position.Y)
                                    {
                                        GestureManager(9);
                                        if (jointRight.Position.Y > jointHead.Position.Y && jointLeft.Position.Y > jointHead.Position.Y)
                                        {
                                            GestureManager(5);
                                        }
                                    }
                                }
                                if (jointRShoulder.Position.Y < jointRight.Position.Y){
                                    GestureManager(7);
                                    if (jointRight.Position.Y > jointHead.Position.Y)
                                    {
                                        GestureManager(4);
                                    }
                                    if (jointLShoulder.Position.Y < jointLeft.Position.Y)
                                    {
                                        GestureManager(9);
                                        if (jointRight.Position.Y > jointHead.Position.Y && jointLeft.Position.Y > jointHead.Position.Y)
                                        {
                                            GestureManager(5);
                                        }
                                    }
                                }
                            }
                            return;
                        }

                    }
                }
            }


        }

        void GestureManager(int gesture)
        {
            switch (gesture)
            {
                case 1: VG_L_Status.Text = "Cruce De Manos";
                    break;
                case 2: VG_L_Status.Text = "Manos Abiertas";
                    break;
                case 3: VG_L_Status.Text = "Mano Izquierda Sobre La Cabeza";
                    break;
                case 4: VG_L_Status.Text = "Mano Derecha Sobre La Cabeza";
                    break;
                case 5: VG_L_Status.Text = "Ambas Manos Sobre La Cabeza";
                    break;
                case 6: VG_L_Status.Text = "Brazos Extendidos";
                    break;
                case 7: VG_L_Status.Text = "Mano Derecha Alzada";
                    break;
                case 8: VG_L_Status.Text = "Mano Izquierda Alzada";
                    break;
                case 9: VG_L_Status.Text = "Ambas Manos Alzadas";
                    break;
                case 10: VG_L_Status.Text = "Pierna Izquierda Abierta";
                    break;
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

        private void ButtonHandler(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            String nombreboton = b.Name;
            switch (nombreboton){
                case "Abrir": VG_Ex_Sensor.IsExpanded = true;
                    break;
                case "Cerrar": VG_Ex_Sensor.IsExpanded = false;
                    break;
            }
        }

    }
}
