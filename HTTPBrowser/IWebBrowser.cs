using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using HttpEntry;
using System.IO;

namespace HTTPBrowser
{
    public abstract class IWebBrowser : IDisposable
    {
        public IWebBrowser()
        {
            this.Config = new RequestConfig();
            Init(this.Config);
        }
        public Stream RequestStream { get; set; }
        /// <summary>
        /// 当修改Config之后，需要调用此方法以应用新的配置。
        /// </summary>
        public void RefreshConfig()
        {
            Init(this.Config);
        }

        public IWebBrowser(RequestConfig config)
        {
            this.Config = config;
            Init(this.Config);
        }

        protected abstract void Init(RequestConfig config);

        public RequestConfig Config { get; private set; }

        public abstract string GetCookie(Uri uri, string name);
        public abstract Dictionary<string, string> GetCookies(Uri uri);

        public abstract void SetCookie(System.Net.Cookie cookie);

        /// <summary>
        /// 下载文本内容
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract IResponsePack Request(Uri uri, Dictionary<string, string> data = null);
        public IResponsePack Request(string url, Dictionary<string, string> data = null)
        {
            return Request(new Uri(url), data);
        }
        public IResponsePack Request(HarRequest request, System.Net.Cookie[] cookie)
        {
            this.Config = new RequestConfig();
            this.Config.ContentType = request.headers.FirstOrDefault(x => x.name == "ContentType")?.value;
            this.Config.Cookies = cookie;
            this.Config.Headers = request.header;
            this.Config.Method = request.method;
            if (request.header.Keys.Contains("Referer"))
                this.Config.Referer = new Uri(request.headers.FirstOrDefault(x => x.name == "Referer")?.value);
            if (request.header.Keys.Contains("User-Agent"))
                this.Config.UserAgent = request.headers.FirstOrDefault(x => x.name == "UserAgent")?.value;
            return Request(new Uri(request.url), request.header);
        }
        /// <summary>
        /// 待完善
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public IResponsePack Request(string url, string method, string referer, string post, Dictionary<string, string> headers, System.Net.Cookie[] cookie)
        {
            this.Config = new RequestConfig();
            this.Config.ContentType = headers.FirstOrDefault(x => x.Key == "ContentType").Value;
            this.Config.Cookies = cookie;
            this.Config.Headers = headers;
            this.Config.Method = method;
            if (headers.Keys.Contains("Referer"))
                this.Config.Referer = new Uri(headers.FirstOrDefault(x => x.Key == "Referer").Value);
            else this.Config.Referer = new Uri(referer);
            if (headers.Keys.Contains("User-Agent"))
                this.Config.UserAgent = headers.FirstOrDefault(x => x.Key == "UserAgent").Value;
            return Request(new Uri(url), headers);
        }
        /// <summary>
        /// 下载json形式的内容（由于有些浏览器默认会将json内容包装成html，所以可能需要额外处理，以提取出其中的json部分。）
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract IResponsePack DownloadJson(Uri uri, Dictionary<string, string> data = null);

        public abstract void Dispose();
    }
}