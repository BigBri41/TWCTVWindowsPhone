namespace TWC.OVP.Controls
{
    using System;
    using System.Threading;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class CustomKeyboardListBox : ListBox
    {
        public event KeyEventHandler CustomKeyDown;

        public CustomKeyboardListBox()
        {
            base.DefaultStyleKey = typeof(ListBox);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.CustomKeyDown != null)
            {
                this.CustomKeyDown(this, e);
            }
            if (!e.Handled)
            {
                base.OnKeyDown(e);
            }
        }
    }
}

