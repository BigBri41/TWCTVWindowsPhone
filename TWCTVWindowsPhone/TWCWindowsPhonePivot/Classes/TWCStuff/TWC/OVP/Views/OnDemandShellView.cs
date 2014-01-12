namespace TWC.OVP.Views
{
    using Caliburn.Micro;
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using TWC.OVP.Controls;
    using TWC.OVP.Framework.Controls;
    using TWC.OVP.ViewModels;

    public class OnDemandShellView : UserControl
    {
        private bool _contentLoaded;
        internal BubbleContentControl assetInfoContentControl;
        internal VisualStateGroup AssetInfoStates;
        internal ContentControl AssetViewer;
        internal Rectangle BackgroundRectangle;
        internal BubbleContentControl captionBubble;
        internal ContentControl CaptionSettings;
        internal VisualStateGroup CaptionSettingsPopupStates;
        internal VisualStateGroup CaptionSettingsStates;
        internal ActionMessage ClickToggleControlBarAction;
        internal OnDemandController controller;
        internal VisualState HideAssetInfo;
        internal VisualState HideCaptionSettings;
        internal VisualState HideSettingsBubble;
        internal Storyboard InfoPanelIn;
        internal ContentControl Interaction;
        internal Grid LayoutRoot;
        internal VisualState ShowAssetInfo;
        internal VisualState ShowCaptionSettings;
        internal VisualState ShowSettingsBubble;
        internal VisualState ShowSmallAssetInfoPopupBubble;
        internal UserControl userControl;

        public OnDemandShellView()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Views/Shell/OnDemandShellView.xaml", UriKind.Relative));
                this.userControl = (UserControl) base.FindName("userControl");
                this.LayoutRoot = (Grid) base.FindName("LayoutRoot");
                this.AssetInfoStates = (VisualStateGroup) base.FindName("AssetInfoStates");
                this.ShowAssetInfo = (VisualState) base.FindName("ShowAssetInfo");
                this.InfoPanelIn = (Storyboard) base.FindName("InfoPanelIn");
                this.HideAssetInfo = (VisualState) base.FindName("HideAssetInfo");
                this.ShowSmallAssetInfoPopupBubble = (VisualState) base.FindName("ShowSmallAssetInfoPopupBubble");
                this.CaptionSettingsStates = (VisualStateGroup) base.FindName("CaptionSettingsStates");
                this.ShowCaptionSettings = (VisualState) base.FindName("ShowCaptionSettings");
                this.HideCaptionSettings = (VisualState) base.FindName("HideCaptionSettings");
                this.CaptionSettingsPopupStates = (VisualStateGroup) base.FindName("CaptionSettingsPopupStates");
                this.ShowSettingsBubble = (VisualState) base.FindName("ShowSettingsBubble");
                this.HideSettingsBubble = (VisualState) base.FindName("HideSettingsBubble");
                this.BackgroundRectangle = (Rectangle) base.FindName("BackgroundRectangle");
                this.AssetViewer = (ContentControl) base.FindName("AssetViewer");
                this.ClickToggleControlBarAction = (ActionMessage) base.FindName("ClickToggleControlBarAction");
                this.controller = (OnDemandController) base.FindName("controller");
                this.assetInfoContentControl = (BubbleContentControl) base.FindName("assetInfoContentControl");
                this.captionBubble = (BubbleContentControl) base.FindName("captionBubble");
                this.CaptionSettings = (ContentControl) base.FindName("CaptionSettings");
                this.Interaction = (ContentControl) base.FindName("Interaction");
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                Caliburn.Micro.Action.Invoke(base.DataContext, "TogglePlay", null, null, null, null);
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        public LiveTVShellViewModel DesignerContext
        {
            get
            {
                return null;
            }
        }
    }
}

