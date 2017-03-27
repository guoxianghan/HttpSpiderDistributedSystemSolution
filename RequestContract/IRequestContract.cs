using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HttpEntry;
using TaskModel;
using HttpSpider;
using HTTPBrowser;
using System.Net;

namespace RequestContract
{
    //[ServiceContract(CallbackContract = typeof(IMessageCallback))]
    [ServiceContract]
    public interface IRequestContract
    {
        [OperationContract]
        HttpResult SingleSpiderRequestTask(int taskid, params Cookie[] c);
        [OperationContract(Name = "BrowserRequestTaskID")]
        ResponsePack_WebBrowser BrowserRequest(int taskid, params Cookie[] c);

        [OperationContract]
        HttpResult SingleSpiderRequestTaskDetail(int taskid, params Cookie[] c);
        [OperationContract]
        ResponsePack_WebBrowser SingleBrowserRequestTaskDetail(int taskid, params Cookie[] c);

        [OperationContract]
        string SingleSpiderRequestTaskDetailHtml(int taskid, params Cookie[] c);
        [OperationContract]
        string SingleBrowserRequestTaskDetailHtml(int taskid, params Cookie[] c);

        [OperationContract]
        List<HttpResult> SpiderRequestTaskID(int taskid, params Cookie[] c);

        [OperationContract(Name = "BrowserRequestTaskIDList")]
        List<ResponsePack_WebBrowser> BrowserRequestTaskID(int taskid, params Cookie[] c);
        [OperationContract]
        HttpResult SingleSpiderRequest(HarRequest harrequest, params Cookie[] c);

        [OperationContract(Name = "BrowserRequestEntity")]
        ResponsePack_WebBrowser BrowserRequest(HarRequest harrequest, params Cookie[] c);
        [OperationContract]
        List<HttpResult> SpiderRequestEntrys(HarRequest[] harRequests, params Cookie[] c);
        [OperationContract]
        List<ResponsePack_WebBrowser> BrowserRequestEntrys(HarEntry[] harEntry, params Cookie[] c);

        [OperationContract]
        string SingleSpiderRequestTaskIDHtml(int taskid, params Cookie[] c);
        [OperationContract]
        string SingleBrowserRequestTaskIDHtml(int taskid, params Cookie[] c);

        [OperationContract(Name = "SpiderRequestTaskIDHtml")]
        string SpiderRequestHtml(int taskid, params Cookie[] c);
        [OperationContract(Name = "BrowserRequestTaskIDHtml")]
        string BrowserRequestHtml(int taskid, params Cookie[] c);
        [OperationContract]
        string SingleSpiderRequestEntryHtml(HarRequest harrequest, params Cookie[] c);
        [OperationContract]
        string BrowserRequestHtml(HarRequest harrequest, params Cookie[] c);

        [OperationContract(Name = "SpiderRequestEntrysHtml")]
        string SpiderRequestHtml(HarRequest[] harRequests, params Cookie[] c);
        [OperationContract(Name = "BrowserRequestEntrysListHtml")]
        string BrowserRequestHtml(HarRequest[] harRequests, params Cookie[] c);
        [OperationContract]
        void Subscribe();

        [OperationContract]
        void Unsubscribe();
    }
}
