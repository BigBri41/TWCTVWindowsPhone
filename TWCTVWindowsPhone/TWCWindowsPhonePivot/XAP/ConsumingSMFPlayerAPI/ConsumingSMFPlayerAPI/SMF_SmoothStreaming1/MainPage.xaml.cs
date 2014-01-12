using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.SilverlightMediaFramework.Core;

namespace SMF_SmoothStreaming1
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            btnRW.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(btnRW_MouseLeftButtonDown), true);
            btnRW.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(btnRW_MouseLeftButtonUp), true);
            btnFF.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(btnFF_MouseLeftButtonDown), true);
            btnFF.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(btnFF_MouseLeftButtonUp), true);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            player.Play();
        }

        private void btnReplay_Click(object sender, RoutedEventArgs e)
        {
            player.Replay();
        }

        private void btnToggleCaptions_Click(object sender, RoutedEventArgs e)
        {
            player.CaptionsVisibility = btnToggleCaptions.IsChecked.Value
                                            ? FeatureVisibility.Visible
                                            : FeatureVisibility.Hidden;
        }

        private void btnNextPlaylistItem_Click(object sender, RoutedEventArgs e)
        {
            player.GoToNextPlaylistItem();
        }

        private void btnPreviousPlaylistItem_Click(object sender, RoutedEventArgs e)
        {
            player.GoToPreviousPlaylistItem();
        }

        private void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            player.IsFullScreen = btnFullScreen.IsChecked.Value;
        }

        private void btnFF_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            player.StartFastForward();
        }

        private void btnFF_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            player.StopFastForward();
        }

        private void btnRW_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            player.StartRewind();
        }

        private void btnRW_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            player.StopRewind();
        }
    }
}
