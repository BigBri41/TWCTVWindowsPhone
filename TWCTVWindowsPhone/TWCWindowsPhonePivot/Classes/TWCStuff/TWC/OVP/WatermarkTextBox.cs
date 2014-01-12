namespace TWC.OVP
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;

    public class WatermarkTextBox : UserControl
    {
        private bool _contentLoaded;
        private bool _textBoxHasFocus;
        internal Button ClearButton;
        internal Grid LayoutRoot;
        internal Path MagnifyingGlassPath;
        internal TextBox MainTextBox;
        internal Rectangle SearchGridBackground;
        internal TextBlock WatermarkTextBlock;

        public WatermarkTextBox()
        {
            RoutedEventHandler handler = null;
            RoutedEventHandler handler2 = null;
            RoutedEventHandler handler3 = null;
            TextChangedEventHandler handler4 = null;
            this.InitializeComponent();
            if (handler == null)
            {
                handler = (RoutedEventHandler) ((s, e) => (this.MainTextBox.Text = string.Empty));
            }
            this.ClearButton.Click += handler;
            if (handler2 == null)
            {
                handler2 = delegate (object s, RoutedEventArgs e) {
                    this._textBoxHasFocus = true;
                    this.UpdateWatermarkVisibility();
                };
            }
            this.MainTextBox.GotFocus += handler2;
            if (handler3 == null)
            {
                handler3 = delegate (object s, RoutedEventArgs e) {
                    this._textBoxHasFocus = false;
                    this.UpdateWatermarkVisibility();
                };
            }
            this.MainTextBox.LostFocus += handler3;
            if (handler4 == null)
            {
                handler4 = (s, e) => this.UpdateWatermarkVisibility();
            }
            this.MainTextBox.TextChanged += handler4;
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Controls/WatermarkTextBox.xaml", UriKind.Relative));
                this.LayoutRoot = (Grid) base.FindName("LayoutRoot");
                this.SearchGridBackground = (Rectangle) base.FindName("SearchGridBackground");
                this.WatermarkTextBlock = (TextBlock) base.FindName("WatermarkTextBlock");
                this.MainTextBox = (TextBox) base.FindName("MainTextBox");
                this.ClearButton = (Button) base.FindName("ClearButton");
                this.MagnifyingGlassPath = (Path) base.FindName("MagnifyingGlassPath");
            }
        }

        private void UpdateWatermarkVisibility()
        {
            if (this._textBoxHasFocus || !string.IsNullOrEmpty(this.MainTextBox.Text))
            {
                this.WatermarkTextBlock.Visibility = Visibility.Collapsed;
                this.ClearButton.Visibility = Visibility.Visible;
                this.MagnifyingGlassPath.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.WatermarkTextBlock.Visibility = Visibility.Visible;
                this.ClearButton.Visibility = Visibility.Collapsed;
                this.MagnifyingGlassPath.Visibility = Visibility.Visible;
            }
        }
    }
}

