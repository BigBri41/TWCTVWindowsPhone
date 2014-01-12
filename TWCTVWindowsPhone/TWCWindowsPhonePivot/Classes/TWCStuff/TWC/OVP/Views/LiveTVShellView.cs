namespace TWC.OVP.Views
{
    using Caliburn.Micro;
    using Microsoft.Expression.Interactivity.Core;
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using TWC.OVP.Controls;
    using TWC.OVP.Framework.Controls;
    using TWC.OVP.ViewModels;

    public class LiveTVShellView : UserControl
    {
        private ListBox _allChannelsListBox;
        private bool _contentLoaded;
        internal AssetInfoBubbleContents assetInfoBubbleContents;
        internal BubbleContentControl assetInfoContentControl;
        internal VisualStateGroup AssetInfoStates;
        internal ContentControl AssetViewer;
        internal Rectangle BackgroundRectangle;
        internal BubbleContentControl captionBubble;
        internal ContentControl CaptionSettings;
        internal VisualStateGroup CaptionSettingsPopupStates;
        internal VisualStateGroup CaptionSettingsStates;
        internal Storyboard CBScrollIn;
        internal Storyboard CBScrollOut;
        internal ChannelBrowserView ChannelBrowser;
        internal Grid ChannelBrowserGrid;
        internal VisualStateGroup ChannelBrowserStates;
        internal ActionMessage ClickHideChannelBrowserAction;
        internal OnDemandController controller;
        internal VisualState HideAssetInfo;
        internal VisualState HideCaptionSettings;
        internal VisualState HideChannelBrowser;
        internal VisualState HideSettingsBubble;
        internal Storyboard InfoPanelIn;
        internal ContentControl Interaction;
        internal Grid LayoutRoot;
        internal ActionMessage MouseMoveShowChannelBrowserAction;
        internal CallMethodAction MouseMoveShowControlerAction;
        internal Rectangle rectangle;
        internal VisualState ShowAssetInfo;
        internal VisualState ShowCaptionSettings;
        internal VisualState ShowChannelBrowser;
        internal VisualState ShowSettingsBubble;
        internal VisualState ShowSmallAssetInfoPopupBubble;
        internal UserControl userControl;

        public LiveTVShellView()
        {
            this.InitializeComponent();
            base.Loaded += new RoutedEventHandler(this.LiveTvShellViewLoaded);
        }

        public static ListBox GetListBox(DependencyObject obj, string name)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                object child = VisualTreeHelper.GetChild(obj, i);
                if ((child.GetType().FullName == typeof(ListBox).FullName) && (((FrameworkElement) child).Name == name))
                {
                    return (ListBox) child;
                }
                object listBox = GetListBox((DependencyObject) child, name);
                if (listBox != null)
                {
                    return (ListBox) listBox;
                }
            }
            return null;
        }

        private void HandleListBoxItemsKeyDown(ListBox listBox)
        {
            foreach (object obj2 in listBox.Items)
            {
                ListBoxItem item = listBox.ItemContainerGenerator.ContainerFromItem(obj2) as ListBoxItem;
                if (item != null)
                {
                    item.KeyDown += new KeyEventHandler(this.ListBoxItemKeyDown);
                }
            }
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Views/Shell/LiveTVShellView.xaml", UriKind.Relative));
                this.userControl = (UserControl) base.FindName("userControl");
                this.LayoutRoot = (Grid) base.FindName("LayoutRoot");
                this.ChannelBrowserStates = (VisualStateGroup) base.FindName("ChannelBrowserStates");
                this.CBScrollIn = (Storyboard) base.FindName("CBScrollIn");
                this.ShowChannelBrowser = (VisualState) base.FindName("ShowChannelBrowser");
                this.CBScrollOut = (Storyboard) base.FindName("CBScrollOut");
                this.HideChannelBrowser = (VisualState) base.FindName("HideChannelBrowser");
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
                this.ClickHideChannelBrowserAction = (ActionMessage) base.FindName("ClickHideChannelBrowserAction");
                this.MouseMoveShowChannelBrowserAction = (ActionMessage) base.FindName("MouseMoveShowChannelBrowserAction");
                this.MouseMoveShowControlerAction = (CallMethodAction) base.FindName("MouseMoveShowControlerAction");
                this.ChannelBrowserGrid = (Grid) base.FindName("ChannelBrowserGrid");
                this.ChannelBrowser = (ChannelBrowserView) base.FindName("ChannelBrowser");
                this.rectangle = (Rectangle) base.FindName("rectangle");
                this.controller = (OnDemandController) base.FindName("controller");
                this.assetInfoContentControl = (BubbleContentControl) base.FindName("assetInfoContentControl");
                this.assetInfoBubbleContents = (AssetInfoBubbleContents) base.FindName("assetInfoBubbleContents");
                this.captionBubble = (BubbleContentControl) base.FindName("captionBubble");
                this.CaptionSettings = (ContentControl) base.FindName("CaptionSettings");
                this.Interaction = (ContentControl) base.FindName("Interaction");
            }
        }

        private void ListBoxItemKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                this.OnRightArrowOpenBrowser();
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                this.OnLeftArrowCloseBrowser();
                e.Handled = true;
            }
        }

        private void LiveTvShellViewChannelBrowserHiding(object sender, RoutedEventArgs e)
        {
            base.Focus();
            this.UnHandleListBoxItemsKeyDown(this._allChannelsListBox);
        }

        private void LiveTvShellViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                this.OnRightArrowOpenBrowser();
            }
            else if (e.Key == Key.Left)
            {
                this.OnLeftArrowCloseBrowser();
            }
            e.Handled = true;
        }

        private void LiveTvShellViewLoaded(object sender, RoutedEventArgs e)
        {
            this._allChannelsListBox = GetListBox(this.ChannelBrowser, "AllChannelsListBox");
            base.KeyDown += new KeyEventHandler(this.LiveTvShellViewKeyDown);
            LiveTVShellViewModel dataContext = base.DataContext as LiveTVShellViewModel;
            if (dataContext != null)
            {
                dataContext.ChannelBrowserHiding += new RoutedEventHandler(this.LiveTvShellViewChannelBrowserHiding);
                dataContext.ChannelBrowserShowing += new RoutedEventHandler(this.LiveTvShellViewModelChannelBrowserShowing);
            }
        }

        private void LiveTvShellViewModelChannelBrowserShowing(object sender, RoutedEventArgs e)
        {
            this.HandleListBoxItemsKeyDown(this._allChannelsListBox);
        }

        private void OnLeftArrowCloseBrowser()
        {
            if (base.DataContext != null)
            {
                Caliburn.Micro.Action.Invoke(base.DataContext, "HideChannelBrowser", null, null, null, null);
            }
            if (this.controller != null)
            {
                Caliburn.Micro.Action.Invoke(this.controller, "HideController", null, null, null, null);
            }
        }

        private void OnRightArrowOpenBrowser()
        {
            if (base.DataContext != null)
            {
                Caliburn.Micro.Action.Invoke(base.DataContext, "ShowChannelBrowserWithTimeout", null, null, null, null);
            }
            if (this.controller != null)
            {
                Caliburn.Micro.Action.Invoke(this.controller, "ShowControllerWithTimeout", null, null, null, null);
            }
        }

        private void UnHandleListBoxItemsKeyDown(ListBox listBox)
        {
            foreach (object obj2 in listBox.Items)
            {
                ListBoxItem item = listBox.ItemContainerGenerator.ContainerFromItem(obj2) as ListBoxItem;
                if (item != null)
                {
                    item.KeyDown -= new KeyEventHandler(this.ListBoxItemKeyDown);
                }
            }
        }
    }
}

