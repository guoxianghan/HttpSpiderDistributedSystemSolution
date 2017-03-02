using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HttpEntry;
using TaskModel;
using HttpSpider;
using HTTPBrowser;
namespace RequestContract
{
    [ServiceContract]
    public interface IRequestContract
    {
        [OperationContract]
        HttpResult SingleSpiderRequestTask(int taskid);
        [OperationContract]
        WebBrowser SingleBrowserRequestTask(int taskid);

        [OperationContract]
        HttpResult SingleSpiderRequestTaskDetail(int taskid);
        [OperationContract]
        WebBrowser SingleBrowserRequestTaskDetail(int taskid);

        [OperationContract]
        string SingleSpiderRequestTaskDetailHtml(int taskid);
        [OperationContract]
        string SingleBrowserRequestTaskDetailHtml(int taskid);

        [OperationContract]
        List<HttpResult> SpiderRequestTaskID(int taskid);
        [OperationContract]
        List<WebBrowser> BrowserRequestTaskID(int taskid);
        [OperationContract]
        HttpResult SingleSpiderRequest(HarEntry harEntry);
        [OperationContract]
        WebBrowser SingleBrowserRequest(HarEntry harEntry);
        [OperationContract]
        List<HttpResult> SpiderRequestEntrys(HarEntry[] HarEntrys);
        [OperationContract]
        List<WebBrowser> BrowserRequestEntrys(HarEntry[] HarEntrys);

        [OperationContract]
        string SingleSpiderRequestTaskIDHtml(int taskid);
        [OperationContract]
        string SingleBrowserRequestTaskIDHtml(int taskid);

        [OperationContract(Name = "SpiderRequestTaskIDHtml")]
        string SpiderRequestHtml(int taskid);
        [OperationContract(Name = "BrowserRequestTaskIDHtml")]
        string BrowserRequestHtml(int taskid);
        [OperationContract]
        string SingleSpiderRequestEntryHtml(HarEntry harEntry);
        [OperationContract]
        string SingleBrowserRequestEntryHtml(HarEntry harEntry);

        [OperationContract(Name = "SpiderRequestEntrysHtml")]
        string SpiderRequestHtml(HarEntry[] HarEntrys);
        [OperationContract]
        string BrowserRequestHtml(HarEntry[] HarEntrys);
    }
}
