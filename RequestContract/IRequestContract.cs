using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;

namespace RequestContract
{
    [ServiceContract]
    public interface IRequestContract
    {
        [OperationContract]
        string Get(string url, List<Cookie> cookies);

        [OperationContract(Name = "POSTSTRING")]
        string Post(string url, List<Cookie> cookies,Dictionary<string, string> data = null, Encoding requestEncoding = null);

        

        [OperationContract(Name = "GetStream")]
        Stream GetStream(string url , string method, List<Cookie> cookies,Dictionary<string, string> data = null, Encoding requestEncoding = null);

        [OperationContract(Name = "HttpServerRequestGet")]
        string HttpServerRequestGet(string url, List<Cookie> cookies, Dictionary<string, string> header = null);
        [OperationContract(Name = "HttpServerRequestPost")]
        string HttpServerRequestPost(string url, List<Cookie> cookies, string post, Dictionary<string, string> header = null, Encoding requestEncoding = null);
        [OperationContract(Name = "HttpServerStream")]
        Stream HttpServerStream(string url, string method, string param, List<Cookie> cookies, Dictionary<string, string> headers = null, Encoding requestEncoding = null);

    }
}
