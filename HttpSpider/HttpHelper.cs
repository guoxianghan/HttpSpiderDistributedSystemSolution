/// <summary>
/// 类说明：HttpSpider类，用来实现Http访问，Post或者Get方式的，直接访问，带Cookie的，带证书的等方式，可以设置代理
/// 重要提示：请不要自行修改本类，如果因为你自己修改后将无法升级到新版本。如果确实有什么问题请到官方网站提建议，
/// 我们一定会及时修改
/// 编码日期：2011-09-20
/// 编 码 人：苏飞
/// 联系方式：361983679  
/// 官方网址：http://www.sufeinet.com/thread-3-1-1.html
/// 修改日期：2014-09-15
/// 版 本 号：1.4.6
/// </summary>
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.Cache;
using System.Collections.Specialized;

namespace HttpSpider
{
    /// <summary>
    /// Http连接操作帮助类
    /// </summary>
    public class HttpSpider
    {
        #region 预定义变量
        private Encoding encoding = Encoding.Default;
        private Encoding postencoding = Encoding.Default;
        private HttpWebRequest request = null;
        private HttpWebResponse response = null;
        #endregion

        #region Public

        /// <summary>
        /// 根据相传入的数据，得到相应页面数据
        /// </summary>
        /// <param name="item">参数类对象</param>
        /// <returns>返回HttpResult类型</returns>
        public HttpResult GetHtmlResult(HttpServer item, out string err)
        {
            err = "";
            //返回参数 
            HttpResult result = new HttpResult();
            try
            {
                //准备参数
                SetRequest(item);
            }
            catch (Exception ex)
            {
                //配置参数时出错
                err = ex.Message;
                if (ex.InnerException != null)
                    err = ex.InnerException.Message;
                return new HttpResult() { HeaderCollection = null, Html = ex.Message , StatusDescription = "配置参数时出错：" + ex.Message};
            }
            try
            {
                //请求数据
                //request = (HttpWebRequest)WebRequest.Create(item.Url);
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    GetData(item, result, out err);
                }
            }
            catch (WebException ex)
            {
                err = ex.Message;
                result.Html = ex.Message;
                //using (response = (HttpWebResponse)ex.Response)
                //{
                //    GetData(item, result, out err);
                //}
            }
            catch (Exception ex)
            {
                err = ex.Message;
                result.Html = ex.Message;
            }
            if (item.IsToLower)
                result.Html = result.Html.ToLower();
            return result;
        }
        #endregion

        #region GetData

