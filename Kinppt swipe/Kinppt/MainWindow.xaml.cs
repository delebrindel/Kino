using System;
using System.Threading;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Threading;
using System.IO;
using System.Speech.Recognition;
using System.Speech.AudioFormat;
using System.Diagnostics;
using System.Windows.Threading;
using Microsoft.Kinect.Toolkit;
using Microsoft.Samples.Kinect.WpfViewers;




namespace Kinppt
{

    public partial class MainWindow : Window
    {
        /// Declaraciones para control de Kinect Sensor Manager. Van En La Partial Class
        private readonly Microsoft.Kinect.Toolkit.KinectSensorChooser sensorChooser = new KinectSensorChooser();
        public static readonly DependencyProperty KinectSensorManagerProperty =
            DependencyProperty.Register(
                "KinectSensorManager",
                typeof(KinectSensorManager),
                typeof(MainWindow),
                new PropertyMetadata(null));

        KinectSensor sensork;
        byte[] bytesc;
        Skeleton[] skeletons;
        
        
  public MainWindow()
        {
            InitializeComponent();
            //INICIALIZA LA VENTANA PRINCIPAL Y MANEJA EL CONTENIDO DE LA CÁMARA
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //INICIALIZA EL SENSOR
            sensork = KinectSensor.KinectSensors.FirstOrDefault();
            //VERIFICA SI ESTÁ CONECTADO A LA COMPUTADORA
            if (sensork == null)
            {
                MessageBox.Show("Se Necesita un sensor Kinect.");
                this.Close();
            }
            sensork.Start();
            //RESOLUCIÓN DE CÁMARA RGB 640x480 FPS=30
            sensork.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            sensork.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
            //RESOLUCIÓN DE CÁMARA DE PROFUNDIDAD 320x240 FPS=30
            sensork.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            sensork.SkeletonStream.Enable();
            sensork.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
            //REINICIA LA ELEVACIÓN DEL SENSOR A 0  
            sensork.ElevationAngle = 0;
            //LLAMA A MÉTODO PARA FINALIZAR
            Application.Current.Exit += new ExitEventHandler(Current_Exit);
            //INICIA MOTOR DE RECONOCIMIENTO DE VOZ
            //InitializeSpeechRecognition();
        }
        //MÉTODO QUE FINALIZA Y LIBERA EL SENSOR KINECT
        void Current_Exit(object sender, ExitEventArgs e)
        {
            //VERIFICA QUE NO SE ESTÉ UTILIZANDO EL RECONOCIMIENTO DE VOZ
            /* if (speechRecognizer != null)
             {
                 speechRecognizer.RecognizeAsyncCancel();
                 speechRecognizer.RecognizeAsyncStop();
             }*/

            //VERIFICA QUE EL SENSOR NO ESTÉ EN USO
            if (sensork != null)
            {
                sensork.AudioSource.Stop();
                sensork.Stop();
                sensork.Dispose();
                sensork = null;
            }
        }

        //SUBIR KINECT
        private void ButtonUpClick(object sender, RoutedEventArgs e)
        {
            if (sensork.ElevationAngle < 23){
                sensork.ElevationAngle += 5;
                 Thread.Sleep(1000);
        }
            else
                MessageBox.Show("No Se Puede Subir Más");
        }

        //BAJAR KINECT
        private void ButtonDownClick(object sender, RoutedEventArgs e)
        {
            if (sensork.ElevationAngle > -23)
            {
                sensork.ElevationAngle -= 5;
                Thread.Sleep(1000);
            }
            else
                MessageBox.Show("No Se Puede Bajar Más");
        }

        //CENTRAR KINECT
        private void ButtonCenterClick(object sender, RoutedEventArgs e)
        {
            if (sensork.ElevationAngle != 0)
                sensork.ElevationAngle = 0;
            else
                MessageBox.Show("El Kinect Se Encuentra Centrado");
        }

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (var image = e.OpenColorImageFrame())
            {
                if (image == null)
                    return;

                if (bytesc == null ||
                    bytesc.Length != image.PixelDataLength)
                {
                    bytesc = new byte[image.PixelDataLength];
                }

                image.CopyPixelDataTo(bytesc);

                //You could use PixelFormats.Bgr32 below to ignore the alpha,
                //or if you need to set the alpha you would loop through the bytes 
                //as in this loop below
                int length = bytesc.Length;
                for (int i = 0; i < length; i += 4)
                {
                    bytesc[i + 3] = 255;
                }

                BitmapSource source = BitmapSource.Create(image.Width,
                    image.Height,
                    96,
                    96,
                    PixelFormats.Bgra32,
                    null,
                    bytesc,
                    image.Width * image.BytesPerPixel);
                videoImage.Source = source;
            }
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                    return;

