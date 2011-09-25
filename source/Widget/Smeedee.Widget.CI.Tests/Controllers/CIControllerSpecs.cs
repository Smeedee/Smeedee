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
// The project webpage is located at http://www.smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Users;
using Moq;
using NUnit.Framework;
using Smeedee.Widget.CI.Controllers;
using Smeedee.Widget.CI.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using Smeedee.DomainModel.Framework;
using TinyMVVM.Framework.Services;
using TinyMVVM.IoC;


namespace Smeedee.Client.Widget.CI.Tests.CIControllerSpecs
{
    public class Shared : ScenarioClass
    {
        protected static CIController controller;
        protected static CISettingsViewModel settingsViewModel = new CISettingsViewModel();

        protected static Mock<IRepository<CIServer>> RepositoryMock;
        protected static Mock<IRepository<Configuration>> configRepoMock = new Mock<IRepository<Configuration>>();
        protected static Mock<IPersistDomainModelsAsync<Configuration>> configPersisterMock = new Mock<IPersistDomainModelsAsync<Configuration>>();
        protected static Mock<IRepository<User>> UserRepoMock;
        protected static Mock<IProgressbar> progressbarMock = new Mock<IProgressbar>();
        protected static Mock<ITimer> timermock = new Mock<ITimer>();
        protected static Mock<CIProject> projectMock;

        protected static DateTime MockStartTime = DateTime.Now;
        protected static DateTime MockEndTime = DateTime.Now.AddMinutes(30);


        protected static IInvokeBackgroundWorker<IEnumerable<CIProject>> BackgroundWorkerInvoker =
            new NoBackgroundWorkerInvocation<IEnumerable<CIProject>>();

        protected When controller_is_notified_to_refresh = () => timermock.Raise(n => n.Elapsed += null, new EventArgs());

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
                Status = Smeedee.DomainModel.CI.BuildStatus.FinishedWithFailure,
                Trigger = new CodeModifiedTrigger("goeran")
            };
            projects.Add(new CIProject("Project 1 main trunk") { Builds = new List<Build>{ inactiveBUild}});

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

        protected Context controller_is_created = CreateController;

        protected When controller_is_spawned = CreateController;


