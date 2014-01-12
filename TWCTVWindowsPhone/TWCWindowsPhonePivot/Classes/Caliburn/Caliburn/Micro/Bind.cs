namespace Caliburn.Micro
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public static class Bind
    {
        public static DependencyProperty AtDesignTimeProperty = DependencyProperty.RegisterAttached("AtDesignTime", typeof(bool), typeof(Bind), new PropertyMetadata(new PropertyChangedCallback(Bind.AtDesignTimeChanged)));
        private static readonly DependencyProperty DataContextProperty = DependencyProperty.RegisterAttached("DataContext", typeof(object), typeof(Bind), new PropertyMetadata(new PropertyChangedCallback(Bind.DataContextChanged)));
        public static DependencyProperty ModelProperty = DependencyProperty.RegisterAttached("Model", typeof(object), typeof(Bind), new PropertyMetadata(new PropertyChangedCallback(Bind.ModelChanged)));
        public static DependencyProperty ModelWithoutContextProperty = DependencyProperty.RegisterAttached("ModelWithoutContext", typeof(object), typeof(Bind), new PropertyMetadata(new PropertyChangedCallback(Bind.ModelWithoutContextChanged)));
        internal static DependencyProperty NoContextProperty = DependencyProperty.RegisterAttached("NoContext", typeof(bool), typeof(Bind), new PropertyMetadata(false));

        private static void AtDesignTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Execute.InDesignMode)
            {
                BindingOperations.SetBinding(d, DataContextProperty, ((bool) e.NewValue) ? new Binding() : null);
            }
        }

        private static void DataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Execute.InDesignMode)
            {
                object obj2 = d.GetValue(AtDesignTimeProperty);
                if (((obj2 != null) && ((bool) obj2)) && (e.NewValue != null))
                {
                    FrameworkElement element = d as FrameworkElement;
                    if (element != null)
                    {
                        ViewModelBinder.Bind(e.NewValue, d, string.IsNullOrEmpty(element.Name) ? element.GetHashCode().ToString() : element.Name);
                    }
                }
            }
        }

        public static bool GetAtDesignTime(DependencyObject dependencyObject)
        {
            return (bool) dependencyObject.GetValue(AtDesignTimeProperty);
        }

        public static object GetModel(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(ModelProperty);
        }

        public static object GetModelWithoutContext(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(ModelWithoutContextProperty);
        }

        private static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe;
            if ((!Execute.InDesignMode && (e.NewValue != null)) && (e.NewValue != e.OldValue))
            {
                fe = d as FrameworkElement;
                if (fe != null)
                {
                    View.ExecuteOnLoad(fe, delegate {
                        object newValue = e.NewValue;
                        string str = e.NewValue as string;
                        if (str != null)
                        {
                            newValue = IoC.GetInstance(null, str);
                        }
                        d.SetValue(View.IsScopeRootProperty, true);
                        string str2 = string.IsNullOrEmpty(fe.Name) ? fe.GetHashCode().ToString() : fe.Name;
                        ViewModelBinder.Bind(newValue, d, str2);
                    });
                }
            }
        }

        private static void ModelWithoutContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe;
            if ((!Execute.InDesignMode && (e.NewValue != null)) && (e.NewValue != e.OldValue))
            {
                fe = d as FrameworkElement;
                if (fe != null)
                {
                    View.ExecuteOnLoad(fe, delegate {
                        object newValue = e.NewValue;
                        string str = e.NewValue as string;
                        if (str != null)
                        {
                            newValue = IoC.GetInstance(null, str);
                        }
                        d.SetValue(View.IsScopeRootProperty, true);
                        string str2 = string.IsNullOrEmpty(fe.Name) ? fe.GetHashCode().ToString() : fe.Name;
                        d.SetValue(NoContextProperty, true);
                        ViewModelBinder.Bind(newValue, d, str2);
                    });
                }
            }
        }

        public static void SetAtDesignTime(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(AtDesignTimeProperty, value);
        }

        public static void SetModel(DependencyObject dependencyObject, object value)
        {
            dependencyObject.SetValue(ModelProperty, value);
        }

        public static void SetModelWithoutContext(DependencyObject dependencyObject, object value)
        {
            dependencyObject.SetValue(ModelWithoutContextProperty, value);
        }
    }
}

