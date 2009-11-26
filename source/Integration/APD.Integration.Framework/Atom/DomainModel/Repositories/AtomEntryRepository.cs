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
    public class AtomEntryRepository<T> : IRepository<T>
    //        where T : AtomEntry
    {
        public AtomEntryRepository(Uri address)
        {
            Address = address;
        }

        public AtomEntryFactory<T> EntryFactory { get; set; }

        public Uri Address { get; set; }

        #region IRepository<T> Members

        public IEnumerable<T> Get(Specification<T> specification)
        {
            string feeds = Address.IsAbsoluteUri ? GetEntriesFromWeb() : GetEntriesFromLocal();

            return GetEntries(feeds, specification);
        }

        #endregion

        private string GetEntriesFromLocal()
        {
            using (var fileReader = new StreamReader(File.OpenRead(Address.OriginalString)))
            {
                return fileReader.ReadToEnd();
            }
        }

        private string GetEntriesFromWeb()
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

        private IEnumerable<T> GetEntries(string response, Specification<T> specification)
        {
            var feeds = new List<T>();

            if (string.IsNullOrEmpty(response))
            {
                return feeds;
            }

            XDocument xdoc = XDocument.Parse(response);

            XNamespace atomNs = "http://www.w3.org/2005/Atom";

            return
                xdoc.Descendants(atomNs + "entry")
                    .Select(entryXml => EntryFactory.Assemble(entryXml))
                    .Where(specification.IsSatisfiedBy);
            //                    .OrderByDescending(entry => entry.Updated);
        }
    }
}