        /// <summary>
        /// 获取数据的并解析的方法
        /// </summary>
        /// <param name="item"></param>
        /// <param name="result"></param>
        private void GetData(HttpServer item, HttpResult result, out string err)
        {
            err = "";
            try
            {
                #region base
                result.StatusCode = response.StatusCode;
                result.ResponseUri = response.ResponseUri;
                result.StatusDescription = response.StatusDescription;
                result.HeaderCollection = response.Headers;
                result.Cookies.Add(response.Cookies);
                if (response.Cookies != null)
                    result.CookieCollection.Add(response.Cookies);
                item.Cookies = request.CookieContainer;
                result.Cookies = request.CookieContainer;

                #endregion
                #region byte
                //处理网页Byte
                byte[] ResponseByte = GetByte();
                result.ResultStream = new MemoryStream(ResponseByte);
                #endregion

                #region Html
                if (ResponseByte != null & ResponseByte.Length > 0)
                {
                    //设置编码
                    SetEncoding(item, result, ResponseByte);
                    //得到返回的HTML
                    if (response.ResponseUri.ToString().Contains("www.airpp.net"))
                    {
                        encoding = System.Text.Encoding.GetEncoding("utf-8");
                        result.Html = encoding.GetString(ResponseByte);
                    }
                    else result.Html = encoding.GetString(ResponseByte);
                }
                else
                {
                    //没有返回任何Html代码
                    result.Html = string.Empty;
                }
                #endregion
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }
        }
        /// <summary>
        /// 设置编码
        /// </summary>
        /// <param name="item">HttpItem</param>
        /// <param name="result">HttpResult</param>
        /// <param name="ResponseByte">byte[]</param>
        private void SetEncoding(HttpServer item, HttpResult result, byte[] ResponseByte)
        {
            //是否返回Byte类型数据
            if (item.ResultType == ResultType.Byte)
                result.ResultByte = ResponseByte;
            try
            {
                encoding = Encoding.GetEncoding(response.CharacterSet);
            }
            catch (Exception)
            { }
            //从这里开始我们要无视编码了
            if (encoding == null)
            {
                Match meta = Regex.Match(Encoding.Default.GetString(ResponseByte), "<meta[^<]*charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                string c = string.Empty;
                if (meta != null && meta.Groups.Count > 0)
                {
                    c = meta.Groups[1].Value.ToLower().Trim();
                }
                if (c.Length > 2)
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(c.Replace("\"", string.Empty).Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk").Trim());
                    }
                    catch
                    {
                        if (string.IsNullOrEmpty(response.CharacterSet))
                        {
                            encoding = Encoding.UTF8;
                        }
                        else
                        {
                            encoding = Encoding.GetEncoding(response.CharacterSet);
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(response.CharacterSet))
                    {
                        encoding = Encoding.UTF8;
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding(response.CharacterSet);
                    }
                }
            }
        }
        /// <summary>
        /// 提取网页Byte
        /// </summary>
        /// <returns></returns>
        private byte[] GetByte()
        {
            byte[] ResponseByte = null;
            using (MemoryStream _stream = new MemoryStream())
            {
                //GZIIP处理
                if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {
                        //开始读取流并设置编码方式
                        new GZipStream(response.GetResponseStream(), CompressionMode.Decompress).CopyTo(_stream, 10240);
                    }
                    catch (Exception)
                    { }
                }
                else
                {
                    try
                    {
                        //开始读取流并设置编码方式
                        response.GetResponseStream().CopyTo(_stream, 10240);
                    }
                    catch (Exception)
                    { }
                }
                //获取Byte
                ResponseByte = _stream.ToArray();
            }
            return ResponseByte;
        }
        #endregion

