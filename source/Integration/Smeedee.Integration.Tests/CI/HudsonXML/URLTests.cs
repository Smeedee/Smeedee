using NUnit.Framework;
using Smeedee.Integration.CI.Hudson_XML.Helper_classes;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Integration.Tests.CI.HudsonXML
{
    [TestFixture]
    class UrlTests
    {
        [Test]
        public void TestUrl()
        {
            URL.AppendToBaseURL("test1", "test2").ShouldBe("test1/test2");
            URL.AppendToBaseURL("test1/test2").ShouldBe("test1/test2");
            URL.AppendToBaseURL("test1", "/test2").ShouldBe("test1/test2");
            URL.AppendToBaseURL("test1/", "/test2").ShouldBe("test1/test2");
            URL.AppendToBaseURL("/test1/", "/test2/").ShouldBe("/test1/test2/");
            URL.AppendToBaseURL("", "/test2/").ShouldBe("/test2/");
            URL.AppendToBaseURL("/test1/", "").ShouldBe("/test1/");
            URL.AppendToBaseURL("dag", "olav", "prest", "e", "gard", "en").ShouldBe("dag/olav/prest/e/gard/en");
        }
    }
}
