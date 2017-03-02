using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HTTPBrowser
{
    internal partial class BybClient : WebClient
    {
        private static readonly CookieContainer _PublicCookies = new CookieContainer();

        private readonly CookieContainer _Cookies = new CookieContainer();

        public CookieContainer Cookies
        {
            get { return Config.UseCookies ? _PublicCookies : _Cookies; }
        }

        public ResponsePack_WebBrowser LastResponsePack;
        public RequestConfig Config = new RequestConfig();

        public BybClient()
        {
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
        }

        public BybClient(RequestConfig config)
        {
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
            Config = config;
            if (Config.UseCookies && Config.Cookies != null)
            {
                foreach (var item in Config.Cookies)
                {
                    _PublicCookies.Add(item);
                }
            }
            if (Config.Proxy != null)
            {
                var p = new WebProxy(Config.Proxy.Address, Config.Proxy.Port);
                if (Config.Proxy.HasCredential)
                {
                    p.Credentials = new NetworkCredential(Config.Proxy.Username, Config.Proxy.Password);
                }
                this.Proxy = p;
            }
        }

        /// <summary>
        /// 对https请求不验证证书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static bool RemoteCertificateValidate(
           object sender, X509Certificate cert,
            X509Chain chain, SslPolicyErrors error)
        {
            // trust any certificate!!!
            return true;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address) as HttpWebRequest;
            req.CookieContainer = Cookies;

            if (Config.PageLoadTimeout.HasValue)
            {
                req.Timeout = (int)Config.PageLoadTimeout * 1000;
            }

            req.Method = Config.Method;
            req.UserAgent = Config.UserAgent;
            if (!string.IsNullOrEmpty(Config.ContentType))
            {
                req.ContentType = Config.ContentType;
            }
            if (Config.Referer != null)
            {
                req.Referer = Config.Referer.OriginalString;
            }
            else if (LastResponsePack != null && LastResponsePack.ResponseUri != null)
            {
                req.Referer = LastResponsePack.ResponseUri.OriginalString;
            }
            if (string.IsNullOrWhiteSpace(req.Referer))
            {
                req.Referer = address.AbsoluteUri;
            }
            if (Config.Headers.ContainsKey("Accept"))
            {
                req.Accept = Config.Headers["Accept"];
            }


            req.Headers["Accept-Encoding"] = "identity;q=1,chunked;q=0";
            req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            req.ProtocolVersion = Version.Parse("1.0");
            req.MaximumAutomaticRedirections = 50;

            foreach (var item in Config.Headers)
            {
                string[] filter = { "Referer", "Accept" };
                if (filter.Contains(item.Key))
                {
                    continue;
                }
                req.Headers[item.Key] = item.Value;
            }

            if (req.Method == "POST" && string.IsNullOrEmpty(req.ContentType))
            {
                req.ContentType = "application/x-www-form-urlencoded";
            }

            var lastEncoding = this.LastResponsePack == null ? null : LastResponsePack.Encoding;
            this.LastResponsePack = new ResponsePack_WebBrowser(req.Method)
            {
                DownloadMethod = req.Method,
                RequestUri = req.RequestUri,
                ReferrerUri = req.Referer == null ? null : new Uri(req.Referer),
                RequestHeaders = req.Headers,
                Encoding = lastEncoding
            };
            return req;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            try
            {
                var response = base.GetWebResponse(request) as HttpWebResponse;
                this.SetAttrByResponse(response);
                return response;
            }
            catch (WebException ex)
            {
                //只要拿到了响应，即便响应为404等异常，皆忽视异常。
                if (ex.Response != null)
                {
                    this.SetAttrByResponse(ex.Response as HttpWebResponse);
                    return ex.Response;
                }

                this.LastResponsePack.Error = ex;
                throw ex;
            }
            finally
            {
                this.LastResponsePack.End();
            }
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            try
            {
                var response = base.GetWebResponse(request, result) as HttpWebResponse;
                this.SetAttrByResponse(response);
                return response;
            }
            catch (WebException ex)
            {
                this.LastResponsePack.Error = ex;
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    this.SetAttrByResponse(ex.Response as HttpWebResponse);
                    return ex.Response;
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                this.LastResponsePack.End();
            }
        }

        private void SetAttrByResponse(HttpWebResponse response)
        {
            if (string.IsNullOrEmpty(response.CharacterSet) || response.CharacterSet == "ISO-8859-1")
            {
                if (!string.IsNullOrEmpty(response.ContentEncoding))
                {
                    this.Encoding = Encoding.GetEncoding(response.ContentEncoding);
                    this.LastResponsePack.Encoding = this.Encoding;
                }
            }
            else
            {
                this.Encoding = Encoding.GetEncoding(response.CharacterSet);
                this.LastResponsePack.Encoding = this.Encoding;
            }
            this.LastResponsePack.ResultStream = response.GetResponseStream();
            this.LastResponsePack.Cookie = response.Cookies;
            this.LastResponsePack.StatusCode = response.StatusCode;
            this.LastResponsePack.ResponseHeaders = response.Headers;
            this.LastResponsePack.ResponseUri = response.ResponseUri;
            this.LastResponsePack.ContentLength = response.ContentLength;
        }

        /// <summary>
        /// 获取某网站的Cookie
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetCookie(Uri uri, string name)
        {
            if (Cookies.GetCookies(uri)[name] == null)
            {
                return null;
            }
            return Cookies.GetCookies(uri)[name].Value;
        }

        public Dictionary<string, string> GetCookies(Uri uri)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            for (int i = 0; i < Cookies.GetCookies(uri).Count; i++)
            {
                var c = Cookies.GetCookies(uri)[i];
                result[c.Name] = c.Value;
            }
            return result;
        }

        public void SetCookie(System.Net.Cookie cookie)
        {
            Cookies.Add(cookie);
        }
    }
}