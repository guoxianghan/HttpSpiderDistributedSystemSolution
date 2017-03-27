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

namespace RequestService
{
    [ServiceBehavior]
    public class RequestService : IRequestContract
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

        public HttpResult SingleSpiderRequest(HarRequest harrequest, params Cookie[] c)
        {
            throw new NotImplementedException();
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
    }
}
