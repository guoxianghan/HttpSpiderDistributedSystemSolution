using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace HTTPBrowser
{
    internal partial class BybClient : WebClient
    {
        /// <summary>
        /// 异步下载文本，目前仅支持GET。
        /// </summary>
        /// <param name="requestState"></param>
        /// <param name="exception"></param>
        public void DownloadStringAsync(RequestState requestState, Exception exception = null)
        {
            if (exception != null)
            {
                Config.RetryCount--;
                if (Config.RetryCount-- < 1)
                {
                    requestState.Completed.Invoke(this.LastResponsePack, requestState.Properties);
                    return;
                }
                if (Config.DownloadRetryWaitDuration.HasValue)
                {
                    Thread.Sleep(Config.DownloadRetryWaitDuration.Value * 1000);
                }

                this.DownloadDataAsync(requestState.Uri, requestState);
                return;
            }

            this.DownloadDataCompleted += (sender, e) =>
            {
                RequestState state = e.UserState as RequestState;
                if (e.Error != null)
                {
                    DownloadStringAsync(state, e.Error);
                }
                else if (e.Cancelled)
                {
                    DownloadStringAsync(state, new OperationCanceledException("操作被取消"));
                }
                else
                {
                    this.LastResponsePack.Data = e.Result;
                    state.Completed.Invoke(this.LastResponsePack, state.Properties);
                }
            };
            this.DownloadDataAsync(requestState.Uri, requestState);
        }

        /// <summary>
        /// 发起HTTP请求，得到一个文本响应，该方法会自动编解码相关文本。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResponsePack_WebBrowser Request(string url, Dictionary<string, string> data = null)
        {
            return Request(new Uri(url), data);
        }


        public ResponsePack_WebBrowser Request(Uri uri, string dataStr, Encoding requestEncoding = null)
        {
            try
            {
                var responseData = new byte[0];
                if (Config.Method == "POST")//POST
                {
                    byte[] bytedata = new byte[0];
                    if (dataStr != null)
                    {
                        bytedata = requestEncoding.GetBytes(dataStr);
                    }
                    responseData = this.UploadData(uri, "POST", bytedata);
                }
                else//GET
                {
                    string url = uri.OriginalString;
                    if (!string.IsNullOrEmpty(dataStr))
                    {
                        if (url.Contains('?'))
                        {
                            url += dataStr;
                        }
                        else
                        {
                            url += "?" + dataStr.TrimStart('&');
                        }
                    }
                    responseData = this.DownloadData(url);
                    
                }
                this.LastResponsePack.Data = responseData;
            }
            catch (Exception ex)
            {
                Config.RetryCount--;
                if (Config.RetryCount > 0)
                {
                    return Request(uri, dataStr, requestEncoding);
                }
            }
            return this.LastResponsePack;
        }

        /// <summary>
        /// 发起HTTP请求，得到一个文本响应，该方法会自动编解码相关文本。
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public ResponsePack_WebBrowser Request(Uri uri, Dictionary<string, string> data = null, Encoding requestEncoding = null)
        {
            if (requestEncoding == null)
            {
                if (this.LastResponsePack != null && this.LastResponsePack.Encoding != null)
                {
                    requestEncoding = this.LastResponsePack.Encoding;
                }
                else
                {
                    requestEncoding = Encoding.UTF8;
                }
            }
            string dataStr = null;
            if (data != null && data.Count > 0)
            {
                dataStr = data.Aggregate("", (total, next) => total + "&" + next.Key + "=" + HttpUtility.UrlEncode(next.Value, requestEncoding));
            }
            return Request(uri, dataStr, requestEncoding);
        }

        /// <summary>
        /// 发起请求，得到一张图片。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        public Bitmap DownLoadImage(string url, int retryCount = 3)
        {
            Bitmap map = null;
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    Stream s = new MemoryStream(this.DownloadData(url));
                    map = new Bitmap(s);
                    return map;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return map;
        }

        public new ResponsePack_WebBrowser UploadFile(string url, string filePath)
        {
            this.Config.Method = "POST";
            var data = base.UploadFile(url, filePath); ;
            this.LastResponsePack.Data = data;
            return this.LastResponsePack;
        }

        public new ResponsePack_WebBrowser UploadFile(Uri uri, string filePath)
        {
            return this.UploadFile(uri.OriginalString, filePath);
        }
    }
}