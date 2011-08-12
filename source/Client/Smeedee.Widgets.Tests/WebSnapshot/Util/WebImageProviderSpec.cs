using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Widgets.WebSnapshot.Util;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widgets.Tests.WebSnapshot.Util
{
    internal class WebImageProviderSpec
    {
        [TestFixture]
        public class When_using_WebImageProvider : Shared
        {
            private string url;
            private string xpath;
            private Bitmap aBitmap;
            private string pictureNodeUrl;

            [Test]
            [Ignore]
            public void Assure_get_bitmap_from_url_returns_null_when_url_is_random_string()
            {
                Given("url is a random string", () => url = "aURL");
                When("getting bitmap from url", () => aBitmap = webImageProvider.GetBitmapFromURL(url));
                Then("should return null", () => aBitmap.ShouldBeNull());
            }

            [Test]
            public void Assure_returns_null_when_get_picture_node_url_from_xpath_when_url_and_xpath_are_random_string()
            {
                Given("url and xpath are random strings", () =>
                                                              {
                                                                  url = "aURL";
                                                                  xpath = "aXpath";
                                                              });
                When("getting picture node from xpath",
                     () => pictureNodeUrl = webImageProvider.GetPictureNodeURLFromXpath(url, xpath));
                Then("should return null", () => pictureNodeUrl.ShouldBeNull());
            }
        }

        public class Shared : ScenarioClass
        {
            protected static WebImageProvider webImageProvider;

            protected static string imageURL;
            protected static string pageURL;
            protected static string xPath;
            protected static string xPathParsed;


            [SetUp]
            public void SetUp()
            {
                imageURL = "http://smeedee.org/images/code.png";
                pageURL = "http://smeedee.org/";
                xPath = "/html/body/div[2]/div[2]/div/div/div/img";
                xPathParsed = "images/code.png/";

                webImageProvider = new WebImageProvider();
                Scenario("");
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
        }
    }
}
