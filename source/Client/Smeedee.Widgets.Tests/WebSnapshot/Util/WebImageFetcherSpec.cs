using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
                var image = WebImageFetcher.GetBitmapFromURL(imageURL);
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
                var page = WebImageFetcher.GetBitmapFromURL("http://smeedee.org");
                page.ShouldBeNull();
            }
        }

        public class Shared
        {
            protected string imageURL;


            [SetUp]
            public void Setup()
            {
                imageURL = "http://smeedee.org/images/code.png";
            }
        }
    }
}
