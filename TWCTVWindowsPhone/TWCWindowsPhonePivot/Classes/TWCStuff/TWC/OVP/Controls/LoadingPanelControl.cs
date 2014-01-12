namespace TWC.OVP.Controls
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using TWC.OVP.Framework.Controls;

    public class LoadingPanelControl : UserControl
    {
        private bool _contentLoaded;
        internal Rectangle BackgroundRectangle;
        internal Grid LayoutRoot;
        internal TextBlock LoadingText;
        internal LoadSpinnerControl spinnerControl;

        public LoadingPanelControl()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Controls/LoadingPanelControl.xaml", UriKind.Relative));
                this.LayoutRoot = (Grid) base.FindName("LayoutRoot");
                this.BackgroundRectangle = (Rectangle) base.FindName("BackgroundRectangle");
                this.spinnerControl = (LoadSpinnerControl) base.FindName("spinnerControl");
                this.LoadingText = (TextBlock) base.FindName("LoadingText");
            }
        }
    }
}

