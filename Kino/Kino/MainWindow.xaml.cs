//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Kino
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Threading;
    using System.Windows.Controls;
 
    
    /// Interaction logic for MainWindow.xaml
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            KMouse.StartKinoMouse();
        }

        
        /// Execute startup tasks
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void ButtonHandler(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            String nombreboton = b.Name;
            switch (nombreboton)
            {
                case "VP_B_OColorStream":
                                            ColorStreamWindow VC = new ColorStreamWindow();
                                            VC.Show();
                                            this.Close();
                                            break;
                case "VP_B_OSkeletonStream":
                                            SkeletonStreamWindow VS = new SkeletonStreamWindow();
                                            VS.Show();
                                            this.Close();
                                            break;
                case "VP_B_OCSSStream":
                                            SkeletonCSWindow VCS = new SkeletonCSWindow();
                                            VCS.Show();
                                            this.Close();
                                            break;
                case "VP_B_OArd":
                                            ArdWindow VA = new ArdWindow();
                                            VA.Show();
                                            this.Close();
                                            break;
                case "VP_B_OPai":
                                            PaintWindow VP = new PaintWindow();
                                            VP.Show();
                                            this.Close();
                                            break;
                case "VP_B_OSpe":
                                            Speech VSs = new Speech();
                                            VSs.Show();
                                            this.Close();
                                            break;
                case "VP_B_OCSArd":
                                            CSArd VCI = new CSArd();
                                            VCI.Show();
                                            this.Close();
                                            break;
                case "VP_B_OGes":
                                            GesturesWindow VG = new GesturesWindow();
                                            VG.Show();
                                            this.Close();
                                            break;
                case "VP_B_OPia":
                                            Piano VPi = new Piano();
                                            VPi.Show();
                                            this.Close();
                                            break;
                case "VP_B_OPru":
                                            Tests VPr = new Tests();
                                            VPr.Show();
                                            this.Close();
                                            break;
            }

            /*
            */
        }

        private void KinoHoverButton_Click(object sender, EventArgs eventArgs)
        {
            TestingMouse Ma = new TestingMouse();
            Ma.Show();
            this.Close();
        }

    }
}