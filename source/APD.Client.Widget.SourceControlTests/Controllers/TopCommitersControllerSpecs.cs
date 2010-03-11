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
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using APD.Client.Framework;
using APD.Client.Widget.SourceControl.Controllers;
using APD.DomainModel.Config;
using APD.DomainModel.Framework.Logging;
using APD.DomainModel.SourceControl;
using APD.DomainModel.Users;

using Moq;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.DomainModel.Framework;

namespace APD.Client.Widget.SourceControlTests.Controllers.TopCommitersControllerSpecs
{
    public class Shared
    {
        protected static int changesetRepositoryGetThreadId;
        protected static Mock<IRepository<Changeset>> changesetRepositoryMock;
        protected static TopCommitersController controller;
        protected static Mock<INotifyWhenToRefresh> iNotifyWhenToRefreshMock;
        protected static Mock<IRepository<User>> userRepositoryMock;
        protected static Mock<ILog> logger = new Mock<ILog>();
        protected static Mock<IPersistDomainModels<Configuration>> configPersisterRepositoryMock =
                new Mock<IPersistDomainModels<Configuration>>();  
        protected static Mock<IRepository<Configuration>> configRepositoryMock =
            new Mock<IRepository<Configuration>>(); 


        private static void SetupConfigRepositoryMock(int timespan)
        {
            configRepositoryMock = new Mock<IRepository<Configuration>>();
            var configuration = new Configuration("Commit Heroes Slide");
            configuration.NewSetting("from date", "");
            configuration.NewSetting("timespan", timespan.ToString());
            var configList = new List<Configuration> { configuration };

            configRepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        }

        protected Context Configuration_timepan_entry_is_correctly_setup_for_one_day = () =>
        {
            SetupConfigRepositoryMock(1);
        };

        protected Context Configuration_timespan_entry_is_correctly_setup_for_2_days = () =>
        {
            SetupConfigRepositoryMock(2);
        };


        protected Context Configuration_timespan_entry_is_changed_from_2_to_1 = () =>
        {
            var configuration = new Configuration("Commit Heroes Slide");
            configuration.NewSetting("from date", "");
            configuration.NewSetting("timespan", "1");
            var configList = new List<Configuration> { configuration };

            configRepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        };

        protected Context Configuration_has_valid_from_date = () =>
        {
            var configuration = new Configuration("Commit Heroes Slide");
            configuration.NewSetting("from date", "2009-12-30");
            configuration.NewSetting("timespan", "1");
            var configList = new List<Configuration> { configuration };

            configRepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        };

        protected Context Configuration_entry_for_from_date_is_changed = () =>
        {
            var configuration = new Configuration("Commit Heroes Slide");
            configuration.NewSetting("from date", "2009-12-20");
            configuration.NewSetting("timespan", "1");
            var configList = new List<Configuration> { configuration };

            configRepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        };

        protected Context Configuration_entry_does_not_exist = () =>
        {
            configRepositoryMock = new Mock<IRepository<Configuration>>();
            configPersisterRepositoryMock = new Mock<IPersistDomainModels<Configuration>>();

            var configlist = new List<Configuration>();
            configRepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configlist);
        };

