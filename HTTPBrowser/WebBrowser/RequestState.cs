using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTTPBrowser
{
    public class RequestState
    {
        public RequestState(Uri uri)
        {
            this.Uri = uri;
        }
        public Uri Uri { get; set; }
        public WebBrowser WebBrowser { get; set; }
        public Action<ResponsePack_WebBrowser, Dictionary<string, object>> Completed { get; set; }
        public Dictionary<string, object> Properties = new Dictionary<string, object>();
    }
}
