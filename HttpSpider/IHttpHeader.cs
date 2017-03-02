using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace HttpHelper
{
    interface IHttpHeader
    {
        //string Username { get; set; }
        //string PassWord { get; set; }
        string VerifyCode { get; set; }

        string Url { get; set; }
        string Referer { get; set; }
        string Host { get; set; }
        string Cookie { get; set; }
        string PostData { get; set; }
        string Method { get; set; }
        string UseAgent { get; set; }
        string Accept { get; set; }
        string AcceptEncoding { get; set; }
        string AcceptLanguage { get; set; }
        string ContentType { get; set; }
        Boolean Expect100Continue { get; set; }
        Boolean Allowautoredirect { get; set; }
        /// <summary>
        /// 获取返回结果
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        HttpResult GetHttpResult();
    }
}
