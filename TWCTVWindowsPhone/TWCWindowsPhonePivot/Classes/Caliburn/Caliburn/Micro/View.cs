namespace Caliburn.Micro
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    public static class View
    {
        public static readonly DependencyProperty ApplyConventionsProperty = DependencyProperty.RegisterAttached("ApplyConventions", typeof(bool?), typeof(View), null);
        public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached("Context", typeof(object), typeof(View), new PropertyMetadata(new PropertyChangedCallback(View.OnContextChanged)));
        private static readonly ContentPropertyAttribute DefaultContentProperty = new ContentPropertyAttribute("Content");
        public static readonly object DefaultContext = new object();
        public static System.Func<object, object> GetFirstNonGeneratedView;
        public static readonly DependencyProperty IsGeneratedProperty = DependencyProperty.RegisterAttached("IsGenerated", typeof(bool), typeof(View), new PropertyMetadata(false, null));
        public static readonly DependencyProperty IsLoadedProperty = DependencyProperty.RegisterAttached("IsLoaded", typeof(bool), typeof(View), new PropertyMetadata(false));
        public static readonly DependencyProperty IsScopeRootProperty = DependencyProperty.RegisterAttached("IsScopeRoot", typeof(bool), typeof(View), new PropertyMetadata(false));
        private static readonly ILog Log = LogManager.GetLog(typeof(View));
        public static DependencyProperty ModelProperty = DependencyProperty.RegisterAttached("Model", typeof(object), typeof(View), new PropertyMetadata(new PropertyChangedCallback(View.OnModelChanged)));

        static View()
        {
            GetFirstNonGeneratedView = delegate (object view) {
                DependencyObject obj2 = view as DependencyObject;
                if (obj2 == null)
                {
                    return view;
                }
                if ((bool) obj2.GetValue(IsGeneratedProperty))
                {
                    if (obj2 is ContentControl)
                    {
                        return ((ContentControl) obj2).Content;
                    }
                    Type type = obj2.GetType();
                    ContentPropertyAttribute attribute = type.GetAttributes<ContentPropertyAttribute>(true).FirstOrDefault<ContentPropertyAttribute>() ?? DefaultContentProperty;
                    return type.GetProperty(attribute.Name).GetValue(obj2, null);
                }
                return obj2;
            };
        }

        public static bool ExecuteOnLoad(FrameworkElement element, RoutedEventHandler handler)
        {
            if ((bool) element.GetValue(IsLoadedProperty))
            {
                handler(element, new RoutedEventArgs());
                return true;
            }
            RoutedEventHandler loaded = null;
            loaded = delegate (object s, RoutedEventArgs e) {
                element.SetValue(IsLoadedProperty, true);
                handler(s, e);
                element.Loaded -= loaded;
            };
            element.Loaded += loaded;
            return false;
        }

        public static bool? GetApplyConventions(DependencyObject d)
        {
            return (bool?) d.GetValue(ApplyConventionsProperty);
        }

        public static object GetContext(DependencyObject d)
        {
            return d.GetValue(ContextProperty);
        }

        public static object GetModel(DependencyObject d)
        {
            return d.GetValue(ModelProperty);
        }

        private static void OnContextChanged(DependencyObject targetLocation, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                object model = GetModel(targetLocation);
                if (model != null)
                {
                    UIElement view = ViewLocator.LocateForModel(model, targetLocation, e.NewValue);
                    SetContentProperty(targetLocation, view);
                    ViewModelBinder.Bind(model, view, e.NewValue);
                }
            }
        }

        private static void OnModelChanged(DependencyObject targetLocation, DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != args.NewValue)
            {
                if (args.NewValue != null)
                {
                    object context = GetContext(targetLocation);
                    UIElement view = ViewLocator.LocateForModel(args.NewValue, targetLocation, context);
                    SetContentProperty(targetLocation, view);
                    ViewModelBinder.Bind(args.NewValue, view, context);
                }
                else
                {
                    SetContentProperty(targetLocation, args.NewValue);
                }
            }
        }

        public static void SetApplyConventions(DependencyObject d, bool? value)
        {
            d.SetValue(ApplyConventionsProperty, value);
        }

        private static void SetContentProperty(object targetLocation, object view)
        {
            FrameworkElement element = view as FrameworkElement;
            if ((element != null) && (element.Parent != null))
            {
                SetContentPropertyCore(element.Parent, null);
            }
            SetContentPropertyCore(targetLocation, view);
        }

        private static void SetContentPropertyCore(object targetLocation, object view)
        {
            try
            {
                Type member = targetLocation.GetType();
                ContentPropertyAttribute attribute = member.GetAttributes<ContentPropertyAttribute>(true).FirstOrDefault<ContentPropertyAttribute>() ?? DefaultContentProperty;
                member.GetProperty(attribute.Name).SetValue(targetLocation, view, null);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        public static void SetContext(DependencyObject d, object value)
        {
            d.SetValue(ContextProperty, value);
        }

        public static void SetModel(DependencyObject d, object value)
        {
            d.SetValue(ModelProperty, value);
        }
    }
}

