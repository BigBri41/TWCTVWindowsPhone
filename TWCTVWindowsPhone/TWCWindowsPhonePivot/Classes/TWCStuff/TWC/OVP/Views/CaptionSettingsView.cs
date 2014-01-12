namespace TWC.OVP.Views
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;

    public class CaptionSettingsView : UserControl
    {
        private bool _contentLoaded;
        internal Border BackgroundRectangle;
        internal Path BoxPath;
        internal Grid CaptionSettingsLayoutRoot;
        internal Grid CharacterSettingsLayoutRoot;
        internal Path CheckedBoxPath;
        internal Path CheckedCPath1;
        internal Path CheckedCPath2;
        internal Button CloseButton;
        internal Path CPath1;
        internal Path CPath2;
        internal Button DefaultsButton;
        internal Border DialogBorder;
        internal Grid FooterGrid;
        internal Canvas GlyphCanvas;
        internal Rectangle Gradient;
        internal Grid HeaderGrid;
        internal Grid LayoutRoot;
        internal Rectangle MetaDataBackground;
        internal UserControl userControl;

        public CaptionSettingsView()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Views/CaptionSettingsView.xaml", UriKind.Relative));
                this.userControl = (UserControl) base.FindName("userControl");
                this.LayoutRoot = (Grid) base.FindName("LayoutRoot");
                this.DialogBorder = (Border) base.FindName("DialogBorder");
                this.BackgroundRectangle = (Border) base.FindName("BackgroundRectangle");
                this.Gradient = (Rectangle) base.FindName("Gradient");
                this.HeaderGrid = (Grid) base.FindName("HeaderGrid");
                this.GlyphCanvas = (Canvas) base.FindName("GlyphCanvas");
                this.BoxPath = (Path) base.FindName("BoxPath");
                this.CPath1 = (Path) base.FindName("CPath1");
                this.CPath2 = (Path) base.FindName("CPath2");
                this.CheckedBoxPath = (Path) base.FindName("CheckedBoxPath");
                this.CheckedCPath1 = (Path) base.FindName("CheckedCPath1");
                this.CheckedCPath2 = (Path) base.FindName("CheckedCPath2");
                this.CaptionSettingsLayoutRoot = (Grid) base.FindName("CaptionSettingsLayoutRoot");
                this.MetaDataBackground = (Rectangle) base.FindName("MetaDataBackground");
                this.CharacterSettingsLayoutRoot = (Grid) base.FindName("CharacterSettingsLayoutRoot");
                this.FooterGrid = (Grid) base.FindName("FooterGrid");
                this.DefaultsButton = (Button) base.FindName("DefaultsButton");
                this.CloseButton = (Button) base.FindName("CloseButton");
            }
        }
    }
}

