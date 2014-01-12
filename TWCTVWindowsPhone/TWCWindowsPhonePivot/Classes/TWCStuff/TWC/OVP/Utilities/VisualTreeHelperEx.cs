namespace TWC.OVP.Utilities
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    public static class VisualTreeHelperEx
    {
        public static DependencyObject FindAncestor(DependencyObject reference, string lookup)
        {
            DependencyObject parent = reference;
            do
            {
                if ((parent is FrameworkElement) && (((FrameworkElement) parent).Name == lookup))
                {
                    return parent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            while (parent != null);
            return null;
        }
    }
}

