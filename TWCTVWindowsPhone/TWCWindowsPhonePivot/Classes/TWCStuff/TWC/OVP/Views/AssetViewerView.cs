namespace TWC.OVP.Views
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using TWC.OVP.Controls;
    using TWC.OVP.ViewModels;

    public class AssetViewerView : UserControl
    {
        private bool _contentLoaded;
        internal LoadingPanelControl assetLoadingControl;
        internal ErrorMessageBox ErrorMessage;
        internal VisualState ErrorMessageNotShown;
        internal VisualState ErrorMessageShown;
        internal VisualStateGroup ErrorStates;
        internal Grid LayoutRoot;
        internal VisualState Loading;
        internal VisualStateGroup LoadingStates;
        internal VisualState NotLoading;
        internal ContentControl Player;

        public AssetViewerView()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Views/AssetViewerView.xaml", UriKind.Relative));
                this.LayoutRoot = (Grid) base.FindName("LayoutRoot");
                this.ErrorStates = (VisualStateGroup) base.FindName("ErrorStates");
                this.ErrorMessageShown = (VisualState) base.FindName("ErrorMessageShown");
                this.ErrorMessageNotShown = (VisualState) base.FindName("ErrorMessageNotShown");
                this.LoadingStates = (VisualStateGroup) base.FindName("LoadingStates");
                this.Loading = (VisualState) base.FindName("Loading");
                this.NotLoading = (VisualState) base.FindName("NotLoading");
                this.Player = (ContentControl) base.FindName("Player");
                this.assetLoadingControl = (LoadingPanelControl) base.FindName("assetLoadingControl");
                this.ErrorMessage = (ErrorMessageBox) base.FindName("ErrorMessage");
            }
        }

        public AssetViewerViewModel DesignerContext
        {
            get
            {
                return null;
            }
        }
    }
}

