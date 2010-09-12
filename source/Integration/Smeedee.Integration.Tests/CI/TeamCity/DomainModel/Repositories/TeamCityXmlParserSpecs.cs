using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.CI;
using Smeedee.Integration.CI.TeamCity.DomainModel.Repositories;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TestExtensions = TinyBDD.Specification.NUnit.TestExtensions;

namespace Smeedee.Integration.Tests.CI.TeamCity.DomainModel.Repositories
{
    [TestFixture][Category("IntegrationTest")]
    public class TeamCityXmlParserSpecs : ScenarioClass
    {
        private static TeamCityXmlParser _xmlParser;
        private static IEnumerable<CIServer> _servers;
        private static Mock<IFetchTeamCityXml> _xmlFetcher;
        private static IEnumerable<CIProject> _projects;
        private static IEnumerable<Build> _builds;

        private Context an_XML_parser_exists = () =>
        {
            _xmlFetcher = new Mock<IFetchTeamCityXml>();
            _xmlFetcher.Setup(o => o.GetTeamCityProjects()).Returns(TeamCityResources.GetProjects);
            _xmlFetcher.Setup(o => o.GetTeamCityBuildConfigurations(It.IsAny<string>())).Returns(TeamCityResources.GetBuildconfigurations);
            _xmlFetcher.Setup(o => o.GetTeamCityBuilds(It.IsAny<string>())).Returns(TeamCityResources.GetBuilds);
            _xmlFetcher.Setup(o => o.GetChanges(It.IsAny<string>())).Returns(TeamCityResources.GetChanges);
            _xmlFetcher.Setup(o => o.GetChangesetInfo(It.IsAny<string>())).Returns(TeamCityResources.GetChangesetInfo);
            _xmlFetcher.Setup(o => o.GetBuildInfo(It.IsAny<string>())).Returns(TeamCityResources.GetBuildinfo);
            _xmlParser = new TeamCityXmlParser(_xmlFetcher.Object);
        };

        private Context there_are_no_changesets_in_the_newest_build = () =>
        {
            _xmlFetcher.Setup(o => o.GetChanges("88")).Returns(TeamCityResources.GetChangesNoChanges);
        };

        private readonly When we_ask_for_a_list_of_projects = () =>
        {
            _servers = _xmlParser.GetTeamCityProjects();
        };

        private When we_ask_for_a_list_of_build_configurations = () =>
        {
            _projects = _xmlParser.GetTeamCityBuildConfigurations("fake string");
        };

        private When we_ask_for_a_list_of_builds = () =>
        {
            _builds = _xmlParser.GetTeamCityBuilds("fake string");
        };

        [Test]
        public void Should_retrieve_the_correct_amount_of_servers_which_are_called_projects_in_team_city()
        {
            Scenario("When parsing projects, should return the correct amount");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_projects);
            Then("It should have the same amount as the raw XML data", 
                () => _servers.Count().ShouldBe(2));
        }

