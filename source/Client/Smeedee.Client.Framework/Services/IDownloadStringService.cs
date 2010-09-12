using System;

namespace Smeedee.Client.Framework.Services
{
    /// <summary>
    /// Downloads a string from a given url. Can be used to fetch xml/JSON kind of data.
    /// </summary>
    public interface IDownloadStringService
    {
        /// <summary>
        /// Download string async.
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="callback">
        /// Callback method will be called when download completed
        /// </param>
        void DownloadAsync(Uri url, Action<string> callback);

        void DownloadAsync(Uri url, Action<string> callback, Action<Exception> onErrorCallback);
    }
}
