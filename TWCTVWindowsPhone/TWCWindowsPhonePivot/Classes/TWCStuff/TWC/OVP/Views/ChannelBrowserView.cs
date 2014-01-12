namespace TWC.OVP.Views
{
    using Caliburn.Micro;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using TWC.OVP;
    using TWC.OVP.Controls;
    using TWC.OVP.Framework.Helpers;
    using TWC.OVP.Messages;
    using TWC.OVP.Utilities;
    using TWC.OVP.ViewModels;

    public class ChannelBrowserView : UserControl
    {
        private ScrollViewer _ChannelBrowserScrollViewer;
        private bool _contentLoaded;
        internal VisualState AllChannels;
        internal ListBox AllChannelsListBox;
        internal TextBlock BrowsingText;
        internal VisualStateGroup DisplayStates;
        internal Grid HeaderGRID;
        internal Rectangle ListBackground;
        internal Grid ListGrid;
        internal Button MainButton;
        internal Rectangle MainButtonBackground;
        internal VisualState NoResults;
        internal TextBlock NoResultsTextBlock;
        internal VisualState OnNext;
        internal RadioButton OnNextRadioButton;
        internal Grid OnNextRadioGrid;
        internal VisualState OnNow;
        internal RadioButton OnNowRadioButton;
        internal VisualStateGroup OnStates;
        internal VisualState Options;
        internal Grid OptionsGrid;
        internal ListBox OptionsListBox;
        internal VisualState OutOfHome;
        internal CustomKeyboardListBox OutOfHomeListBox;
        internal Grid RadialGrid;
        internal VisualState RecentHistory;
        internal ListBox RecentHistoryListBox;
        internal Grid Root;
        internal Grid SearchGrid;
        internal Grid SearchGridOuter;
        internal UserControl userControl;
        internal TWC.OVP.WatermarkTextBox WatermarkTextBox;

        public ChannelBrowserView()
        {
            this.InitializeComponent();
            this.AllChannelsListBox.ApplyTemplate();
            this.AllChannelsListBox.Loaded += new RoutedEventHandler(this.AllChannelsListBox_Loaded);
            base.add_DataContextChanged(new DependencyPropertyChangedEventHandler(this.ChannelBrowserView_DataContextChanged));
            this.OutOfHomeListBox.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OutOfHomeListBox_MouseLeftButtonDown), true);
            this.OptionsListBox.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OptionsListBox_MouseLeftButtonDown), true);
        }

        private void AllChannelsListBox_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrllVeiw = this.GetScrllVeiw(this.AllChannelsListBox);
            BindingHelper.RegisterForNotification("VerticalOffset", scrllVeiw, new PropertyChangedCallback(this.OnVerticalOffsetChanged));
        }

        private void ChannelBrowserView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyChangedBase) base.DataContext).PropertyChanged += new PropertyChangedEventHandler(this.ChannelBrowserView_PropertyChanged);
        }

        private void ChannelBrowserView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            if (propertyName != null)
            {
                if (!(propertyName == "CurrentChannel"))
                {
                    if (!(propertyName == "FilteredChannels"))
                    {
                        return;
                    }
                }
                else
                {
                    this.ScrollCurrentChannelIntoView();
                    return;
                }
                if (this.ViewModel.FilteredChannels.Contains(this.ViewModel.CurrentChannel))
                {
                    this.ScrollCurrentChannelIntoView();
                }
                else
                {
                    this.ScrollToTop();
                }
            }
        }

        private ScrollViewer GetScrllVeiw(ListBox listBox)
        {
            Grid child = VisualTreeHelper.GetChild(listBox, 0) as Grid;
            Border border = VisualTreeHelper.GetChild(child, 0) as Border;
            this._ChannelBrowserScrollViewer = border.FindName("ScrollViewer") as ScrollViewer;
            return this._ChannelBrowserScrollViewer;
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Views/ChannelBrowserView.xaml", UriKind.Relative));
                this.userControl = (UserControl) base.FindName("userControl");
                this.Root = (Grid) base.FindName("Root");
                this.DisplayStates = (VisualStateGroup) base.FindName("DisplayStates");
                this.AllChannels = (VisualState) base.FindName("AllChannels");
                this.RecentHistory = (VisualState) base.FindName("RecentHistory");
                this.Options = (VisualState) base.FindName("Options");
                this.NoResults = (VisualState) base.FindName("NoResults");
                this.OutOfHome = (VisualState) base.FindName("OutOfHome");
                this.OnStates = (VisualStateGroup) base.FindName("OnStates");
                this.OnNow = (VisualState) base.FindName("OnNow");
                this.OnNext = (VisualState) base.FindName("OnNext");
                this.MainButtonBackground = (Rectangle) base.FindName("MainButtonBackground");
                this.ListBackground = (Rectangle) base.FindName("ListBackground");
                this.HeaderGRID = (Grid) base.FindName("HeaderGRID");
                this.MainButton = (Button) base.FindName("MainButton");
                this.BrowsingText = (TextBlock) base.FindName("BrowsingText");
                this.SearchGridOuter = (Grid) base.FindName("SearchGridOuter");
                this.SearchGrid = (Grid) base.FindName("SearchGrid");
                this.WatermarkTextBox = (TWC.OVP.WatermarkTextBox) base.FindName("WatermarkTextBox");
                this.OnNextRadioGrid = (Grid) base.FindName("OnNextRadioGrid");
                this.OnNowRadioButton = (RadioButton) base.FindName("OnNowRadioButton");
                this.OnNextRadioButton = (RadioButton) base.FindName("OnNextRadioButton");
                this.ListGrid = (Grid) base.FindName("ListGrid");
                this.AllChannelsListBox = (ListBox) base.FindName("AllChannelsListBox");
                this.RecentHistoryListBox = (ListBox) base.FindName("RecentHistoryListBox");
                this.OutOfHomeListBox = (CustomKeyboardListBox) base.FindName("OutOfHomeListBox");
                this.OptionsGrid = (Grid) base.FindName("OptionsGrid");
                this.OptionsListBox = (ListBox) base.FindName("OptionsListBox");
                this.RadialGrid = (Grid) base.FindName("RadialGrid");
                this.NoResultsTextBlock = (TextBlock) base.FindName("NoResultsTextBlock");
            }
        }

        public void OnVerticalOffsetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((App) Application.Current).EventAggregator.Publish(new ChannelBrowserScrollMessage(Convert.ToInt32((double) e.NewValue)));
        }

        private void OptionsListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement originalSource = e.OriginalSource as FrameworkElement;
            if ((originalSource != null) && (VisualTreeHelperEx.FindAncestor(originalSource, "OptionListItemContainerRoot") != null))
            {
                this.ViewModel.FilterSelectionChanged();
            }
        }

        private void OutOfHomeListBox_CustomKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            switch (e.Key)
            {
                case Key.Up:
                    this.ViewModel.PreviousChannel();
                    return;

                case Key.Right:
                    break;

                case Key.Down:
                    if (!this.ViewModel.NextChannel())
                    {
                        ((App) Application.Current).EventAggregator.Publish(new ErrorMessage(MessageText.TitleOOH, MessageText.OOHChannelNotAvailable, null, ErrorMessageType.Message, 0x2710, null, true));
                    }
                    break;

                default:
                    return;
            }
        }

        private void OutOfHomeListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement) e.OriginalSource).Name == "")
            {
                ((App) Application.Current).EventAggregator.Publish(new ErrorMessage(MessageText.TitleOOH, MessageText.OOHChannelNotAvailable, null, ErrorMessageType.Message, 0x2710, null, true));
            }
        }

        private void ScrollCurrentChannelIntoView()
        {
            this.AllChannelsListBox.ScrollIntoView(this.ViewModel.CurrentChannel);
            this.OutOfHomeListBox.ScrollIntoView(this.ViewModel.CurrentChannel);
            this.RecentHistoryListBox.ScrollIntoView(this.ViewModel.CurrentChannel);
        }

        private void ScrollToTop()
        {
            if (this.AllChannelsListBox.Items.Count > 0)
            {
                try
                {
                    this.AllChannelsListBox.UpdateLayout();
                    this.AllChannelsListBox.ScrollIntoView(this.AllChannelsListBox.Items[0]);
                }
                catch (Exception)
                {
                }
            }
        }

        public ChannelBrowserViewModel ViewModel
        {
            get
            {
                return (base.DataContext as ChannelBrowserViewModel);
            }
        }
    }
}

