namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public static class BindingScope
    {
        public static System.Func<DependencyObject, IEnumerable<FrameworkElement>> GetNamedElements;

        static BindingScope()
        {
            GetNamedElements = delegate (DependencyObject elementInScope) {
                DependencyObject parent = elementInScope;
                DependencyObject reference = elementInScope;
                DependencyObject obj4 = null;
                Dictionary<DependencyObject, DependencyObject> dictionary = new Dictionary<DependencyObject, DependencyObject>();
                while (true)
                {
                    if (parent == null)
                    {
                        parent = reference;
                        break;
                    }
                    if ((parent is UserControl) || ((bool) parent.GetValue(View.IsScopeRootProperty)))
                    {
                        break;
                    }
                    if (parent is ContentPresenter)
                    {
                        obj4 = parent;
                    }
                    else if ((parent is ItemsPresenter) && (obj4 != null))
                    {
                        dictionary[parent] = obj4;
                        obj4 = null;
                    }
                    reference = parent;
                    parent = VisualTreeHelper.GetParent(reference);
                }
                List<FrameworkElement> list = new List<FrameworkElement>();
                Queue<DependencyObject> queue = new Queue<DependencyObject>();
                queue.Enqueue(parent);
                while (queue.Count > 0)
                {
                    DependencyObject key = queue.Dequeue();
                    FrameworkElement item = key as FrameworkElement;
                    if (!((item == null) || string.IsNullOrEmpty(item.Name)))
                    {
                        list.Add(item);
                    }
                    if (!(key is UserControl) || (key == parent))
                    {
                        if (dictionary.ContainsKey(key))
                        {
                            queue.Enqueue(dictionary[key]);
                        }
                        else
                        {
                            int childrenCount = VisualTreeHelper.GetChildrenCount(key);
                            if (childrenCount > 0)
                            {
                                for (int j = 0; j < childrenCount; j++)
                                {
                                    DependencyObject child = VisualTreeHelper.GetChild(key, j);
                                    queue.Enqueue(child);
                                }
                            }
                            else
                            {
                                ContentControl control = key as ContentControl;
                                if (control != null)
                                {
                                    if (control.Content is DependencyObject)
                                    {
                                        queue.Enqueue(control.Content as DependencyObject);
                                    }
                                }
                                else
                                {
                                    ItemsControl control2 = key as ItemsControl;
                                    if (control2 != null)
                                    {
                                        control2.Items.OfType<DependencyObject>().Apply<DependencyObject>(new Action<DependencyObject>(queue.Enqueue));
                                    }
                                }
                            }
                        }
                    }
                }
                return list;
            };
        }

        public static FrameworkElement FindName(this IEnumerable<FrameworkElement> elementsToSearch, string name)
        {
            return Enumerable.FirstOrDefault<FrameworkElement>(elementsToSearch, x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}

