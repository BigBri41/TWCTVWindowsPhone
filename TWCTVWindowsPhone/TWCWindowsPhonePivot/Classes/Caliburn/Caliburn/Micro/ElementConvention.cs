namespace Caliburn.Micro
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public class ElementConvention
    {
        public Func<Type, string, PropertyInfo, FrameworkElement, ElementConvention, bool> ApplyBinding = (viewModelType, path, property, element, convention) => ConventionManager.SetBindingWithoutBindingOverwrite(viewModelType, path, property, element, convention, convention.GetBindableProperty(element));
        public System.Func<TriggerBase> CreateTrigger;
        public Type ElementType;
        public System.Func<DependencyObject, DependencyProperty> GetBindableProperty;
        public string ParameterProperty;

        [CompilerGenerated]
        private static bool <.ctor>b__0(Type viewModelType, string path, PropertyInfo property, FrameworkElement element, ElementConvention convention)
        {
            return ConventionManager.SetBindingWithoutBindingOverwrite(viewModelType, path, property, element, convention, convention.GetBindableProperty(element));
        }
    }
}

