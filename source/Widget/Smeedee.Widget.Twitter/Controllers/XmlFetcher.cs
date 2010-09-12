using System;
using System.Net;
using System.Xml.Linq;
using Smeedee.Client.Framework.Services;

namespace Smeedee.Widget.Twitter.Controllers
{
    public class XmlFetcher : IFetchXml
    {
        private readonly WebClient webClient = new WebClient();
        public event EventHandler<AsyncResponseEventArgs<XDocument>> GetXmlCompleted;

        public XmlFetcher()
        {
            webClient.DownloadStringCompleted += DownloadStringCompleted;
        }

        void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var result = e.Error != null ? null : XDocument.Parse(e.Result);

            if (GetXmlCompleted != null)
                    GetXmlCompleted(this, new AsyncResponseEventArgs<XDocument>(result));
        }

        public void BeginGetXml(string url)
        {
           webClient.DownloadStringAsync(new Uri(url));
        }
    }
}
