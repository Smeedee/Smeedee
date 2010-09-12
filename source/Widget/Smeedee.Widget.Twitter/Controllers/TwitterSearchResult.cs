using System.Collections.Generic;
using Smeedee.Widget.Twitter.ViewModel;

namespace Smeedee.Widget.Twitter.Controllers
{
    public class TwitterSearchResult
    {
        public const string NO_TWEETS_FOUND_ERROR_MESSAGE = "No tweets found";
        public const string UNABLE_TO_LOAD_TWEETS_ERROR_MESSAGE = "Unable to load tweets";
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }
        public List<TweetViewModel> TweetList { get; set; } 

    }
}
