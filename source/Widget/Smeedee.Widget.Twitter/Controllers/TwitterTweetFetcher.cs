using System;
using System.Collections.Generic;
using System.Linq;
using Smeedee.Client.Framework.Services;
using System.Xml.Linq;
using Smeedee.Widgets.SL.Twitter.ViewModel;

namespace Smeedee.Widgets.SL.Twitter.Controllers
{
    public class TwitterTweetFetcher : IFetchTweets
    {
        private const string XML_NAMESPACE = "http://www.w3.org/2005/Atom";
        private readonly IFetchXml xmlFetcher;
        public event EventHandler<AsyncResponseEventArgs<TwitterSearchResult>> GetTweetsCompleted;

        public TwitterTweetFetcher(IFetchXml xmlFetcher)
        {
            this.xmlFetcher = xmlFetcher;
            xmlFetcher.GetXmlCompleted += GetXmlCompleted;
        }

        private void GetXmlCompleted(object sender, AsyncResponseEventArgs<XDocument> e)
        {
            if (e.ReturnValue == null)
                MakeAndReturnUnableToLoadErrorResult();
            else
                MakeAndReturnAbleToLoadResult(e);
        }

        private void MakeAndReturnAbleToLoadResult(AsyncResponseEventArgs<XDocument> e)
        {
            var entries = GetEntries(e);

            if (entries.Count == 0)
                MakeAndReturnNoTweetsFoundResult();
            else
                MakeAndReturnNormalResult(entries);
        }

        private void MakeAndReturnNormalResult(List<XElement> entries)
        {
            var result = new TwitterSearchResult();
            result.Error = false;
            result.TweetList = MakeTweetViewModelList(entries);

            if (GetTweetsCompleted != null)
                GetTweetsCompleted(this, new AsyncResponseEventArgs<TwitterSearchResult>(result));
        }

        private void MakeAndReturnNoTweetsFoundResult()
        {
            var result = new TwitterSearchResult();
            result.Error = true;
            result.ErrorMessage = "No tweets found";
            result.TweetList = null;

            if (GetTweetsCompleted != null)
                GetTweetsCompleted(this, new AsyncResponseEventArgs<TwitterSearchResult>(result));
        }

        private void MakeAndReturnUnableToLoadErrorResult()
        {
            var result = new TwitterSearchResult();
            result.Error = true;
            result.ErrorMessage = "Unable to load tweets";
            result.TweetList = null;

            if (GetTweetsCompleted != null)
                GetTweetsCompleted(this, new AsyncResponseEventArgs<TwitterSearchResult>(result));
        }

        private  List<XElement> GetEntries(AsyncResponseEventArgs<XDocument> e)
        {
            var xDocument = e.ReturnValue;
            return GetEntriesFromXDocument(xDocument);
        }

        private List<XElement> GetEntriesFromXDocument(XDocument xDocument)
        {
            return xDocument.Root.Elements(XName.Get("entry", XML_NAMESPACE)).ToList();
        }

        private List<TweetViewModel> MakeTweetViewModelList(List<XElement> entries)
        {
            return entries.Select(MakeTweetViewModel).ToList();
        }


        private TweetViewModel MakeTweetViewModel(XElement entry)
        {
            return new TweetViewModel
            {
                Date = DateTime.Parse(entry.Element(XName.Get("published", XML_NAMESPACE)).Value),
                Message = entry.Element(XName.Get("title", XML_NAMESPACE)).Value,
                UserImageUrl = entry.Elements(XName.Get("link", XML_NAMESPACE)).ElementAt(1).Attribute(XName.Get("href", "")).Value,
                Username = entry.Element(XName.Get("author", XML_NAMESPACE)).Element(XName.Get("name", XML_NAMESPACE)).Value
            };
        }

        public void BeginGetTweets(string search, int maxNumberOfElements)
        {
            string query = MakeQuery(search, maxNumberOfElements);
            xmlFetcher.BeginGetXml(query);
        }

        private string MakeQuery(string search, int maxNumberOfElements)
        {
            return String.Format(
                "http://search.twitter.com/search.atom?q={0}&rpp={1}",
                Uri.EscapeDataString(search), maxNumberOfElements);
        }
    }
}