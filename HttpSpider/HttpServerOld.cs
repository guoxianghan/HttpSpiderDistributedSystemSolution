using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpHelper
{
    public class HttpServerOld : IHttpHeader
    {
        public HttpHelper httpHelper = new HttpHelper();

        #region IHttpLogin 成员

        public string Url { get; set; }

        //public string Username { get; set; }

        //public string PassWord { get; set; }

        public string Referer { get; set; }

        public string Method { get; set; }

        public string Host { get; set; }

        public string Cookie { get; set; }

        public string PostData { get; set; }

        public string VerifyCode { get; set; }

        public string UseAgent { get; set; }

        public string Accept { get; set; }

        public string AcceptEncoding { get; set; }

        public string AcceptLanguage { get; set; }
        string _CerPath = "";
        public string CerPath { get { return _CerPath; } set { _CerPath = value; } }

        public string ContentType { get; set; }
        public string Origin { get; set; }
        public string UACPU { get; set; }
        public string x_requested_with { get; set; }
        public int Content_Length = 0;
        System.Net.WebHeaderCollection headerCollection = new System.Net.WebHeaderCollection();
        public System.Net.WebHeaderCollection HeaderCollection
        {
            get { return headerCollection; }
            set { headerCollection = value; }
        }
        public bool IsContent_Lenght = false;
        private Boolean _expect100continue = true;
        /// <summary>
        ///  获取或设置一个 System.Boolean 值，该值确定是否使用 100-Continue 行为。如果 POST 请求需要 100-Continue 响应，则为 true；否则为 false。默认值为 true。
        /// </summary>
        public Boolean Expect100Continue
        {
            get { return _expect100continue; }
            set { _expect100continue = value; }
        }

        bool keepAlive = true;

        public bool KeepAlive
        {
            get { return keepAlive; }
            set { keepAlive = value; }
        }
        public Boolean Allowautoredirect { get; set; }

        public virtual HttpResult GetHttpResult()
        {
            string err;
            HttpServer item = new HttpServer();
            item.CookieCollection = new System.Net.CookieCollection();
            item.Allowautoredirect = this.Allowautoredirect;
            item.PostData = this.PostData;
            item.Cookie = this.Cookie;
            item.Url = this.Url;
            item.Method = this.Method;
            item.Host = this.Host;
            item.Accept = this.Accept;
            item.ContentType = this.ContentType;
            item.CerPath = this.CerPath;
            if (!string.IsNullOrEmpty(this.Referer))
                item.Referer = this.Referer;
            if (!string.IsNullOrEmpty(this.UseAgent))
                item.UserAgent = this.UseAgent;
            if (!string.IsNullOrEmpty(this.Cookie))
                item.Cookie = this.Cookie;
            if (!string.IsNullOrEmpty(this.UACPU))
                item.UserAgent = this.UseAgent;
            if (!string.IsNullOrEmpty(this.Accept))
                item.Accept = this.Accept;
            if (!string.IsNullOrEmpty(this.Referer))
                item.Referer = this.Referer;
            if (!string.IsNullOrEmpty(this.AcceptEncoding))
                item.headerCollection.Add("Accept-Encoding", this.AcceptEncoding);
            if (!string.IsNullOrEmpty(this.AcceptLanguage))
                item.headerCollection.Add("Accept-Language", this.AcceptLanguage);
            if (!string.IsNullOrEmpty(this.Origin))
                item.headerCollection.Add("Origin", this.Origin);
            if (!string.IsNullOrEmpty(this.UACPU))
                item.headerCollection.Add("UA-CPU", this.UACPU);
            if (!string.IsNullOrEmpty(this.x_requested_with))
                item.headerCollection.Add("x-requested-with", this.x_requested_with);
            item.Expect100Continue = this.Expect100Continue;

            item.KeepAlive = this.keepAlive;
            if (IsContent_Lenght)
            {
                item.IsContent_Length = true;//需要设置Content_Lenght
                item.Content_Length = this.Content_Length;
            }
            if (headerCollection.Count>0)
                item.headerCollection.Add(headerCollection);
            return httpHelper.GetHtmlResult(item,out err);
        }

        /// <summary>
        /// 初始化所有属性
        /// </summary>
        public void Init()
        {
            this._expect100continue = false;
            this.Accept = "";
            this.AcceptEncoding = "";
            this.AcceptLanguage = "";
            this.Allowautoredirect = false;
            this.ContentType = "";
            this.Cookie = "";
            this.Expect100Continue = true;
            this.headerCollection.Clear();
            //this.HeaderCollection.Clear();
            this.Host = "";
            this.IsContent_Lenght = false;
            this.KeepAlive = true;
            this.Method = "GET";
            this.Origin = "";
            this.PostData = "";
            this.Referer = "";
            this.UACPU = "";
            this.Url = "";
            this.UseAgent = "";
            this.VerifyCode = "";
            this.x_requested_with = "";
        }
        #endregion
    }
}
