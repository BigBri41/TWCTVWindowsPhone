namespace TWC.OVP.Controls
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using TWC.OVP.Framework.Controls;

    public class ErrorMessageBox : UserControl
    {
        private bool _contentLoaded;
        internal Rectangle BackgroundRectangle;
        internal Button ButtonOK;
        internal Button CloseButton;
        internal VisualStateGroup DetailsEnabledStates;
        internal VisualState DetailsNotVisible;
        internal VisualStateGroup DetailsStates;
        internal TextBox DetailsTextBox;
        internal VisualState DetailsVisible;
        internal Rectangle DialogRectangle;
        internal VisualState ErrorMessageDetailDisabled;
        internal VisualState ErrorMessageDetailEnabled;
        internal TextBlock ErrorMessageTextBlock;
        public static readonly DependencyProperty ErrorMessageTextProperty = DependencyProperty.Register("ErrorMessageText", typeof(string), typeof(ErrorMessageBox), new PropertyMetadata(null));
        internal TextBlock ErrorMessageTitleTextBlock;
        internal TextBlock Icon;
        internal Grid InnerDialogGrid;
        internal Grid LayoutRoot;
        internal TextToggleButton ShowDetailsToggle;
        internal Grid SimpleErrorDetails;

        public event EventHandler<EventArgs> ErrorMessageClosed;

        public ErrorMessageBox()
        {
            this.InitializeComponent();
            this.CloseButton.Click += new RoutedEventHandler(this.CloseButtonClick);
            this.ButtonOK.Click += new RoutedEventHandler(this.CloseButtonClick);
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.ErrorMessageClosed != null)
            {
                this.ErrorMessageClosed(this, EventArgs.Empty);
            }
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Controls/ErrorMessageBox.xaml", UriKind.Relative));
                this.LayoutRoot = (Grid) base.FindName("LayoutRoot");
                this.DetailsEnabledStates = (VisualStateGroup) base.FindName("DetailsEnabledStates");
                this.ErrorMessageDetailDisabled = (VisualState) base.FindName("ErrorMessageDetailDisabled");
                this.ErrorMessageDetailEnabled = (VisualState) base.FindName("ErrorMessageDetailEnabled");
                this.DetailsStates = (VisualStateGroup) base.FindName("DetailsStates");
                this.DetailsVisible = (VisualState) base.FindName("DetailsVisible");
                this.DetailsNotVisible = (VisualState) base.FindName("DetailsNotVisible");
                this.BackgroundRectangle = (Rectangle) base.FindName("BackgroundRectangle");
                this.InnerDialogGrid = (Grid) base.FindName("InnerDialogGrid");
                this.DialogRectangle = (Rectangle) base.FindName("DialogRectangle");
                this.CloseButton = (Button) base.FindName("CloseButton");
                this.ErrorMessageTitleTextBlock = (TextBlock) base.FindName("ErrorMessageTitleTextBlock");
                this.ErrorMessageTextBlock = (TextBlock) base.FindName("ErrorMessageTextBlock");
                this.ButtonOK = (Button) base.FindName("ButtonOK");
                this.Icon = (TextBlock) base.FindName("Icon");
                this.SimpleErrorDetails = (Grid) base.FindName("SimpleErrorDetails");
                this.ShowDetailsToggle = (TextToggleButton) base.FindName("ShowDetailsToggle");
                this.DetailsTextBox = (TextBox) base.FindName("DetailsTextBox");
            }
        }

        public string ErrorMessageText
        {
            get
            {
                return (string) base.GetValue(ErrorMessageTextProperty);
            }
            set
            {
                base.SetValue(ErrorMessageTextProperty, value);
            }
        }
    }
}

