using System;
using System.Collections.Generic;
using System.Text;

namespace HTTPBrowser
{
    public partial class WebBrowser
    {
        #region static

        public static ResponsePack_WebBrowser Get(string url, Dictionary<string, string> data = null)
        {
            return Get(new Uri(url), data);
        }

        public static ResponsePack_WebBrowser Get(Uri uri, Dictionary<string, string> data = null)
        {
            BybClient browser = new BybClient();
            return browser.Request(uri, data);
        }

        public static ResponsePack_WebBrowser Post(string url, Dictionary<string, string> data = null, Encoding requestEncoding = null)
        {
            return Post(new Uri(url), data, requestEncoding);
        }

        public static ResponsePack_WebBrowser Post(Uri uri, Dictionary<string, string> data = null, Encoding requestEncoding = null)
        {
            BybClient browser = new BybClient();
            browser.Config.Method = "POST";
            return browser.Request(uri, data, requestEncoding);
        }

        #endregion static
    }
}