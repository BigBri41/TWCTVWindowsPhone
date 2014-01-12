namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;

    public class TypeMappingConfiguration
    {
        public string DefaultSubNamespaceForViewModels = "ViewModels";
        public string DefaultSubNamespaceForViews = "Views";
        public bool IncludeViewSuffixInViewModelNames = true;
        public string NameFormat = "{0}{1}";
        public bool UseNameSuffixesInMappings = true;
        public string ViewModelSuffix = "ViewModel";
        public List<string> ViewSuffixList = new List<string>(new string[] { "View", "Page" });
    }
}

