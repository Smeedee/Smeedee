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
using Moq;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.DomainModel.Framework;



namespace APD.Client.Widget.SourceControlTests.Controllers.CommitStatisticsControllerSpecs
{
    public class Shared
    {
        protected static Mock<IRepository<Changeset>> changesetRepositoryMock;
        protected static CommitStatisticsController controller;
        protected static int changesetRepositoryGetThreadId;
        protected static Mock<INotifyWhenToRefresh> iNotifyWhenToRefreshMock;
        protected static Mock<IInvokeBackgroundWorker<IEnumerable<Changeset>>> backgroundWorkerInvokerMock;
        protected static Mock<IPersistDomainModels<Configuration>> configPersisterRepositoryMock =
        new Mock<IPersistDomainModels<Configuration>>();
        protected static Mock<IRepository<Configuration>> configRepositoryMock =
            new Mock<IRepository<Configuration>>();


        private static void SetupConfigRepositoryMock(int timespan)
        {
            var configuration = new Configuration("Commit Statistics");
            configuration.NewSetting("timespan", timespan.ToString());
            var configList = new List<Configuration> { configuration };

            configRepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configList);
        }

        protected Context Configuration_entry_does_not_exist = () =>
        {
            var configlist = new List<Configuration>();
            configRepositoryMock.Setup(
                r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configlist);
        };

        protected Context Configuration_timespan_entry_is_correctly_setup_for_14_days = () =>
        {
            SetupConfigRepositoryMock(14);
        };

        protected Context Configuration_timespan_entry_is_correctly_setup_for_10_days = () =>
        {
            SetupConfigRepositoryMock(10);
        };

        
        protected Context Configuration_timespan_entry_is_changed_to_5 = () =>
        {
            SetupConfigRepositoryMock(5);
        };

        protected Context Configuration_timespan_entry_is_set_up_for_1_day = () =>
        {
            SetupConfigRepositoryMock(0);
        };

        protected Context Configuration_timespan_entry_is_set_up_for_0_days = () =>
        {
            SetupConfigRepositoryMock(0);
        };

        protected Context Configuration_is_set_up_with_negative_timespan = () =>
        {
            SetupConfigRepositoryMock(-2);
        };

