using APD.Integration.CI.CruiseControl.DomainModel.Repositories;
using APD.Integration.CI.Hudson.DomainModel.Repositories;
using APD.Integration.CI.Hudson.DomainModel.Repositories.Data;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.CI.Hudson.Tests.DomainModel.Repositories
{
    [TestFixture]
    public class When_connecting_to_server
    {
        [Test]
        public void should_return_null_when_server_is_unavailable()
        {
            IRequestProxy requestProxy = new RequestProxy("http://localhost:9999");
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("Hudson server is down");

                scenario.When("trying to fetch root");

                scenario.Then("an empty root object will be returned", () =>
                {
                    Root root = requestProxy.Execute<Root>("api/json");
                    root.ShouldBeNull();
                });
            });
        }

        [Test]
        [ExpectedException(typeof(HudsonRepositoryException))]
        public void exception_should_be_thrown_when_specifying_empty_url()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("an empty url is specified");

                scenario.When("trying to create a RequestProxy");

                scenario.Then("an exception should be thrown", () =>
                {
                    IRequestProxy requestProxy = new RequestProxy("");
                });
            });
        }

    }
}
