#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using APD.Client.Framework;
using APD.Client.Framework.Commands;
using APD.Client.Framework.Settings;
using APD.Client.Framework.ViewModels;
using APD.Client.Widget.CI.Controllers;
using APD.DomainModel.CI;
using APD.DomainModel.Framework.Logging;
using APD.DomainModel.Users;
using APD.Tests.SpecialMocks;
using Moq;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.DomainModel.Framework;


namespace APD.Client.Widget.CI.Tests.CIControllerSpecs
{
    public class Shared
    {
        protected static CIController Controller;

        protected static Mock<IRepository<CIServer>> RepositoryMock;
        protected static Mock<IRepository<User>> UserRepoMock;
        protected static Mock<IClientSettingsReader> SettingsReaderMock;

        protected static DateTime MockStartTime = DateTime.Now;
        protected static DateTime MockEndTime = DateTime.Now.AddMinutes(30);

        protected static Mock<FreezeViewCommandPublisher> FreezeViewCmdPublisher = 
            new IEventAggregatorDependantMock<FreezeViewCommandPublisher>();

        protected static Mock<UnFreezeViewCommandPublisher> UnFreezeViewCmdPublisher = 
            new IEventAggregatorDependantMock<UnFreezeViewCommandPublisher>();

        protected static IInvokeBackgroundWorker<IEnumerable<CIProject>> BackgroundWorkerInvoker =
            new NoBackgroundWorkerInvocation<IEnumerable<CIProject>>();



        protected Context there_are_active_projects_in_CI_tool = () =>
        {
            RepositoryMock = new Mock<IRepository<CIServer>>();

            var normalBuild = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                                          new CodeModifiedTrigger("tuxbear"));

            var systemBuild = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                                          new EventTrigger("Trigger test"));

