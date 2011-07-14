using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using Smeedee.Widgets.WebSnapshot.Util;

namespace Smeedee.Widgets.Tests.WebSnapshot.Util
{
    public class WebImageFetcherSpec
    {
        [TestFixture]
        public class When_image_URL_is_specified : Shared
        {

            [Test]
            public void Then_assure_it_can_be_downloaded()
            {
                providerMock.Setup(w => w.GetBitmapFromURL(imageURL)).Returns(bitmap);
                var image = webImageFetcher.GetBitmapFromURL(imageURL);
                image.ShouldBeInstanceOfType<Bitmap>();
                image.ShouldNotBeNull();
            }
            
        }

        [TestFixture]
        public class When_normal_URL_is_specified : Shared
        {
            [Test]
            public void Then_assure_it_is_not_downloaded()
            {
                providerMock.Setup(w => w.GetBitmapFromURL(pageURL)).Returns(null as Bitmap);
                webImageFetcher.GetBitmapFromURL(pageURL).ShouldBeNull();
            }
        }

        [TestFixture]
        public class When_normal_URL_and_Xpath_is_specified : Shared
        {
            [Test]
            public void Then_assure_it_can_be_downloaded()
            {
                providerMock.Setup(w => w.GetPictureNodeURLFromXpath(pageURL, xPath)).Returns(xPathParsed);
                providerMock.Setup(w => w.GetBitmapFromURL(imageURL)).Returns(bitmap);
                var image = webImageFetcher.GetBitmapFromURL(pageURL, xPath);
                image.ShouldBeInstanceOfType<Bitmap>();
                image.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_picture_URL_and_Xpath_is_specified : Shared
        {
            [Test]
            public void Then_assure_xpath_is_ignored_and_picture_is_downloaded()
            {
                providerMock.Setup(w => w.GetPictureNodeURLFromXpath(imageURL, xPath)).Returns(imageURL);
                providerMock.Setup(w => w.GetBitmapFromURL(imageURL)).Returns(bitmap);
                var image = webImageFetcher.GetBitmapFromURL(imageURL, xPath);
                image.ShouldBeInstanceOfType<Bitmap>();
                image.ShouldNotBeNull();
                providerMock.Verify(w => w.GetBitmapFromURL(imageURL));
            }
        }

        public class Shared
        {
            protected Mock<IWebImageProvider> providerMock;
            protected WebImageFetcher webImageFetcher;
            protected Bitmap bitmap;
            protected string imageURL;
            protected string pageURL;
            protected string xPath;
            protected string xPathParsed;

            [SetUp]
            public void Setup()
            {
                providerMock = new Mock<IWebImageProvider>();
                webImageFetcher = new WebImageFetcher(providerMock.Object);
                bitmap = new Bitmap(1,1);
                imageURL = "http://smeedee.org/images/code.png";
                pageURL = "http://smeedee.org/";
                xPath = "/html/body/div[2]/div[2]/div/div/div/img";
                xPathParsed = "images/code.png/";
            }
        }
    }
}
