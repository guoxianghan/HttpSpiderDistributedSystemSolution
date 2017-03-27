using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RequestContract;
using System.ServiceModel;
using HTTPBrowser;
using System.IO;
using Newtonsoft.Json;
using HttpEntry;
using System.Net;

namespace RequestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            WebBrowser web = new WebBrowser();
            //ResponsePack_WebBrowser r = web.Request("https://www.baidu.com", null) as ResponsePack_WebBrowser;
            string json = File.ReadAllText(@"C:\Users\Guo\Desktop\zc-paimai.taobao.com.har");
            HttpHar h = JsonConvert.DeserializeObject<HttpHar>(json);
            Cookie[] c = new Cookie[0];
            var r = web.Request(h.log.entries[0].request, c);

            web.RefreshConfig();
            RequestConfig req = new RequestConfig();
            ChannelFactory<IRequestContract> channelFactory;
            using (channelFactory = new ChannelFactory<RequestContract.IRequestContract>("Request"))
            {
                IRequestContract ito = channelFactory.CreateChannel();
                while (true)
                {
                    ResponsePack_WebBrowser res = ito.BrowserRequest(h.log.entries[0].request);
                    string html = ito.BrowserRequestHtml(h.log.entries[0].request);
                    Console.WriteLine(res);

                }





                // action?.Invoke(channelFactory.CreateChannel());
            }
        }
    }
}
