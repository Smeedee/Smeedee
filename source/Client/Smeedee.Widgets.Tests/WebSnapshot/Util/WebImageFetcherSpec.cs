using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Resources;
using Moq;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using Smeedee.Widgets.WebSnapshot.Util;
using System.Windows.Media.Imaging;

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
                image.ShouldBeInstanceOfType<WriteableBitmap>();
                image.ShouldNotBeNull();
            }
            
        }

        [TestFixture]
        public class When_normal_URL_is_specified : Shared
        {
            [Test]
            public void Then_assure_it_is_not_downloaded()
            {
                providerMock.Setup(w => w.GetBitmapFromURL(pageURL)).Returns(null as WriteableBitmap);
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
                image.ShouldBeInstanceOfType<WriteableBitmap>();
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
                image.ShouldBeInstanceOfType<WriteableBitmap>();
                image.ShouldNotBeNull();
            }
        }

        public class Shared
        {
            protected Mock<IWebImageProvider> providerMock;
            protected WebImageFetcher webImageFetcher;
            protected WriteableBitmap bitmap;
            protected string imageURL;
            protected string pageURL;
            protected string xPath;
            protected string xPathParsed;

            [SetUp]
            public void Setup()
            {
                providerMock = new Mock<IWebImageProvider>();
                webImageFetcher = new WebImageFetcher(providerMock.Object);

                string imgUrl = "..\\..\\WebSnapshot\\Util\\not-blank.bmp";
                Uri uri = new Uri(imgUrl, UriKind.Relative);
                BitmapImage src = new BitmapImage(uri);
                bitmap = new WriteableBitmap(src);

                imageURL = "http://smeedee.org/images/code.png";
                pageURL = "http://smeedee.org/";
                xPath = "/html/body/div[2]/div[2]/div/div/div/img";
                xPathParsed = "images/code.png/";

                //WebImageFetcher f = new WebImageFetcher(new WebImageProvider());
                //bitmap = f.GetBitmapFromURL(imageURL);
                //webImageFetcher = f;

                // uncomment the above lines to run non-mocked
            }
        }
    }
}
