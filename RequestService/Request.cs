using HTTPBrowser;
using HttpEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace RequestService
{
    public static class Request
    {
        static WebBrowser web;
        public static ResponsePack_WebBrowser GetRequest(HarRequest harrequest, params Cookie[] c)
        {
            web = new WebBrowser();
            var r = web.Request(harrequest, c);
            return (ResponsePack_WebBrowser)r;
        }
        public static string GetRequestHtml(HarRequest harrequest, params Cookie[] c)
        {
            web = new WebBrowser();
            var r = web.Request(harrequest, c);
            return r.PageSource;
        }

    }
}
