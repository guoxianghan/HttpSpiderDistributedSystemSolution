using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json; 
using System.IO;
using HttpSpider;
using HttpEntry;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string json = File.ReadAllText(@"C:\Users\Sier\Desktop\m.ctrip.com.har");
            //string json = File.ReadAllText(@"C:\Users\Sier\Desktop\aio.liantuo.com.har");
            HttpHar h = JsonConvert.DeserializeObject<HttpHar>(json);

            HttpServer s = new HttpServer();
            var r = s.GetHttpResult(h.log.entries[0].request);

            s = new HttpServer();
            s.Host = "";
            s.Url = "";
            s.Method = "";
            s.PostData = "";
            r = s.GetHttpResult();
        }
    }
}
