using System;
using APD.DomainModel.CI;
using APD.DomainModel.Framework;
using APD.Integration.Framework.Atom.DomainModel.Repositories;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;

namespace APD.Plugin.TeamCity.Tests.Integration
{
    public class Shared : ScenarioClass
    {
        internal const string TEAMCITY_FEED_ADDRESS = @"http://80.203.160.221:10001/feed.html";

        protected AtomEntryRepository<CIProject> repository;
        protected Exception thrownException;

        [TearDown]
        public void RunTests()
        {
            StartScenario();
        }
    }

    [TestFixture]
    public class Spawning_repository : Shared
    {
        [Test]
        public void Assert_no_connection_exception_is_thrown()
        {
            Given("repository has been created", () =>
            {
                repository = new AtomEntryRepository<CIProject>(new Uri(TEAMCITY_FEED_ADDRESS));
            });
            When("asking for build entries", () =>
            {
                try {
                    repository.Get(new AllSpecification<CIProject>()); 
                }
                catch (Exception e)
                {
                    thrownException = e;
                }
            });
            Then("assert no exception is thrown", () => Assert.IsNull(thrownException));
        }
    }
}