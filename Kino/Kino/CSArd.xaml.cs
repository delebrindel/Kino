//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Kino
{
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Threading.Tasks;
    using Microsoft.Kinect;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using Microsoft.Kinect.Toolkit;
    using Microsoft.Samples.Kinect.WpfViewers;
    using System;
    using System.Windows.Data;
    using System.IO.Ports;
 
    
    /// Interaction logic for MainWindow.xaml
    
    public partial class CSArd : Window
    {
        private SerialPort puertoArd;
        Functions f = new Functions();

        private readonly KinectSensorChooser sensorChooser = new KinectSensorChooser();

        /// Bitmap that will hold color information
        private WriteableBitmap colorBitmap;


        /// Intermediate storage for the color data received from the camera
        private byte[] colorPixels;

        /// Width of output drawing
        private const float RenderWidth = 640.0f;


        /// Height of our output drawing
        private const float RenderHeight = 480.0f;


        /// Thickness of drawn joint lines
        private const double JointThickness = 3;

        /// Thickness of body center ellipse
        private const double BodyCenterThickness = 10;

        /// Thickness of clip edge rectangles
        private const double ClipBoundsThickness = 10;

        /// Brush used to draw skeleton center point
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// Brush used for drawing joints that are currently tracked
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// Brush used for drawing joints that are currently inferred
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// Pen used for drawing bones that are currently tracked
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// Pen used for drawing bones that are currently inferred
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// Drawing group for skeleton rendering output
        private DrawingGroup drawingGroup;

        /// Drawing image that we will display
        private DrawingImage imageSource;

        /// Declaraciones para control de Kinect Sensor Manager. Van En La Partial Class
        public static readonly DependencyProperty KinectSensorManagerProperty =
            DependencyProperty.Register(
                "KinectSensorManager",
                typeof(KinectSensorManager),
                typeof(MainWindow),
                new PropertyMetadata(null));

        public CSArd()
        {
            /// Declaraciones para control de Kinect Sensor Manager (2), Van En El Constructor De La Clase
            this.KinectSensorManager = new KinectSensorManager();
            this.KinectSensorManager.KinectSensorChanged += this.KinectSensorChanged;
            this.DataContext = this.KinectSensorManager;

            InitializeComponent();

            // Bind the KinectSensor from the sensorChooser to the KinectSensor on the KinectSensorManager  
            //Van En El Constructor De La Clase Tras Inicializar El Componente
            sensorChooser.Start();
            var kinectSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.KinectSensorManager, KinectSensorManager.KinectSensorProperty, kinectSensorBinding);
        }

        
        /// Execute startup tasks
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

            puertoArd = new SerialPort();
            puertoArd.PortName = "COM3";
            puertoArd.BaudRate = 9600;
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Esqueleto.Source = this.imageSource;

        }

        /// Declaración Para Kinect Sensor Chooser (3)
        public KinectSensorManager KinectSensorManager
        {
            get { return (KinectSensorManager)GetValue(KinectSensorManagerProperty); }
            set { SetValue(KinectSensorManagerProperty, value); }
        }

        /// Declaración Para Kinect Sensor Chooser (4)
        private void KinectSensorChanged(object sender, KinectSensorManagerEventArgs<KinectSensor> args)
        {
            if (null != args.OldValue)
            {
                this.UninitializeKinectServices(args.OldValue);
            }


            if (null != args.NewValue)
            {
                this.InitializeKinectServices(this.KinectSensorManager, args.NewValue);
            }
        }


        // Kinect enabled apps should customize which Kinect services it initializes here.
        private void InitializeKinectServices(KinectSensorManager kinectSensorManager, KinectSensor sensor)
        {
            // Application should enable all streams first.
            kinectSensorManager.ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;
            kinectSensorManager.ColorStreamEnabled = true;
            kinectSensorManager.SkeletonStreamEnabled = true;


            kinectSensorManager.TransformSmoothParameters = new TransformSmoothParameters
            {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            };

            // Turn on the color stream to receive color frames
            KinectSensorManager.KinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            KinectSensorManager.KinectSensor.SkeletonStream.Enable();

            // Allocate space to put the pixels we'll receive
            this.colorPixels = new byte[KinectSensorManager.KinectSensor.ColorStream.FramePixelDataLength];

            // This is the bitmap we'll display on-screen
            this.colorBitmap = new WriteableBitmap(KinectSensorManager.KinectSensor.ColorStream.FrameWidth, KinectSensorManager.KinectSensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

            // Set the image we display to point to the bitmap where we'll put the image data
            this.Image.Source = this.colorBitmap;
            // Set the image we display to point to the bitmap where we'll put the image data
            this.Image.Source = this.colorBitmap;

            // Add an event handler to be called whenever there is new color frame data
            KinectSensorManager.KinectSensor.ColorFrameReady += this.SensorColorFrameReady;
            KinectSensorManager.KinectSensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

            kinectSensorManager.SkeletonStreamEnabled = true;
            kinectSensorManager.ColorStreamEnabled = true;
            kinectSensorManager.KinectSensorEnabled = true;


            if (KinectSensorManager.KinectSensor.ElevationAngle != 0)
                KinectSensorManager.KinectSensor.ElevationAngle = 0;
        }

        // Kinect enabled apps should uninitialize all Kinect services that were initialized in InitializeKinectServices() here.
        private void UninitializeKinectServices(KinectSensor sensor)
        {
            KinectSensorManager.KinectSensorEnabled = false;
        }

        private void Arduino_ButtonHandler(object sender, RoutedEventArgs e)
        {


            Button b = (Button)sender;
            String nombreboton = b.Name;
            switch (nombreboton)
            {
                case "VP_B_OPort":
                                    if (!puertoArd.IsOpen)
                                        puertoArd.Open();
                                    else
                                        MessageBox.Show("Ya está abierto el puerto!!");
                                    ControlUpd();
                                    break;
                case "VP_B_CPort":
                                    if (puertoArd.IsOpen)
                                        puertoArd.Close();
                                    else
                                        MessageBox.Show("El Puerto Está Cerrado!!");
                                    ControlUpd();
                                    break;
                case "VP_B_HLed":
                                    if (puertoArd.IsOpen)
                                        puertoArd.Write("A");
                                    else
                                        MessageBox.Show("El Puerto Está Cerrado!!");
                                    break;
                case "VP_B_LLed":
                                    if (puertoArd.IsOpen)
                                        puertoArd.Write("B");
                                    else
                                        MessageBox.Show("El Puerto Está Cerrado!!");
                                    break;
                case "VP_B_BLed":
                                    if (puertoArd.IsOpen)
                                        puertoArd.Write("C");
                                    else
                                        MessageBox.Show("El Puerto Está Cerrado!!");
                                    break;
            }

            /*
            */
        }


        //Update Controls
        private void ControlUpd()
        {
            VP_L_Alt.Text = KinectSensorManager.KinectSensor.ElevationAngle.ToString();
            if (!puertoArd.IsOpen)
            {
                VP_L_Port.Text = "Cerrado";
                VP_B_HLed.IsEnabled = false;
                VP_B_LLed.IsEnabled = false;
                VP_B_BLed.IsEnabled = false;
            }
            else
            {
                VP_L_Port.Text = "Abierto";
                VP_B_HLed.IsEnabled = true;
                VP_B_LLed.IsEnabled = true;
                VP_B_BLed.IsEnabled = true;
            }
        }

        /// Draws indicators to show which edges are clipping skeleton data
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// Execute shutdown tasks
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != KinectSensorManager.KinectSensor)
            {
                this.KinectSensorManager.KinectSensor.Stop();
            }
            if (puertoArd.IsOpen == true)
            {
                puertoArd.Close();
            }
            MainWindow VP = new MainWindow();
            VP.Show();
            
        }

        /// Event handler for Kinect sensor's ColorFrameReady event
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }

        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        /// Event handler
        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Disconnected:
                    VP_L_Estado.Text = "Se Requiere Un Sensor Kinect";
                    break;
                case KinectStatus.Initializing:
                    VP_L_Estado.Text = "Inicializando...";
                    break;
                case KinectStatus.Connected:
                    VP_L_Estado.Text = "Connectado!";
                    break;
            }
        }

        /// Draws a skeleton's bones and joints
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
        }

        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = KinectSensorManager.KinectSensor.MapSkeletonPointToDepth(
                                                                             skelpoint,
                                                                             DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// Draws a bone line between two joints
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        /// Handles the checking or unchecking of the seated mode combo box
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void CheckBoxSeatedModeChanged(object sender, RoutedEventArgs e)
        {
            if (null != KinectSensorManager.KinectSensor)
            {
                if (this.VP_Ch_Sentado.IsChecked.GetValueOrDefault())
                {
                    KinectSensorManager.KinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                }
                else
                {
                    KinectSensorManager.KinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                }
            }
        }

        private void ButtonUpClick(object sender, RoutedEventArgs e)
        {
            if (KinectSensorManager.KinectSensor.ElevationAngle < 23)
                KinectSensorManager.KinectSensor.ElevationAngle += 5;
            else
                MessageBox.Show("No Se Puede Subir Más");         
            ControlUpd();
        }

        private void ButtonDownClick(object sender, RoutedEventArgs e)
        {
            if (KinectSensorManager.KinectSensor.ElevationAngle > -23)
                KinectSensorManager.KinectSensor.ElevationAngle -= 5;
            else
                MessageBox.Show("No Se Puede Bajar Más");
            ControlUpd();
        }

        private void ButtonCenterClick(object sender, RoutedEventArgs e)
        {
            if (KinectSensorManager.KinectSensor.ElevationAngle != 0)
                KinectSensorManager.KinectSensor.ElevationAngle = 0;
            else
                MessageBox.Show("El Kinect Se Encuentra Centrado");
            ControlUpd();
        }

        private void OpenColorStream(object sender, RoutedEventArgs e)
        {

            ColorStreamWindow VC= new ColorStreamWindow();
            VC.Show();
            this.Close();
        }

    }
}