            var unknownBuild = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                                           new UnknownTrigger());

            var nonExistingUserBuild = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                                                   new CodeModifiedTrigger("I_DONT_EXIST"));

            var projects = new List<CIProject>();

            projects.Add(new CIProject("Project 1") { Builds = new List<Build> { normalBuild } });
            projects.Add(new CIProject("Project 2") { Builds = new List<Build> { systemBuild } });
            projects.Add(new CIProject("Project 3") { Builds = new List<Build> { unknownBuild } });
            projects.Add(new CIProject("Project 4") { Builds = new List<Build> { nonExistingUserBuild } });

            var servers = new List<CIServer>();
            servers.Add(new CIServer("What the fuck", "http://www.whatthefuck.com"));
            projects.ForEach(p => 
                servers.First().AddProject(p));

            RepositoryMock.Setup(r => r.Get(It.IsAny<AllSpecification<CIServer>>())).Returns(servers);
        };

        protected Context there_are_active_projects_in_CI_tool_with_different_BuildStatuses = () =>
        {
            RepositoryMock = new Mock<IRepository<CIServer>>();

            var normalBuild1 = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                                          new CodeModifiedTrigger("tuxbear"));

            var normalBuild2 = CreateBuild(DomainModel.CI.BuildStatus.FinishedWithFailure,
                              new CodeModifiedTrigger("tuxbear"));

            var normalBuild3 = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                              new CodeModifiedTrigger("tuxbear"));

            var systemBuild = CreateBuild(DomainModel.CI.BuildStatus.FinishedWithFailure,
                                          new EventTrigger("Trigger test"));

            var unknownBuild = CreateBuild(DomainModel.CI.BuildStatus.Building,
                                           new UnknownTrigger());

            var nonExistingUserBuild = CreateBuild(DomainModel.CI.BuildStatus.Unknown,
                                                   new CodeModifiedTrigger("I_DONT_EXIST"));

            var projects = new List<CIProject>();

            projects.Add(new CIProject("Project 1") { Builds = new List<Build> { normalBuild1 } });
            projects.Add(new CIProject("Project 2") { Builds = new List<Build> { systemBuild } });
            projects.Add(new CIProject("Project 3") { Builds = new List<Build> { unknownBuild } });
            projects.Add(new CIProject("Project 4") { Builds = new List<Build> { nonExistingUserBuild } });
            projects.Add(new CIProject("Project 5") { Builds = new List<Build> { normalBuild2 } });
            projects.Add(new CIProject("Project 6") { Builds = new List<Build> { normalBuild3 } });

            var servers = new List<CIServer>();
            servers.Add(new CIServer("What the fuck", "http://www.whatthefuck.com"));
            projects.ForEach(p =>
                servers.First().AddProject(p));

            RepositoryMock.Setup(r => r.Get(It.IsAny<AllSpecification<CIServer>>())).Returns(servers);
        };

        protected Context there_are_active_and_inactive_Projects_in_CI_tool = () =>
        {
            RepositoryMock = new Mock<IRepository<CIServer>>();

            var projects = new List<CIProject>();
            var normalBuild = CreateBuild(DomainModel.CI.BuildStatus.FinishedSuccefully,
                              new CodeModifiedTrigger("tuxbear"));
            projects.Add(new CIProject("Project 1") { Builds = new List<Build> { normalBuild } });

            var inactiveBUild = new Build()
            {
                StartTime = DateTime.Now.AddDays(-91),
                FinishedTime = DateTime.Now.AddDays(-91).AddMinutes(10),
                Status = APD.DomainModel.CI.BuildStatus.FinishedWithFailure,
                Trigger = new CodeModifiedTrigger("goeran")
            };
            projects.Add(new CIProject("Project 1 main trunk") {Builds = new List<Build>{ inactiveBUild}});

            var servers = new List<CIServer>();
            servers.Add(new CIServer("What the fuck", "http://www.whatthefuck.com"));
            projects.ForEach(p =>
                servers.First().AddProject(p));

            RepositoryMock.Setup(r => r.Get(It.IsAny<AllSpecification<CIServer>>())).Returns(servers);
        };

        protected Context user_exist_in_userdb = () =>
        {
            UserRepoMock = new Mock<IRepository<User>>();

            var normalUser = new User()
            {
                Email = "tuxbear@start.no",
                ImageUrl = "http://www.blah.com",
                Firstname = "Ole Andre",
                Username = "tuxbear"
            };

            var unknownUser = new User()
            {
                Firstname = "Unknown",
                Surname = "User",
                Username = "unknown",
                Email = "noemail",
                ImageUrl = "nourl"
            };

            var systemUser = new User()
            {
                Firstname = "System user",
                Username = "system",
                Email = "noemail",
                ImageUrl = "nourl"
            };

            UserRepoMock.Setup(r => r.Get(It.Is<UserByUsername>(s => s.Username.Equals("tuxbear")))).Returns(
                new List<User>() {normalUser});
            UserRepoMock.Setup(r => r.Get(It.Is<UserByUsername>(s => s.Username.Equals("unknown")))).Returns(
                new List<User>() {unknownUser});
            UserRepoMock.Setup(r => r.Get(It.Is<UserByUsername>(s => s.Username.Equals("system")))).Returns(
                new List<User>() {systemUser});
        };

        protected Context settings_are_configured = () =>
        {
            SettingsReaderMock = new Mock<IClientSettingsReader>();
            SettingsReaderMock.Setup(r => r.ReadSetting(It.Is<string>(s => s.Equals("CI_play_sound")))).Returns(true);
            SettingsReaderMock.Setup(r => r.ReadSetting(It.IsAny<string>())).Returns(null);
        };

        protected Context controller_is_created = () => { CreateController(); };

        protected When controller_is_spawned = () => { CreateController(); };


        private static void CreateController()
        {
            Controller = new CIController(RepositoryMock.Object,
                                          UserRepoMock.Object,
                                          new ManualNotifyRefresh(), 
                                          new NoUIInvocation(),
                                          FreezeViewCmdPublisher.Object,
                                          UnFreezeViewCmdPublisher.Object,
                                          new Mock<IVisibleModule>().Object,
                                          SettingsReaderMock.Object,
                                          BackgroundWorkerInvoker,
                                          new Mock<ILog>().Object);
        }

        protected static Build CreateBuild(DomainModel.CI.BuildStatus status, Trigger trigger)
        {
            return new Build()
            {
                StartTime = MockStartTime,
                FinishedTime = MockEndTime,
                Status = status,
                Trigger = trigger
            };
        }
    }


    [TestFixture]
    public class When_spawned : Shared
    {
        [Test]
        public void should_query_for_projects()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb).
                         And(settings_are_configured);

                scenario.When(controller_is_spawned);

                scenario.Then("assure it query for all projects latest build", 
                    () => RepositoryMock.Verify(c => c.Get(It.IsAny<AllSpecification<CIServer>>())));
            });
        }

        [Test]
        public void Should_query_for_user()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb).
                         And(settings_are_configured);

                scenario.When(controller_is_spawned);

                scenario.Then("assure it can query for user in userdb repository", 
                    () => UserRepoMock.Verify( r => r.Get(It.Is<UserByUsername>(s => s.Username.Equals("tuxbear")))));
            });
        }

        [Test]
        public void Should_load_projects_into_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb).
                         And(settings_are_configured);

                scenario.When(controller_is_spawned);

                scenario.Then("assure projects are loaded into ViewModel", 
                    () => Controller.ViewModel.Data.Count.ShouldNotBe(0));
            });
        }

        [Test]
        public void Should_load_data_into_ProjectInfoViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb).
                         And(settings_are_configured);

                scenario.When(controller_is_spawned);

                scenario.Then("assure data is loaded into ProjectInfoViewModel",() =>
                {
                    var firstProject = Controller.ViewModel.Data.Single(p => p.ProjectName.Equals("Project 1"));
                    var latestBuild = firstProject.LatestBuild;

                    firstProject.ProjectName.ShouldBe("Project 1");
                    latestBuild.TriggerCause.ShouldBe("Code Modified");
                    latestBuild.TriggeredBy.Name.ShouldBe("Ole Andre");
                    latestBuild.StartTime.ShouldBe(MockStartTime);
                    latestBuild.FinishedTime.ShouldBe(MockEndTime);
                    latestBuild.Status.ShouldBe(BuildStatus.Successful);
                });
            });
               
        }
    }


    [TestFixture]
    public class When_successfully_queried_for_projectinfos : Shared
    {
        protected Context build_has_non_existing_user = () =>
        {
            UserRepoMock = new Mock<IRepository<User>>();
            UserRepoMock.Setup(r => r.Get(It.IsAny<Specification<User>>())).Returns(new List<User>());
        };


        [Test]
        public void assure_hasConnectionProblems_is_set_to_false()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb).
                         And(settings_are_configured).
                         And(controller_is_created);

                scenario.When("CIProject data successfully queried");

                scenario.Then("assure hasConnectionProblems is false", 
                    () => Controller.ViewModel.HasConnectionProblems.ShouldBeFalse());
            });
        }

        [Test]
        public void non_existing_username_should_return_person_with_username_set()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(build_has_non_existing_user).
                         And(there_are_active_projects_in_CI_tool).
                         And(settings_are_configured);

                scenario.When(controller_is_spawned);

                scenario.Then("non-exisiting user should return a person instance with username set",
                    () => Controller.ViewModel.Data.Single(p => p.ProjectName.Equals("Project 4"))
                                        .LatestBuild.TriggeredBy.Username.ShouldBe("I_DONT_EXIST"));
            });            
        }

        [Test]
        public void unknown_trigger_should_result_in_unknown_user_and_cause()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb).
                         And(settings_are_configured);

                scenario.When(controller_is_spawned);

                scenario.Then("cause and user should be unknown", () =>
                {
                    var buildWithUnknownTrigger = Controller.ViewModel.Data.Single(p => p.ProjectName.Equals("Project 3"));

                    buildWithUnknownTrigger.LatestBuild.TriggeredBy.Name.ShouldBe("Unknown User");
                    buildWithUnknownTrigger.LatestBuild.TriggerCause.ShouldBe("Unknown");
                });
            });
        }

        [Test]
        public void eventTrigger_should_result_in_system_user_and_event_cause()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb).
                         And(settings_are_configured);
                
                scenario.When(controller_is_spawned);

                scenario.Then("should return system user and cause", () =>
                {
                    var eventTriggeredProject = Controller.ViewModel.Data.Single(p => p.ProjectName.Equals("Project 2"));

                    eventTriggeredProject.LatestBuild.TriggeredBy.Name.ShouldBe("System user");
                    eventTriggeredProject.LatestBuild.TriggerCause.ShouldBe("Trigger test");
                });
            });
        }


        [Test]
        public void should_notify_when_one_or_more_build_fails()
        {
            //TODO write test
            Assert.IsTrue(true);
        }

        [Test]
        public void should_notify_when_all_buils_are_successful()
        {
            //TODO write test
            Assert.IsTrue(true);
        }

        [Test]
        public void Assure_inactive_Projects_are_excluded()
        {
            //Projects with latest build older than 90 days are considered 
            //inactive
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_active_and_inactive_Projects_in_CI_tool).
                         And(user_exist_in_userdb).
                         And(settings_are_configured);
                scenario.When(controller_is_spawned);
                scenario.Then("assure inactive projects are excluded", () =>
                {
                    var activeProjects = RepositoryMock.Object.Get(new AllSpecification<CIServer>()).
                        First().Projects.Where(p => p.LatestBuild.StartTime > DateTime.Now.AddDays(-90));
                    Controller.ViewModel.Data.Count().ShouldBe(activeProjects.Count());
                });
            });
        }

        [Test]
        public void Assure_Projects_are_sorted_by_LatestBuilds_Status()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_active_projects_in_CI_tool_with_different_BuildStatuses).
                    And(user_exist_in_userdb).
                    And(settings_are_configured);
                scenario.When(controller_is_spawned);
                scenario.Then("assure Projects are sorted by LatesBuild Status", () =>
                {
                    Controller.ViewModel.Data.Count.ShouldBe(6);
                    Controller.ViewModel.Data[0].LatestBuild.Status.ShouldBe(BuildStatus.Failed);
                    Controller.ViewModel.Data[1].LatestBuild.Status.ShouldBe(BuildStatus.Failed);
                    Controller.ViewModel.Data[2].LatestBuild.Status.ShouldBe(BuildStatus.Unknown);
                    Controller.ViewModel.Data[3].LatestBuild.Status.ShouldBe(BuildStatus.Building);
                    Controller.ViewModel.Data[4].LatestBuild.Status.ShouldBe(BuildStatus.Successful);
                    Controller.ViewModel.Data[5].LatestBuild.Status.ShouldBe(BuildStatus.Successful);
                });
            });
        }
    }

    [TestFixture]
    public class When_query_fails : Shared
    {
        [Test]
        public void hasConnectionProblems_should_be_true()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb).
                         And(settings_are_configured).
                         And("there are connection problems", () => 
                            RepositoryMock.Setup(
                                r => r.Get(It.IsAny<AllSpecification<CIServer>>())).Throws(new HttpException())
                         );
                
                scenario.When(controller_is_spawned);

                scenario.Then("HasConnections problems should be true", () => 
                    Controller.ViewModel.HasConnectionProblems.ShouldBeTrue());
            });
        }
    }
}