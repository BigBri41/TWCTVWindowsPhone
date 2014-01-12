namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;

    public static class ViewLocator
    {
        public static string ContextSeparator = ".";
        private static string defaultSubNsViewModels;
        private static string defaultSubNsViews;
        public static System.Func<Type, Type, string> DeterminePackUriFromType;
        public static System.Func<Type, UIElement> GetOrCreateViewType;
        private static bool includeViewSuffixInVmNames;
        public static System.Func<object, DependencyObject, object, UIElement> LocateForModel;
        public static System.Func<Type, DependencyObject, object, UIElement> LocateForModelType;
        public static System.Func<Type, DependencyObject, object, Type> LocateTypeForModelType;
        private static readonly ILog Log = LogManager.GetLog(typeof(ViewLocator));
        public static System.Func<string, string> ModifyModelTypeAtDesignTime;
        private static string nameFormat;
        public static Caliburn.Micro.NameTransformer NameTransformer = new Caliburn.Micro.NameTransformer();
        public static System.Func<string, object, IEnumerable<string>> TransformName;
        private static bool useNameSuffixesInMappings;
        private static string viewModelSuffix;
        private static readonly List<string> ViewSuffixList = new List<string>();

        static ViewLocator()
        {
            GetOrCreateViewType = delegate (Type viewType) {
                UIElement element = IoC.GetAllInstances(viewType).FirstOrDefault<object>() as UIElement;
                if (element != null)
                {
                    InitializeComponent(element);
                    return element;
                }
                if (!((!viewType.IsInterface && !viewType.IsAbstract) && typeof(UIElement).IsAssignableFrom(viewType)))
                {
                    return new TextBlock { Text = string.Format("Cannot create {0}.", viewType.FullName) };
                }
                element = (UIElement) Activator.CreateInstance(viewType);
                InitializeComponent(element);
                return element;
            };
            ModifyModelTypeAtDesignTime = delegate (string modelTypeName) {
                if (modelTypeName.StartsWith("_"))
                {
                    int index = modelTypeName.IndexOf(".");
                    modelTypeName = modelTypeName.Substring(index + 1);
                    index = modelTypeName.IndexOf(".");
                    modelTypeName = modelTypeName.Substring(index + 1);
                }
                return modelTypeName;
            };
            TransformName = delegate (string typeName, object context) {
                System.Func<string, string> func;
                if (context == null)
                {
                    func = r => r;
                    return NameTransformer.Transform(typeName, func);
                }
                string contextstr = ContextSeparator + context;
                string regEx = string.Empty;
                string captureGroup = string.Empty;
                if (useNameSuffixesInMappings)
                {
                    regEx = "(" + string.Join("|", ViewSuffixList.ToArray()) + ")";
                    captureGroup = RegExHelper.GetCaptureGroup("suffix", regEx);
                }
                string patternregex = string.Format(nameFormat, @"\${basename}", captureGroup) + "$";
                string replaceregex = "${basename}" + contextstr;
                func = r => Regex.Replace(r, patternregex, replaceregex);
                return from n in NameTransformer.Transform(typeName, func)
                    where n.EndsWith(contextstr)
                    select n;
            };
            LocateTypeForModelType = delegate (Type modelType, DependencyObject displayLocation, object context) {
                string fullName = modelType.FullName;
                if (Execute.InDesignMode)
                {
                    fullName = ModifyModelTypeAtDesignTime(fullName);
                }
                fullName = fullName.Substring(0, (fullName.IndexOf("`") < 0) ? fullName.Length : fullName.IndexOf("`"));
                IEnumerable<string> source = TransformName(fullName, context);
                Type type = (from n in source
                    join t in from a in AssemblySource.Instance select a.GetExportedTypes() on n equals t.FullName
                    select t).FirstOrDefault<Type>();
                if (type == null)
                {
                    Log.Warn("View not found. Searched: {0}.", new object[] { string.Join(", ", source.ToArray<string>()) });
                }
                return type;
            };
            LocateForModelType = delegate (Type modelType, DependencyObject displayLocation, object context) {
                Type arg = LocateTypeForModelType(modelType, displayLocation, context);
                return (arg == null) ? new TextBlock() : GetOrCreateViewType(arg);
            };
            LocateForModel = delegate (object model, DependencyObject displayLocation, object context) {
                IViewAware aware = model as IViewAware;
                if (aware != null)
                {
                    UIElement view = aware.GetView(context) as UIElement;
                    if (view != null)
                    {
                        Log.Info("Using cached view for {0}.", new object[] { model });
                        return view;
                    }
                }
                return LocateForModelType(model.GetType(), displayLocation, context);
            };
            DeterminePackUriFromType = delegate (Type viewModelType, Type viewType) {
                string oldValue = viewType.Assembly.GetAssemblyName();
                string str2 = viewType.FullName.Replace(oldValue, string.Empty).Replace(".", "/") + ".xaml";
                if (!Application.Current.GetType().Assembly.GetAssemblyName().Equals(oldValue))
                {
                    return "/" + oldValue + ";component" + str2;
                }
                return str2;
            };
            ConfigureTypeMappings(new TypeMappingConfiguration());
        }

        public static void AddDefaultTypeMapping(string viewSuffix = "View")
        {
            if (useNameSuffixesInMappings)
            {
                AddNamespaceMapping(string.Empty, string.Empty, viewSuffix);
                AddSubNamespaceMapping(defaultSubNsViewModels, defaultSubNsViews, viewSuffix);
            }
        }

        public static void AddNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = "View")
        {
            string regEx = RegExHelper.NamespaceToRegEx(nsSource + ".");
            if (!string.IsNullOrEmpty(nsSource))
            {
                regEx = "^" + regEx;
            }
            string captureGroup = RegExHelper.GetCaptureGroup("origns", regEx);
            string[] nsTargetsRegEx = (from t in nsTargets select t + ".").ToArray<string>();
            AddTypeMapping(captureGroup, null, nsTargetsRegEx, viewSuffix);
        }

        public static void AddNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = "View")
        {
            AddNamespaceMapping(nsSource, new string[] { nsTarget }, viewSuffix);
        }

        public static void AddSubNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = "View")
        {
            string str2;
            string rxbeforetgt;
            string rxaftertgt;
            string regEx = RegExHelper.NamespaceToRegEx(nsSource + ".");
            string namespaceCaptureGroup = rxbeforetgt = str2 = rxaftertgt = string.Empty;
            if (!string.IsNullOrEmpty(nsSource))
            {
                if (!nsSource.StartsWith("*"))
                {
                    namespaceCaptureGroup = RegExHelper.GetNamespaceCaptureGroup("nsbefore");
                    rxbeforetgt = "${nsbefore}";
                }
                if (!nsSource.EndsWith("*"))
                {
                    str2 = RegExHelper.GetNamespaceCaptureGroup("nsafter");
                    rxaftertgt = "${nsafter}";
                }
            }
            string captureGroup = RegExHelper.GetCaptureGroup("subns", regEx);
            string nsSourceReplaceRegEx = namespaceCaptureGroup + captureGroup + str2;
            string[] nsTargetsRegEx = (from t in nsTargets select rxbeforetgt + t + "." + rxaftertgt).ToArray<string>();
            AddTypeMapping(nsSourceReplaceRegEx, null, nsTargetsRegEx, viewSuffix);
        }

        public static void AddSubNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = "View")
        {
            AddSubNamespaceMapping(nsSource, new string[] { nsTarget }, viewSuffix);
        }

        public static void AddTypeMapping(string nsSourceReplaceRegEx, string nsSourceFilterRegEx, string[] nsTargetsRegEx, string viewSuffix = "View")
        {
            RegisterViewSuffix(viewSuffix);
            List<string> list = new List<string>();
            string str = useNameSuffixesInMappings ? viewSuffix : string.Empty;
            foreach (string str2 in nsTargetsRegEx)
            {
                list.Add(str2 + string.Format(nameFormat, "${basename}", str));
            }
            string nameCaptureGroup = RegExHelper.GetNameCaptureGroup("basename");
            string viewModelSuffix = string.Empty;
            if (useNameSuffixesInMappings)
            {
                viewModelSuffix = ViewLocator.viewModelSuffix;
                if (!(ViewLocator.viewModelSuffix.Contains(viewSuffix) || !includeViewSuffixInVmNames))
                {
                    viewModelSuffix = viewSuffix + viewModelSuffix;
                }
            }
            string globalFilterPattern = string.IsNullOrEmpty(nsSourceFilterRegEx) ? null : (nsSourceFilterRegEx + string.Format(nameFormat, @"[A-Za-z_]\w*", viewModelSuffix) + "$");
            string captureGroup = RegExHelper.GetCaptureGroup("suffix", viewModelSuffix);
            NameTransformer.AddRule(nsSourceReplaceRegEx + string.Format(nameFormat, nameCaptureGroup, captureGroup) + "$", list.ToArray(), globalFilterPattern);
        }

        public static void AddTypeMapping(string nsSourceReplaceRegEx, string nsSourceFilterRegEx, string nsTargetRegEx, string viewSuffix = "View")
        {
            AddTypeMapping(nsSourceReplaceRegEx, nsSourceFilterRegEx, new string[] { nsTargetRegEx }, viewSuffix);
        }

        public static void ConfigureTypeMappings(TypeMappingConfiguration config)
        {
            if (string.IsNullOrEmpty(config.DefaultSubNamespaceForViews))
            {
                throw new ArgumentException("DefaultSubNamespaceForViews field cannot be blank.");
            }
            if (string.IsNullOrEmpty(config.DefaultSubNamespaceForViewModels))
            {
                throw new ArgumentException("DefaultSubNamespaceForViewModels field cannot be blank.");
            }
            if (string.IsNullOrEmpty(config.NameFormat))
            {
                throw new ArgumentException("NameFormat field cannot be blank.");
            }
            if (config.UseNameSuffixesInMappings && string.IsNullOrEmpty(config.ViewModelSuffix))
            {
                throw new ArgumentException("ViewModelSuffix field cannot be blank if UseNameSuffixesInMappings is true.");
            }
            NameTransformer.Clear();
            ViewSuffixList.Clear();
            defaultSubNsViews = config.DefaultSubNamespaceForViews;
            defaultSubNsViewModels = config.DefaultSubNamespaceForViewModels;
            nameFormat = config.NameFormat;
            useNameSuffixesInMappings = config.UseNameSuffixesInMappings;
            viewModelSuffix = config.ViewModelSuffix;
            ViewSuffixList.AddRange(config.ViewSuffixList);
            includeViewSuffixInVmNames = config.IncludeViewSuffixInViewModelNames;
            SetAllDefaults();
        }

        public static void InitializeComponent(object element)
        {
            MethodInfo method = element.GetType().GetMethod("InitializeComponent", BindingFlags.Public | BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(element, null);
            }
        }

        public static void RegisterViewSuffix(string viewSuffix)
        {
            if (Enumerable.Count<string>(ViewSuffixList, s => s == viewSuffix) == 0)
            {
                ViewSuffixList.Add(viewSuffix);
            }
        }

        private static void SetAllDefaults()
        {
            if (useNameSuffixesInMappings)
            {
                ViewSuffixList.ForEach(new Action<string>(ViewLocator.AddDefaultTypeMapping));
            }
            else
            {
                AddSubNamespaceMapping(defaultSubNsViewModels, defaultSubNsViews, "View");
            }
        }
    }
}

