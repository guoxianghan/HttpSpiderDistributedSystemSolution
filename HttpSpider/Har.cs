using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpSpider
{
    /// <summary>
    /// 由搜狗浏览器或谷歌浏览器生成的Http请求响应实体
    /// </summary>
    public class HttpHar
    {
        public Log log { get; set; }
    }

    public class Log
    {
        public string version { get; set; }
        public Creator creator { get; set; }
        public Page[] pages { get; set; }
        public HarEntry[] entries { get; set; }
    }

    public class Creator
    {
        public string name { get; set; }
        public string version { get; set; }
    }

    public class Page
    {
        public DateTime startedDateTime { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public Pagetimings pageTimings { get; set; }
    }

    public class Pagetimings
    {
        public float onContentLoad { get; set; }
        public float onLoad { get; set; }
    }

    public class HarEntry
    {
        public DateTime startedDateTime { get; set; }
        public float time { get; set; }
        public HarRequest request { get; set; }
        public Response response { get; set; }
        public Cache cache { get; set; }
        public Timings timings { get; set; }
        public string connection { get; set; }
        public string pageref { get; set; }
    }

    public class HarRequest
    {
        public string method { get; set; }
        public string url { get; set; }
        public string httpVersion { get; set; }
        public List<Header> headers { get; set; }

        public Dictionary<string, string> header
        {
            get
            {
                var d = new Dictionary<string, string>();
                foreach (var item in headers)
                {
                    d.Add(item.name, item.value);
                }
                return d;
            }
        }
        public object[] queryString { get; set; }
        public Cooky[] cookies { get; set; }
        public int headersSize { get; set; }
        public int bodySize { get; set; }
        public PostData postData { get; set; }
        /// <summary>
        /// 设置或获取Post参数编码,默认的为Default编码
        /// </summary>
        public Encoding PostEncoding { get; internal set; } = Encoding.Default;
        /// <summary>
        /// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转
        /// </summary>
        public bool Allowautoredirect { get; internal set; } = true;
    }
    public class PostData
    {
        public string mimeType { get; set; }
        public string text { get; set; }
        [Newtonsoft.Json.JsonProperty("params")]
        public Header[] postData { get; set; }
    }

    public class Header
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Cooky
    {
        public string name { get; set; }
        public string value { get; set; }
        public object expires { get; set; }
        public bool httpOnly { get; set; }
        public bool secure { get; set; }
    }

    public class Response
    {
        public int status { get; set; }
        public string statusText { get; set; }
        public string httpVersion { get; set; }
        public Header1[] headers { get; set; }
        public object[] cookies { get; set; }
        public Content content { get; set; }
        public string redirectURL { get; set; }
        public int headersSize { get; set; }
        public int bodySize { get; set; }
        public int _transferSize { get; set; }
    }

    public class Content
    {
        public int size { get; set; }
        public string mimeType { get; set; }
        public float compression { get; set; }
        public string text { get; set; }
        public string encoding { get; set; }
    }

    public class Header1
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Cache
    {
    }

    public class Timings
    {
        public float blocked { get; set; }
        public float dns { get; set; }
        public float connect { get; set; }
        public float send { get; set; }
        public float wait { get; set; }
        public float receive { get; set; }
        public int ssl { get; set; }
    }

}
