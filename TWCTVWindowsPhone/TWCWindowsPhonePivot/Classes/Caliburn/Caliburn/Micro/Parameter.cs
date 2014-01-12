namespace Caliburn.Micro
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Interactivity;

    public class Parameter : DependencyObject, IAttachedObject
    {
        private DependencyObject associatedObject;
        private WeakReference owner;
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(Parameter), new PropertyMetadata(new PropertyChangedCallback(Parameter.OnValueChanged)));

        internal void MakeAwareOf(ActionMessage owner)
        {
            this.Owner = owner;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Parameter parameter = (Parameter) d;
            if (parameter.Owner != null)
            {
                parameter.Owner.UpdateAvailability();
            }
        }

        void IAttachedObject.Attach(DependencyObject dependencyObject)
        {
            this.associatedObject = dependencyObject;
        }

        void IAttachedObject.Detach()
        {
            this.associatedObject = null;
        }

        private ActionMessage Owner
        {
            get
            {
                return ((this.owner == null) ? null : (this.owner.Target as ActionMessage));
            }
            set
            {
                this.owner = new WeakReference(value);
            }
        }

        DependencyObject IAttachedObject.AssociatedObject
        {
            get
            {
                return this.associatedObject;
            }
        }

        [Category("Common Properties")]
        public object Value
        {
            get
            {
                return base.GetValue(ValueProperty);
            }
            set
            {
                base.SetValue(ValueProperty, value);
            }
        }
    }
}

