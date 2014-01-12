namespace TWC.OVP.Views
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using TWC.OVP.Framework.Controls;

    public class LiveAssetInfoView : UserControl
    {
        private bool _contentLoaded;
        internal VisualStateGroup AssetInfoLoadingStates;
        internal BubbleContentControl assetInfoPanel;
        internal BubbleContentControl assetInfoPanel_Copy;
        internal Grid LayoutRoot;
        internal Grid LoadedGrid;
        internal VisualState LoadedState;
        internal VisualState Loading;
        internal Grid LoadingGrid;
        internal LoadSpinnerControl Spinner;

        public LiveAssetInfoView()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Views/LiveAssetInfoView.xaml", UriKind.Relative));
                this.LayoutRoot = (Grid) base.FindName("LayoutRoot");
                this.AssetInfoLoadingStates = (VisualStateGroup) base.FindName("AssetInfoLoadingStates");
                this.Loading = (VisualState) base.FindName("Loading");
                this.LoadedState = (VisualState) base.FindName("LoadedState");
                this.LoadedGrid = (Grid) base.FindName("LoadedGrid");
                this.assetInfoPanel = (BubbleContentControl) base.FindName("assetInfoPanel");
                this.LoadingGrid = (Grid) base.FindName("LoadingGrid");
                this.assetInfoPanel_Copy = (BubbleContentControl) base.FindName("assetInfoPanel_Copy");
                this.Spinner = (LoadSpinnerControl) base.FindName("Spinner");
            }
        }
    }
}

