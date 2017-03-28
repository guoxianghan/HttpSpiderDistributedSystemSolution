using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using CommonHelper;
using HttpSpider;
using HTTPBrowser;
using RequestContract;

namespace RequestService
{
    [ServiceBehavior]
    public class RequestService : IRequestContract
    {
        public string Get(string url, List<Cookie> cookies)
        {
            Logger.WriteLog("Get");
            var r = WebBrowser.Get(url, cookies);
            return r.PageSource;
        }

        public Stream GetStream(string url, string method, List<Cookie> cookies, Dictionary<string, string> data = null, Encoding requestEncoding = null)
        {
            Logger.WriteLog("GetStream");
            if (method.ToLower() == "get")
            {
                var r = WebBrowser.Get(url, cookies);
                return r.ResultStream;
            }
            else
            {
                var r = WebBrowser.Post(url, cookies, data, requestEncoding);
                return r.ResultStream;
            }
        }

        public string HttpServerRequestGet(string url, List<Cookie> cookies, Dictionary<string, string> header = null)
        {
            Logger.WriteLog("HttpServerRequestGet");
            HttpServer s = new HttpServer();
            s.Url = url;
            s.Method = "get";
            if (cookies != null)
                cookies.ForEach(x => s.Cookies.Add(x));
            if (header != null)
                foreach (var VARIABLE in header)
                {
                    s.HeaderCollection.Add(VARIABLE.Key, VARIABLE.Value);
                }
            var r = s.GetHttpResult();
            return r.Html;
        }

        public string HttpServerRequestPost(string url, List<Cookie> cookies, string post, Dictionary<string, string> header = null, Encoding requestEncoding = null)
        {
            Logger.WriteLog("HttpServerRequestPost");
            HttpServer s = new HttpServer();
            s.Url = url;
            s.Method = "post";
            s.PostData = post;
            if (requestEncoding != null)
                s.PostEncoding = requestEncoding;
            if (cookies != null)
                cookies.ForEach(x => s.Cookies.Add(x));
            if (header != null)
                foreach (var VARIABLE in header)
                {
                    s.HeaderCollection.Add(VARIABLE.Key, VARIABLE.Value);
                }
            var r = s.GetHttpResult();
            return r.Html;
        }

        public Stream HttpServerStream(string url, string method, string param, List<Cookie> cookies, Dictionary<string, string> headers = null, Encoding requestEncoding = null)
        {
            Logger.WriteLog("HttpServerStream");
            HttpServer s = new HttpServer();
            s.Url = url;
            s.Method = method;
            s.PostData = param;
            if (requestEncoding != null)
                s.PostEncoding = requestEncoding;
            if (cookies != null)
                cookies.ForEach(x => s.Cookies.Add(x));
            if (headers != null)
                foreach (var VARIABLE in headers)
                {
                    s.HeaderCollection.Add(VARIABLE.Key, VARIABLE.Value);
                }
            var r = s.GetHttpResult();
            return r.ResultStream;
        }

        

        public string Post(string url, List<Cookie> cookies, Dictionary<string, string> data = null, Encoding requestEncoding = null)
        {
            Logger.WriteLog("Post");
            var r = WebBrowser.Post(url, cookies,data, requestEncoding);
            return r.PageSource;
        }
    }
}
