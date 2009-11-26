using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

using APD.DomainModel.Framework;
using APD.Integration.Framework.Atom.DomainModel.Factories;


namespace APD.Integration.Framework.Atom.DomainModel.Repositories
{
    public class AtomFeedRepository<TFeed, TEntry> : IRepository<TFeed>
    {
        public AtomFeedRepository(Uri address)
        {
            Address = address;
        }

        public AtomFeedFactory<TFeed, TEntry> FeedFactory { get; set; }

        public Uri Address { get; set; }

        #region IRepository<TFeed> Members

        public IEnumerable<TFeed> Get(Specification<TFeed> specification)
        {
            string feeds = Address.IsAbsoluteUri ? GetFeedsFromWeb() : GetFeedsFromLocal();

            return GetFeeds(feeds, specification);
        }

        #endregion

        private string GetFeedsFromLocal()
        {
            using (var fileReader = new StreamReader(File.OpenRead(Address.OriginalString)))
            {
                return fileReader.ReadToEnd();
            }
        }

        private string GetFeedsFromWeb()
        {
            try
            {
                WebRequest webRequest = WebRequest.Create(Address);
                WebResponse webResponse = webRequest.GetResponse();

                using (var responseReader = new StreamReader(webResponse.GetResponseStream()))
                {
                    return responseReader.ReadToEnd();
                }
            }
            catch (Exception exception)
            {
                throw new AtomFeedRepositoryException(
                    string.Format("Could not connect to atom feed at '{0}'.", Address), exception);
            }
        }

        private IEnumerable<TFeed> GetFeeds(string response, Specification<TFeed> specification)
        {
            var feeds = new List<TFeed>();

            if (string.IsNullOrEmpty(response))
            {
                return feeds;
            }

            XDocument xdoc = XDocument.Parse(response);

            XNamespace atomNs = "http://www.w3.org/2005/Atom";

            return
                xdoc.Descendants(atomNs + "feed")
                    .Select(entryXml => FeedFactory.Assemble(entryXml))
                    .Where(specification.IsSatisfiedBy);
            //                    .OrderByDescending(entry => entry.Updated);
        }
    }
}