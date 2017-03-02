using Beyebe.HTTP.Browser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Beyebe.BCrawler.Common;
using Newtonsoft.Json;

namespace Beyebe.HTTP.Browser
{
    public class BProxy
    {
        public BProxy(string uri, string username, string password)
        {
            if (!Regex.IsMatch(uri, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d+"))
            {
                throw new Exception("错误的代理地址格式：" + uri);
            }
            this.Address = uri.Substring(0, uri.IndexOf(':'));
            this.Port = Convert.ToInt32(uri.Substring(uri.IndexOf(':') + 1));
            SetCredential(username, password);
        }
        public BProxy(string uri):this(uri, null, null){}
        public static WebProxy Get()
        {
            var response = WebBrowser.Get("http://proxy.bdatahub.com/API/Get.ashx");
            if (response.Error != null ||  string.IsNullOrWhiteSpace(response.PageSource))
            {
                return null;
            }
            var bProxy = response.PageSource.Convert2Model<BProxy>();
            return bProxy.Convert2WebProxy();
        }
        public WebProxy Convert2WebProxy()
        {
            WebProxy result = new WebProxy(this.Address,this.Port);
            result.Credentials = new NetworkCredential(this.Username, this.Password);
            return result;
        }

        public void SetCredential(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            if (!string.IsNullOrWhiteSpace(this.Username) && !string.IsNullOrWhiteSpace(this.Password))
            {
                this.Credential = new NetworkCredential(this.Username, this.Password);
            }
        }
        public bool IsAvailable()
        {
            WebBrowser browser = new WebBrowser(new RequestConfig() { PageLoadTimeout = 5, Method = "POST" });
            var proxy = new WebProxy(this.Address,this.Port);
            if (this.Credential != null)
            {
                proxy.Credentials = this.Credential;
            }
            browser.WebProxy = proxy;
            var key = Guid.NewGuid().ToString();
            var response = browser.DownloadString("http://bdatahub.com/api/ProxyTester.ashx", new Dictionary<string, string>() { { "key", key } });
            return response.PageSource == key;
        }

        public string Address { get;set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public NetworkCredential Credential { get; private set; }
        public override int GetHashCode()
        {
            return (this.Address + this.Port + this.Username + this.Password).GetHashCode();
        }
    }
}
