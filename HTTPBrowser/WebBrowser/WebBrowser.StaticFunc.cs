using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HTTPBrowser
{
    public partial class WebBrowser
    {
        #region static

        public static ResponsePack_WebBrowser Get(string url, List<Cookie> cookies, Dictionary<string, string> data = null)
        {
            return Get(new Uri(url), cookies, data);
        }

        public static ResponsePack_WebBrowser Get(Uri uri, List<Cookie> cookies, Dictionary<string, string> data = null)
        {
            BybClient browser = new BybClient();
            if (cookies != null && cookies.Count != 0)
                foreach (var VARIABLE in cookies)
                {
                    browser.SetCookie(VARIABLE);
                }
            return browser.Request(uri, data);
        }

        public static ResponsePack_WebBrowser Post(string url, List<Cookie> cookies, Dictionary<string, string> data = null, Encoding requestEncoding = null)
        {
            return Post(new Uri(url), cookies, data, requestEncoding);
        }

        public static ResponsePack_WebBrowser Post(Uri uri, List<Cookie> cookies, Dictionary<string, string> data = null, Encoding requestEncoding = null)
        {
            BybClient browser = new BybClient();
            if (cookies != null && cookies.Count != 0)
                foreach (var VARIABLE in cookies)
                {
                    browser.SetCookie(VARIABLE);
                }
            browser.Config.Method = "POST";
            return browser.Request(uri, data, requestEncoding);
        }

        #endregion static
    }
}