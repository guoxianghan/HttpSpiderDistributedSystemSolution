﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using HttpEntry;

namespace HttpSpider
{
    /// <summary>
    /// Http请求参考类 HttpWebRequest属性
    /// </summary>
    public class HttpServer //: HttpWebRequest
    {
        /// <summary>
        /// 下载成功时事件  
        /// </summary>
        public event DownloadCompleteCallBack OnDownloadDataCompleted;
        public HttpHelper HttpSpider = new HttpHelper();
        /// <summary>
        /// 请求URL必须填写
        /// </summary>
        public string Url { get; set; }
        string _Method = "GET";
        /// <summary>
        /// 请求方式默认为GET方式,当为POST方式时必须设置Postdata的值
        /// </summary>
        public string Method
        {
            get { return _Method; }
            set { _Method = value; }
        }
        int _Timeout = 10000;
        /// <summary>
        /// 默认请求超时时间
        /// </summary>
        public int Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        int _ReadWriteTimeout = 10000;
        /// <summary>
        /// 默认写入Post数据超时间
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return _ReadWriteTimeout; }
            set { _ReadWriteTimeout = value; }
        }
        /// <summary>
        /// 设置Host的标头信息
        /// </summary>
        public string Host { get; set; }
        Boolean _KeepAlive = true;
        /// <summary>
        ///  获取或设置一个值 默认值true，该值指示是否与 Internet 资源建立持久性连接默认为true。
        /// </summary>
        public Boolean KeepAlive
        {
            get { return _KeepAlive; }
            set { _KeepAlive = value; }
        }
        string _Accept = "text/html, application/xhtml+xml, */*";
        /// <summary>
        /// 请求标头值 默认为text/html, application/xhtml+xml, */*
        /// </summary>
        public string Accept
        {
            get { return _Accept; }
            set { _Accept = value; }
        }
        string _ContentType = "text/html";
        /// <summary>
        /// 请求返回类型默认 text/html
        /// </summary>
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        string _UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; Trident/4.0; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";
        /// <summary>
        ///  默认 windows server 2003,32,IE8;  
        ///  Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; Trident/4.0; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)
        /// </summary>
        public string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }

        CookieContainer _CookieContainer = new CookieContainer();
        public CookieContainer Cookies { get { return _CookieContainer; } set { _CookieContainer = value; } }

        /// <summary>
        /// 返回数据编码默认为NUll,可以自动识别,一般为utf-8,gbk,gb2312
        /// </summary>
        public Encoding Encoding { get; set; }
        private PostDataType _PostDataType = PostDataType.String;
        /// <summary>
        /// Post的数据类型
        /// </summary>
        public PostDataType PostDataType
        {
            get { return _PostDataType; }
            set { _PostDataType = value; }
        }
        /// <summary>
        /// Post请求时要发送的字符串Post数据
        /// </summary>
        public string PostData { get; set; }
        /// <summary>
        /// Post请求时要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostdataByte { get; set; }

        CookieCollection _cookieCollection = new CookieCollection();
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection
        {
            get { return _cookieCollection; }
            set { _cookieCollection = value; }
        }
        /// <summary>
        /// 请求时的Cookie
        /// </summary>
        //public string Cookie { get; set; }
        /// <summary>
        /// 来源地址，上次访问地址
        /// </summary>
        public string Referer { get; set; }
        /// <summary>
        /// 证书绝对路径
        /// </summary>
        public string CerPath { get; set; }
        /// <summary>
        /// 设置代理对象，不想使用IE默认配置就设置为Null，而且不要设置ProxyIp
        /// </summary>
        public WebProxy WebProxy { get; set; }
        private Boolean isToLower = false;
        /// <summary>
        /// 是否设置为全文小写，默认为不转化
        /// </summary>
        public Boolean IsToLower
        {
            get { return isToLower; }
            set { isToLower = value; }
        }
        public string Cache_Control { get; set; }
        Boolean allowautoredirect = false;
        /// <summary>
        /// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转
        /// </summary>
        public Boolean Allowautoredirect
        {
            get { return allowautoredirect; }
            set { allowautoredirect = value; }
        }
        private int connectionlimit = 1024;
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int Connectionlimit
        {
            get { return connectionlimit; }
            set { connectionlimit = value; }
        }
        /// <summary>
        /// 代理Proxy 服务器用户名
        /// </summary>
        public string ProxyUserName { get; set; }
        /// <summary>
        /// 代理 服务器密码
        /// </summary>
        public string ProxyPwd { get; set; }
        /// <summary>
        /// 代理 服务IP,如果要使用IE代理就设置为ieproxy
        /// </summary>
        public string ProxyIp { get; set; }
        private ResultType resulttype = ResultType.String;
        /// <summary>
        /// 设置返回类型String和Byte
        /// </summary>
        public ResultType ResultType
        {
            get { return resulttype; }
            set { resulttype = value; }
        }
        private WebHeaderCollection header = new WebHeaderCollection();
        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection HeaderCollection
        {
            get { return header; }
            set { header = value; }
        }
        /// <summary>
        //     获取或设置用于请求的 HTTP 版本。返回结果:用于请求的 HTTP 版本。默认为 System.Net.HttpVersion.Version11。
        /// </summary>
        public Version ProtocolVersion { get; set; }
        private Boolean _expect100continue = true;
        /// <summary>
        ///  获取或设置一个 System.Boolean 值，该值确定是否使用 100-Continue 行为。如果 POST 请求需要 100-Continue 响应，则为 true；否则为 false。默认值为 true。
        /// </summary>
        public Boolean Expect100Continue
        {
            get { return _expect100continue; }
            set { _expect100continue = value; }
        }
        /// <summary>
        /// 设置509证书集合
        /// </summary>
        public X509CertificateCollection ClentCertificates { get; set; }
        /// <summary>
        /// 设置或获取Post参数编码,默认的为Default编码
        /// </summary>
        public Encoding PostEncoding { get; set; }
        private ResultCookieType _ResultCookieType = ResultCookieType.CookieCollection;
        /// <summary>
        /// Cookie返回类型,默认的是只返回字符串类型
        /// </summary>
        public ResultCookieType ResultCookieType
        {
            get { return _ResultCookieType; }
            set { _ResultCookieType = value; }
        }
        private ICredentials _ICredentials = CredentialCache.DefaultCredentials;
        /// <summary>
        /// 获取或设置请求的身份验证信息。
        /// </summary>
        public ICredentials ICredentials
        {
            get { return _ICredentials; }
            set { _ICredentials = value; }
        }
        /// <summary>
        /// 设置请求将跟随的重定向的最大数目
        /// </summary>
        public int MaximumAutomaticRedirections { get; set; }
        private DateTime? _IfModifiedSince = null;
        /// <summary>
        /// 获取和设置IfModifiedSince，默认为当前日期和时间
        /// </summary>
        public DateTime? IfModifiedSince
        {
            get { return _IfModifiedSince; }
            set { _IfModifiedSince = value; }
        }

        string _AcceptEncoding = "gzip, deflate";
        /// <summary>
        /// 默认值 gzip, deflate
        /// </summary>
        public string AcceptEncoding
        {
            get { return _AcceptEncoding; }
            set { _AcceptEncoding = value; }
        }

        string _AcceptLanguage = "zh-cn";

        /// <summary>
        /// 默认值 zh-cn
        /// </summary>
        public string AcceptLanguage
        {
            get { return _AcceptLanguage; }
            set { _AcceptLanguage = value; }
        }
        /// <summary>
        /// 重定向的URL
        /// </summary>
        public Uri ResponseUri { get; set; }
        long content_Length = -1;
        public long Content_Length
        { get { return content_Length; } set { content_Length = value; } }
        public string Origin { get; set; }
        public string UACPU { get; set; }
        public string x_requested_with { get; set; }

        public void DownloadDataCompleted(HttpResult result)
        {
            if (OnDownloadDataCompleted != null)
                OnDownloadDataCompleted(this, result.ResultStream);
        }

        public virtual HttpResult GetHttpResult()
        {
            string err;
            if (!string.IsNullOrEmpty(this.AcceptEncoding) && this.HeaderCollection.Get("Accept-Encoding") == null)
                this.HeaderCollection.Add("Accept-Encoding", this.AcceptEncoding);
            if (!string.IsNullOrEmpty(this.AcceptLanguage) && this.HeaderCollection.Get("Accept-Language") == null)
                this.HeaderCollection.Add("Accept-Language", this.AcceptLanguage);
            else this.HeaderCollection.Set("Accept-Language", this.AcceptLanguage);
            if (!string.IsNullOrEmpty(this.Origin) && this.HeaderCollection.Get("Origin") == null)
                this.HeaderCollection.Add("Origin", this.Origin);
            if (!string.IsNullOrEmpty(this.UACPU) && this.HeaderCollection.Get("UA-CPU") == null)
                this.HeaderCollection.Add("UA-CPU", this.UACPU);
            if (!string.IsNullOrEmpty(this.x_requested_with) && this.HeaderCollection.Get("x-requested-with") == null)
                this.HeaderCollection.Add("x-requested-with", this.x_requested_with);
            if (!string.IsNullOrEmpty(this.Cache_Control) && this.HeaderCollection.Get("Cache-Control") == null)
                this.HeaderCollection.Add("Cache-Control", this.Cache_Control);

            HttpResult result = HttpSpider.GetHtmlResult(this, out err);
            return result;
        }
        Func<HarRequest, CookieContainer, HttpResult> RequestCallback;
        public virtual HttpResult GetHttpResult(HarRequest request, CookieContainer cookie = null)
        {

            string err;
            var r = HarToHttpServer(request);
            HttpResult result = HttpSpider.GetHtmlResult(r, out err);

            return result;
        }

        public static HttpServer HarToHttpServer(HarRequest request)
        {
            var r = new HttpServer();
            //r.Host = request.header["Host"];
            r.UserAgent = request.headers.FirstOrDefault(x => x.name.ToLower() == "User-Agent".ToLower())?.value;
             
            r.Accept = request.headers.FirstOrDefault(x => x.name.ToLower() == "Accept".ToLower())?.value; 
            r.ContentType = request.headers.FirstOrDefault(x => x.name.ToLower() == "Content-Type".ToLower())?.value;
            r.Allowautoredirect = request.Allowautoredirect;
            r.Referer = request.headers.FirstOrDefault(x => x.name.ToLower() == "Referer".ToLower())?.value;

            r.PostEncoding = request.PostEncoding;
            r.Method = request.method;
            r.Url = request.url;
            r.PostData = request.postData?.text;
            foreach (var item in request.headers)
            {
                if (string.IsNullOrEmpty(r.HeaderCollection.Get(item.name)))
                {
                    try
                    {
                        if ("User-Agent,Host,Accept,Connection,Content-Type,Referer,Content-Length".ToLower().Contains(item.name.ToLower()))
                        {
                            continue;
                        }
                        r.HeaderCollection.Add(item.name, item.value);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return r;
        }

        /// <summary>
        /// 初始化所有属性
        /// </summary>
        public void Init()
        {
            this._expect100continue = false;
            this.Accept = "*/*";
            this.AcceptEncoding = "gzip, deflate";
            this.AcceptLanguage = "zh-cn";
            this.Allowautoredirect = false;
            this.ContentType = "";
            this.Expect100Continue = true;
            this.HeaderCollection.Clear();
            this.content_Length = -1;
            this.Cache_Control = null;
            this.Host = "";
            this.KeepAlive = true;
            this.Method = "GET";
            this.Origin = "";
            this.PostData = "";
            this.Referer = "";
            this.UACPU = "";
            this.Url = "";
            this.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; Trident/4.0; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";
            this.x_requested_with = "";
        }
    }

}
