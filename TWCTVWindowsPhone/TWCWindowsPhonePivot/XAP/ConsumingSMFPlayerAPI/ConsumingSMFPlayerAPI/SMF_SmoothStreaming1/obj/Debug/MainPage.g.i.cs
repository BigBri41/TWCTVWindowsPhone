﻿#pragma checksum "C:\programs\WindowsPhoneAppsNew\TimeWarnerMyTV\TWCWindowsPhonePivot\XAP\ConsumingSMFPlayerAPI\ConsumingSMFPlayerAPI\SMF_SmoothStreaming1\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E8C95B2AE975F862DAD73F4E639F2A27"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.2012
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace SMF_SmoothStreaming1 {
    
    
    public partial class MainPage : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Button btnPlay;
        
        internal System.Windows.Controls.Button btnReplay;
        
        internal System.Windows.Controls.Button btnRW;
        
        internal System.Windows.Controls.Button btnFF;
        
        internal System.Windows.Controls.Button btnPreviousPlaylistItem;
        
        internal System.Windows.Controls.Button btnNextPlaylistItem;
        
        internal System.Windows.Controls.Primitives.ToggleButton btnToggleCaptions;
        
        internal System.Windows.Controls.Primitives.ToggleButton btnFullScreen;
        
        internal Microsoft.SilverlightMediaFramework.Core.SMFPlayer player;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/SMF_SmoothStreaming1;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.btnPlay = ((System.Windows.Controls.Button)(this.FindName("btnPlay")));
            this.btnReplay = ((System.Windows.Controls.Button)(this.FindName("btnReplay")));
            this.btnRW = ((System.Windows.Controls.Button)(this.FindName("btnRW")));
            this.btnFF = ((System.Windows.Controls.Button)(this.FindName("btnFF")));
            this.btnPreviousPlaylistItem = ((System.Windows.Controls.Button)(this.FindName("btnPreviousPlaylistItem")));
            this.btnNextPlaylistItem = ((System.Windows.Controls.Button)(this.FindName("btnNextPlaylistItem")));
            this.btnToggleCaptions = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("btnToggleCaptions")));
            this.btnFullScreen = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("btnFullScreen")));
            this.player = ((Microsoft.SilverlightMediaFramework.Core.SMFPlayer)(this.FindName("player")));
        }
    }
}
