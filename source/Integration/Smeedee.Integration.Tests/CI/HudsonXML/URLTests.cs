using NUnit.Framework;
using Smeedee.Integration.CI.Hudson_XML.Helper_classes;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Integration.Tests.CI.HudsonXML
{
    [TestFixture][Category("IntegrationTest")]
    class UrlTests
    {
        [Test]
        public void TestUrl()
        {
            URL.Combine("test1", "test2").ShouldBe("test1/test2");
            URL.Combine("test1/test2").ShouldBe("test1/test2");
            URL.Combine("test1", "/test2").ShouldBe("test1/test2");
            URL.Combine("test1/", "/test2").ShouldBe("test1/test2");
            URL.Combine("/test1/", "/test2/").ShouldBe("/test1/test2/");
            URL.Combine("", "/test2/").ShouldBe("/test2/");
            URL.Combine("/test1/", "").ShouldBe("/test1/");
            URL.Combine("dag", "olav", "prest", "e", "gard", "en").ShouldBe("dag/olav/prest/e/gard/en");
        }
    }
}
