using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Samples.Kinect.WpfViewers;
using System;
using System.Windows.Data;

namespace KinoControlLib
{
    /// <summary>
    /// Interaction logic for ColorStreamWindow.xaml
    /// </summary>
    public partial class ColorStream : UserControl
    {
        public KinectSensor sensor;
        ///Bytes para el color
        private byte[] colorPixels;


        /// Bitmap that will hold color information
        private WriteableBitmap colorBitmap;

        public void Start640x480CStream()
        {
            InitializeComponent();
            SensorChooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(SensorChooser_KinectSensorChanged);
        }
        
        public void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    sensor.Stop();
                    KExpander.IsExpanded = true;
                }
            }
        }

        void SensorChooser_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor old = (KinectSensor)e.OldValue;

            StopKinect(old);

            sensor = (KinectSensor)e.NewValue;

            if (sensor == null)
            {
                KExpander.IsExpanded = true;
                return;
            }
            else
            {
                KExpander.IsExpanded = false;
            }

            TransformSmoothParameters parameters = new TransformSmoothParameters();
            parameters.Smoothing = 0.7f;
            parameters.Correction = 0.3f;
            parameters.Prediction = 0.4f;
            parameters.JitterRadius = 1.0f;
            parameters.MaxDeviationRadius = 0.5f;

            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            // Allocate space to put the pixels we'll receive
            this.colorPixels = new byte[SensorChooser.Kinect.ColorStream.FramePixelDataLength];

            // This is the bitmap we'll display on-
            this.colorBitmap = new WriteableBitmap(SensorChooser.Kinect.ColorStream.FrameWidth, SensorChooser.Kinect.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

            if (Window.GetWindow(this).WindowState != WindowState.Minimized)
            {
                // Set the image we display to point to the bitmap where we'll put the image data
                this.Image.Source = this.colorBitmap;
            }

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
            Sensor_ColorFrameReady(e);
        }


        /// Event handler for Kinect sensor's ColorFrameReady event
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_ColorFrameReady(AllFramesReadyEventArgs e)
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
    }
}
