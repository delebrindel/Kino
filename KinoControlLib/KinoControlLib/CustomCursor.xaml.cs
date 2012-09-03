

using System;
using System.Windows;
using System.Windows.Controls;
namespace KinoControlLib
{

    public partial class CustomCursor : UserControl
    {

        public CustomCursor()
        {

            InitializeComponent();

        }



        public void SetCursor(string resource)
        {

            Uri uri = new Uri(resource, UriKind.Relative);

            MyCursor.Source = new System.Windows.Media.Imaging.BitmapImage(uri);

        }



        public void MoveTo(Point pt)
        {

            this.SetValue(Canvas.LeftProperty, pt.X);

            this.SetValue(Canvas.TopProperty, pt.Y);

        }

    }

}

