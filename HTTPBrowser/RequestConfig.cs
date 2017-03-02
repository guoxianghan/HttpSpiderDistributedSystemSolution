using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HTTPBrowser
{
    public class RequestConfig
    {
        public string ContentType { get; set; }
        public RequestConfig() { }
        public RequestConfig(string method)
        {
            Method = method;
        }
        private string _Method = "GET";
        public string Method
        {
            get { return _Method; }
            set
            {
                var method = value.ToUpper();
                string[] list = { "GET", "POST", "DELETE", "PUT", "HEAD" };
                if (list.Contains(method))
                {
                    _Method = method;
                }
                else
                {
                    throw new Exception("非法的Method：" + method);
                }
            }
        }
        public int RetryCount = 3;
        public bool UseCookies = true;
        public string UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36";
        public Uri Referer { get; set; }
        public System.Net.Cookie[] Cookies { get; set; }
        /// <summary>
        /// 页面下载超时时间，单位（s）。
        /// </summary>
        public int? PageLoadTimeout { get; set; }
        /// <summary>
        /// 页面下载完成后，等待js执行完毕的超时时间，单位（s）。
        /// </summary>
        public int? ScriptTimeout { get; set; }

        public int? DownloadRetryWaitDuration { get; set; }

        public Dictionary<string, string> Headers = new Dictionary<string, string>();

        public string DownloadFileDirectory { get; set; }

        public Proxy Proxy { get; set; }
    }
}
