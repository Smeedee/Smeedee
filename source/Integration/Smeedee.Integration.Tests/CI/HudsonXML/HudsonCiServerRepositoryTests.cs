using NUnit.Framework;
using Smeedee.Integration.CI.HudsonXML.DomainModel.Repositories;

namespace Smeedee.Integration.Tests.CI.HudsonXML
{
    [TestFixture]
    class HudsonCiServerRepositoryTests
    {
        [Test]
        public void AssureGetDoesntCrash()
        {
            var repo = new HudsonCiServerRepository(new HudsonXmlParser(new FakeHudsonXmlFetcher()));
            repo.Get(null);
        }
    }
}
