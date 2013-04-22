using System;
using System.Windows;
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


namespace KWpf
{   /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "In a full-fledged application, the SpeechRecognitionEngine object should be properly disposed. For the sake of simplicity, we're omitting that code in this sample.")]
    public partial class Paint : Window
    {
        private const float ClickThreshold = 0.1f; //.2
        private const float SkeletonMaxX = 0.60f; //.6
        private const float SkeletonMaxY = 0.40f;

        DrawingAttributes inkAttributes = new DrawingAttributes();
        SoundPlayer player = new SoundPlayer();


        public Paint()
        {
            InitializeComponent();
            Mouse.StartKinoMouse();
            this.Topmost = true;
            {
                this.Show();
                this.Focus();
            }
        }

        private void Pincel_Click(object sender, EventArgs eventArgs)
        {
            inkAttributes.IsHighlighter = false;
            inkAttributes.Color = Colors.Black;
            Dibujo.EditingMode = InkCanvasEditingMode.Ink;
            inkAttributes.Height = 10;
            inkAttributes.Width = 10;
            Dibujo.DefaultDrawingAttributes = inkAttributes;
            Dibujo.UseCustomCursor = true;
            Dibujo.Cursor = Cursors.Cross;
            player.Play();

        }

    }
}
