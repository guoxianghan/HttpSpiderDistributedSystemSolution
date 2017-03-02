using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace HttpSpider
{
    /// <summary>
    /// Http返回参数类
    /// </summary>
    public class HttpResult
    {
        /// <summary>
        /// Http请求返回的Cookie
        /// </summary>
        //public string Cookie { get; set; } 
        CookieCollection _CookieCollection = new CookieCollection();
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection { get { return _CookieCollection; } set { _CookieCollection = value; } }
        /// <summary>
        /// 返回的String类型数据 只有ResultType.String时才返回数据，其它情况为空
        /// </summary>
        public string Html { get; set; }
        /// <summary>
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空
        /// </summary>
        public byte[] ResultByte { get; set; }
        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection HeaderCollection { get; set; }
        /// <summary>
        /// 返回状态说明
        /// </summary>
        public string StatusDescription { get; set; }
        /// <summary>
        /// 返回状态码,默认为OK
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// 重定向的URL
        /// </summary>
        public Uri ResponseUri { get; set; }

        public MemoryStream ResultStream { get; set; }

        CookieContainer _CookieContainer = new CookieContainer();
        public CookieContainer Cookies { get { return _CookieContainer; } set { _CookieContainer = value; } }
    }
}
