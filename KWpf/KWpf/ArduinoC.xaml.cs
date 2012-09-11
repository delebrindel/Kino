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


    /// Interaction logic for MainWindow.xaml

    public partial class ArduinoC : Window
    {
        private SerialPort puertoArd;

        public ArduinoC()
        {
            InitializeComponent();
        }


        /// Execute startup tasks
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            puertoArd = new SerialPort();
            puertoArd.PortName = "COM3";
            puertoArd.BaudRate = 9600;
            ControlUpd();

        }

        private void Arduino_ButtonHandler(object sender, RoutedEventArgs e)
        {


            Button b = (Button)sender;
            String nombreboton = b.Name;
            switch (nombreboton)
            {
                case "VArd_B_OPort":
                    if (!puertoArd.IsOpen)
                        puertoArd.Open();
                    else
                        MessageBox.Show("Ya está abierto el puerto!!");
                    ControlUpd();
                    break;
                case "VArd_B_CPort":
                    if (puertoArd.IsOpen)
                        puertoArd.Close();
                    else
                        MessageBox.Show("El Puerto Está Cerrado!!");
                    ControlUpd();
                    break;
                case "VArd_B_HLed":
                    if (puertoArd.IsOpen)
                        puertoArd.Write("A");
                    else
                        MessageBox.Show("El Puerto Está Cerrado!!");
                    break;
                case "VArd_B_LLed":
                    if (puertoArd.IsOpen)
                        puertoArd.Write("B");
                    else
                        MessageBox.Show("El Puerto Está Cerrado!!");
                    break;
                case "VArd_B_BLed":
                    if (puertoArd.IsOpen)
                        puertoArd.Write("C");
                    else
                        MessageBox.Show("El Puerto Está Cerrado!!");
                    break;
                case "VArd_B_FBLed":
                    if (puertoArd.IsOpen)
                        puertoArd.Write("D");
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
            if (!puertoArd.IsOpen)
            {
                VArd_L_Port.Text = "Cerrado";
                VArd_B_CPort.IsEnabled = false;
                VArd_B_OPort.IsEnabled = true;
                VArd_B_HLed.IsEnabled = false;
                VArd_B_LLed.IsEnabled = false;
                VArd_B_BLed.IsEnabled = false;
                VArd_B_FBLed.IsEnabled = false;
            }
            else
            {
                VArd_L_Port.Text = "Abierto";
                VArd_B_OPort.IsEnabled = false;
                VArd_B_CPort.IsEnabled = true;
                VArd_B_HLed.IsEnabled = true;
                VArd_B_LLed.IsEnabled = true;
                VArd_B_BLed.IsEnabled = true;
                VArd_B_FBLed.IsEnabled = true;
            }
        }

        /// Execute shutdown tasks
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (puertoArd.IsOpen == true)
            {
                puertoArd.Close();
            }
            MainWindow VP = new MainWindow();
            VP.Show();

        }

    }
}