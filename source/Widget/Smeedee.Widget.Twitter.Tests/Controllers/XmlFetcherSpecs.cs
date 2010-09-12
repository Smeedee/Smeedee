using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Widget.Twitter.Controllers;

namespace Smeedee.Widget.Twitter.Tests.Controllers
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
