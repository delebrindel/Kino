﻿#pragma checksum "..\..\ArdWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5C730604534BBB0E5F91CD1D21D72801"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17626
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Kinect.Toolkit;
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


namespace Kino {
    
    
    /// <summary>
    /// ArdWindow
    /// </summary>
    public partial class ArdWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\ArdWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Kino.ArdWindow Arduino;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\ArdWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid layoutGrid;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\ArdWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.StatusBar BotonesArd;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\ArdWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button VArd_B_OPort;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\ArdWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button VArd_B_CPort;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\ArdWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button VArd_B_HLed;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\ArdWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button VArd_B_LLed;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\ArdWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button VArd_B_BLed;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\ArdWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button VArd_B_FBLed;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\ArdWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.StatusBar ControlesKin;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\ArdWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock VArd_LS_Port;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\ArdWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock VArd_L_Port;
        
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
            System.Uri resourceLocater = new System.Uri("/Kino;component/ardwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ArdWindow.xaml"
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
            this.Arduino = ((Kino.ArdWindow)(target));
            
            #line 5 "..\..\ArdWindow.xaml"
            this.Arduino.Loaded += new System.Windows.RoutedEventHandler(this.WindowLoaded);
            
            #line default
            #line hidden
            
            #line 5 "..\..\ArdWindow.xaml"
            this.Arduino.Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.layoutGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.BotonesArd = ((System.Windows.Controls.Primitives.StatusBar)(target));
            return;
            case 4:
            this.VArd_B_OPort = ((System.Windows.Controls.Button)(target));
            
            #line 49 "..\..\ArdWindow.xaml"
            this.VArd_B_OPort.Click += new System.Windows.RoutedEventHandler(this.Arduino_ButtonHandler);
            
            #line default
            #line hidden
            return;
            case 5:
            this.VArd_B_CPort = ((System.Windows.Controls.Button)(target));
            
            #line 50 "..\..\ArdWindow.xaml"
            this.VArd_B_CPort.Click += new System.Windows.RoutedEventHandler(this.Arduino_ButtonHandler);
            
            #line default
            #line hidden
            return;
            case 6:
            this.VArd_B_HLed = ((System.Windows.Controls.Button)(target));
            
            #line 51 "..\..\ArdWindow.xaml"
            this.VArd_B_HLed.Click += new System.Windows.RoutedEventHandler(this.Arduino_ButtonHandler);
            
            #line default
            #line hidden
            return;
            case 7:
            this.VArd_B_LLed = ((System.Windows.Controls.Button)(target));
            
            #line 52 "..\..\ArdWindow.xaml"
            this.VArd_B_LLed.Click += new System.Windows.RoutedEventHandler(this.Arduino_ButtonHandler);
            
            #line default
            #line hidden
            return;
            case 8:
            this.VArd_B_BLed = ((System.Windows.Controls.Button)(target));
            
            #line 53 "..\..\ArdWindow.xaml"
            this.VArd_B_BLed.Click += new System.Windows.RoutedEventHandler(this.Arduino_ButtonHandler);
            
            #line default
            #line hidden
            return;
            case 9:
            this.VArd_B_FBLed = ((System.Windows.Controls.Button)(target));
            
            #line 54 "..\..\ArdWindow.xaml"
            this.VArd_B_FBLed.Click += new System.Windows.RoutedEventHandler(this.Arduino_ButtonHandler);
            
            #line default
            #line hidden
            return;
            case 10:
            this.ControlesKin = ((System.Windows.Controls.Primitives.StatusBar)(target));
            return;
            case 11:
            this.VArd_LS_Port = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 12:
            this.VArd_L_Port = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
