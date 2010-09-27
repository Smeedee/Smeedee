using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Smeedee.Client.Framework.Services;
using Smeedee.Widgets.SL.Twitter.Controllers;
using Smeedee.Widgets.SL.Tests.Twitter;

namespace Smeedee.Widgets.SL.Twitter.Tests.Controllers
{
    [TestClass]
    public class TwitterTweetFetcherSpecs
    {
        private Mock<IFetchXml> _xmlFetcher;
        private TwitterTweetFetcher _tweetFetcher;

        [TestInitialize]
        public void Setup()
        {
            _xmlFetcher = new Mock<IFetchXml>();
            SetupXmlFetcherToReturnSampleResult(); 
            _tweetFetcher = new TwitterTweetFetcher(_xmlFetcher.Object);
        }

        private void SetupXmlFetcherToReturnSampleResult()
        {
            _xmlFetcher.Setup(m => m.BeginGetXml(It.IsAny<string>())).Raises( m =>  m.GetXmlCompleted += null, new AsyncResponseEventArgs<XDocument>(XDocument.Load(new MemoryStream(Resource1.searchResults))));
        }

        [TestMethod]
        public void Should_create_correct_query_when_getting_tweets()
        {
            _tweetFetcher.BeginGetTweets("hello", 5);
            _xmlFetcher.Verify(x => x.BeginGetXml("http://search.twitter.com/search.atom?q=hello&rpp=5"));
        }

        [TestMethod]
        public void Should_URL_encode_multi_word_search_queries()
        {
            _tweetFetcher.BeginGetTweets("Hello World", 5);
            _xmlFetcher.Verify(x => x.BeginGetXml("http://search.twitter.com/search.atom?q=Hello%20World&rpp=5"));
        }

        [TestMethod]
        public void Should_URL_encode_hash_sign()
        {
            _tweetFetcher.BeginGetTweets("#twitter", 5);
            _xmlFetcher.Verify(x => x.BeginGetXml("http://search.twitter.com/search.atom?q=%23twitter&rpp=5"));
        }

        [TestMethod]
        public void integration_test()
        {
            var liveXmlFetcher = new XmlFetcher();
            var twitterTweetFetcher = new TwitterTweetFetcher(liveXmlFetcher);

            twitterTweetFetcher.GetTweetsCompleted += ((o, e) => Assert.AreEqual(10, e.ReturnValue.TweetList.Count));

            twitterTweetFetcher.BeginGetTweets("hello", 10);
        }

        [TestMethod]
        public void Should_make_correct_TweetViewModel()
        {
            _tweetFetcher.GetTweetsCompleted += ((o, e) =>
                {
                    var tweetViewModels = e.ReturnValue.TweetList;
                    var firstTweetViewModel = tweetViewModels[0];

                    Assert.AreEqual("HotSmart (Juan J. Ramirez)", firstTweetViewModel.Username);
                    Assert.AreEqual(DateTime.Parse("2010-06-17T18:20:15Z"), firstTweetViewModel.Date);
                    Assert.AreEqual(
                        "@DiyanaAlcheva Hello Diyana will you please take a look at this and RT? www.EatSlowlySite.com",
                        firstTweetViewModel.Message);
                    Assert.AreEqual(
                        "http://a1.twimg.com/profile_images/368668250/J._Ramirez_normal.jpg",
                        firstTweetViewModel.UserImageUrl);
                });

            _tweetFetcher.BeginGetTweets("hello", 1);
        }

        [TestMethod]
        public void UserImageUrl_Should_be_an_image_url()
        {
            _tweetFetcher.GetTweetsCompleted += ((o, e) =>
                {
                    var tweets = e.ReturnValue.TweetList;
                    foreach (var tweetViewModel in tweets)
                    {
                        Assert.IsTrue(IsImageUrl(tweetViewModel.UserImageUrl));
                    }
                });

            _tweetFetcher.BeginGetTweets("hello", 10);
        }

        [TestMethod]
        public void Should_get_error_on_empty_search()
        {
            var xmlFetcher = new Mock<IFetchXml>();
            xmlFetcher.Setup(m => m.BeginGetXml(It.IsAny<string>())).Raises(m => m.GetXmlCompleted += null, new AsyncResponseEventArgs<XDocument>(XDocument.Load(new MemoryStream(Resource1.emptySearchResult))));
            var fetcher = new TwitterTweetFetcher(xmlFetcher.Object);

            fetcher.GetTweetsCompleted += ((o, e) =>
                                               {
                                                   Assert.IsTrue(e.ReturnValue.Error);
                                                   Assert.AreEqual(TwitterSearchResult.NO_TWEETS_FOUND_ERROR_MESSAGE,
                                                                   e.ReturnValue.ErrorMessage);
                                               });
            fetcher.BeginGetTweets("hello", 10);
        }

        [TestMethod]
        public void Should_get_error_on_webClient_error()
        {
            var xmlFetcher = new Mock<IFetchXml>();
            xmlFetcher.Setup(m => m.BeginGetXml(It.IsAny<string>())).Raises(m => m.GetXmlCompleted += null, new AsyncResponseEventArgs<XDocument>(null));
            var fetcher = new TwitterTweetFetcher(xmlFetcher.Object);

            fetcher.GetTweetsCompleted += ((o, e) =>
                                               {
                                                   Assert.IsTrue(e.ReturnValue.Error);
                                                   Assert.AreEqual(
                                                       TwitterSearchResult.UNABLE_TO_LOAD_TWEETS_ERROR_MESSAGE,
                                                       e.ReturnValue.ErrorMessage);
                                               });
            fetcher.BeginGetTweets("hello", 10);
        }

        private bool IsImageUrl(string userImageUrl)
        {
            string[] imageSuffixes = {".jpg", ".png", ".JPG", ".PNG", ".jpeg", ".JPEG"};

            foreach (var imageSuffix in imageSuffixes)
            {
                if (userImageUrl.EndsWith(imageSuffix))
                    return true;
            }
            return false;
        }
    }
}
