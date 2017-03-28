using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RequestContract;
using HttpEntry;
using HttpSpider;
using HTTPBrowser;
using TaskModel;
using System.ServiceModel;
using System.Net;
using CommonHelper;

namespace RequestService
{
    [ServiceBehavior]
    public class RequestService_old : IRequestContract_old
    {
        static List<IMessageCallback> subscribers = new List<IMessageCallback>();
        public List<ResponsePack_WebBrowser> BrowserRequestEntrys(HarEntry[] harEntry, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public string BrowserRequestHtml(int taskid, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public string BrowserRequestHtml(HarRequest[] harRequests, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public List<ResponsePack_WebBrowser> BrowserRequestTaskID(int taskid, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public ResponsePack_WebBrowser BrowserRequest(HarRequest harrequest, params Cookie[] c)
        {
            return Request.GetRequest(harrequest, c);
        }

        public string BrowserRequestHtml(HarRequest harrequest, params Cookie[] c)
        {
            return Request.GetRequestHtml(harrequest, c);
        }

        public ResponsePack_WebBrowser BrowserRequest(int taskid, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public ResponsePack_WebBrowser SingleBrowserRequestTaskDetail(int taskid, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public string SingleBrowserRequestTaskDetailHtml(int taskid, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public string SingleBrowserRequestTaskIDHtml(int taskid, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public HttpResult SpiderRequest(HttpServer httpserver, params Cookie[] c)
        {
            Logger.WriteLog("SpiderRequest");
            httpserver.Cookies = new CookieContainer();
            if (c != null && c.Count() != 0)
                foreach (var item in c)
                    httpserver.Cookies.Add(item);
            var r = httpserver.GetHttpResult();
            return r;
        }

        public string SingleSpiderRequestEntryHtml(HarRequest harrequest, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public HttpResult SingleSpiderRequestTask(int taskid, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public HttpResult SingleSpiderRequestTaskDetail(int taskid, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public string SingleSpiderRequestTaskDetailHtml(int taskid, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public string SingleSpiderRequestTaskIDHtml(int taskid, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public List<HttpResult> SpiderRequestEntrys(HarRequest[] harRequests, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public string SpiderRequestHtml(int taskid, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public string SpiderRequestHtml(HarRequest[] harRequests, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public List<HttpResult> SpiderRequestTaskID(int taskid, params Cookie[] c)
        {
            throw new NotImplementedException();
        }

        public void Subscribe()
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe()
        {
            throw new NotImplementedException();
        }

        public string SpiderRequestHtml(HttpServer httpitem, params Cookie[] c)
        {
            Logger.WriteLog("SpiderRequestHtml");
            string err = "";
            HttpHelper d = new HttpHelper();
            httpitem.Cookies = new CookieContainer();
            HttpResult r = null;
            if (c != null && c.Count() != 0)
                foreach (var item in c)
                    httpitem.Cookies.Add(item);
            try
            {
                r = d.GetHtmlResult(httpitem, out err);
                return r.Html;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string BrowserRequestHtml(string url, params Cookie[] c)
        {
            Logger.WriteLog("Get");
            var r = WebBrowser.Get(url, c.ToList());
            return r.PageSource;
        }

        public ResponsePack_WebBrowser Get(string url, List<Cookie> cookies, Dictionary<string, string> data = null)
        {
            Logger.WriteLog("Get");
            var r = WebBrowser.Get(url, cookies, data);
            return r;
        }

        public ResponsePack_WebBrowser Post(string url, List<Cookie> cookies, Dictionary<string, string> data = null, Encoding requestEncoding = null)
        {
            Logger.WriteLog("Post");
            return WebBrowser.Post(url, cookies, data, requestEncoding);
        }
    }
}