                if (skeletons == null ||
                    skeletons.Length != skeletonFrame.SkeletonArrayLength)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                }

                skeletonFrame.CopySkeletonDataTo(skeletons);

                Skeleton closestSkeleton = (from s in skeletons
                                            where s.TrackingState == SkeletonTrackingState.Tracked &&
                                                  s.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked
                                            select s).OrderBy(s => s.Joints[JointType.Head].Position.Z)
                                                    .FirstOrDefault();

                if (closestSkeleton == null)
                    return;

                var head = closestSkeleton.Joints[JointType.Head];
                var rightHand = closestSkeleton.Joints[JointType.HandRight];
                var leftHand = closestSkeleton.Joints[JointType.HandLeft];
                var rightShoulder = closestSkeleton.Joints[JointType.ShoulderRight];
                var leftShoulder = closestSkeleton.Joints[JointType.ShoulderLeft];

                        if (head.TrackingState != JointTrackingState.Tracked ||
                            rightHand.TrackingState != JointTrackingState.Tracked ||
                            leftHand.TrackingState != JointTrackingState.Tracked ||
                            rightShoulder.TrackingState != JointTrackingState.Tracked ||
                            leftShoulder.TrackingState != JointTrackingState.Tracked
                           )
                        {
                            return;
                        }
 
