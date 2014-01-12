namespace TWC.OVP.Services
{
    using Caliburn.Micro;
    using System;
    using System.Net;
    using System.Windows.Browser;

    public class CookieHelper
    {
        public static CookieCollection GetCookies()
        {
            CookieCollection cookies = new CookieCollection();
            delegate {
                foreach (string str in HtmlPage.Document.Cookies.Split(new char[] { ';' }))
                {
                    string[] strArray2 = str.Split(new char[] { '=' });
                    if (strArray2.Length == 2)
                    {
                        Cookie cookie = new Cookie(strArray2[0].Trim(), strArray2[1].Trim());
                        cookies.Add(cookie);
                    }
                }
            }.OnUIThread();
            return cookies;
        }

        public static void SetCookiesToBrowser(CookieCollection cookies)
        {
            foreach (Cookie cookie in cookies)
            {
                DateTime time = cookie.Expires.ToUniversalTime();
                string str = cookie.Name + "=" + cookie.Value + ";expires=" + time.ToString("R");
                HtmlPage.Document.SetProperty("cookie", str);
            }
        }
    }
}

