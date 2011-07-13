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
                webImageFetcherMock.Setup(w => w.GetBitmapFromURL(imageURL)).Returns(bitmap);
                var webImageFetcher = webImageFetcherMock.Object;
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
                webImageFetcherMock.Setup(w => w.GetBitmapFromURL(pageURL)).Returns(null as Bitmap);
                webImageFetcherMock.Object.GetBitmapFromURL(pageURL);
                webImageFetcherMock.Verify(w => w.GetBitmapFromURL(pageURL));
            }
        }

        [TestFixture]
        public class When_normal_URL_and_Xpath_is_specified : Shared
        {
            [Test]
            public void Then_assure_it_can_be_downloaded()
            {
                webImageFetcherMock.Setup(w => w.GetBitmapFromURL(pageURL, xPath)).Returns(bitmap);
                var webImageFetcher = webImageFetcherMock.Object;
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
                webImageFetcherMock.Setup(w => w.GetBitmapFromURL(imageURL, xPath)).Returns(bitmap);
                var webImageFetcher = webImageFetcherMock.Object;
                var image = webImageFetcher.GetBitmapFromURL(imageURL, xPath);

                image.ShouldBeInstanceOfType<Bitmap>();
                image.ShouldNotBeNull();
                webImageFetcherMock.Verify(w => w.GetBitmapFromURL(imageURL, xPath));
            }
        }

        public class Shared
        {
            protected Mock<IWebImageFetcher> webImageFetcherMock;
            protected Bitmap bitmap;
            protected string imageURL;
            protected string pageURL;
            protected string xPath;

            [SetUp]
            public void Setup()
            {
                webImageFetcherMock = new Mock<IWebImageFetcher>();
                bitmap = new Bitmap(1,1);
                imageURL = "http://smeedee.org/images/code.png";
                pageURL = "http://smeedee.org/";
                xPath = "/html/body/div[2]/div[2]/div/div/div/img";  
            }
        }
    }
}