        private static void CreateController()
        {
            var viewModel = new CIViewModel();
            var settingsViewModel = new CISettingsViewModel();
        	settingsViewModel.ConfigureDependencies(config =>
        	{
        		config.Bind<IUIInvoker>().To<NoUIInvokation>();
        	});
            controller = new CIController(viewModel, 
                                          settingsViewModel,
                                          UserRepoMock.Object,
                                          RepositoryMock.Object,
                                          configRepoMock.Object,
                                          configPersisterMock.Object,
                                          new NoBackgroundWorkerInvocation<IEnumerable<CIProject>>(),
                                          timermock.Object,
                                          new NoUIInvokation(),
                                          new Mock<ILog>().Object,
                                          progressbarMock.Object);
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

        protected static void MockProject()
        {
            projectMock.Setup(r => r.SystemId).Returns("fakeID");
        }

        protected Context settingsViewmodel_is_instantiated = () =>
        {
            RepositoryMock = new Mock<IRepository<CIServer>>();
            configRepoMock = new Mock<IRepository<Configuration>>();
            configPersisterMock = new Mock<IPersistDomainModelsAsync<Configuration>>();
            progressbarMock = new Mock<IProgressbar>();

            var settingsViewModel = new CISettingsViewModel();
			settingsViewModel.ConfigureDependencies(config =>
			{
				config.Bind<IUIInvoker>().To<NoUIInvokation>();
			});
        };

        protected When viewmodel_is_given_two_servers_but_no_projects = () =>
        {
            MockProject();
            var servers = new List<CIServer>()
                           {
                               new CIServer("server1", ""),
                               new CIServer("server2", "")
                           };
            RepositoryMock.Setup(r => r.Get(It.IsAny<Specification<CIServer>>())).Returns(servers);
        };

        protected When viewmodel_is_given_one_server_with_two_projects = () =>
        {
            MockProject();
            var servers = new List<CIServer>()
                              {
                                  new CIServer("server1", "")
                                      {
                                          Projects = new List<CIProject>
                                                         {
                                                             projectMock.Object,
                                                             projectMock.Object
                                                         }
                                      }
                              };
            RepositoryMock.Setup(r => r.Get(It.IsAny<Specification<CIServer>>())).Returns(servers);
        };

        protected When viewmodel_is_given_no_servers = () =>
        {
            MockProject();
            var servers = new List<CIServer>();
            RepositoryMock.Setup(r => r.Get(It.IsAny<Specification<CIServer>>())).Returns(servers);
        };

        protected When viewmodel_is_given_one_project_with_no_builds = () =>
        {
            MockProject();
            var servers = new List<CIServer>()
                              {
                                new CIServer("server1", "")
                                    {
                                        Projects = new List<CIProject>
                                        {
                                            projectMock.Object
                                        }
                                    }
                              };
            RepositoryMock.Setup(r => r.Get(It.IsAny<Specification<CIServer>>())).Returns(servers);
        };

        protected Then viewmodel_GetSelectedProjects_should_return_empty_list = () =>
        {
            var result = controller.GetSelectedAndActiveProjects();
            result.ShouldNotBe(null);
            result.Count().ShouldBe(0);
        };

        protected Then viewmodel_wraps_two_servers = () =>
        {
            
            controller.GetSelectedAndActiveProjects();
             settingsViewModel.Servers.Count().ShouldBe(2);
        };

        protected Then viewmodel_wraps_two_projects_in_first_server = () =>
        {
            controller.GetSelectedAndActiveProjects();
            settingsViewModel.Servers.First().Projects.Count().ShouldBe(2);
        };

        protected Then settingsViewModel_add_build_method_of_first_project_has_been_called = () =>
        {
            controller.GetSelectedAndActiveProjects();
            settingsViewModel.Servers.First().Projects.Count().ShouldBe(1);
            projectMock.Verify(p => p.AddBuild(It.IsAny<Build>()), Times.Once());
        };

        protected Then settingsViewModel_should_have_a_active_threshold_property = () =>
        {
            settingsViewModel.InactiveProjectThreshold.ShouldNotBeNull();
        };



        [SetUp]
        public void Setup()
        {
            progressbarMock = new Mock<IProgressbar>();
        }
    }


    [TestFixture]
    public class When_spawned_2 : Shared
    {
        [Test]
        public void should_query_for_projects()
        {
                Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb);

                When(controller_is_spawned);

                Then("assure it query for all projects latest build", 
                    () => RepositoryMock.Verify(c => c.Get(It.IsAny<AllSpecification<CIServer>>())));
        }

        [Test]
        public void Should_query_for_user()
        {
                Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb);

                When(controller_is_spawned);