        protected Context there_are_changesets_in_SourceControl_system = () =>
        {
            changesetRepositoryMock = new Mock<IRepository<Changeset>>();

            var changesets = new List<Changeset>();
            changesets.Add(new Changeset()
                           {
                               Revision = 1,
                               Time = DateTime.Today,
                               Comment = "Repository created",
                               Author = new Author()
                                        {
                                            Username = "goeran"
                                        }
                           });
            changesets.Add(new Changeset()
                           {
                               Revision = 2,
                               Time = DateTime.Today,
                               Comment = "Added build script",
                               Author = new Author()
                                        {
                                            Username = "goeran"
                                        }
                           });
            changesets.Add(new Changeset()
                           {
                               Revision = 3,
                               Time = DateTime.Today,
                               Comment = "Added unit testing framework",
                               Author = new Author()
                                        {
                                            Username = "dagolap"
                                        }
                           });

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<AllChangesetsSpecification>())).
                Returns(changesets).Callback(() =>
                    changesetRepositoryGetThreadId = Thread.CurrentThread.ManagedThreadId);
        };

        protected Context controller_is_spawned = CreateController;

        protected Context there_are_only_three_changesets_this_week = () =>
        {
            changesetRepositoryMock = new Mock<IRepository<Changeset>>();

            var changesets = new List<Changeset>();
            changesets.Add(new Changeset()
            {
                Revision = 1,
                Time = DateTime.Today.AddDays(-6),
                Comment = "Repository created",
                Author = new Author()
                {
                    Username = "goeran"
                }
            });
            changesets.Add(new Changeset()
            {
                Revision = 2,
                Time = DateTime.Today.AddDays(-3),
                Comment = "Added build script",
                Author = new Author()
                {
                    Username = "goeran"
                }
            });
            changesets.Add(new Changeset()
            {
                Revision = 3,
                Time = DateTime.Today,
                Comment = "Added unit testing framework",
                Author = new Author()
                {
                    Username = "dagolap"
                }
            });

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<AllChangesetsSpecification>())).
                Returns(changesets).Callback(() =>
                                             changesetRepositoryGetThreadId =
                                             Thread.CurrentThread.ManagedThreadId);
        };


        protected When the_controller_is_created = CreateController;

        protected static void CreateController()
        {
            backgroundWorkerInvokerMock = new Mock<IInvokeBackgroundWorker<IEnumerable<Changeset>>>();
            iNotifyWhenToRefreshMock = new Mock<INotifyWhenToRefresh>();
            controller = new CommitStatisticsController(iNotifyWhenToRefreshMock.Object,
                                                        changesetRepositoryMock.Object,
                                                        new NoUIInvocation(),
                                                        configRepositoryMock.Object,
                                                        configPersisterRepositoryMock.Object,
                                                        new NoBackgroundWorkerInvocation<IEnumerable<Changeset>>(),
                                                        new DatabaseLogger(new LogEntryMockPersister()));
        }
    }

    public class LogEntryMockPersister : IPersistDomainModels<LogEntry>
    {
        public static List<LogEntry> entries;

        public LogEntryMockPersister()
        {
            entries = new List<LogEntry>();
        }
        
        public void Save(LogEntry domainModel)
        {
            entries.Add(domainModel);
        }

        public void Save(IEnumerable<LogEntry> domainModels)
        {
            entries.AddRange(domainModels);
        }
    }


    [TestFixture]
    public class When_spawned : Shared
    {

        [Test]
        public void Assure_Configuration_is_created_if_it_does_not_exist()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_entry_does_not_exist);

                scenario.When(the_controller_is_created);

                scenario.Then("assure Configuration is created if it doesn't exist", () =>
                configPersisterRepositoryMock.Verify(r => r.Save(It.IsAny<Configuration>()), Times.Once()));
            });
        }

        
        [Test]
        public void Should_get_timespan_from_configrepo_if_settings_are_set()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_timespan_entry_is_correctly_setup_for_14_days);

                scenario.When(the_controller_is_created);

                scenario.Then(
                    "the configuration service should be asked for the Commit Statistics configuration",
                    () => configRepositoryMock.Verify(
                        r => r.Get(It.IsAny<Specification<Configuration>>()),Times.AtLeastOnce()));
            });
        }

        [Test]
        public void Should_add_2_datapoints_if_timespan_is_negative_or_zero()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_entry_does_not_exist).
                    And(controller_is_spawned);
                scenario.When("data is loaded");
                scenario.Then("viweModel should have 2 data points", ()=>
                {
                    controller.ViewModel.Data.Count.ShouldBe(2);
                });
               
            });
        }

        [Test]
        public void Should_treat_timepan_of_1_as_timespan_of_2()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_timespan_entry_is_set_up_for_1_day).
                    And(controller_is_spawned);
                scenario.When("data is loaded");
                scenario.Then("timespan should be considered to be two days", () =>
                {
                    controller.ViewModel.Data.Count.ShouldBe(2);
                });
            });
        }

        [Test]
        public void Should_throw_exception_if_negative_configuration_timespan_is_used()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_is_set_up_with_negative_timespan).
                    And(controller_is_spawned);

                scenario.When("data is loaded");

                scenario.Then("exception should be thrown when trying to parse negative timespan", ()=>
                {
                    LogEntryMockPersister.entries.Where(r => r.Message.Contains("Start date must be before or at end date")).Count().ShouldBe(1);
                });
            });
        }
         
        
        [Test]
        public void Assert_groups_all_changesets_on_the_same_date()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).And(Configuration_timespan_entry_is_correctly_setup_for_14_days);
                scenario.When(the_controller_is_created);
                scenario.Then("all the changesets on the same date sould be grouped", () =>
                {
                    controller.ViewModel.Data.Count.ShouldBe(14);
                });
            });
        }


        [Test]
        public void Assure_data_loading_is_performed_on_the_specified_thread()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system);

                scenario.When(the_controller_is_created);

                scenario.Then("assure data loading is performed on the supplied thread (this case: current thread)", () =>
                    changesetRepositoryGetThreadId.ShouldBe(Thread.CurrentThread.ManagedThreadId));
            });
        }

        [Test]
        public void Assure_data_loading_is_delegated_to_backgroundWorkerInvoker()
        {
            iNotifyWhenToRefreshMock = new Mock<INotifyWhenToRefresh>();

            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And("the controller is spawned using a BackgroundWorkerInvoker mock", () =>
                    {
                        backgroundWorkerInvokerMock =
                            new Mock<IInvokeBackgroundWorker<IEnumerable<Changeset>>>();
                        controller = new CommitStatisticsController(
                            iNotifyWhenToRefreshMock.Object,
                            changesetRepositoryMock.Object,
                            new NoUIInvocation(),
                            configRepositoryMock.Object,
                            configPersisterRepositoryMock.Object,
                            backgroundWorkerInvokerMock.Object,
                            new DatabaseLogger(new LogEntryMockPersister()));
                    });

                scenario.When("data is loading");

                scenario.Then("assure data loading is performed delegated to the BackgroundWorkerInvoker object", () =>
                    backgroundWorkerInvokerMock.Verify(w => w.RunAsyncVoid(It.IsAny<Action>()), Times.Exactly(2)));
            });
        }

        [Test]
        public void Assure_timespan_from_config_is_used_when_loading_data()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_timespan_entry_is_correctly_setup_for_10_days).
                    And(controller_is_spawned);

                scenario.When("data is loaded");

                scenario.Then("the value fom configuration should be used to define commit timespan", () =>
                {
                    controller.ViewModel.Data.Count.ShouldBe(10);
                });
            });
        }

        [Test]
        public void Assure_we_have_data_points_for_all_days_in_the_week()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_only_three_changesets_this_week).
                    And(Configuration_timespan_entry_is_correctly_setup_for_14_days);

                scenario.When(the_controller_is_created);
                scenario.Then("we should have data points for dates with no commits", () =>
                          {

                              var controllerData = controller.ViewModel.Data;

                              controllerData.Count.ShouldBe(14);

                              int[] expectedCommits = new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1 };
                              DateTime[] testDates = new DateTime[]
                              {
                                  DateTime.Today.AddDays(-13),
                                  DateTime.Today.AddDays(-12),
                                  DateTime.Today.AddDays(-11),
                                  DateTime.Today.AddDays(-10),
                                  DateTime.Today.AddDays(-9),
                                  DateTime.Today.AddDays(-8),
                                  DateTime.Today.AddDays(-7),
                                  DateTime.Today.AddDays(-6),
                                  DateTime.Today.AddDays(-5),
                                  DateTime.Today.AddDays(-4),
                                  DateTime.Today.AddDays(-3),
                                  DateTime.Today.AddDays(-2),
                                  DateTime.Today.AddDays(-1),
                                  DateTime.Today
                              };


                              for (int day = 0; day < testDates.Length; day++)
                              {
                                  VerifyThatTheDateAndNumberOfCommitsAreCorrect(
                                      controllerData[day], testDates[day], expectedCommits[day]).ShouldBeTrue();
                              }

                          }
                    );
            });

        }

        private bool VerifyThatTheDateAndNumberOfCommitsAreCorrect
            (APD.Client.Widget.SourceControl.ViewModels.CommitStatisticsForDate commit, DateTime expectedDate, int expectedNumberOfCommits)
        {
            if (commit == null)
                return false;

            if (commit.Date.Date != expectedDate.Date)
                return false;

            if (commit.NumberOfCommits != expectedNumberOfCommits)
                return false;

            return true;

        }
    }



    [TestFixture]
    public class When_notified_to_refresh : Shared
    {
        [Test]
        public void should_query_repository_for_changesets()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(there_are_changesets_in_SourceControl_system);

                scenario.When("the controller is created and asked to refresh", () =>
                {
                    CreateController();
                    iNotifyWhenToRefreshMock.Raise(n => n.Refresh += null, new RefreshEventArgs());
                });

                scenario.Then("it should query repository for changesets", () =>
                    changesetRepositoryMock.Verify(r => r.Get(It.IsAny<AllChangesetsSpecification>()), Times.Exactly(2)));
            });
        }

        [Test]
        public void assure_new_config_value_reloades_viewModel()
        {
            Scenario.StartNew(this, scenario =>
            {
                bool viewModelChanged = false;
                scenario.Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_timespan_entry_is_correctly_setup_for_10_days).
                    And(controller_is_spawned).

                    And("subscribe to ViewModel CollectionChanged", () =>
                    {
                        
                        controller.ViewModel.Data.CollectionChanged +=
                            (o, e) => { viewModelChanged = true; };
                    });

                scenario.When("controller is notified to refresh", () =>
                {
                    Configuration_timespan_entry_is_changed_to_5();
                    iNotifyWhenToRefreshMock.Raise(n => n.Refresh += null, new RefreshEventArgs());
                });

                scenario.Then("assure data is loaded into the viewModel according to config value", () =>
                {
                    controller.ViewModel.Data.Count.ShouldBe(5);
                    controller.ViewModel.Data.Where(d => d.NumberOfCommits.Equals(3)).Last();
                    changesetRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Changeset>>()),
                                                   Times.Exactly(2));
                    viewModelChanged.ShouldBeTrue();
                });
            });   
        }
    }
}