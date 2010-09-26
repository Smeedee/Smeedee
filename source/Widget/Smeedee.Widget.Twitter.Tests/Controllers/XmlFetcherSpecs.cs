using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Widgets.SL.Twitter.Controllers;

namespace Smeedee.Widgets.SL.Twitter.Tests.Controllers
{
    [TestClass]
    public class XmlFetcherSpecs
    {
        private XmlFetcher _xmlFetcher;

        [TestInitialize]
        public void Setup()
        {
            _xmlFetcher = new XmlFetcher();
        }

        [TestMethod]
        public void Assert_nonexisting_Url_returns_null()
        {
            _xmlFetcher.GetXmlCompleted += ((o, e) => Assert.IsNull(e.ReturnValue));
            _xmlFetcher.BeginGetXml("http://jfksdlfjklasfjkslafjkaslfjkasl.fjsdklfjksdl");
        }

        [TestMethod]
        public void Assert_existing_Url_returns_XDocument()
        {
            _xmlFetcher.GetXmlCompleted += ((o, e) => Assert.IsTrue(e.ReturnValue.GetType() == typeof (XDocument)));
            _xmlFetcher.BeginGetXml("http://search.twitter.com/search.atom?q=twitter&rpp=10");
        }

    }
}
