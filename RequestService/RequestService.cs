using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RequestContract;
using HttpEntry;
using HttpSpider;
using HTTPBrowser;
using TaskModel;
namespace RequestService
{
    public class RequestService : IRequestContract
    {
        public HttpResult SingleSpiderRequestTask(int taskid)
        {
            throw new NotImplementedException();
        }
        public WebBrowser SingleBrowserRequestTask(int taskid)
        {
            throw new NotImplementedException();
        }
        public List<HttpResult> SpiderRequestTaskID(int taskid)
        {
            throw new NotImplementedException();
        }

        public List<WebBrowser> BrowserRequestTaskID(int taskid)
        {
            throw new NotImplementedException();
        }

        public HttpResult SingleSpiderRequest(HarEntry harEntry)
        {
            throw new NotImplementedException();
        }

        public WebBrowser SingleBrowserRequest(HarEntry harEntry)
        {
            throw new NotImplementedException();
        }

        public List<HttpResult> SpiderRequestEntrys(HarEntry[] HarEntrys)
        {
            throw new NotImplementedException();
        }

        public List<WebBrowser> BrowserRequestEntrys(HarEntry[] HarEntrys)
        {
            throw new NotImplementedException();
        }


        public string SingleSpiderRequestTaskIDHtml(int taskid)
        {
            throw new NotImplementedException();
        }
        public string SingleBrowserRequestTaskIDHtml(int taskid)
        {
            throw new NotImplementedException();
        }
        public string SpiderRequestHtml(int taskid)
        {
            throw new NotImplementedException();
        }

        public string BrowserRequestHtml(int taskid)
        {
            throw new NotImplementedException();
        }

        public string SingleSpiderRequestEntryHtml(HarEntry harEntry)
        {
            throw new NotImplementedException();
        }

        public string SingleBrowserRequestEntryHtml(HarEntry harEntry)
        {
            throw new NotImplementedException();
        }

        public string SpiderRequestHtml(HarEntry[] HarEntrys)
        {
            throw new NotImplementedException();
        }

        public string BrowserRequestHtml(HarEntry[] HarEntrys)
        {
            throw new NotImplementedException();
        }

        public HttpResult SingleSpiderRequestTaskDetail(int taskid)
        {
            throw new NotImplementedException();
        }

        public WebBrowser SingleBrowserRequestTaskDetail(int taskid)
        {
            throw new NotImplementedException();
        }

        public string SingleSpiderRequestTaskDetailHtml(int taskid)
        {
            throw new NotImplementedException();
        }

        public string SingleBrowserRequestTaskDetailHtml(int taskid)
        {
            throw new NotImplementedException();
        }
    }
}
