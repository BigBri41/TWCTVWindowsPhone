namespace Caliburn.Micro
{
    using System;

    public static class RegExHelper
    {
        public const string NameRegEx = @"[A-Za-z_]\w*";
        public const string NamespaceRegEx = @"([A-Za-z_]\w*\.)*";
        public const string SubNamespaceRegEx = @"[A-Za-z_]\w*\.";

        public static string GetCaptureGroup(string groupName, string regEx)
        {
            return ("(?<" + groupName + ">" + regEx + ")");
        }

        public static string GetNameCaptureGroup(string groupName)
        {
            return GetCaptureGroup(groupName, @"[A-Za-z_]\w*");
        }

        public static string GetNamespaceCaptureGroup(string groupName)
        {
            return GetCaptureGroup(groupName, @"([A-Za-z_]\w*\.)*");
        }

        public static string NamespaceToRegEx(string srcNamespace)
        {
            return srcNamespace.Replace(".", @"\.").Replace(@"*\.", @"([A-Za-z_]\w*\.)*");
        }
    }
}

