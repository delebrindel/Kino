﻿#pragma checksum "..\..\MainWindowsinmodif.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "88EAAEC190DEF71B09DF66D96EA7D1AD"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.17626
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Kinect.Toolkit;
using Microsoft.Samples.Kinect.WpfViewers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Kinppt {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\MainWindowsinmodif.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image videoImage;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\MainWindowsinmodif.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle recmanoizquierda;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\MainWindowsinmodif.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle recmanoderecha;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\MainWindowsinmodif.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Ellipse elipsecabeza;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\MainWindowsinmodif.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Kinect.Toolkit.KinectSensorChooserUI SensorChooserUI;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\MainWindowsinmodif.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.StatusBar BotonesKin;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\MainWindowsinmodif.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button VP_B_SubirKin;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\MainWindowsinmodif.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button VP_B_CentrarKin;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\MainWindowsinmodif.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button VP_B_BajarKin;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Kinppt;component/mainwindowsinmodif.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindowsinmodif.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.videoImage = ((System.Windows.Controls.Image)(target));
            return;
            case 2:
            this.recmanoizquierda = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 3:
            this.recmanoderecha = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 4:
            this.elipsecabeza = ((System.Windows.Shapes.Ellipse)(target));
            return;
            case 5:
            this.SensorChooserUI = ((Microsoft.Kinect.Toolkit.KinectSensorChooserUI)(target));
            return;
            case 6:
            this.BotonesKin = ((System.Windows.Controls.Primitives.StatusBar)(target));
            return;
            case 7:
            this.VP_B_SubirKin = ((System.Windows.Controls.Button)(target));
            
            #line 32 "..\..\MainWindowsinmodif.xaml"
            this.VP_B_SubirKin.Click += new System.Windows.RoutedEventHandler(this.ButtonUpClick);
            
            #line default
            #line hidden
            return;
            case 8:
            this.VP_B_CentrarKin = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\MainWindowsinmodif.xaml"
            this.VP_B_CentrarKin.Click += new System.Windows.RoutedEventHandler(this.ButtonCenterClick);
            
            #line default
            #line hidden
            return;
            case 9:
            this.VP_B_BajarKin = ((System.Windows.Controls.Button)(target));
            
            #line 34 "..\..\MainWindowsinmodif.xaml"
            this.VP_B_BajarKin.Click += new System.Windows.RoutedEventHandler(this.ButtonDownClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

