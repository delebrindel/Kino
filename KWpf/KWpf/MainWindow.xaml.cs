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
    using System.Windows.Controls;


    /// Interaction logic for MainWindow.xaml

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            KMouse.StartKinoMouse();
        }


        private void KinoHoverButtonPa_Click(object sender, EventArgs eventArgs)
        {
            PaintWindow VP = new PaintWindow();
            VP.Show();
            this.Close();
        }

        private void KinoHoverButtonSp_Click(object sender, EventArgs eventArgs)
        {
            Speech VS = new Speech();
            VS.Show();
            this.Close();
        }

        private void KinoHoverButtonMt_Click(object sender, EventArgs eventArgs)
        {
            MTouch VMt = new MTouch();
            KMouse.KMouseOn = 0;
            VMt.Show();
            this.Close();
        }
        private void KinoHoverButtonPp_Click(object sender, EventArgs eventArgs)
        {
            PPControl VPp = new PPControl();
            KMouse.KMouseOn = 0;
            VPp.Show();
            this.Close();
        }
        private void KinoHoverButtonPia_Click(object sender, EventArgs eventArgs)
        {
            Movement VPi = new Movement();
            KMouse.KMouseOn = 0;
            VPi.Show();
            this.Close();
        }
        private void KinoHoverButtonMvm_Click(object sender, EventArgs eventArgs)
        {
            Movement VMv = new Movement();
            KMouse.KMouseOn = 0;
            VMv.Show();
            this.Close();
        }
        private void KinoHoverButtonIns_Click(object sender, EventArgs eventArgs)
        {
            Instrument VIn = new Instrument();
            KMouse.KMouseOn = 0;
            VIn.Show();
            this.Close();
        }
    }
}