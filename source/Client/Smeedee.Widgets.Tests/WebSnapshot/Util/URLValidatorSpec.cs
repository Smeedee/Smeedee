using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using Smeedee.Widgets.WebSnapshot.Util;

namespace Smeedee.Widgets.Tests.WebSnapshot.Util
{
    public class URLValidatorSpec
    {

        [TestFixture]
        public class When_image_URL_is_given : Shared
        {
            [Test]
            public void Then_assure_it_is_validated_as_image_url()
            {
                URLValidator.IsPictureURL(imageURL).ShouldBeTrue();
            }

            [Test]
            public void Then_assure_it_is_valid_as_normal_url()
            {
                URLValidator.IsValidUrl(imageURL).ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_page_URL_is_given : Shared
        {
            [Test]
            public void Then_assure_it_is_valid_as_normal_url()
            {
                URLValidator.IsValidUrl(url).ShouldBeTrue();
            }

            [Test]
            public void Then_assure_it_is_not_valid_image_url()
            {
                URLValidator.IsPictureURL(url).ShouldBeFalse();
            }

            [Test]
            public void Then_assure_it_is_valid_as_file_url()
            {
                URLValidator.IsValidUrl(fileURL).ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_invalid_URL_is_given : Shared
        {
            [Test]
            public void Then_assure_it_is_not_valid_normal_url()
            {
                URLValidator.IsValidUrl(invalid).ShouldBeFalse();
            }

            [Test]
            public void Then_assure_it_is_not_valid_image_url()
            {
                URLValidator.IsPictureURL(invalid).ShouldBeFalse();
            }
        }

        public class Shared
        {
            protected string imageURL;
            protected string url;
            protected string invalid;

            protected string fileURL;

            protected string existURL;

            [SetUp]
            public void Setup()
            {
                imageURL = "http://smeedee.org/images/code.png";
                url = "http://smeedee.org/";
                invalid = "mister spock is not an url";

                fileURL = "file://afile.txt";
            }
        }
    }
}
