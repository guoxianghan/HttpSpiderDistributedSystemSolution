using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace HTTPBrowser
{
    [Serializable]
    public abstract class IResponsePack
    {
        public IResponsePack(string downloadMethod)
        {
            this.Timer = Stopwatch.StartNew();
            this.DownloadMethod = downloadMethod;
        }

        public void End()
        {
            Timer.Stop();
        }

        private Stopwatch Timer;
        public TimeSpan DownloadTime { get { return this.Timer.Elapsed; } }
        public abstract string PageSource { get; set; }
        public Stream ResultStream { get; set; }
        public CookieCollection Cookie { get; set; }
        public Uri RequestUri { get; set; }
        public Uri ResponseUri { get; set; }
        public Exception Error { get; set; }

        public string DownloadMethod { get; set; }
    }
}