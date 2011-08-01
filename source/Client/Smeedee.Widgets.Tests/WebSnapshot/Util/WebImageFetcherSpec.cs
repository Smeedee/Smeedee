using System;
using System.Collections.Generic;
using System.Drawing;
using Moq;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using Smeedee.Widgets.WebSnapshot.Util;

namespace Smeedee.Widgets.Tests.WebSnapshot.Util
{
    [TestFixture]
    public class When_image_URL_is_specified : Shared
    {
        [Test]
        public void Then_assure_it_can_be_downloaded()
        {
            Given(fetcher_is_created);
            When("image URL is specified", () =>
                providerMock.Setup(w => w.GetBitmapFromURL(imageURL)).Returns(bitmap));
            Then("it can be downloaded", () => 
                webImageFetcher.GetBitmapFromURL(imageURL).ShouldBeInstanceOfType<Bitmap>());
        }
    }

    [TestFixture]
    public class When_normal_URL_is_specified : Shared
    {
        [Test]
        public void Then_assure_it_is_not_downloaded()
        {
            Given(fetcher_is_created);
            When("pageURL is given instead of pictureURL", () =>
                providerMock.Setup(w => w.GetBitmapFromURL(pageURL)).Returns(null as Bitmap));
            Then("it is not downloaded", () =>
                webImageFetcher.GetBitmapFromURL(pageURL).ShouldBeNull());
        }
    }

    [TestFixture]
    public class When_normal_URL_and_Xpath_is_specified : Shared
    {
        [Test]
        public void Then_assure_it_can_be_downloaded()
        {
            Given(fetcher_is_created);
            When("normal URL and XPath is specified", () =>
            {
                providerMock.Setup(w => w.GetPictureNodeURLFromXpath(pageURL, xPath)).Returns(xPathParsed);
                providerMock.Setup(w => w.GetBitmapFromURL(imageURL)).Returns(bitmap);
            });
            Then("it can be downloaded", () => 
                webImageFetcher.GetBitmapFromURL(pageURL, xPath).ShouldBeInstanceOfType<Bitmap>());
        }
    }

    [TestFixture]
    public class When_picture_URL_and_Xpath_is_specified : Shared
    {
        [Test]
        public void Then_assure_xpath_is_ignored_and_picture_is_downloaded()
        {
            Given(fetcher_is_created);
            When("image URL and XPath is specified", () =>
            {
                providerMock.Setup(w => w.GetPictureNodeURLFromXpath(imageURL, xPath)).Returns(imageURL);
                providerMock.Setup(w => w.GetBitmapFromURL(imageURL)).Returns(bitmap);
            });
            Then("XPath is ignored and image is downloaded", () => 
                webImageFetcher.GetBitmapFromURL(imageURL, xPath).ShouldBeInstanceOfType<Bitmap>());
            
        }
    }

    public class Shared : ScenarioClass
    {
        protected static Mock<IWebImageProvider> providerMock;
        protected static WebImageFetcher webImageFetcher;
        protected static Bitmap bitmap;
        protected static string imageURL;
        protected static string pageURL;
        protected static string xPath;
        protected static string xPathParsed;

        protected Context fetcher_is_created = () => CreateProvider();

        private static void CreateProvider()
        {
            providerMock = new Mock<IWebImageProvider>();
            webImageFetcher = new WebImageFetcher(providerMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            string imgUrl = "..\\..\\WebSnapshot\\Util\\not-blank.bmp";
            bitmap = new Bitmap(imgUrl);

            imageURL = "http://smeedee.org/images/code.png";
            pageURL = "http://smeedee.org/";
            xPath = "/html/body/div[2]/div[2]/div/div/div/img";
            xPathParsed = "images/code.png/";

            //WebImageFetcher f = new WebImageFetcher(new WebImageProvider());
            //bitmap = f.GetBitmapFromURL(imageURL);
            //webImageFetcher = f;

            // uncomment the above lines to run non-mocked
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }
}

