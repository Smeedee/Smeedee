using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using Smeedee.Integration.VCS.TFSVC.DomainModel.Services;

using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.VCS.TFSVC.DomainModel.Services.TFSCredentialsCheckerSpecs
{
    public class Shared : ScenarioClass
    {
        protected const string SVN = "svn";
        protected const string CORRECT_USERNAME = "smeedee";
        protected const string CORRECT_PASSWORD = "dlog4321.";
        protected const string REPOSITORY_URL = "http://80.203.160.221:8080/tfs";
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
            repositoryUrl = REPOSITORY_URL;
        };

        protected Context Credentials_are_valid = () =>
        {
            username = CORRECT_USERNAME;
            password = CORRECT_PASSWORD;
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

    [TestFixture][Category("IntegrationTest")]
    [Ignore("TFS Server is down")]
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
                credentialsChecker.Check(SVN, repositoryUrl, CORRECT_USERNAME, CORRECT_PASSWORD).ShouldBeTrue();
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