        #region SetRequest
        public void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
        /// <summary>
        /// 为请求准备参数
        /// </summary>
        ///<param name="item">参数列表</param>
        private void SetRequest(HttpServer item)
        {
            // 验证证书,初始化request
            SetCer(item);
            //设置Header参数

            request.CookieContainer = item.Cookies;
            if (item.KeepAlive)
            {
                SetHeaderValue(request.Headers, "Connection", "Keep-Alive");
                request.KeepAlive = item.KeepAlive;
            }
            if (item.HeaderCollection != null && item.HeaderCollection.Count > 0)
                foreach (string key in item.HeaderCollection.AllKeys)
                {
                    request.Headers.Add(key, item.HeaderCollection[key]);
                }

            // 设置代理
            SetProxy(item);
            if (item.ProtocolVersion != null)
                request.ProtocolVersion = item.ProtocolVersion;
            request.ServicePoint.Expect100Continue = item.Expect100Continue;
            //请求方式Get或者Post
            try
            {
                request.Method = item.Method;
            }
            catch (Exception)
            {
                throw;
            }
            request.Timeout = item.Timeout;
            request.ReadWriteTimeout = item.ReadWriteTimeout;

            if (item.Content_Length != -1)
                request.ContentLength = item.Content_Length;

            if (!string.IsNullOrWhiteSpace(item.Host))
            {
                request.Host = item.Host;
            }
            if (item.IfModifiedSince != null)
                request.IfModifiedSince = Convert.ToDateTime(item.IfModifiedSince);
            //Accept
            request.Accept = item.Accept;
            //ContentType返回类型
            request.ContentType = item.ContentType;
            //UserAgent客户端的访问类型，包括浏览器版本和操作系统信息
            request.UserAgent = item.UserAgent;
            // 编码
            encoding = item.Encoding;
            //设置安全凭证
            request.Credentials = item.ICredentials;
            //设置Cookie
            SetCookie(item);
            //来源地址
            request.Referer = item.Referer;
            //是否执行跳转功能
            request.AllowAutoRedirect = item.Allowautoredirect;
            if (item.MaximumAutomaticRedirections > 0)
            {
                request.MaximumAutomaticRedirections = item.MaximumAutomaticRedirections;
            }
            //设置Post数据
            SetPostData(item);
            //设置最大连接
            if (item.Connectionlimit > 0)
                request.ServicePoint.ConnectionLimit = item.Connectionlimit;
        }
        /// <summary>
        /// 设置证书
        /// </summary>
        /// <param name="item"></param>
        private void SetCer(HttpServer item)
        {
            if (!string.IsNullOrWhiteSpace(item.CerPath))
            {
                //这一句一定要写在创建连接的前面。使用回调的方法进行证书验证。
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                //初始化对像，并设置请求的URL地址
                request = (HttpWebRequest)WebRequest.Create(item.Url);
                SetCerList(item);
                //将证书添加到请求里
                request.ClientCertificates.Add(new X509Certificate(item.CerPath));
            }
            else
            {
                //初始化对像，并设置请求的URL地址
                request = (HttpWebRequest)WebRequest.Create(item.Url);
                SetCerList(item);
            }
        }
        /// <summary>
        /// 设置多个证书
        /// </summary>
        /// <param name="item"></param>
        private void SetCerList(HttpServer item)
        {
            if (item.ClentCertificates != null && item.ClentCertificates.Count > 0)
            {
                foreach (X509Certificate c in item.ClentCertificates)
                {
                    request.ClientCertificates.Add(c);
                }
            }
        }
        /// <summary>
        /// 设置CookieCollection
        /// </summary>
        /// <param name="item">Http参数</param>
        private void SetCookie(HttpServer item)
        {
            //设置CookieCollection
            if (item.ResultCookieType == ResultCookieType.CookieCollection)
            {
                if (item.CookieCollection != null && item.CookieCollection.Count > 0)
                    request.CookieContainer.Add(item.CookieCollection);
            }
        }
        /// <summary>
        /// 设置Post数据
        /// </summary>
        /// <param name="item">Http参数</param>
        private void SetPostData(HttpServer item)
        {
            //验证在得到结果时是否有传入数据
            if (request.Method.Trim().ToLower().Contains("post"))
            {
                if (item.PostEncoding != null)
                {
                    postencoding = item.PostEncoding;
                }
                byte[] buffer = null;
                //写入Byte类型
                if (item.PostDataType == PostDataType.Byte && item.PostdataByte != null && item.PostdataByte.Length > 0)
                {
                    //验证在得到结果时是否有传入数据
                    buffer = item.PostdataByte;
                }//写入文件
                else if (item.PostDataType == PostDataType.FilePath && !string.IsNullOrWhiteSpace(item.PostData))
                {
                    StreamReader r = new StreamReader(item.PostData, postencoding);
                    buffer = postencoding.GetBytes(r.ReadToEnd());
                    r.Close();
                } //写入字符串
                else if (!string.IsNullOrWhiteSpace(item.PostData))
                {
                    buffer = postencoding.GetBytes(item.PostData);
                }
                if (buffer != null)
                {
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }
            }
        }
        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="item">参数对象</param>
        private void SetProxy(HttpServer item)
        {
            bool isIeProxy = false;
            if (!string.IsNullOrWhiteSpace(item.ProxyIp))
            {
                isIeProxy = item.ProxyIp.ToLower().Contains("ieproxy");
            }
            if (!string.IsNullOrWhiteSpace(item.ProxyIp) && !isIeProxy)
            {
                //设置代理服务器
                if (item.ProxyIp.Contains(":"))
                {
                    string[] plist = item.ProxyIp.Split(':');
                    WebProxy myProxy = new WebProxy(plist[0].Trim(), Convert.ToInt32(plist[1].Trim()));
                    //建议连接
                    myProxy.Credentials = new NetworkCredential(item.ProxyUserName, item.ProxyPwd);
                    //给当前请求对象
                    request.Proxy = myProxy;
                }
                else
                {
                    WebProxy myProxy = new WebProxy(item.ProxyIp, false);
                    //建议连接
                    myProxy.Credentials = new NetworkCredential(item.ProxyUserName, item.ProxyPwd);
                    //给当前请求对象
                    request.Proxy = myProxy;
                }
            }
            else if (isIeProxy)
            {
                //设置为IE代理
            }
            else
            {
                request.Proxy = item.WebProxy;
            }
        }
        #endregion

        #region private main
        /// <summary>
        /// 回调验证证书问题
        /// </summary>
        /// <param name="sender">流对象</param>
        /// <param name="certificate">证书</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="errors">SslPolicyErrors</param>
        /// <returns>bool</returns>
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }
        #endregion
    }

}