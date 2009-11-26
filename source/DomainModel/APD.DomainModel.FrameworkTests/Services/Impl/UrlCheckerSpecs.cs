using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Framework.Services.Impl;

using TinyBDD.Dsl.GivenWhenThen;
using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace APD.DomainModel.FrameworkTests.Services.Impl.UrlCheckerSpecs
{
    public class Shared : ScenarioClass
    {
        protected static UrlChecker urlChecker;
        protected static string url;

        protected Context UrlChecker_is_Created = () =>
        {
            urlChecker = new UrlChecker();
        };

        protected Context url_exists = () =>
        {
            url = "https://smeedee.org:8443/svn";
        };

        protected Context url_does_not_exists = () =>
        {
            url = "http://smeedeedoesnotexists.com";
        };

        protected When checking_url = () =>
        {

        };

        [TearDown]
        public void TearDown()
        {
            //Run();
        }
    }

    [TestFixture]
    public class When_url_exists : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("URL exists");
            Given(url_exists).
                And(UrlChecker_is_Created);

            When(checking_url);
        }

        [Test]
        public void Assure_Check_returns_true()
        {
            Then("assure Check returns true", () =>
                urlChecker.Check(url).ShouldBeTrue());
        }
    }

    [TestFixture]
    public class When_url_doest_not_exists : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("URL does not exists");
            Given(url_does_not_exists).
                And(UrlChecker_is_Created);

            When(checking_url);
        }

        [Test]
        public void Assure_Check_returns_false()
        {
            Then("assure Check returns false", () =>
                urlChecker.Check(url).ShouldBeFalse());
        }
    }
}
