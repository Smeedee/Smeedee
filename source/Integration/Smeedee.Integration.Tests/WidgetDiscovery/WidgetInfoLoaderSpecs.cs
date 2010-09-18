using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Smeedee.Integration.WidgetDiscovery;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Integration.Tests.WidgetDiscovery
{
    public class WidgetInfoLoaderSpecs
    {
        [TestFixture][Category("IntegrationTest")]
        public class when_loading_widgetInfo
        {
            private WidgetInfoLoader loader;

            [SetUp]
            public void Setup()
            {
                loader = new WidgetInfoLoader(new XapUnpacker());
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void Assure_non_existing_directory_throws_argumentException()
            {
                var dirInfo = new DirectoryInfo("i do not exist for sure 1337");
                if( dirInfo.Exists )
                    Assert.Fail("The directory should not exist. Change the test to a dir that does not exist");
                var result = loader.FromDirectory(dirInfo);
            }

            [Test]
            [Ignore]
            public void Assure_all_widgets_are_returned()
            {
                var dirWithXapWithWidget = new DirectoryInfo(@"..\..\Resources");
                if( dirWithXapWithWidget.Exists == false )
                    Assert.Fail("The sample XAP directory must exist! Please fix the test environment. In the dir there must be a xap with widgets.");

                var result = loader.FromDirectory(dirWithXapWithWidget);
                result.Count().ShouldBe(3);
            }
        }
    }
}
