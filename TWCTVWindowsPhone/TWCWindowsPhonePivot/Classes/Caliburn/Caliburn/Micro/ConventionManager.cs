namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Interactivity;

    public static class ConventionManager
    {
        public static System.Action<Binding, PropertyInfo> ApplyBindingMode;
        public static System.Action<Binding, ElementConvention, PropertyInfo> ApplyStringFormat;
        public static System.Action<DependencyProperty, DependencyObject, Binding, PropertyInfo> ApplyUpdateSourceTrigger;
        public static System.Action<Binding, Type, PropertyInfo> ApplyValidation;
        public static System.Action<Binding, DependencyProperty, PropertyInfo> ApplyValueConverter;
        public static IValueConverter BooleanToVisibilityConverter = new Caliburn.Micro.BooleanToVisibilityConverter();
        public static System.Action<FrameworkElement, DependencyProperty, Type, string> ConfigureSelectedItem;
        public static Func<FrameworkElement, DependencyProperty, Type, string, Binding, bool> ConfigureSelectedItemBinding;
        public static DataTemplate DefaultHeaderTemplate = ((DataTemplate) XamlReader.Load("<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><TextBlock Text=\"{Binding DisplayName, Mode=TwoWay}\" /></DataTemplate>"));
        public static DataTemplate DefaultItemTemplate = ((DataTemplate) XamlReader.Load("<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:cal='clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro'> <ContentControl cal:View.Model=\"{Binding}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" /></DataTemplate>"));
        public static System.Func<string, IEnumerable<string>> DerivePotentialSelectionNames;
        private static readonly Dictionary<Type, ElementConvention> ElementConventions = new Dictionary<Type, ElementConvention>();
        public static bool IncludeStaticProperties = false;
        private static readonly ILog Log = LogManager.GetLog(typeof(ConventionManager));
        public static bool OverwriteContent = false;
        public static Action<Type, string, PropertyInfo, FrameworkElement, ElementConvention, DependencyProperty> SetBinding;
        public static System.Func<string, string> Singularize = original => (original.EndsWith("ies") ? (original.TrimEnd(new char[] { 's' }).TrimEnd(new char[] { 'e' }).TrimEnd(new char[] { 'i' }) + "y") : original.TrimEnd(new char[] { 's' }));

        static ConventionManager()
        {
            DerivePotentialSelectionNames = delegate (string name) {
                string str = Singularize(name);
                return new string[] { "Active" + str, "Selected" + str, "Current" + str };
            };
            SetBinding = delegate (Type viewModelType, string path, PropertyInfo property, FrameworkElement element, ElementConvention convention, DependencyProperty bindableProperty) {
                Binding binding = new Binding(path);
                ApplyBindingMode(binding, property);
                ApplyValueConverter(binding, bindableProperty, property);
                ApplyStringFormat(binding, convention, property);
                ApplyValidation(binding, viewModelType, property);
                ApplyUpdateSourceTrigger(bindableProperty, element, binding, property);
                BindingOperations.SetBinding(element, bindableProperty, binding);
            };
            ApplyBindingMode = delegate (Binding binding, PropertyInfo property) {
                MethodInfo setMethod = property.GetSetMethod();
                binding.Mode = ((property.CanWrite && (setMethod != null)) && setMethod.IsPublic) ? BindingMode.TwoWay : BindingMode.OneWay;
            };
            ApplyValidation = delegate (Binding binding, Type viewModelType, PropertyInfo property) {
                if (typeof(INotifyDataErrorInfo).IsAssignableFrom(viewModelType))
                {
                    binding.ValidatesOnNotifyDataErrors = true;
                    binding.ValidatesOnExceptions = true;
                }
                if (typeof(IDataErrorInfo).IsAssignableFrom(viewModelType))
                {
                    binding.ValidatesOnDataErrors = true;
                    binding.ValidatesOnExceptions = true;
                }
            };
            ApplyValueConverter = delegate (Binding binding, DependencyProperty bindableProperty, PropertyInfo property) {
                if ((bindableProperty == UIElement.VisibilityProperty) && typeof(bool).IsAssignableFrom(property.PropertyType))
                {
                    binding.Converter = BooleanToVisibilityConverter;
                }
            };
            ApplyStringFormat = delegate (Binding binding, ElementConvention convention, PropertyInfo property) {
                if (typeof(DateTime).IsAssignableFrom(property.PropertyType))
                {
                    binding.StringFormat = "{0:MM/dd/yyyy}";
                }
            };
            ApplyUpdateSourceTrigger = (System.Action<DependencyProperty, DependencyObject, Binding, PropertyInfo>) ((bindableProperty, element, binding, info) => (binding.UpdateSourceTrigger = (UpdateSourceTrigger) 1));
            ConfigureSelectedItem = delegate (FrameworkElement selector, DependencyProperty selectedItemProperty, Type viewModelType, string path) {
                if (!HasBinding(selector, selectedItemProperty))
                {
                    int startIndex = path.LastIndexOf('.');
                    startIndex = (startIndex == -1) ? 0 : (startIndex + 1);
                    string arg = path.Substring(startIndex);
                    foreach (string str2 in DerivePotentialSelectionNames(arg))
                    {
                        if (viewModelType.GetPropertyCaseInsensitive(str2) != null)
                        {
                            string str3 = path.Replace(arg, str2);
                            Binding binding = new Binding(str3) {
                                Mode = BindingMode.TwoWay
                            };
                            if (ConfigureSelectedItemBinding(selector, selectedItemProperty, viewModelType, str3, binding))
                            {
                                BindingOperations.SetBinding(selector, selectedItemProperty, binding);
                                Log.Info("SelectedItem binding applied to {0}.", new object[] { selector.Name });
                                break;
                            }
                            Log.Info("SelectedItem binding not applied to {0} due to 'ConfigureSelectedItemBinding' customization.", new object[] { selector.Name });
                        }
                    }
                }
            };
            ConfigureSelectedItemBinding = (selector, selectedItemProperty, viewModelType, selectionPath, binding) => true;
            AddElementConvention<DatePicker>(DatePicker.SelectedDateProperty, "SelectedDate", "SelectedDateChanged");
            AddElementConvention<HyperlinkButton>(ContentControl.ContentProperty, "DataContext", "Click");
            AddElementConvention<PasswordBox>(PasswordBox.PasswordProperty, "Password", "PasswordChanged");
            AddElementConvention<UserControl>(UIElement.VisibilityProperty, "DataContext", "Loaded");
            AddElementConvention<Image>(Image.SourceProperty, "Source", "Loaded");
            AddElementConvention<ToggleButton>(ToggleButton.IsCheckedProperty, "IsChecked", "Click");
            AddElementConvention<ButtonBase>(ContentControl.ContentProperty, "DataContext", "Click");
            AddElementConvention<TextBox>(TextBox.TextProperty, "Text", "TextChanged");
            AddElementConvention<TextBlock>(TextBlock.TextProperty, "Text", "DataContextChanged");
            AddElementConvention<ProgressBar>(RangeBase.ValueProperty, "Value", "ValueChanged");
            AddElementConvention<Selector>(ItemsControl.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding = delegate (Type viewModelType, string path, PropertyInfo property, FrameworkElement element, ElementConvention convention) {
                if (!SetBindingWithoutBindingOrValueOverwrite(viewModelType, path, property, element, convention, ItemsControl.ItemsSourceProperty))
                {
                    return false;
                }
                ConfigureSelectedItem(element, Selector.SelectedItemProperty, viewModelType, path);
                ApplyItemTemplate((ItemsControl) element, property);
                return true;
            };
            AddElementConvention<ItemsControl>(ItemsControl.ItemsSourceProperty, "DataContext", "Loaded").ApplyBinding = delegate (Type viewModelType, string path, PropertyInfo property, FrameworkElement element, ElementConvention convention) {
                if (!SetBindingWithoutBindingOrValueOverwrite(viewModelType, path, property, element, convention, ItemsControl.ItemsSourceProperty))
                {
                    return false;
                }
                ApplyItemTemplate((ItemsControl) element, property);
                return true;
            };
            AddElementConvention<ContentControl>(ContentControl.ContentProperty, "DataContext", "Loaded").GetBindableProperty = delegate (DependencyObject foundControl) {
                ContentControl control = (ContentControl) foundControl;
                if (!(!(control.Content is DependencyObject) || OverwriteContent))
                {
                    return null;
                }
                if (control.ContentTemplate == null)
                {
                    Log.Info("ViewModel bound on {0}.", new object[] { control.Name });
                    return View.ModelProperty;
                }
                Log.Info("Content bound on {0}. Template or content was present.", new object[] { control.Name });
                return ContentControl.ContentProperty;
            };
            AddElementConvention<Shape>(UIElement.VisibilityProperty, "DataContext", "MouseLeftButtonUp");
            AddElementConvention<FrameworkElement>(UIElement.VisibilityProperty, "DataContext", "Loaded");
        }

        public static ElementConvention AddElementConvention(ElementConvention convention)
        {
            return (ElementConventions[convention.ElementType] = convention);
        }

        public static ElementConvention AddElementConvention<T>(DependencyProperty bindableProperty, string parameterProperty, string eventName)
        {
            ElementConvention convention = new ElementConvention {
                ElementType = typeof(T),
                GetBindableProperty = element => bindableProperty,
                ParameterProperty = parameterProperty,
                CreateTrigger = () => new System.Windows.Interactivity.EventTrigger { EventName = eventName }
            };
            return AddElementConvention(convention);
        }

        public static void ApplyHeaderTemplate(FrameworkElement element, DependencyProperty headerTemplateProperty, DependencyProperty headerTemplateSelectorProperty, Type viewModelType)
        {
            object obj2 = element.GetValue(headerTemplateProperty);
            object obj3 = (headerTemplateSelectorProperty != null) ? element.GetValue(headerTemplateSelectorProperty) : null;
            if (((obj2 == null) && (obj3 == null)) && typeof(IHaveDisplayName).IsAssignableFrom(viewModelType))
            {
                element.SetValue(headerTemplateProperty, DefaultHeaderTemplate);
                Log.Info("Header template applied to {0}.", new object[] { element.Name });
            }
        }

        public static void ApplyItemTemplate(ItemsControl itemsControl, PropertyInfo property)
        {
            if (((string.IsNullOrEmpty(itemsControl.DisplayMemberPath) && !HasBinding(itemsControl, ItemsControl.DisplayMemberPathProperty)) && (itemsControl.ItemTemplate == null)) && property.PropertyType.IsGenericType)
            {
                Type c = property.PropertyType.GetGenericArguments().First<Type>();
                if (!c.IsValueType && !typeof(string).IsAssignableFrom(c))
                {
                    itemsControl.ItemTemplate = DefaultItemTemplate;
                    Log.Info("ItemTemplate applied to {0}.", new object[] { itemsControl.Name });
                }
            }
        }

        public static ElementConvention GetElementConvention(Type elementType)
        {
            ElementConvention convention;
            if (elementType == null)
            {
                return null;
            }
            ElementConventions.TryGetValue(elementType, out convention);
            return (convention ?? GetElementConvention(elementType.BaseType));
        }

        public static PropertyInfo GetPropertyCaseInsensitive(this Type type, string propertyName)
        {
            List<Type> list = new List<Type> {
                type
            };
            if (type.IsInterface)
            {
                list.AddRange(type.GetInterfaces());
            }
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
            if (IncludeStaticProperties)
            {
                flags |= BindingFlags.Static;
            }
            return Enumerable.FirstOrDefault<PropertyInfo>(from interfaceType in list select interfaceType.GetProperty(propertyName, flags), property => property != null);
        }

        public static bool HasBinding(FrameworkElement element, DependencyProperty property)
        {
            return (element.GetBindingExpression(property) != null);
        }

        public static bool SetBindingWithoutBindingOrValueOverwrite(Type viewModelType, string path, PropertyInfo property, FrameworkElement element, ElementConvention convention, DependencyProperty bindableProperty)
        {
            if ((bindableProperty == null) || HasBinding(element, bindableProperty))
            {
                return false;
            }
            if (element.GetValue(bindableProperty) != null)
            {
                return false;
            }
            SetBinding(viewModelType, path, property, element, convention, bindableProperty);
            return true;
        }

        public static bool SetBindingWithoutBindingOverwrite(Type viewModelType, string path, PropertyInfo property, FrameworkElement element, ElementConvention convention, DependencyProperty bindableProperty)
        {
            if ((bindableProperty == null) || HasBinding(element, bindableProperty))
            {
                return false;
            }
            SetBinding(viewModelType, path, property, element, convention, bindableProperty);
            return true;
        }
    }
}