        protected Context a_new_changeset_is_committed = () =>
        {
            List<Changeset> changesets = GenerateChangesetData();

            changesets.Add(new Changeset
            {
                Revision = 4,
                Time = DateTime.Now,
                Comment = "Added support for superfeature",
                Author = new Author
                {
                    Username = "goeran"
                }
            });

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).
                Returns(changesets.Where(c => c.Revision == 4)).Callback(
                () => changesetRepositoryGetThreadId = Thread.CurrentThread.ManagedThreadId);

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<AllChangesetsSpecification>())).
                Returns(changesets).Callback(
                () => changesetRepositoryGetThreadId = Thread.CurrentThread.ManagedThreadId);
        };

        protected Context a_new_changeset_with_a_new_user_is_committed = () =>
        {
            List<Changeset> changesets = GenerateChangesetData();

            changesets.Add(new Changeset
            {
                Revision = 5,
                Time = DateTime.Now,
                Comment = "Added support for superfeature",
                Author = new Author
                {
                    Username = "heine"
                }
            });

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<ChangesetsAfterRevisionSpecification>())).
                Returns(changesets.Where(c => c.Revision == 5)).Callback(
                () => changesetRepositoryGetThreadId = Thread.CurrentThread.ManagedThreadId);

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<AllChangesetsSpecification>())).
                Returns(changesets).Callback(
                () => changesetRepositoryGetThreadId = Thread.CurrentThread.ManagedThreadId);
        };

        protected When the_Controller_is_created = CreateController;
        protected Context the_Controller_is_spawned = CreateController;

        protected Context there_are_changesets_in_SourceControl_system = () =>
        {
            changesetRepositoryMock = new Mock<IRepository<Changeset>>();

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).
                Returns(GenerateChangesetData()).Callback(
                () => changesetRepositoryGetThreadId = Thread.CurrentThread.ManagedThreadId);
        };

        protected Context there_are_changesets_with_null_Author = () =>
        {
            changesetRepositoryMock = new Mock<IRepository<Changeset>>();

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).
                Returns(new List<Changeset>
                {
                    new Changeset
                    {
                        Revision = 1,
                        Time = DateTime.Now.AddDays(-1),
                        Comment = "Repository created",
                        Author = null
                    },
                    new Changeset
                    {
                        Revision = 2,
                        Time = DateTime.Now.AddDays(-1),
                        Comment = "Repository updated",
                        Author = new Author(null)
                    },
                    new Changeset
                    {
                        Revision = 3,
                        Time = DateTime.Now.AddDays(-1),
                        Comment = "Repository updated some more",
                        Author = new Author("tuxbear")
                    }
                });
        };

        protected Context there_are_no_users_in_userdb = () =>
        {
            userRepositoryMock = new Mock<IRepository<User>>();
            userRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<User>>())).Returns(new List<User>());
        };

        protected Context there_are_users_in_userdb = () =>
        {
            userRepositoryMock = new Mock<IRepository<User>>();

            var users = new List<User>
            {
                new User
                {
                    Username = "goeran",
                    Email = "mail@goeran.no",
                    Firstname = "Gøran",
                    Surname = "Hansen",
                    ImageUrl = "http://goeran.no/avatar.jpg"
                },
                new User
                {
                    Username = "dagolap",
                    Email = "mail@goeran.no",
                    Firstname = "Dag",
                    ImageUrl = "http://goeran.no/avatar.jpg"
                },
                new User
                {
                    Username =  "heine",
                    Email = "heine@heine.no",
                    Firstname = "Heine",
                    ImageUrl =  "http://goeran.no/avatar.jpg"
                }
            };

            userRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<User>>())).Returns(users);
        };

        private static List<Changeset> GenerateChangesetData()
        {
            var changesets = new List<Changeset>();
            changesets.Add(new Changeset
            {
                Revision = 1,
                Time = DateTime.Now.AddDays(-2),
                Comment = "Repository created",
                Author = new Author
                {
                    Username = "goeran"
                }
            });
            changesets.Add(new Changeset
            {
                Revision = 2,
                Time = DateTime.Now.AddDays(-1),
                Comment = "Added build script",
                Author = new Author
                {
                    Username = "goeran"
                }
            });
            changesets.Add(new Changeset
            {
                Revision = 3,
                Time = DateTime.Now.AddDays(-2),
                Comment = "Added unit testing framework",
                Author = new Author
                {
                    Username = "dagolap"
                }
            });
            return changesets;
        }

        private static void CreateController()
        {
            iNotifyWhenToRefreshMock = new Mock<INotifyWhenToRefresh>();
            controller = new TopCommitersController(iNotifyWhenToRefreshMock.Object,
                                                    changesetRepositoryMock.Object,
                                                    userRepositoryMock.Object,
                                                    configRepositoryMock.Object,
                                                    configPersisterRepositoryMock.Object,
                                                    new NoUIInvocation(),
                                                    new NoBackgroundWorkerInvocation<IEnumerable<Changeset>>(),
                                                    logger.Object
                                                    );
        }
    }

    public class LogEntryMockPersister : IPersistDomainModels<LogEntry>
    {
        public void Save(LogEntry domainModel) { }
        public void Save(IEnumerable<LogEntry> domainModels) { }
    }

    [TestFixture]
    public class Controller_is_spawned : Shared
    {

        [Test]
        public void Assure_Configuration_is_created_if_it_does_not_exist()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_users_in_userdb).
                And(Configuration_entry_does_not_exist).
                And(there_are_changesets_in_SourceControl_system);

                scenario.When(the_Controller_is_created);

                scenario.Then("assure Configuration is created if it doesn't exist", () =>
                configPersisterRepositoryMock.Verify(r => r.Save(It.IsAny<Configuration>()), Times.Once()));
            });
        }

        [Test]
        public void Should_get_timespan_from_configrepo_if_settings_are_set()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_users_in_userdb).
                    And(Configuration_timepan_entry_is_correctly_setup_for_one_day).
                    And(there_are_changesets_in_SourceControl_system);

                scenario.When(the_Controller_is_created);

                scenario.Then("the configuration service should be asked for the Topcommiter Slide configuration", () =>
                configRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Configuration>>()), Times.Once()));
            });
        }

        [Test]
        public void Assure_viewmodel_is_updated_with_value_from_configrepo()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_no_users_in_userdb).
                    And(Configuration_timepan_entry_is_correctly_setup_for_one_day).
                    And(there_are_changesets_in_SourceControl_system);

                scenario.When(the_Controller_is_created);
                scenario.Then("the controller should set the date on the viewmodel", ()=>
                {
                    controller.ViewModel.SinceDate.Date.ShouldBe(DateTime.Now.Date.AddDays(-1));
                });
            });
        }

        [Test]
        public void Assure_fromDate_configuration_has_precedence_over_timespan()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_no_users_in_userdb).
                    And(Configuration_has_valid_from_date).
                    And(there_are_changesets_in_SourceControl_system);
                scenario.When(the_Controller_is_created);
                scenario.Then("the controller should use from date if set in stead of timespan", ()=>
                {
                    controller.ViewModel.SinceDate.Date.ShouldBe(new DateTime(2009, 12, 30));
                });
            });
        }
        
        
        [Test]
        public void Assure_controller_querys_ChangesetRepository_for_all_changesets()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system)
                    .And(there_are_users_in_userdb);

                scenario.When(the_Controller_is_created);

                scenario.Then("assure it query ChangesetRepository for all changesets", () =>
                    changesetRepositoryMock.Verify(r =>r.Get(It.IsAny<Specification<Changeset>>()),Times.Once()));
            });
        }
    }

    [TestFixture]
    public class Loading_data : Shared
    {
        [Test]
        public void Assure_data_loading_is_performed_on_the_specified_thread()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_users_in_userdb).
                    And(there_are_changesets_in_SourceControl_system)
                    .And(Configuration_has_valid_from_date)
                    .And(the_Controller_is_spawned);

                scenario.When("data is loading");

                scenario.Then(
                    "assure data loading is performed on the supplied thread (this case: current thread)",() =>
                    changesetRepositoryGetThreadId.ShouldBe(Thread.CurrentThread.ManagedThreadId));
            });
        }

        [Test]
        public void Assure_the_controller_can_handle_null_author()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_no_users_in_userdb).
                    And(there_are_changesets_with_null_Author).
                    And(Configuration_has_valid_from_date).
                    And(the_Controller_is_spawned);

                scenario.When("Loading data");

                scenario.Then("No Exception should be thrown",() =>
                {
                    logger.Verify(l => l.WriteEntry(It.IsAny<ErrorLogEntry>()), Times.Never());
                });
            });
        }
    }


    [TestFixture]
    public class Data_is_loaded : Shared
    {
        
        [Test]
        public void Assure_data_is_loaded_into_the_viewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(there_are_users_in_userdb).
                    And(Configuration_timespan_entry_is_correctly_setup_for_2_days).
                    And(the_Controller_is_spawned);

                scenario.When("loadData() is run");

                scenario.Then("assure data is correctly loaded into the viewModel", () =>
                {
                    Thread.Sleep(200);
                    controller.ViewModel.Data.Count.ShouldBe(2);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("goeran")).First().
                        NumberOfCommits.ShouldBe(2);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("dagolap")).First().
                        NumberOfCommits.ShouldBe(1);
                });
            });
        }


        [Test]
        public void Should_use_timespan_from_config_when_loading_data()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_users_in_userdb).
                    And(Configuration_timepan_entry_is_correctly_setup_for_one_day).
                    And(there_are_changesets_in_SourceControl_system).
                    And(the_Controller_is_spawned);

                scenario.When("data is loaded");

                scenario.Then("the value from the configuration should be used in caluculating top committers", () =>
                {
                    controller.ViewModel.Data.Count.ShouldBe(1);
                    controller.ViewModel.Data[0].NumberOfCommits.ShouldBe(1);
                });
            });
        }
    }

    [TestFixture]
    public class When_notified_to_refresh : Shared
    {
        [Test]
        public void changed_NumberOfCommits_should_not_update_view_model()
        {
            Scenario.StartNew(this, scenario =>
            {
                bool viewModelChanged = false;
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(there_are_users_in_userdb).
                     And(Configuration_timespan_entry_is_correctly_setup_for_2_days).
                    And(the_Controller_is_spawned).
                    And(a_new_changeset_is_committed).
                    And("subscribe to ViewModel CollectionChanged",
                        () =>
                        {
                            controller.ViewModel.Data.CollectionChanged +=
                                (o, e) => { viewModelChanged = true; };
                        });

                scenario.When("controller is notified to refresh",
                    () => { iNotifyWhenToRefreshMock.Raise(n => n.Refresh += null, new RefreshEventArgs()); });

                scenario.Then("assure new NumberOfCommits is loaded into the viewModel",
                    () =>
                    {
                        controller.ViewModel.Data.Count.ShouldBe(2);
                        controller.ViewModel.Data.Where(d => d.Username.Equals("goeran")).First().NumberOfCommits.ShouldBe(3);
                        controller.ViewModel.Data.Where(d => d.Username.Equals("dagolap")).First().NumberOfCommits.ShouldBe(1);
                    }).
                    And("the CollectionChanged event in ViewModel should not have been fired",
                        () => { viewModelChanged.ShouldBeFalse(); });
            });
        }

        [Test]
        //[Ignore("Due to threading issues")]
        public void Assure_new_config_values_will_update_ViewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                bool viewModelChanged = false;
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(there_are_users_in_userdb).
                    And(Configuration_timespan_entry_is_correctly_setup_for_2_days).
                    And(the_Controller_is_spawned).
                    And(Configuration_timespan_entry_is_changed_from_2_to_1)
                    
                    .And("subscribe to ViewModel CollectionChanged", () =>
                        {
                            controller.ViewModel.Data.CollectionChanged +=
                                (o, e) => { viewModelChanged = true; };
                        });

                scenario.When("controller is notified to refresh",() =>
                    {
                        iNotifyWhenToRefreshMock.Raise(n => n.Refresh += null, new RefreshEventArgs());
                    });

                scenario.Then("assure new NumberOfCommits is loaded into the viewModel", () =>
                      {
                          controller.ViewModel.Data.Count.ShouldBe(1);
                          controller.ViewModel.Data.Where(d => d.Username.Equals("goeran")).First().
                            NumberOfCommits.ShouldBe(1);
                          changesetRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Changeset>>()),
                                                         Times.Exactly(2));

                          viewModelChanged.ShouldBeTrue();
                      });
            });
        }

        [Test]
        public void Assure_data_is_reloaded_each_day_if_only_timespan_is_correctly_configured()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(there_are_users_in_userdb).
                    And(Configuration_timespan_entry_is_correctly_setup_for_2_days).
                    And(the_Controller_is_spawned)

                    .And("controller was last updated one day ago", () =>
                    {
                        controller.lastUpdated = DateTime.Now.AddDays(-1);
                    });

                scenario.When("controller is notified to refresh", () =>
                {
                    iNotifyWhenToRefreshMock.Raise(n => n.Refresh += null, new RefreshEventArgs());
                });

                scenario.Then("data should be reloaded into the viewmodel", () => 
                    changesetRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Changeset>>()),Times.Exactly(2)));
            });
        }


        [Test]
        public void new_user_in_changesets_should_update_view_model()
        {
            Scenario.StartNew(this, scenario =>
            {
                bool viewModelChanged = false;
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(there_are_users_in_userdb).
                    And(Configuration_timespan_entry_is_correctly_setup_for_2_days).
                    And(the_Controller_is_spawned).
                    And(a_new_changeset_with_a_new_user_is_committed).
                    And("subscribe to ViewModel CollectionChanged",
                        () =>
                        {
                            controller.ViewModel.Data.CollectionChanged +=
                                (o, e) => { viewModelChanged = true; };
                        });

                scenario.When("controller is notified to refresh", () =>
                    iNotifyWhenToRefreshMock.Raise(n => n.Refresh += null,new RefreshEventArgs()));

                scenario.Then("the viewmodel should be updated", () =>
                {
                    controller.ViewModel.Data.Count.ShouldBe(3);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("goeran")).First().
                        NumberOfCommits.ShouldBe(2);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("dagolap")).First().
                        NumberOfCommits.ShouldBe(1);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("heine")).First().
                        NumberOfCommits.ShouldBe(1);
                }).
                    And("the CollectionChanged event in ViewModel should have been fired",
                        () => { viewModelChanged.ShouldBeTrue(); });
            });
        }

        [Test]
        public void should_query_repository_for_changesets()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(there_are_users_in_userdb).
                    And(the_Controller_is_spawned);

                scenario.When("controller is notified to refresh", () =>
                iNotifyWhenToRefreshMock.Raise(n => n.Refresh += null,new RefreshEventArgs()));

                scenario.Then("it should query repository for changesets", () =>
                    changesetRepositoryMock.Verify(r =>r.Get(It.IsAny<Specification<Changeset>>()), Times.Exactly(2)));
            });
        }

        [Test]
        public void unchanged_changeset_should_not_update_view_model()
        {
            Scenario.StartNew(this, scenario =>
            {
                bool viewModelChanged = false;
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(there_are_users_in_userdb).
                    And(the_Controller_is_spawned).
                    And("subscribe_to_ViewModel_CollectionChanged",() =>
                        {
                            controller.ViewModel.Data.CollectionChanged +=
                                (o, e) => { viewModelChanged = true; };
                        });

                scenario.When("controller is notified to refresh", () =>
                    iNotifyWhenToRefreshMock.Raise(n => n.Refresh += null,new RefreshEventArgs()));

                scenario.Then("the viewmodel should not be updated",
                              () => { viewModelChanged.ShouldBeFalse(); });
            });
        }
    }
}