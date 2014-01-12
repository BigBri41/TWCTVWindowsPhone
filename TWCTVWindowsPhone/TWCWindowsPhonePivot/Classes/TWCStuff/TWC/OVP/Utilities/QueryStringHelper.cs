namespace TWC.OVP.Utilities
{
    using System;
    using System.Windows.Browser;

    public static class QueryStringHelper
    {
        public static string DemoQueryStringParm = "DemoMode";
        public static string DemoStreamSourceParm = "StreamSource";

        public static DemoMode CurrentDemoMode
        {
            get
            {
                DemoMode result = DemoMode.Default;
                if (HtmlPage.Document.QueryString.ContainsKey(DemoQueryStringParm))
                {
                    string str = HtmlPage.Document.QueryString[DemoQueryStringParm];
                    Enum.TryParse<DemoMode>(str, out result);
                    return result;
                }
                return result;
            }
        }

        public static Uri StreamSource
        {
            get
            {
                if (HtmlPage.Document.QueryString.ContainsKey(DemoStreamSourceParm))
                {
                    Uri uri;
                    Uri.TryCreate(HtmlPage.Document.QueryString[DemoStreamSourceParm], UriKind.Absolute, out uri);
                    return uri;
                }
                return null;
            }
        }

        public static bool StreamSourceActive
        {
            get
            {
                return ((CurrentDemoMode == DemoMode.TestLab) && (StreamSource != null));
            }
        }

        public enum DemoMode
        {
            Default,
            Mock,
            TestLab,
            Manual
        }
    }
}

