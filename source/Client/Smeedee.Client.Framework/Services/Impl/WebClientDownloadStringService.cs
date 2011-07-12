using System;
using System.Net;
using Smeedee.Client.Framework.Services;

namespace Smeedee.Client.Framework.SL.Services.Impl
{
    public class WebClientDownloadStringService : IDownloadStringService
    {
        public WebClientDownloadStringService()
        {
        }

        public void DownloadAsync(Uri url, Action<string> callback)
        {
            DownloadAsync(url, callback, null);
        }

        public void DownloadAsync(Uri url, Action<string> callback, Action<Exception> onErrorCallback)
        {
            var webClient = new WebClient();
            if (callback != null)
            {
                webClient.DownloadStringCompleted += (o, e) =>
                {
                    if (e.Error != null && onErrorCallback != null)
                        onErrorCallback(e.Error);

                    callback(e.Result);
                    ((WebClient)o).DownloadStringCompleted -= null;
                };
            }
            webClient.DownloadStringAsync(url);
        }
    }
}
