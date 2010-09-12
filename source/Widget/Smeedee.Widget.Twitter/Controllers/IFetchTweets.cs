using System;
using Smeedee.Client.Framework.Services;

namespace Smeedee.Widget.Twitter.Controllers
{
    public interface IFetchTweets
    {
        void BeginGetTweets(string search, int maxNumberOfElements);
        event EventHandler<AsyncResponseEventArgs<TwitterSearchResult>>  GetTweetsCompleted;
    }
}