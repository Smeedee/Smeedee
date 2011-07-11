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

        //[TestFixture]
        //public class When_snapshot_is_generated : Shared
        //{
        //    [Test]
        //    public void Then_assure_it_is_not_blank()
        //    {
        //        var snap = webSnapshotter.GetSnapshot(url);
                
        //    }
        //}

        [TestFixture]
        public class When_blank_bitmap_is_generated : Shared
        {
            [Test]
            public void Then_assure_it_is_identified_as_blank()
            {
                var blankBitmap = new Bitmap(10, 10);
                for (int i = 0; i < blankBitmap.Width; i++)
                {
                    for (int j = 0; j < blankBitmap.Height; j++)
                    {
                        blankBitmap.SetPixel(i,j,Color.White);                        
                    }
                }
                webSnapshotter.IsBlank(blankBitmap).ShouldBeTrue();
            }
        }


        public class Shared
        {
            protected WebSnapshotter webSnapshotter;
            protected string url;

            [SetUp]
            public void Setup()
            {
                webSnapshotter = new WebSnapshotter();
                url = "http://www.smeedee.org/";
            }
        }

    }
}
