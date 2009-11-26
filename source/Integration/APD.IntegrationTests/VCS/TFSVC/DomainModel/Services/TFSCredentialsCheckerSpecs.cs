using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using APD.Integration.VCS.TFSVC.DomainModel.Services;

using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.VCS.TFSVC.DomainModel.Services.TFSCredentialsCheckerSpecs
{
    public class Shared : ScenarioClass
    {
        protected const string SVN = "svn";
        protected static string repositoryUrl;
        protected static string username;
        protected static string password;
        protected static TFSCredentialsChecker credentialsChecker;

        protected Context CredentialsChecker_is_created = () =>
        {
            credentialsChecker = new TFSCredentialsChecker();
        };

        protected Context Repository_URL_is_valid = () =>
        {
            repositoryUrl = "https://tfs08.codeplex.com";
        };

        protected Context Credentials_are_valid = () =>
        {
            username = "smeedee_cp";
            password = "haldis.";
        };

        protected Context Credentials_are_invalid = () =>
        {
            username = "haldis1337";
            password = "haldis_is_leet";
        };

        protected When testing_credentials = () =>
        {

        };

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }

    [TestFixture]
    //[Ignore]
    public class When_testing_Credentials : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("Testing TFS credentials");
            Given(CredentialsChecker_is_created).
                And(Repository_URL_is_valid);

            When(testing_credentials);
        }

        [Test]
        public void Assure_valid_Credentials_return_true()
        {
            Then("assure valid Credentials return true", () =>
            {
                credentialsChecker.Check(SVN, repositoryUrl, "smeedee_cp", "haldis").ShouldBeTrue();
            });
        }

        [Test]
        public void Assure_invalid_Credentials_return_false()
        {
            Then("assure invalid Credentials return false", () =>
                credentialsChecker.Check(SVN, repositoryUrl, "haldis1337", "haldis_leet").ShouldBeFalse());
        }
    }
}
