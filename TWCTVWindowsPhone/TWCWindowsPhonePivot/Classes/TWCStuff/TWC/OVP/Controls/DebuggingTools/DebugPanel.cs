namespace TWC.OVP.Controls.DebuggingTools
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    public class DebugPanel : UserControl
    {
        private bool _contentLoaded;
        internal Grid LayoutRoot;

        public DebugPanel()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Controls/DebuggingTools/DebugPanel.xaml", UriKind.Relative));
                this.LayoutRoot = (Grid) base.FindName("LayoutRoot");
            }
        }
    }
}

