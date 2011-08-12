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
        public class When_getting_bitmap_from_url : Shared
        {
            private static Bitmap gotBitmap;

            [Test]
            public void Assure_returns_bitmap_when_url_is_not_a_picture_url()
            {
                Given("url is a random string", () => url = "aURL");
                When(getting_bitmap_from_url);
                Then("should return null", () =>
                {
                    gotBitmap.ShouldBeInstanceOfType<Bitmap>();
                    gotBitmap.ShouldNotBeNull();
                });
            }

            [Test]
            public void Assure_returns_null_when_url_is_an_empty_image_url()
            {
                Given("url is a random string", () => url = emptyImageURL);
                When(getting_bitmap_from_url);
                Then("should return null", () => gotBitmap.ShouldBeNull());
            }

            [Test]
            public void Assure_returns_bitmap_when_url_is_a_picture_url()
            {
                Given("url is a picture url", () => url = imageURL);
                When(getting_bitmap_from_url);
                Then("should return", () =>
                {
                    gotBitmap.ShouldBeInstanceOfType<Bitmap>();
                    gotBitmap.ShouldNotBeNull();
                });
            }
            private When getting_bitmap_from_url = () => gotBitmap = webImageProvider.GetBitmapFromURL(url);
        }

        [TestFixture]
        public class When_getting_picture_node_url_from_xpath_and_url : Shared
        {
            private static string gotPictureNodeUrl;

            [Test]
            public void Assure_returns_null_when_url_and_xpath_are_random_string()
            {
                Given("url and xpath are random strings", () =>
                {
                    url = "aURL";
                    xpath = "aXpath";
                });
                When(getting_picture_node_from_xpath);
                Then("should return null", () => gotPictureNodeUrl.ShouldBeNull());
            }

            [Test]
            public void Assure_returns_null_when_xpath_is_random_string()
            {
                Given("url and xpath are random strings", () =>
                {
                    url = imageURL;
                    xpath = "aXpath";
                });
                When(getting_picture_node_from_xpath);
                Then("should return null", () => gotPictureNodeUrl.ShouldBeNull());
            }

            [Test]
            public void Assure_returns_string_when_url_and_xpath_are_correct()
            {
                Given("url and xpath are random strings", () =>
                {
                    url = pageURL;
                    xpath = xPath;
                });
                When(getting_picture_node_from_xpath);
                Then("should return null", () => 
                { 
                    gotPictureNodeUrl.ShouldBeInstanceOfType<string>(); 
                    gotPictureNodeUrl.ShouldNotBeNull(); 
                });
            }
            private When getting_picture_node_from_xpath = () => gotPictureNodeUrl = webImageProvider.GetPictureNodeURLFromXpath(url, xpath);
        }

        public class Shared : ScenarioClass
        {
            protected static WebImageProvider webImageProvider;

            protected static string url;
            protected static string xpath;
            protected static string imageURL;
            protected static string emptyImageURL;

            protected static string pageURL;
            protected static string xPath;

            [SetUp]
            public void SetUp()
            {
                imageURL = "http://smeedee.org/images/code.png";
                emptyImageURL = "blabla.png";
                pageURL = "http://smeedee.org/";
                xPath = "/html/body/div[2]/div[2]/div/div/div/img";

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
