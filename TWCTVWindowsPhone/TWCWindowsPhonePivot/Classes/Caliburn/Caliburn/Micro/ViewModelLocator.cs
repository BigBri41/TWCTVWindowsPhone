namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Windows;

    public static class ViewModelLocator
    {
        private static string defaultSubNsViewModels;
        private static string defaultSubNsViews;
        private static bool includeViewSuffixInVmNames;
        public static string InterfaceCaptureGroupName = "isinterface";
        public static System.Func<object, object> LocateForView;
        public static System.Func<Type, object> LocateForViewType;
        public static System.Func<Type, bool, Type> LocateTypeForViewType;
        private static readonly ILog Log = LogManager.GetLog(typeof(ViewModelLocator));
        private static string nameFormat;
        public static readonly Caliburn.Micro.NameTransformer NameTransformer = new Caliburn.Micro.NameTransformer();
        public static System.Func<string, bool, IEnumerable<string>> TransformName;
        private static bool useNameSuffixesInMappings;
        private static string viewModelSuffix;
        private static readonly List<string> ViewSuffixList = new List<string>();

        static ViewModelLocator()
        {
            TransformName = delegate (string typeName, bool includeInterfaces) {
                System.Func<string, string> func;
                if (includeInterfaces)
                {
                    func = r => r;
                }
                else
                {
                    string interfacegrpregex = @"\${" + InterfaceCaptureGroupName + "}$";
                    func = r => Regex.IsMatch(r, interfacegrpregex) ? string.Empty : r;
                }
                return from n in NameTransformer.Transform(typeName, func)
                    where n != string.Empty
                    select n;
            };
            LocateTypeForViewType = delegate (Type viewType, bool searchForInterface) {
                string fullName = viewType.FullName;
                IEnumerable<string> source = TransformName(fullName, searchForInterface);
                Type type = (from n in source
                    join t in from a in AssemblySource.Instance select a.GetExportedTypes() on n equals t.FullName
                    select t).FirstOrDefault<Type>();
                if (type == null)
                {
                    Log.Warn("View Model not found. Searched: {0}.", new object[] { string.Join(", ", source.ToArray<string>()) });
                }
                return type;
            };
            LocateForViewType = delegate (Type viewType) {
                Type type = LocateTypeForViewType(viewType, false);
                if (type != null)
                {
                    object obj2 = IoC.GetInstance(type, null);
                    if (obj2 != null)
                    {
                        return obj2;
                    }
                }
                type = LocateTypeForViewType(viewType, true);
                return (type != null) ? IoC.GetInstance(type, null) : null;
            };
            LocateForView = delegate (object view) {
                if (view == null)
                {
                    return null;
                }
                FrameworkElement element = view as FrameworkElement;
                if ((element != null) && (element.DataContext != null))
                {
                    return element.DataContext;
                }
                return LocateForViewType(view.GetType());
            };
            ConfigureTypeMappings(new TypeMappingConfiguration());
        }

        public static void AddDefaultTypeMapping(string viewSuffix = "View")
        {
            if (useNameSuffixesInMappings)
            {
                AddNamespaceMapping(string.Empty, string.Empty, viewSuffix);
                AddSubNamespaceMapping(defaultSubNsViews, defaultSubNsViewModels, viewSuffix);
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
            Action<string> action = null;
            Action<string> func;
            List<string> replist = new List<string>();
            string interfacegrp = "${" + InterfaceCaptureGroupName + "}";
            if (useNameSuffixesInMappings)
            {
                if (!(!viewModelSuffix.Contains(viewSuffix) && includeViewSuffixInVmNames))
                {
                    string nameregex = string.Format(nameFormat, "${basename}", viewModelSuffix);
                    func = delegate (string t) {
                        replist.Add(t + "I" + nameregex + interfacegrp);
                        replist.Add(t + "I${basename}" + interfacegrp);
                        replist.Add(t + nameregex);
                        replist.Add(t + "${basename}");
                    };
                }
                else
                {
                    string nameregex = string.Format(nameFormat, "${basename}", "${suffix}" + viewModelSuffix);
                    func = delegate (string t) {
                        replist.Add(t + "I" + nameregex + interfacegrp);
                        replist.Add(t + nameregex);
                    };
                }
            }
            else
            {
                if (action == null)
                {
                    action = delegate (string t) {
                        replist.Add(t + "I${basename}" + interfacegrp);
                        replist.Add(t + "${basename}");
                    };
                }
                func = action;
            }
            nsTargetsRegEx.ToList<string>().ForEach(t => func(t));
            string str = useNameSuffixesInMappings ? viewSuffix : string.Empty;
            string globalFilterPattern = string.IsNullOrEmpty(nsSourceFilterRegEx) ? null : (nsSourceFilterRegEx + string.Format(nameFormat, @"[A-Za-z_]\w*", str) + "$");
            string nameCaptureGroup = RegExHelper.GetNameCaptureGroup("basename");
            string captureGroup = RegExHelper.GetCaptureGroup("suffix", str);
            NameTransformer.AddRule(nsSourceReplaceRegEx + string.Format(nameFormat, nameCaptureGroup, captureGroup) + "$" + RegExHelper.GetCaptureGroup(InterfaceCaptureGroupName, string.Empty), replist.ToArray(), globalFilterPattern);
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

        public static string MakeInterface(string typeName)
        {
            string str = string.Empty;
            if (typeName.Contains("[["))
            {
                int index = typeName.IndexOf("[[");
                str = typeName.Substring(index);
                typeName = typeName.Remove(index);
            }
            int num2 = typeName.LastIndexOf(".");
            return (typeName.Insert(num2 + 1, "I") + str);
        }

        private static void SetAllDefaults()
        {
            if (useNameSuffixesInMappings)
            {
                ViewSuffixList.ForEach(new Action<string>(ViewModelLocator.AddDefaultTypeMapping));
            }
            else
            {
                AddSubNamespaceMapping(defaultSubNsViews, defaultSubNsViewModels, "View");
            }
        }
    }
}