                Then("assure it can query for user in userdb repository", 
                    () => UserRepoMock.Verify( r => r.Get(It.Is<UserByUsername>(s => s.Username.Equals("tuxbear")))));
        }

        [Test]
        public void Should_load_projects_into_ViewModel()
        {
                Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb);

                When(controller_is_spawned);

                Then("assure projects are loaded into ViewModel", 
                    () => controller.ViewModel.Data.Count.ShouldNotBe(0));
        }

        [Test]
        public void Should_load_data_into_ProjectInfoViewModel()
        {
                Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb);

                When(controller_is_spawned);

                Then("assure data is loaded into ProjectInfoViewModel",() =>
                {
                    var firstProject = controller.ViewModel.Data.Single(p => p.ProjectName.Equals("Project 1"));
                    var latestBuild = firstProject.LatestBuild;

                    firstProject.ProjectName.ShouldBe("Project 1");
                    latestBuild.TriggerCause.ShouldBe("Code Modified");
                    latestBuild.TriggeredBy.Name.ShouldBe("Ole Andre");
                    latestBuild.StartTime.ShouldBe(MockStartTime);
                    latestBuild.FinishedTime.ShouldBe(MockEndTime);
                    latestBuild.Status.ShouldBe(Smeedee.Widget.CI.BuildStatus.Successful);
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

        protected Context inactivity_threshold_is_90_days = () =>
        {
            var config = new Configuration("CIWidgetSettings");
            config.NewSetting("FilterInactiveProjects", "True");
            config.NewSetting("InactiveProjectThreshold", "90");
            configRepoMock.Setup(r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(new List<Configuration> { config });
        };

        [Test]
        public void assure_hasConnectionProblems_is_set_to_false()
        {
                Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb).
                         And(controller_is_created);

                When("CIProject data successfully queried");

                Then("assure hasConnectionProblems is false", 
                    () => controller.ViewModel.HasConnectionProblems.ShouldBeFalse());
        }

        [Test]
        public void non_existing_username_should_return_person_with_username_set()
        {
                Given(build_has_non_existing_user).
                         And(there_are_active_projects_in_CI_tool);

                When(controller_is_spawned);

                Then("non-exisiting user should return a person instance with username set",
                    () => controller.ViewModel.Data.Single(p => p.ProjectName.Equals("Project 4"))
                                        .LatestBuild.TriggeredBy.Username.ShouldBe("I_DONT_EXIST"));
        }

        [Test]
        public void unknown_trigger_should_result_in_unknown_user_and_cause()
        {
                Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb);

                When(controller_is_spawned);

                Then("cause and user should be unknown", () =>
                {
                    var buildWithUnknownTrigger = controller.ViewModel.Data.Single(p => p.ProjectName.Equals("Project 3"));

                    buildWithUnknownTrigger.LatestBuild.TriggeredBy.Name.ShouldBe("Unknown User");
                    buildWithUnknownTrigger.LatestBuild.TriggerCause.ShouldBe("Unknown");
                });
        }

        [Test]
        public void eventTrigger_should_result_in_system_user_and_event_cause()
        {
                Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb);
                
                When(controller_is_spawned);

                Then("should return system user and cause", () =>
                {
                    var eventTriggeredProject = controller.ViewModel.Data.Single(p => p.ProjectName.Equals("Project 2"));

                    eventTriggeredProject.LatestBuild.TriggeredBy.Name.ShouldBe("System user");
                    eventTriggeredProject.LatestBuild.TriggerCause.ShouldBe("Trigger test");
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
                Given(there_are_active_and_inactive_Projects_in_CI_tool)
                    .And(user_exist_in_userdb)
                    .And(inactivity_threshold_is_90_days);

                When(controller_is_spawned);

                Then("assure inactive projects are excluded", () =>
                {
                    var activeProjects = RepositoryMock.Object.Get(new AllSpecification<CIServer>()).
                        First().Projects.Where(p => p.LatestBuild.StartTime > DateTime.Now.AddDays(-90));
                    controller.ViewModel.Data.Count().ShouldBe(activeProjects.Count());
                });
        }

        [Test]
        public void Assure_Projects_are_sorted_by_LatestBuilds_Status()
        {
                Given(there_are_active_projects_in_CI_tool_with_different_BuildStatuses).
                    And(user_exist_in_userdb);

                When(controller_is_spawned);

                Then("assure Projects are sorted by LatesBuild Status", () =>
                {
                    controller.ViewModel.Data.Count.ShouldBe(6);
                    controller.ViewModel.Data[0].LatestBuild.Status.ShouldBe(Smeedee.Widget.CI.BuildStatus.Failed);
                    controller.ViewModel.Data[1].LatestBuild.Status.ShouldBe(Smeedee.Widget.CI.BuildStatus.Failed);
                    controller.ViewModel.Data[2].LatestBuild.Status.ShouldBe(Smeedee.Widget.CI.BuildStatus.Unknown);
                    controller.ViewModel.Data[3].LatestBuild.Status.ShouldBe(Smeedee.Widget.CI.BuildStatus.Building);
                    controller.ViewModel.Data[4].LatestBuild.Status.ShouldBe(Smeedee.Widget.CI.BuildStatus.Successful);
                    controller.ViewModel.Data[5].LatestBuild.Status.ShouldBe(Smeedee.Widget.CI.BuildStatus.Successful);
                });
        }
    }

    [TestFixture]
    public class When_query_fails : Shared
    {
        [Test]
        public void hasConnectionProblems_should_be_true()
        {
                Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb).
                         And("there are connection problems", () => 
                            RepositoryMock.Setup(
                                r => r.Get(It.IsAny<AllSpecification<CIServer>>())).Throws(new HttpException())
                         );
                
                When(controller_is_spawned);

                Then("HasConnections problems should be true", () => 
                    controller.ViewModel.HasConnectionProblems.ShouldBeTrue());
        }
    }

    [TestFixture]
    public class When_loading_data : Shared
    {

        [Test]
        public void Assure_progressbar_is_shown_while_loading_data()
        {
                Given(there_are_active_projects_in_CI_tool).
                         And(user_exist_in_userdb).
                         And(controller_is_created);

                When(controller_is_notified_to_refresh);

                Then("the loadingNotifyer should be shown during spawn and on refresh", () =>
                {
                    progressbarMock.Verify(l => l.ShowInView(It.IsAny<string>()), Times.Exactly(2));
                    //Controller.ViewModel.IsLoading.ShouldBe(true); when async get is implemented
            });
        }

        [Test]
        public void Assure_progressbar_is_hidden_after_loading_data()
        {
                Given(there_are_active_projects_in_CI_tool).
                    And(user_exist_in_userdb);

                When(controller_is_spawned);

                Then("", () =>
                {
                    progressbarMock.Verify(l => l.HideInView(), Times.Once());
                    controller.ViewModel.IsLoading.ShouldBe(false);
                });
        }
    }

    [TestFixture]
    public class When_settingsViewModel_is_instansiated : Shared
    {
        private static CISettingsViewModel viewmodel;
        private static Mock<IRepository<CIServer>> serverRepo;
        private static Mock<IRepository<Configuration>> configRepo;
        private static Mock<CIProject> projectMock = new Mock<CIProject>();

        [Test]
        public void assure_wraps_two_servers_when_given_two_servers()
        {
            Given(settingsViewmodel_is_instantiated);
            When(viewmodel_is_given_two_servers_but_no_projects);
            Then(viewmodel_wraps_two_servers);
        }

        [Test]
        public void assure_returns_empty_project_list_not_null_when_no_projects()
        {
            Given(settingsViewmodel_is_instantiated);
            When(viewmodel_is_given_two_servers_but_no_projects);
            Then(viewmodel_GetSelectedProjects_should_return_empty_list);
        }

        [Test]
        public void assure_returns_empty_project_list_not_null_when_no_servers()
        {
            Given(settingsViewmodel_is_instantiated);
            When(viewmodel_is_given_no_servers);
            Then(viewmodel_GetSelectedProjects_should_return_empty_list);
        }

        [Test]
        public void assure_project_with_no_builds_gets_placeholder_build()
        {
            Given(settingsViewmodel_is_instantiated);
            When(viewmodel_is_given_one_project_with_no_builds);
            Then(settingsViewModel_add_build_method_of_first_project_has_been_called);
        }

        [Test]
        public void assure_viewmodel_has_InactiveProjectThresholdProperty()
        {
            Given(settingsViewmodel_is_instantiated);
            When("");
            Then(settingsViewModel_should_have_a_active_threshold_property);
        }

        [Test]
        public void assure_wraps_two_projects_when_given_two_projects()
        {
            Given(settingsViewmodel_is_instantiated);
            When(viewmodel_is_given_one_server_with_two_projects);
            Then(viewmodel_wraps_two_projects_in_first_server);
        }
    }
}