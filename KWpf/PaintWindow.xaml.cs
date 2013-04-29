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
using System.Runtime.InteropServices;


namespace KWpf
{   /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "In a full-fledged application, the SpeechRecognitionEngine object should be properly disposed. For the sake of simplicity, we're omitting that code in this sample.")]
    public partial class PaintWindow : Window
    {
        private const float ClickThreshold = 0.1f; //.2
        private const float SkeletonMaxX = 0.60f; //.6
        private const float SkeletonMaxY = 0.40f;
        private int bd = 0;
        DrawingAttributes inkAttributes = new DrawingAttributes();
        SoundPlayer player = new SoundPlayer();


        public PaintWindow()
        {
            InitializeComponent();
            Expansor.IsExpanded = true;
            this.Topmost = true;
            {
                this.Show();
                this.Focus();
            }
            player.SoundLocation = @"..\..\Sounds\Click.wav";
        }




        private void Window_Loaded(object sender, RoutedEventArgs e)
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
            Dibujo.UseCustomCursor = true;
            Dibujo.Cursor = Cursors.Cross;
            Dibujo.DefaultDrawingAttributes = inkAttributes;
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

                            bool leftClick;

                            // figure out whether the mouse button is down based on where the opposite hand is
                            if ((LeftHand.IsChecked.GetValueOrDefault() && jointRight.Position.Y > ClickThreshold) ||
                                    (!LeftHand.IsChecked.GetValueOrDefault() && jointLeft.Position.Y > ClickThreshold))
                            {
                                leftClick = true;
                                if (bd == 0)
                                {
                                    bd = 1;
                                }
                                else
                                {
                                    bd = 0;
                                }

                            }
                            else
                                leftClick = false;

                            Status.Text = cursorX + ", " + cursorY + ", " + leftClick;
                            Status2.Text = bd.ToString();
                            NativeMethods.SendMouseInput(cursorX, cursorY, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, leftClick);

                            return;
                        }
                    }
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


        #region Botones

        private void VPai_HB_Pinc_Click(object sender, EventArgs eventArgs)
        {
            inkAttributes.IsHighlighter = false;
            inkAttributes.Color = Colors.Black;
            Dibujo.EditingMode = InkCanvasEditingMode.Ink;
            inkAttributes.Height = 10;
            inkAttributes.Width = 10;
            Dibujo.DefaultDrawingAttributes = inkAttributes;
            VPai_L_SelectedTool.Text = "Pincel";
            Dibujo.UseCustomCursor = true;
            Dibujo.Cursor = Cursors.Cross;
            player.Play();
        }

        private void VPai_HB_Lapi_Click(object sender, EventArgs eventArgs)
        {
            inkAttributes.IsHighlighter = false;
            inkAttributes.Color = Colors.Black;
            Dibujo.EditingMode = InkCanvasEditingMode.Ink;
            inkAttributes.Height = 3;
            inkAttributes.Width = 3;
            Dibujo.DefaultDrawingAttributes = inkAttributes;
            VPai_L_SelectedTool.Text = "Lápiz";
            Dibujo.UseCustomCursor = true;
            Dibujo.Cursor = Cursors.Cross;
            player.Play();
        }

        private void VPai_HB_Goma_Click(object sender, EventArgs eventArgs)
        {
            inkAttributes.IsHighlighter = false;
            Dibujo.EraserShape = new RectangleStylusShape(10, 10);
            Dibujo.EditingMode = InkCanvasEditingMode.EraseByPoint;
            inkAttributes.Height = 10;
            inkAttributes.Width = 10;
            Dibujo.DefaultDrawingAttributes = inkAttributes;
            VPai_L_SelectedTool.Text = "Goma";
            Dibujo.UseCustomCursor = true;
            Dibujo.Cursor = Cursors.Cross;
            player.Play();
        }

        private void VPai_HB_Borr_Click(object sender, EventArgs eventArgs)
        {
            inkAttributes.IsHighlighter = false;
            Dibujo.EraserShape = new RectangleStylusShape(10, 10);
            Dibujo.EditingMode = InkCanvasEditingMode.EraseByStroke;
            inkAttributes.Height = 10;
            inkAttributes.Width = 10;
            Dibujo.DefaultDrawingAttributes = inkAttributes;
            VPai_L_SelectedTool.Text = "Borrar";
            Dibujo.UseCustomCursor = true;
            Dibujo.Cursor = Cursors.Cross;
            player.Play();
        }


        private void VPai_HB_Resa_Click(object sender, EventArgs eventArgs)
        {
            inkAttributes.IsHighlighter = true;
            Dibujo.EditingMode = InkCanvasEditingMode.Ink;
            inkAttributes.Color = Colors.Yellow;
            inkAttributes.Height = 10;
            inkAttributes.Width = 10;
            Dibujo.DefaultDrawingAttributes = inkAttributes;
            VPai_L_SelectedTool.Text = "Resaltador";
            Dibujo.UseCustomCursor = true;
            Dibujo.Cursor = Cursors.Cross;
            player.Play();
        }

        private void VPai_HB_Nuev_Click(object sender, EventArgs eventArgs)
        {
            player.SoundLocation = @"..\..\Sounds\Exclam.wav";
            Dibujo.Strokes.Clear();
            Dibujo.UseCustomCursor = true;
            Dibujo.Cursor = Cursors.Cross;
            player.Play();
            player.SoundLocation = @"..\..\Sounds\Click.wav";
        }

        private void VPai_HB_Guard_Click(object sender, EventArgs eventArgs)
        {
            String ruta = "";
            player.SoundLocation = @"..\..\Sounds\Harp.wav";
            double width = Dibujo.ActualWidth;
            double height = Dibujo.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(Dibujo);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), new System.Windows.Size(width, height)));
            }
            bmpCopied.Render(dv);
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                // from System.Media.BitmapImage to System.Drawing.Bitmap 
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bmpCopied));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }

            EncoderParameter qualityParam =
         new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 85L);

            // Jpeg image codec
            ImageCodecInfo jpegCodec = getEncoderInfo("image/jpeg");

            if (jpegCodec == null)
                return;

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;
            Bitmap btm = new Bitmap(bitmap);
            bitmap.Dispose();
            getpath();
            ruta = @"..\..\Saved\" + getpath();
            btm.Save(ruta, jpegCodec, encoderParams);
            player.Play();
            MessageBox.Show("Guardado Exitoso En " + ruta);
            btm.Dispose();

            player.SoundLocation = @"..\..\Sounds\Click.wav";
        }
        #endregion Botones


        private ImageCodecInfo getEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        private String getpath()
        {
            String path = "";
            path = "KP_" + string.Format("{0:dd-MM-yyyy_hh-mm-ss}", DateTime.Now) + ".jpg";
            return (path);
        }

        private void Expansor2_Expanded(object sender, RoutedEventArgs e)
        {

        }

        private void B_SubKinect_Click(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser.Kinect.ElevationAngle = kinectSensorChooser.Kinect.ElevationAngle + 4;
        }

        private void B_BajKinect_Click(object sender, RoutedEventArgs e)
        {

            kinectSensorChooser.Kinect.ElevationAngle = kinectSensorChooser.Kinect.ElevationAngle - 4;
        }


    }
}
