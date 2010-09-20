using NUnit.Framework;
using Smeedee.Integration.CI.HudsonXML.DomainModel.Repositories;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Integration.Tests.CI.HudsonXML
{
    [TestFixture]
    class IntegrationTests
    {
        private FakeHudsonXmlFetcher fakeHudsonXmlFetcher;
        private HudsonXmlParser hudsonXmlParser;

        [SetUp]
        public void Setup()
        {
            fakeHudsonXmlFetcher = new FakeHudsonXmlFetcher();
            hudsonXmlParser = new HudsonXmlParser(fakeHudsonXmlFetcher);
        }

        [Test]
        public void TestGetHudsonProjects()
        {
            var list = hudsonXmlParser.GetProjects();
            (list.Count > 0).ShouldBeTrue();
        }
    }
}
