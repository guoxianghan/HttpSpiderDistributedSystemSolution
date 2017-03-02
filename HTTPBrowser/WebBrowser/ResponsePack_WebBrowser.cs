using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HTTPBrowser
{
    /// <summary>
    /// 发起一个Http请求后得到的响应
    /// </summary>
    public class ResponsePack_WebBrowser : IResponsePack
    {
        public ResponsePack_WebBrowser(string downloadMethod):base(downloadMethod){}
        private string _PageSource;

        public override string PageSource
        {
            get
            {
                if (string.IsNullOrEmpty(_PageSource) && Data != null)
                {
                    _PageSource = this.Encoding.GetString(Data);
                }
                return _PageSource;
            }
            set { _PageSource = value; }
        }
        private Encoding _Encoding = null;

        private static Encoding LastEncoding = null;
        public byte[] Data { get; set; }
        /// <summary>
        /// 页面编码
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                if (_Encoding == null)
                {
                    if (this.Data == null)
                    {
                        return null;
                    }
                    if (ContentType != null && ContentType.ToLower().Contains("html"))
                    {
                        var str = Encoding.UTF8.GetString(Data);
                        string charsetReg = @"(meta.*?charset=""?(?<Charset>[^\s""'>;]+)""?)|(xml.*?encoding=""?(?<Charset>[^\s"">;]+)""?)";
                        Match match = Regex.Match(str, charsetReg, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        if (match.Success)
                        {
                            try
                            {
                                string encodeing = match.Groups["Charset"].Value;
                                _Encoding = Encoding.GetEncoding(encodeing);
                            }
                            catch { }
                        }
                    }

                    if (_Encoding == null)
                    {
                        string encodeing = NChardetHelper.RecogCharset(this.Data, NChardetLanguage.CHINESE, this.Data.Length);
                        if (!string.IsNullOrEmpty(encodeing))
                        {
                            _Encoding = Encoding.GetEncoding(encodeing);
                        }
                    }

                }

                if (_Encoding!=null)
                {
                    LastEncoding = _Encoding;
                    return _Encoding;
                }
                else if (LastEncoding != null)
                {
                    return LastEncoding;
                }
                else
                {
                    return Encoding.Default;
                }
            }
            internal set { _Encoding = value; }
        }
        public HttpStatusCode StatusCode { get; internal set; }
        public WebHeaderCollection ResponseHeaders { get; internal set; }
        public WebHeaderCollection RequestHeaders { get; internal set; }
        public Uri ReferrerUri { get; internal set; }

        public long ContentLength { get;internal set; }
        public string ContentType
        {
            get
            {
                if (ResponseHeaders != null)
                {
                    return ResponseHeaders["Content-Type"];
                }
                return null;
            }
        }
    }
}
