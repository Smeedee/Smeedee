using System;
using System.Net;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Framework.Logging;

namespace Smeedee.Client.Framework.SL.Services.Impl
{
    public class WebClientDownloadStringService : IDownloadStringService
    {
        private readonly ILog logger;

        public WebClientDownloadStringService(ILog logger)
        {
            this.logger = logger;
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

                    try
                    {
                        callback(e.Result);
                    }
                    catch (Exception)
                    {
                    }
                    ((WebClient)o).DownloadStringCompleted -= null;
                };
            }
            webClient.DownloadStringAsync(url);
        }
    }
}