        [Test]
        public void Server_should_have_correct_url()
        {
            Scenario("When parsing projects, their corresponding domain servers should have the correct URL");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_projects);
            Then("It's members should have the correct urls", 
                () => _servers.ElementAt(0).Url.ShouldBe("/httpAuth/app/rest/projects/id:project2"));
        }

        [Test]
        public void Server_should_have_correct_name()
        {
            Scenario("When parsing projects, their corresponding domain servers should have the correct name");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_projects);
            Then("It's members should have the correct urls",
                () => _servers.ElementAt(0).Name.ShouldBe("Smeedee"));
        }

        [Test]
        public void Should_ask_the_xml_fetcher_once_for_projects()
        {
            Scenario("When parsing projects, should ask xml fetcher once for projects");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_projects);
            Then("It should ask the XML fetcher for data, exactly once.", 
                () => _xmlFetcher.Verify(o => o.GetTeamCityProjects(), Times.Once()));
        }

        [Test]
        public void Should_retrieve_the_correct_amount_of_buildconfigs_aka_projects()
        {
            Scenario("When parsing buildconfigurations, should return the correct amount");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_build_configurations);
            Then("It should have the same amount as the raw XML data", 
                () => _projects.Count().ShouldBe(4));
        }

        [Test]
        public void Projects_should_have_correct_name()
        {
            Scenario("When parsing buildconfigurations, they should have the correct name");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_build_configurations);
            Then("They should have the correct name",
                () => _projects.First().ProjectName.ShouldBe("Summer of Code 2010 - Oslo"));
        }

        [Test]
        public void Projects_should_have_correct_system_id()
        {
            Scenario("When parsing buildconfigurations, they should have the correct system id");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_build_configurations);
            Then("They should have the correct system id",
                () => _projects.First().SystemId.ShouldBe("bt5"));
        }

        [Test]
        public void Should_retrieve_the_correct_amount_of_builds()
        {
            Scenario("When parsing builds, should return the correct amount");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_builds);

            Then("It should have the same amount as the raw XML data", 
                () => _builds.Count().ShouldBe(25));
        }

        [Test]
        public void Should_ask_the_xml_fetcher_once_for_buildconfigs()
        {
            Scenario("When parsing buildconfigurations, should ask xml fetcher once for build configurations");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_build_configurations);
            Then("It should ask the XML fetcher for data, exactly once",
                 () => _xmlFetcher.Verify(o => o.GetTeamCityBuildConfigurations(It.IsAny<string>()), Times.Once()));
        }

        [Test]
        public void Should_get_the_correct_build_status_for_failed_builds()
        {
            Scenario("When retrieving build info for failed builds");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_builds);
            Then("Builds with failed exit status should be marked as such",
                () => _builds.ElementAt(0).Status.ShouldBe(BuildStatus.FinishedWithFailure));
        }

        [Test]
        public void Should_get_the_correct_build_status_for_successful_builds()
        {
            Scenario("When retrieving build info for successful builds");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_builds);
            Then("Builds with successful exit status should be marked as such", 
                () => _builds.ElementAt(1).Status.ShouldBe(BuildStatus.FinishedSuccefully));
        }

        [Test]
        public void Should_have_systemId_set_to_buildId()
        {
            Scenario("When retrieving build info");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_builds);
            Then("Builds should have their systemId set to TeamCity's buildId",
                () => _builds.ElementAt(0).SystemId.ShouldBe("88"));
        }

        [Test]
        public void Should_have_system_user_as_invoker_if_no_changesets_exist()
        {
            Scenario("When retrieving build info");

            Given(an_XML_parser_exists).
            And(there_are_no_changesets_in_the_newest_build);
            When(we_ask_for_a_list_of_builds);
            Then("The newest build should have it's invoker set to system user",
                 () => _builds.ElementAt(0).Trigger.InvokedBy.ShouldBe("System"));
                      
        }

        [Test]
        public void Should_have_latest_changesets_committer_as_invoker_if_at_least_one_changeset_exist()
        {
            Scenario("When retrieving build info");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_builds);
            Then("The newest build should have it's invoker set to the correct user",
                 () => _builds.ElementAt(0).Trigger.InvokedBy.ShouldBe("d.nyvik"));

        }

        [Test]
        public void Should_have_correct_start_date_on_build()
        {
            Scenario("When retrieving build info");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_builds);
            Then("The newest build should have it's start date set correctly",
                 () => _builds.ElementAt(0).StartTime.ShouldBe(new DateTime(2010, 06, 26, 23, 56, 50)));

        }

        [Test]
        public void Should_have_correct_end_date_on_build()
        {
            Scenario("When retrieving build info");

            Given(an_XML_parser_exists);
            When(we_ask_for_a_list_of_builds);
            Then("The newest build should have it's start date set correctly",
                 () => _builds.ElementAt(0).FinishedTime.ShouldBe(new DateTime(2010, 06, 26, 23, 59, 41)));

        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }
}