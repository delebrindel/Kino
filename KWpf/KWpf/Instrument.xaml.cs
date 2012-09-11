//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace KWpf
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Threading;
    using Microsoft.Kinect;
    using System.IO.Ports;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using Microsoft.Kinect.Toolkit;
    using System.Media;
    using System.Runtime.InteropServices;
    using System.Text;


    /// Interaction logic for MainWindow.xaml

    public partial class Instrument : Window
    {
        int FueraCuadro = 0;
        WMPLib.WindowsMediaPlayer a = new WMPLib.WindowsMediaPlayer();


        private readonly KinectSensorChooser sensorChooser = new KinectSensorChooser();

        /// Active Kinect sensor
        private KinectSensor sensor;


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

        /// Brush used for drawing joints that are currently inferred
        private readonly Brush handBrush= Brushes.AliceBlue;

        /// Pen used for drawing bones that are currently tracked
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// Pen used for drawing bones that are currently inferred
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// Pen used for drawing bones that are currently inferred
        private readonly Pen handPen = new Pen(Brushes.Teal, 1);

        /// Drawing group for skeleton rendering output
        private DrawingGroup drawingGroup;

        /// Drawing image that we will display
        private DrawingImage imageSource;

        public Instrument()
        {
            InitializeComponent();
        }

        /// Execute startup tasks
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Esqueleto.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }
            if (null != this.sensor)
            {

                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Turn on the color stream to receive color frames
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);


                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);


                // Add an event handler to be called whenever there is new color frame data
                this.sensor.ColorFrameReady += this.SensorColorFrameReady;

                //Kinect Sensor Status
                KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.VP_L_Estado.Text = "No hay Kinect";
            }
            if (sensor.ElevationAngle != 0)
                sensor.ElevationAngle = 0;

            ControlUpd();

        }


        //Update Controls
        private void ControlUpd()
        {
            VP_L_Alt.Text = sensor.ElevationAngle.ToString();
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
            if (null != this.sensor)
            {
                this.sensor.Stop();
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
            DrawHands(skeleton, drawingContext);

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
            DepthImagePoint depthPoint = this.sensor.MapSkeletonPointToDepth(skelpoint,DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        private void DrawHands(Skeleton skeleton, DrawingContext drawingContext)
        {
            System.Collections.Generic.ICollection<Typeface> typefaces = Fonts.GetTypefaces("file:///C:\\Windows\\Fonts\\#Georgia");
            Joint manoIzq = skeleton.Joints[JointType.HandLeft];
            Joint manoDer = skeleton.Joints[JointType.HandRight];
            drawingContext.DrawEllipse(handBrush, handPen, this.SkeletonPointToScreen(manoIzq.Position), 20.0, 20.0);
            drawingContext.DrawEllipse(handBrush, handPen, this.SkeletonPointToScreen(manoDer.Position), 20.0, 20.0);
            ReconocePos(manoIzq, manoDer, drawingContext);
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
            if (null != this.sensor)
            {
                if (this.VP_Ch_Sentado.IsChecked.GetValueOrDefault())
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                }
                else
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                }
            }
        }

        private void ButtonUpClick(object sender, RoutedEventArgs e)
        {
            if (sensor.ElevationAngle < 23)
                sensor.ElevationAngle += 5;
            else
                MessageBox.Show("No Se Puede Subir Más");
            ControlUpd();
        }

        private void ButtonDownClick(object sender, RoutedEventArgs e)
        {
            if (sensor.ElevationAngle > -23)
                sensor.ElevationAngle -= 5;
            else
                MessageBox.Show("No Se Puede Bajar Más");
            ControlUpd();
        }

        private void ButtonCenterClick(object sender, RoutedEventArgs e)
        {
            if (sensor.ElevationAngle != 0)
                sensor.ElevationAngle = 0;
            else
                MessageBox.Show("El Kinect Se Encuentra Centrado");
            ControlUpd();
        }

        private void ReconocePos(Joint manoIzq, Joint manoDer, DrawingContext drawingContext)
        {
            Point PMIzq = this.SkeletonPointToScreen(manoIzq.Position);
            Point PMDer = this.SkeletonPointToScreen(manoDer.Position);

            Point TextoPos=new Point(10,400);


            Typeface newTypeface = new Typeface("Arial");
            FormattedText CadenaIX = new FormattedText(PMIzq.X.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, newTypeface, 40.0, handBrush);
            FormattedText CadenaIY = new FormattedText(PMIzq.Y.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, newTypeface, 40.0, handBrush);
            FormattedText CadenaDX = new FormattedText(PMDer.X.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, newTypeface, 40.0, handBrush);
            FormattedText CadenaDY = new FormattedText(PMDer.Y.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, newTypeface, 40.0, handBrush);
            //drawingContext.DrawText(CadenaIY,TextoPos);
            //drawingContext.DrawText(CadenaI, this.SkeletonPointToScreen(manoIzq.Position));


            if ((PMIzq.X < 120 && (PMIzq.Y > 20 && PMIzq.Y < 120)) || PMDer.X < 120 && (PMDer.Y > 20 && PMDer.Y < 120))
                Cuadros(1);
            else if (PMIzq.X < 120 && (PMIzq.Y > 160 && PMIzq.Y < 260) || (PMDer.X < 120 && (PMDer.Y > 160 && PMDer.Y < 260)))
                Cuadros(2);
            else if (PMDer.X > 520 && (PMDer.Y > 20 && PMDer.Y < 120) || (PMIzq.X > 520 && (PMIzq.Y > 20 && PMIzq.Y < 120)))
                Cuadros(3);
            else if (PMDer.X > 520 && (PMDer.Y > 160 && PMDer.Y < 260) || (PMIzq.X > 520 && (PMIzq.Y > 160 && PMIzq.Y < 260)))
                Cuadros(4);
            else
                Cuadros(0); 
        }

        private void Cuadros(int cuadro)
        {
            SolidColorBrush Relleno=new SolidColorBrush(Colors.Green);
            SolidColorBrush Vacio=new SolidColorBrush(Colors.White);

            if (cuadro == 1)
            {
                VI_CuaSupIzq.Fill = Relleno;
                ReproduceSon(1);
            }
            else if (cuadro == 2)
            {
                VI_CuaInfIzq.Fill = Relleno;
                ReproduceSon(2);
            }
            else if (cuadro == 3)
            {
                VI_CuaSupDer.Fill = Relleno;
                ReproduceSon(3);
            }
            else if (cuadro == 4)
            {
                VI_CuaInfDer.Fill = Relleno;
                ReproduceSon(4);
            }
            else
            {
                FueraCuadro = 0;
                VI_CuaSupIzq.Fill = Vacio;
                VI_CuaInfIzq.Fill = Vacio;
                VI_CuaSupDer.Fill = Vacio;
                VI_CuaInfDer.Fill = Vacio;
            }

        }

        public void ReproduceSon(int opc){
            String dir="";
            if (opc == 1&& FueraCuadro==0)
            {
                dir = "C:/Users/America Movil/Documents/GitHub/Kino/KWpf/KWpf/Sounds/Drums/bass.mp3";
                FueraCuadro = 1;
                a.URL = dir;
                a.controls.play();
            }
            else if (opc == 2 && FueraCuadro==0)
            {
                dir = "C:/Users/America Movil/Documents/GitHub/Kino/KWpf/KWpf/Sounds/Drums/ride.mp3";
                FueraCuadro = 1;
                a.URL = dir;
                a.controls.play();
            }
            else if (opc == 3 && FueraCuadro == 0)
            {
                dir = "C:/Users/America Movil/Documents/GitHub/Kino/KWpf/KWpf/Sounds/Drums/unknow.mp3";
                FueraCuadro = 1;
                a.URL = dir;
                a.controls.play();
            }
            else if (opc == 4 && FueraCuadro == 0)
            {
                dir = "C:/Users/America Movil/Documents/GitHub/Kino/KWpf/KWpf/Sounds/Drums/tarola.mp3";
                FueraCuadro = 1;
                a.URL = dir;
                a.controls.play();
            }
        }

    }
}