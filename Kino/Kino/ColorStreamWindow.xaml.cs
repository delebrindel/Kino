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

namespace Kino
{
    /// <summary>
    /// Interaction logic for ColorStreamWindow.xaml
    /// </summary>
    public partial class ColorStreamWindow : Window
    {        
        public ColorStreamWindow()
        {
            InitializeComponent();
            CStream.Start640x480CStream();
            ControlUpd();
        }


        private void ButtonHandler(object sender, RoutedEventArgs e)
        {


            Button b = (Button)sender;
            String nombreboton = b.Name;
            switch (nombreboton)
            {
                case "VP_B_OPort":  MessageBox.Show("Ya está abierto el puerto!!");
                                    ControlUpd();
                                    break;
            }

            /*
            */
        }


        //Update Controls
        private void ControlUpd()
        {
            if (CStream.sensor != null)
            {
                VP_L_Alt.Text = CStream.sensor.ElevationAngle.ToString();
            }

        }
        
        private void ButtonUpClick(object sender, RoutedEventArgs e)
        {
            if (CStream.sensor.ElevationAngle < 23)
                CStream.sensor.ElevationAngle += 5;
            else
                MessageBox.Show("No Se Puede Subir Más");         
            ControlUpd();
        }

        private void ButtonDownClick(object sender, RoutedEventArgs e)
        {
            if (CStream.sensor.ElevationAngle > -23)
                CStream.sensor.ElevationAngle -= 5;
            else
                MessageBox.Show("No Se Puede Bajar Más");
            ControlUpd();
        }

        private void ButtonCenterClick(object sender, RoutedEventArgs e)
        {
            if (CStream.sensor.ElevationAngle != 0)
                CStream.sensor.ElevationAngle = 0;
            else
                MessageBox.Show("El Kinect Se Encuentra Centrado");
            ControlUpd();
        }
        // Al Cerrar La Ventana, Deshacerse Del Sensor
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != CStream.sensor)
            {
                CStream.sensor.Stop();
            }
            MainWindow VP = new MainWindow();
            VP.Show();

        }
    }
}
