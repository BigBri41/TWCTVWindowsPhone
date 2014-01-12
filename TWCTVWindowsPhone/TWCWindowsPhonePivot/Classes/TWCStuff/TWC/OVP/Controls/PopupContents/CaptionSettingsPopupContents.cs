namespace TWC.OVP.Controls.PopupContents
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using TWC.OVP.Framework.Controls;

    public class CaptionSettingsPopupContents : UserControl
    {
        private bool _contentLoaded;
        internal TextToggleButton CaptionsToggle;
        internal Button ClosedCaptioningSettings;
        internal VisualState HideSettings;
        internal ListBox languageListBox;
        internal VisualStateGroup LanguageOptionStates;
        internal Rectangle MouseOverRect;
        internal Grid SettingsGrid;
        internal VisualState ShowSettings;
        internal UserControl userControl;

        public CaptionSettingsPopupContents()
        {
            this.InitializeComponent();
            base.Loaded += new RoutedEventHandler(this.CaptionSettingsPopupContentsLoaded);
        }

        private void CaptionSettingsPopupContentsLoaded(object sender, RoutedEventArgs e)
        {
            if (this.CaptionsToggle.IsChecked == true)
            {
                VisualStateManager.GoToState(this, "ShowSettings", true);
            }
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Controls/CaptionSettingsPopupContents.xaml", UriKind.Relative));
                this.userControl = (UserControl) base.FindName("userControl");
                this.LanguageOptionStates = (VisualStateGroup) base.FindName("LanguageOptionStates");
                this.ShowSettings = (VisualState) base.FindName("ShowSettings");
                this.HideSettings = (VisualState) base.FindName("HideSettings");
                this.MouseOverRect = (Rectangle) base.FindName("MouseOverRect");
                this.CaptionsToggle = (TextToggleButton) base.FindName("CaptionsToggle");
                this.SettingsGrid = (Grid) base.FindName("SettingsGrid");
                this.languageListBox = (ListBox) base.FindName("languageListBox");
                this.ClosedCaptioningSettings = (Button) base.FindName("ClosedCaptioningSettings");
            }
        }
    }
}

