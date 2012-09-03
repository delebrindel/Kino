using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Threading;

namespace KinoControlLib
{
    /// <summary>
    /// Interaction logic for Button.xaml
    /// </summary>
    public partial class KinoHoverButton : UserControl
    {
        /// <summary>
        /// Timer Interval
        /// </summary>
        public int Time
        {
            get { return (int)this.GetValue(TimeProperty); }
            set { this.SetValue(TimeProperty, value); }
        }
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(
            "Time", typeof(int), typeof(KinoHoverButton), new PropertyMetadata((int)3));

        /// <summary>
        /// Fill Color
        /// </summary>
        public Brush PathFill
        {
            get { return (Brush)this.GetValue(PathFillProperty); }
            set { this.SetValue(PathFillProperty, value); }
        }
        public static readonly DependencyProperty PathFillProperty = DependencyProperty.Register(
            "PathFill", typeof(Brush), typeof(KinoHoverButton), new PropertyMetadata(Brushes.Yellow));

        /// <summary>
        /// Display Text Color
        /// </summary>
        public Brush DisplayTextColor
        {
            get { return (Brush)this.GetValue(DisplayTextColorProperty); }
            set { this.SetValue(DisplayTextColorProperty, value); }
        }
        public static readonly DependencyProperty DisplayTextColorProperty = DependencyProperty.Register(
            "DisplayTextColor", typeof(Brush), typeof(KinoHoverButton), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// Display Text
        /// </summary>
        public string DisplayText
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }
        public double DisplayTextSize
        {
            get { return (double)this.GetValue(FontSizeProperty); }
            set { this.SetValue(FontSizeProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "DisplayText", typeof(string), typeof(KinoHoverButton), new PropertyMetadata("Display Text"));

        public delegate void ClickHandler(object sender, EventArgs eventArgs);
        public event ClickHandler Click;

        private const double MaxFillHeight = 121;
        private const double MinFillHeight = 8;
        private Duration _duration;
        private readonly Duration ReverseDuration = new Duration(new TimeSpan(0, 0, 0, 1));
        private DoubleAnimation da;

        public KinoHoverButton()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        void StartFill()
        {
            da = new DoubleAnimation(Mask.ActualHeight, MinFillHeight, _duration);
            da.Completed += new EventHandler(da_Completed);
            Mask.BeginAnimation(Canvas.HeightProperty, da);
        }

        void da_Completed(object sender, EventArgs e)
        {
            if (Click != null)
                Click(sender, e);
            Mask.BeginAnimation(Canvas.HeightProperty, null);
        }

        void RemoveFill()
        {
            da.Completed -= da_Completed;
            da = new DoubleAnimation(Mask.ActualHeight, MaxFillHeight, ReverseDuration);
            Mask.BeginAnimation(Canvas.HeightProperty, da);
        }

        private void SrKinectButtonMouseEnter(object sender, MouseEventArgs e)
        {
            StartFill();
        }

        private void SrKinectButtonMouseLeave(object sender, MouseEventArgs e)
        {
            RemoveFill();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this._duration = new Duration(new TimeSpan(0, 0, 0, this.Time));
        }
    }
}