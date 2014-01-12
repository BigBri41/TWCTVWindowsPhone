namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Interactivity;

    public static class ViewModelBinder
    {
        public static bool ApplyConventionsByDefault = true;
        public static System.Action<object, DependencyObject, object> Bind;
        public static System.Func<IEnumerable<FrameworkElement>, Type, IEnumerable<FrameworkElement>> BindActions;
        public static System.Func<IEnumerable<FrameworkElement>, Type, IEnumerable<FrameworkElement>> BindProperties;
        public static readonly DependencyProperty ConventionsAppliedProperty = DependencyProperty.RegisterAttached("ConventionsApplied", typeof(bool), typeof(ViewModelBinder), null);
        public static System.Action<IEnumerable<FrameworkElement>, Type> HandleUnmatchedElements;
        private static readonly ILog Log = LogManager.GetLog(typeof(ViewModelBinder));

        static ViewModelBinder()
        {
            BindProperties = delegate (IEnumerable<FrameworkElement> namedElements, Type viewModelType) {
                List<FrameworkElement> list = new List<FrameworkElement>();
                foreach (FrameworkElement element in namedElements)
                {
                    string str = element.Name.Trim(new char[] { '_' });
                    string[] strArray = str.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    PropertyInfo propertyCaseInsensitive = viewModelType.GetPropertyCaseInsensitive(strArray[0]);
                    Type propertyType = viewModelType;
                    for (int j = 1; (j < strArray.Length) && (propertyCaseInsensitive != null); j++)
                    {
                        propertyType = propertyCaseInsensitive.PropertyType;
                        propertyCaseInsensitive = propertyType.GetPropertyCaseInsensitive(strArray[j]);
                    }
                    if (propertyCaseInsensitive == null)
                    {
                        list.Add(element);
                        Log.Info("Binding Convention Not Applied: Element {0} did not match a property.", new object[] { element.Name });
                    }
                    else
                    {
                        ElementConvention elementConvention = ConventionManager.GetElementConvention(element.GetType());
                        if (elementConvention == null)
                        {
                            list.Add(element);
                            Log.Warn("Binding Convention Not Applied: No conventions configured for {0}.", new object[] { element.GetType() });
                        }
                        else if (elementConvention.ApplyBinding(propertyType, str.Replace('_', '.'), propertyCaseInsensitive, element, elementConvention))
                        {
                            Log.Info("Binding Convention Applied: Element {0}.", new object[] { element.Name });
                        }
                        else
                        {
                            Log.Info("Binding Convention Not Applied: Element {0} has existing binding.", new object[] { element.Name });
                            list.Add(element);
                        }
                    }
                }
                return list;
            };
            BindActions = delegate (IEnumerable<FrameworkElement> namedElements, Type viewModelType) {
                MethodInfo[] methods = viewModelType.GetMethods();
                List<FrameworkElement> elementsToSearch = namedElements.ToList<FrameworkElement>();
                foreach (MethodInfo info in methods)
                {
                    FrameworkElement item = elementsToSearch.FindName(info.Name);
                    if (item == null)
                    {
                        Log.Info("Action Convention Not Applied: No actionable element for {0}.", new object[] { info.Name });
                    }
                    else
                    {
                        elementsToSearch.Remove(item);
                        System.Windows.Interactivity.TriggerCollection triggers = Interaction.GetTriggers(item);
                        if ((triggers != null) && (triggers.Count > 0))
                        {
                            Log.Info("Action Convention Not Applied: Interaction.Triggers already set on {0}.", new object[] { item.Name });
                        }
                        else
                        {
                            string attachText = info.Name;
                            ParameterInfo[] parameters = info.GetParameters();
                            if (parameters.Length > 0)
                            {
                                attachText = attachText + "(";
                                foreach (ParameterInfo info2 in parameters)
                                {
                                    string name = info2.Name;
                                    string key = "$" + name.ToLower();
                                    if (MessageBinder.SpecialValues.ContainsKey(key))
                                    {
                                        name = key;
                                    }
                                    attachText = attachText + name + ",";
                                }
                                attachText = attachText.Remove(attachText.Length - 1, 1) + ")";
                            }
                            Log.Info("Action Convention Applied: Action {0} on element {1}.", new object[] { info.Name, attachText });
                            Message.SetAttach(item, attachText);
                        }
                    }
                }
                return elementsToSearch;
            };
            HandleUnmatchedElements = delegate (IEnumerable<FrameworkElement> elements, Type viewModelType) {
            };
            Bind = delegate (object viewModel, DependencyObject view, object context) {
                Log.Info("Binding {0} and {1}.", new object[] { view, viewModel });
                if ((bool) view.GetValue(Caliburn.Micro.Bind.NoContextProperty))
                {
                    Caliburn.Micro.Action.SetTargetWithoutContext(view, viewModel);
                }
                else
                {
                    Caliburn.Micro.Action.SetTarget(view, viewModel);
                }
                IViewAware aware = viewModel as IViewAware;
                if (aware != null)
                {
                    Log.Info("Attaching {0} to {1}.", new object[] { view, aware });
                    aware.AttachView(view, context);
                }
                if (!((bool) view.GetValue(ConventionsAppliedProperty)))
                {
                    FrameworkElement element = View.GetFirstNonGeneratedView(view) as FrameworkElement;
                    if (element != null)
                    {
                        if (!ShouldApplyConventions(element))
                        {
                            Log.Info("Skipping conventions for {0} and {1}.", new object[] { element, viewModel });
                        }
                        else
                        {
                            Type type = viewModel.GetType();
                            IEnumerable<FrameworkElement> enumerable = BindingScope.GetNamedElements(element);
                            enumerable.Apply<FrameworkElement>(x => x.SetValue(View.IsLoadedProperty, element.GetValue(View.IsLoadedProperty)));
                            enumerable = BindActions(enumerable, type);
                            enumerable = BindProperties(enumerable, type);
                            HandleUnmatchedElements(enumerable, type);
                            view.SetValue(ConventionsAppliedProperty, true);
                        }
                    }
                }
            };
        }

        public static bool ShouldApplyConventions(FrameworkElement view)
        {
            return View.GetApplyConventions(view).GetValueOrDefault(ApplyConventionsByDefault);
        }
    }
}

