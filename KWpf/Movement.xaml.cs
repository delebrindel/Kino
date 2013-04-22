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
using System.Windows.Threading;
using System.Resources;
using KWpf.Properties;


namespace KWpf
{
    /// <summary>
    /// Interaction logic for Piano.xaml
    /// 

    /// </summary>
    public partial class Movement : Window
    {
        public float HandsTogTresh = 0.2f;
        public float HandsSepTresh = 0.4f;
        public KinectSensor sensor;
        public const float ClickThreshold = .1f; //.2
        private const float SkeletonMaxX = 0.60f; //.6
        private const float SkeletonMaxY = 0.40f;
        Random random = new Random();
        public int KMouseOn = 0;
        public int Limit = 3;
        public int TValue=10;
        public int Score=0;
        public Boolean CAceptado = false;
        public Boolean Inicializado = false;
        public String Parte;
        public int PTranslate;
        public double MaxXCord = SystemParameters.PrimaryScreenWidth;
        public double MaxYCord = SystemParameters.PrimaryScreenHeight;
        public double MinYCord = 0;
        public double MinXCord = 0;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();



        public Movement()
        {
            InitializeComponent();
            StartTimer();
            Expansor.IsExpanded = true;
            kinectSensorChooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser_KinectSensorChanged);
        }


        public void StartTimer()
        {
            TValue = 5;
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            Musica.Play();

        }

        private String TraduceParte()
        {
            String TParte = "";
            switch (PTranslate)
            {
                case 1:     TParte = "Mano Izquerda Arriba Del Hombro";
                            Stick.Source = new BitmapImage(new Uri("/Resources/Manizqhom.PNG", UriKind.Relative)); ;
                            break;
                case 2:     TParte = "Mano Derecha Arriba Del Hombro";
                            Stick.Source = new BitmapImage(new Uri("/Resources/Manderhom.PNG", UriKind.Relative)); ;
                            break;
                case 3:     TParte = "Manos Juntas";
                            Stick.Source = new BitmapImage(new Uri("/Resources/Manjuntas.PNG", UriKind.Relative)); ;
                            break;
                case 4:     TParte = "Manos Separadas";
                            Stick.Source = new BitmapImage(new Uri("/Resources/Manab.PNG", UriKind.Relative)); ;
                            break;
                case 5:     TParte = "Mano Izquerda Sobre La Cabeza";
                            Stick.Source = new BitmapImage(new Uri("/Resources/Manizqsup.PNG", UriKind.Relative)); ;
                            break;
                case 6:     TParte = "Mano Derecha Sobre La Cabeza";
                            Stick.Source = new BitmapImage(new Uri("/Resources/Mandersup.PNG", UriKind.Relative)); ;
                            break;
                case 7:     TParte = "Ambas Manos Sobre La Cabeza";
                            Stick.Source = new BitmapImage(new Uri("/Resources/Mansupcab.PNG", UriKind.Relative)); ;
                            break;
                case 8:     TParte = "Guacamole 8";
                            break;
                case 9:     TParte = "Guacamole 9";
                            break;
                case 10:    TParte = "Guacamole 10";
                            break;
                case 11:    TParte = "Guacamole 11";
                            break;
                case 12:    TParte = "Guacamole 12";
                            break;
            }
            return (TParte);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Inicializado == false)
            {
                if (TValue < 1)
                {
                    Vmv_TB_Part.Text = "Start!";
                    TValue = Limit + 1;
                    Inicializado = true;
                    GeneraParte();
                }
                else
                {
                    Vmv_TB_Part.Text = TValue.ToString() + " Segundos Para Iniciar";
                    TValue -= 1;
                }
            }
            else
            {
                TValue -= 1;
                TimerValue.Text = TValue.ToString();
                CommandManager.InvalidateRequerySuggested();
                if (TValue < 1)
                {
                    TimerOver.Text = "Se Acabó!";
                    Rechazar();
                    TValue = Limit + 1;
                    GeneraParte();
                }
                else
                {
                    TimerOver.Text = "Apúrate!";
                }
            }
        }

        private int GeneraParte()
        {
            PTranslate= random.Next(1, 8);
            Parte=TraduceParte();
            Vmv_TB_Part.Text = Parte;
            CAceptado = false;
            return PTranslate;
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
                            Validacion(sd);
                            // get the left and right hand Joints
                            /*
                            // scale those Joints to the primary screen width and height
                            Joint scaledRight = jointRight.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);
                            Joint scaledLeft = jointLeft.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);
                            */
                        }
                        return;
                    }
                }
            }


        }

        private void Validacion(Skeleton sd){
            Joint LeftShoulder =sd.Joints[JointType.ShoulderLeft];
            Joint RightShoulder = sd.Joints[JointType.ShoulderRight];
            Joint RightHand = sd.Joints[JointType.HandRight];
            Joint LeftHand = sd.Joints[JointType.HandLeft];
            Joint Head = sd.Joints[JointType.Head];

            if (CAceptado == false)
            {
                if (PTranslate == 1 && (LeftShoulder.Position.Y < LeftHand.Position.Y) && (RightShoulder.Position.Y > RightHand.Position.Y))
                {
                    Aceptar();
                }
                if (PTranslate == 2 && (RightShoulder.Position.Y < RightHand.Position.Y) && (LeftShoulder.Position.Y > LeftHand.Position.Y))
                {
                    Aceptar();
                }
                if (PTranslate == 3&& (RightHand.Position.X - LeftHand.Position.X)< HandsTogTresh)
                {
                    Aceptar();
                }
                if (PTranslate == 4 && (RightHand.Position.X - LeftHand.Position.X) > HandsSepTresh)
                {
                    Aceptar();
                }
                if (PTranslate == 5 && (Head.Position.Y < LeftHand.Position.Y) && (Head.Position.Y > RightHand.Position.Y))
                {
                    Aceptar();
                }
                if (PTranslate == 6 && (Head.Position.Y < RightHand.Position.Y) && (Head.Position.Y > LeftHand.Position.Y))
                {
                    Aceptar();
                }
                if (PTranslate == 7 && (Head.Position.Y < RightHand.Position.Y) && (Head.Position.Y < LeftHand.Position.Y))
                {
                    Aceptar();
                }
            }
        }

        private void Aceptar()
        {
            Score += 5;
            Vmv_TB_Points.Text = Score.ToString();
            TValue = Limit + 1;
            CAceptado = true;
            Respuesta.Source = new BitmapImage(new Uri("/Resources/icorrect.PNG", UriKind.Relative)); ;
            GeneraParte();
        }

        private void Rechazar()
        {
            Score -= 5;
            Vmv_TB_Points.Text = Score.ToString();
            TValue = Limit + 1;
            CAceptado = true;
            GeneraParte();
            if (Score < 0)
                GameOver();
            Respuesta.Source = new BitmapImage(new Uri("/Resources/iwrong.PNG", UriKind.Relative)); ;
        }

        private void GameOver()
        {
            MessageBox.Show("¡Pelas!");
            dispatcherTimer.Stop();
            this.Close();
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

        private void Musica_MediaEnded(object sender, RoutedEventArgs e)
        {
            Musica.Stop();
            Musica.Position = TimeSpan.FromSeconds(0);
            Musica.Play();
        }
    }
}
