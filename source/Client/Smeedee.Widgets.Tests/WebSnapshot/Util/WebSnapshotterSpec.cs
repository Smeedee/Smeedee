using System;
using System.Drawing;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using Smeedee.Widgets.WebSnapshot.Util;

namespace Smeedee.Widgets.Tests.WebSnapshot.Util
{
    public class WebSnapshotterSpec
    {
        [TestFixture]
        public class When_URL_is_specified : Shared
        {
            [Test]
            public void Then_assure_property_was_set_to_URL()
            {
                webSnapshotter.GetSnapshot(url);
                webSnapshotter.Url.ShouldBe(url);
            }

            [Test]
            public void Then_assure_property_is_updated_when_giving_new_URL()
            {
                webSnapshotter.GetSnapshot(url);
                webSnapshotter.Url.ShouldBe(url);

                webSnapshotter.GetSnapshot("http://example.org");
                webSnapshotter.Url.ShouldBe("http://example.org");
            }
        }

        [TestFixture]
        public class When_blank_bitmap_is_generated : Shared
        {
            [Test]
            public void Then_assure_it_is_identified_as_blank()
            {
                var blankBitmap = GetBlankBitmap();
                webSnapshotter.IsBlank(blankBitmap).ShouldBeTrue();
            }
        }

        [TestFixture]
        public class When_supported_site_is_snapshotted : Shared
        {
            [Test]
            public void Then_assure_it_is_not_blank()
            {
                var notBlankImage = GetColorfullBitmap();
                var snapshot = webSnapshotter.GetSnapshot(url);
                webSnapshotter.IsBlank(snapshot).ShouldBeFalse();
            }
        }

        [TestFixture]
        public class When_unsupported_site_is_snapshotted : Shared
        {
            [Test]
            public void Then_assure_it_is_detected_as_blank()
            {
                var blankImage = GetBlankBitmap();
                var snapshot = webSnapshotter.GetSnapshot("http://google.com");
                webSnapshotter.IsBlank(snapshot).ShouldBeTrue();
            }
        }

        public class Shared : ScenarioClass
        {
            protected static string url;

            protected static WebSnapshotter webSnapshotter;

            [SetUp]
            public void Setup()
            {
                webSnapshotter = new WebSnapshotter();
                url = "http://www.smeedee.org/";
            }

            public Bitmap GetBlankBitmap()
            {
                var blankBitmap = new Bitmap(10, 10);
                for (int i = 0; i < blankBitmap.Width; i++)
                {
                    for (int j = 0; j < blankBitmap.Height; j++)
                    {
                        blankBitmap.SetPixel(i, j, Color.White);
                    }
                }
                return blankBitmap;
            }

            public Bitmap GetColorfullBitmap()
            {
                var colorfulBitmap = new Bitmap(10, 10);
                var rand = new Random();
                for (int i = 0; i < colorfulBitmap.Width; i++)
                {
                    for (int j = 0; j < colorfulBitmap.Height; j++)
                    {
                        var randomColor = Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
                        colorfulBitmap.SetPixel(i, j, randomColor);
                    }
                }
                return colorfulBitmap;
            }
        }
    }
}