                ProcessForwardBackGesture(head, rightHand, leftHand, leftShoulder, rightShoulder);
            }
        }

        public struct Vector3
        {
            public float X;
            public float Y;
            public float Z;
            public DateTime date;
        }
        public enum Gesture
        {
            None,
            Swipe
        }

        List<Vector3> positionListRight = new List<Vector3>();
        List<Vector3> positionListLeft = new List<Vector3>();
        List<Gesture> gestureAcceptedList = new List<Gesture>();
        const float SwipeMinimalLength = 0.2f; //longitud máxima del movimiento swipe original era .4
        const float SwipeMaximalHeight = 0.1f; //altura máxima del movimiento desde inicio hasta fin
        const int SwipeMinimalDuration = 100; //en ms original era 200
        const int SwipeMaximalDuration = 500; //ms
        DateTime lastGestureDate = DateTime.Now;
        int MinimalPeriodBetweenGestures = 0; //ms transcurridos entre dos gestos
        bool adelante = false;
        bool atras = false;



        private void ProcessForwardBackGesture(Joint head, Joint rightHand, Joint leftHand, Joint leftShoulder, Joint rightShoulder)
        {

            if (!atras && !adelante)
            {

                positionListRight.Add(new Vector3()
                {
                    X = rightHand.Position.X,
                    Y = rightHand.Position.Y,
                    Z = rightHand.Position.Z,
                    date = DateTime.Now
                });

                if (SwipeAdvance())
                {
                    adelante = true;
                    System.Windows.Forms.SendKeys.SendWait("{Right}");
                }
                if (positionListRight.Count() > 20)
                {
                    positionListRight.RemoveAt(0);
                }
            }
            else
            {
                adelante = false;
            }

            if (!atras && !adelante)
            {
                positionListLeft.Add(new Vector3()
           {
               X = leftHand.Position.X,
               Y = leftHand.Position.Y,
               Z = leftHand.Position.Z,
               date = DateTime.Now
           });

                if (SwipeBack())
                {
                    atras = true;
                    System.Windows.Forms.SendKeys.SendWait("{Left}");
                }
                if (positionListLeft.Count() > 20)
                {
                    positionListLeft.RemoveAt(0);
                }
            }
            else
            {
                atras = false;
            }
        }

      bool SwipeAdvance()
        {
            int start = 0;
            for (int index = 0; index < positionListRight.Count - 1; index++)
            {
                if ((Math.Abs(positionListRight[0].Y - positionListRight[index].Y) > SwipeMaximalHeight) || (positionListRight[index + 1].X - positionListRight[index].X > -0.01f))
                {
                    start = index;
                }
                if ((Math.Abs(positionListRight[index].X - positionListRight[start].X) > SwipeMinimalLength))
                {
                    double totalMilliseconds = (positionListRight[index].date - positionListRight[start].date).TotalMilliseconds;
                    if (totalMilliseconds >= SwipeMinimalDuration && totalMilliseconds <= SwipeMaximalDuration)
                    {
                        if (DateTime.Now.Subtract(lastGestureDate).TotalMilliseconds > MinimalPeriodBetweenGestures)
                        {
                            gestureAcceptedList.Add(Gesture.Swipe);
                            lastGestureDate = DateTime.Now;
                            positionListRight.Clear();
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        bool SwipeBack()
        {
            int start = 0;
            for (int index = 0; index < positionListLeft.Count - 1; index++)
            {
                if ((Math.Abs(positionListLeft[0].Y - positionListLeft[index].Y) > SwipeMaximalHeight) || (positionListLeft[index + 1].X - positionListLeft[index].X < -0.01f))
                {
                    start = index;
                }
                if ((Math.Abs(positionListLeft[index].X - positionListLeft[start].X) > SwipeMinimalLength))
                {
                    double totalMilliseconds = (positionListLeft[index].date - positionListLeft[start].date).TotalMilliseconds;
                    if (totalMilliseconds >= SwipeMinimalDuration && totalMilliseconds <= SwipeMaximalDuration)
                    {
                        if (DateTime.Now.Subtract(lastGestureDate).TotalMilliseconds > MinimalPeriodBetweenGestures)
                        {
                            gestureAcceptedList.Add(Gesture.Swipe);
                            lastGestureDate = DateTime.Now;
                            positionListLeft.Clear();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /*#region Speech Recognition Methods

        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = ri =>
            {
                string value;
                ri.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "es-MX".Equals(ri.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        private void InitializeSpeechRecognition()
        {
            RecognizerInfo ri = GetKinectRecognizer();
            if (ri == null)
            {
                MessageBox.Show(
                    @"There was a problem initializing Speech Recognition.
Ensure you have the Microsoft Speech SDK installed.",
                    "Failed to load Speech SDK",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            try
            {
                speechRecognizer = new SpeechRecognitionEngine(ri.Id);
            }
            catch
            {
                MessageBox.Show(
                    @"There was a problem initializing Speech Recognition.
Ensure you have the Microsoft Speech SDK installed and configured.",
                    "Failed to load Speech SDK",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            var comandos = new Choices();
            comandos.Add("Mostrar Ventana");
            comandos.Add("Ocultar Ventana");
            comandos.Add("Mostrar Círculos");
            comandos.Add("Ocultar Círculos");

            var gb = new GrammarBuilder();
            //Specify the culture to match the recognizer in case we are running in a different culture.                                 
            gb.Culture = ri.Culture;
            gb.Append(comandos);

            // Create the actual Grammar instance, and then load it into the speech recognizer.
            var g = new Grammar(gb);

            speechRecognizer.LoadGrammar(g);
            speechRecognizer.SpeechRecognized += SreSpeechRecognized;
            speechRecognizer.SpeechHypothesized += SreSpeechHypothesized;
            speechRecognizer.SpeechRecognitionRejected += SreSpeechRecognitionRejected;

            this.readyTimer = new DispatcherTimer();
            this.readyTimer.Tick += this.ReadyTimerTick;
            this.readyTimer.Interval = new TimeSpan(0, 0, 4);
            this.readyTimer.Start();

        }

        private void ReadyTimerTick(object sender, EventArgs e)
        {
            this.StartSpeechRecognition();
            this.readyTimer.Stop();
            this.readyTimer = null;
        }

        private void StartSpeechRecognition()
        {
            if (sensork == null || speechRecognizer == null)
                return;

            var audioSource = this.sensork.AudioSource;
            audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            var kinectStream = audioSource.Start();

            speechRecognizer.SetInputToAudioStream(
                    kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);

        }

        void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Trace.WriteLine("\nSpeech Rejected, confidence: " + e.Result.Confidence);
        }

        void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Trace.Write("\rSpeech Hypothesized: \t{0}", e.Result.Text);
        }

        void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            const double igualdad = 0.5;

            if (e.Result.Confidence < igualdad)
            {
                MessageBox.Show("No se reconoce comando, confiabilidad: " + e.Result.Confidence);
                //Trace.WriteLine("\nSpeech Rejected filtered, confidence: " + e.Result.Confidence));
                return;
            }
            MessageBox.Show("Comando Reconocido, confiabilidad: " + e.Result.Confidence+": \t{0}",e.Result.Text);
            //Trace.WriteLine("\nSpeech Recognized, confidence: " + e.Result.Confidence + ": \t{0}", e.Result.Text);

            if (e.Result.Text == "Mostrar Ventana")
            {
                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    this.Topmost = true;
                    this.WindowState = System.Windows.WindowState.Normal;
                });
            }
            else if (e.Result.Text == "Ocultar Ventana")
            {
                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    this.Topmost = false;
                    this.WindowState = System.Windows.WindowState.Minimized;
                });
            }
        }

        #endregion*/

    }
}
