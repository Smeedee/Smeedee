using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Smeedee.Client.Framework.SL.MEF.Services
{
    public class HttpDownloadService : IDownloadService
    {
        private WebClient webClient = new WebClient();

        public event DownloadStringCompletedEventHandler DownloadStringCompleted;

        public HttpDownloadService()
        {
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
        }

        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (DownloadStringCompleted != null)
                DownloadStringCompleted(sender, e);
        }

        public void DownloadStringAsync(Uri uri)
        {
            webClient.DownloadStringAsync(uri);
        }
    }
}
