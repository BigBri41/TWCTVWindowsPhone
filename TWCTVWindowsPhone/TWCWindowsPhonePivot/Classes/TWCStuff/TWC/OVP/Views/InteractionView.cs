namespace TWC.OVP.Views
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using TWC.OVP.Controls;

    public class InteractionView : UserControl
    {
        private bool _contentLoaded;
        internal ErrorMessageBox ErrorMessage;
        internal VisualState ErrorMessageNotShown;
        internal VisualState ErrorMessageShown;
        internal VisualStateGroup ErrorStates;
        internal Grid LayoutRoot;

        public InteractionView()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Views/InteractionView.xaml", UriKind.Relative));
                this.LayoutRoot = (Grid) base.FindName("LayoutRoot");
                this.ErrorStates = (VisualStateGroup) base.FindName("ErrorStates");
                this.ErrorMessageShown = (VisualState) base.FindName("ErrorMessageShown");
                this.ErrorMessageNotShown = (VisualState) base.FindName("ErrorMessageNotShown");
                this.ErrorMessage = (ErrorMessageBox) base.FindName("ErrorMessage");
            }
        }
    }
}

