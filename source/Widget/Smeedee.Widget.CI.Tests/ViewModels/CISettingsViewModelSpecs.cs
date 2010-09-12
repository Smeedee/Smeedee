using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.Tests;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.Widget.CI.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;
using TinyMVVM.IoC;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.CI.Tests.ViewModels
{
    internal class CISettingsViewModelSpecs : ScenarioClass
    {
        private static CISettingsViewModel viewmodel;
        private static Mock<IRepository<CIServer>> serverRepo;
        private static Mock<IRepository<Configuration>> configRepo;
        private static Mock<CIProject> projectMock = new Mock<CIProject>();

        [Test]
        public void assure_wraps_two_servers_when_given_two_servers()
        {
            Given(viewmodel_is_instantiated);
            When(viewmodel_is_given_two_servers_but_no_projects);
            Then(viewmodel_wraps_two_servers);
        }

        [Test]
        public void assure_returns_empty_project_list_not_null_when_no_projects()
        {
            Given(viewmodel_is_instantiated);
            When(viewmodel_is_given_two_servers_but_no_projects);
            Then(viewmodel_GetSelectedProjects_should_return_empty_list);
        }

        [Test]
        public void assure_returns_empty_project_list_not_null_when_no_servers()
        {
            Given(viewmodel_is_instantiated);
            When(viewmodel_is_given_no_servers);
            Then(viewmodel_GetSelectedProjects_should_return_empty_list);
        }

        [Test]
        public void assure_project_with_no_builds_gets_placeholder_build()
        {
            Given(viewmodel_is_instantiated);
            When(viewmodel_is_given_one_project_with_no_builds);
            Then(viewmodel_add_build_method_of_first_project_has_been_called);
        }

        [Test]
        public void assure_viewmodel_has_InactiveProjectThresholdProperty()
        {
            Given(viewmodel_is_instantiated);
            When("");
            Then(viewmodel_should_have_a_active_threshold_property);
        }

        [Test]
        public void assure_wraps_two_projects_when_given_two_projects()
        {
            Given(viewmodel_is_instantiated);
            When(viewmodel_is_given_one_server_with_two_projects);
            Then(viewmodel_wraps_two_projects_in_first_server);
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        } 

        private static void MockProject()
        {
            projectMock.Setup(r => r.SystemId).Returns("fakeID");
        }

        private Context viewmodel_is_instantiated = () =>
        {
            serverRepo = new Mock<IRepository<CIServer>>();
            configRepo = new Mock<IRepository<Configuration>>();
            var configPersister = new Mock<IPersistDomainModelsAsync<Configuration>>();
            var widgetMock = new Mock<Client.Framework.ViewModel.Widget>();
            var progressbarMock = new Mock<IProgressbar>();

            viewmodel = new CISettingsViewModel(serverRepo.Object, configRepo.Object, configPersister.Object, widgetMock.Object, progressbarMock.Object);
			viewmodel.ConfigureDependencies(config =>
			{
				config.Bind<IUIInvoker>().To<NoUIInvokation>();
			});
        };

        private When viewmodel_is_given_two_servers_but_no_projects = () =>
        {
            MockProject();
            var servers = new List<CIServer>()
                           {
                               new CIServer("server1", ""),
                               new CIServer("server2", "")
                           };
            serverRepo.Setup(r => r.Get(It.IsAny<Specification<CIServer>>())).Returns(servers);
        };

        private When viewmodel_is_given_one_server_with_two_projects = () =>
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
            serverRepo.Setup(r => r.Get(It.IsAny<Specification<CIServer>>())).Returns(servers);
        };
        
        private When viewmodel_is_given_no_servers = () =>
        {
            MockProject();
            var servers = new List<CIServer>();
            serverRepo.Setup(r => r.Get(It.IsAny<Specification<CIServer>>())).Returns(servers);
        };

        private When viewmodel_is_given_one_project_with_no_builds = () =>
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
            serverRepo.Setup(r => r.Get(It.IsAny<Specification<CIServer>>())).Returns(servers);
        };

        private Then viewmodel_GetSelectedProjects_should_return_empty_list = () =>
        {
            var result = viewmodel.GetSelectedAndActiveProjects();
            result.ShouldNotBe(null);
            result.Count().ShouldBe(0);
        };

        private Then viewmodel_wraps_two_servers = () =>
        {
            viewmodel.GetSelectedAndActiveProjects();
            viewmodel.Servers.Count().ShouldBe(2);
        };

        private Then viewmodel_wraps_two_projects_in_first_server = () =>
        {
            viewmodel.GetSelectedAndActiveProjects();
            viewmodel.Servers.First().Projects.Count().ShouldBe(2);
        };

        private Then viewmodel_add_build_method_of_first_project_has_been_called = () =>
        {
            viewmodel.GetSelectedAndActiveProjects();
            viewmodel.Servers.First().Projects.Count().ShouldBe(1);
            projectMock.Verify(p => p.AddBuild(It.IsAny<Build>()), Times.Once());
        };

        private Then viewmodel_should_have_a_active_threshold_property = () =>
        {
            var prev = viewmodel.InactiveProjectThreshold;
            viewmodel.InactiveProjectThreshold = prev;
            //if this compiles, test has passed
        };
    }
}
