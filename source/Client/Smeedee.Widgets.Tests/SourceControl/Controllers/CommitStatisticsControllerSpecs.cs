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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.SourceControl;
using Moq;
using NUnit.Framework;
using Smeedee.Widgets.SourceControl.Controllers;
using Smeedee.Widgets.SourceControl.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Widgets.Tests.SourceControl.Controllers
{
    public class CommitStatisticsControllerSpecs
    {
        [TestFixture]
        public class When_spawned : Shared
        {
            [Test]
            public void Assure_Configuration_is_created_if_it_does_not_exist()
            {
                Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_entry_does_not_exist);

                When(the_controller_is_created);

                Then("assure default Configuration is created if it doesn't exist", () =>
                settingsViewModel.CommitTimespanDays.ShouldBe(14));
            }


            [Test]
            public void Should_get_timespan_from_configrepo_if_settings_are_set()
            {

                Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_timespan_entry_is_correctly_setup_for_10_days);

                When(the_controller_is_created);

                Then(
                    "the configuration should show timespan",
                    () => settingsViewModel.CommitTimespanDays.ShouldBe(10));
            }

            [Test]
            public void Should_use_the_projectStart_if_timespan_is_neagative_or_zero()
            {
                Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_entry_does_not_exist).
                    And(controller_is_spawned);
                When("data is loaded");
                Then("viweModel should have one data point",
                    () => controller.ViewModel.Data.Count.ShouldBe(1));
            }

            [Test]
            public void Should_handle_timespan_of_one_day()
            {
                Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_timespan_entry_is_set_up_for_1_day).
                    And(controller_is_spawned);
                When("data is loaded");
                Then("viewModel should have one point",
                    () => controller.ViewModel.Data.Count.ShouldBe(2));
            }

            [Test]
            public void Should_only_throw_exception_if_configvalue_is_loaded() //Functionality not realy tested because of threading. Should add delay to config mock? Do i have to write a whole new mock to do this?
            {
                Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_timespan_entry_is_correctly_setup_for_10_days).
                    And(controller_is_spawned);
                When("data is loaded");
                Then("exception should not be thrown", () =>
                {
                    LogEntryMockPersister.entries.Count.ShouldBe(0);
                });
            }

            [Test]
            public void Assert_groups_all_changesets_on_the_same_date()
            {
                Given(there_are_changesets_in_SourceControl_system).And(Configuration_timespan_entry_is_correctly_setup_for_14_days);
                When(the_controller_is_created);
                Then("all the changesets on the same date sould be grouped", () =>
                {
                    controller.ViewModel.Data.Count.ShouldBe(1);
                });
            }

            [Test]
            [Ignore("We are using async repository for dataloading")]
            public void Assure_data_loading_is_performed_on_the_specified_thread()
            {
                Given(there_are_changesets_in_SourceControl_system);

                When(the_controller_is_created);

                Then("assure data loading is performed on the supplied thread (this case: current thread)", () =>
                    changesetRepositoryGetThreadId.ShouldBe(Thread.CurrentThread.ManagedThreadId));
            }

            [Test]
            [Ignore("We are using async repository for dataloading")]
            public void Assure_data_loading_is_delegated_to_backgroundWorkerInvoker()
            {
                ITimerMock = new Mock<ITimer>();
                Given(there_are_changesets_in_SourceControl_system).
                    And("the controller is spawned using a BackgroundWorkerInvoker mock", () =>
                    {
                        backgroundWorkerInvokerMock =
                            new Mock<IInvokeBackgroundWorker<IEnumerable<Changeset>>>();

                        controller = new CommitStatisticsController(new BindableViewModel<CommitStatisticsForDate>(),
                                                                    new CommitStatisticsSettingsViewModel(),
                                                                    changesetRepositoryMock.Object,
                                                                    backgroundWorkerInvokerMock.Object,
                                                                    ITimerMock.Object,
                                                                    new NoUIInvokation(),
                                                                    configRepositoryMock.Object,
                                                                    configPersisterRepositoryMock.Object,
                                                                    new Logger(new LogEntryMockPersister()),
                                                                    loadingNotifierMock.Object,
                                                                    new Mock<IWidget>().Object
                                                                    );
                    });

                When("data is loading");

                Then("assure data loading is performed delegated to the BackgroundWorkerInvoker object", () =>
                {
                    backgroundWorkerInvokerMock.Verify(w => w.RunAsyncVoid(It.IsAny<Action>()), Times.AtLeast(1));
                });
            }

            [Test]
            public void Assure_timespan_from_config_is_used_when_loading_data()
            {
                Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_timespan_entry_is_correctly_setup_for_10_days).
                    And(controller_is_spawned);

                When("data is loaded");

                Then("the timespan should only be as big as the data permitts", () =>
                {
                    controller.ViewModel.Data.Count.ShouldBe(1);
                });
            }

            [Test]
            public void Assure_we_have_data_points_for_all_days_in_the_week()
            {
                Given(there_are_only_four_changesets_the_last_two_weeks).
                    And(Configuration_timespan_entry_is_correctly_setup_for_14_days);

                When(the_controller_is_created);
                Then("we should have data points for dates with no commits", () =>
                {

                    var controllerData = controller.ViewModel.Data;

                    controllerData.Count.ShouldBe(14);

                    int[] expectedCommits = new int[] { 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1 };
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
            }

            private bool VerifyThatTheDateAndNumberOfCommitsAreCorrect
                (CommitStatisticsForDate commit, DateTime expectedDate, int expectedNumberOfCommits)
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
            public void the_timer_is_started_when()
            {
                Given(CreateController);
                When("the program starts");
                Then("the timer is also started", () => ITimerMock.Verify(t => t.Start(60000)));
            }

            [Test]
            public void should_query_repository_for_changesets()
            {
                Given(there_are_changesets_in_SourceControl_system).And(Configuration_timespan_entry_is_correctly_setup_for_3_days);

                When("the controller is created and asked to refresh", () =>
                {
                    CreateController();
                    ITimerMock.Raise(n => n.Elapsed += null, new EventArgs());
                });

                Then("it should query repository for changesets",
                    () => changesetRepositoryMock.Verify(r => r.BeginGet(It.IsAny<ChangesetsAfterRevisionSpecification>()), Times.Between(0, 100, Range.Inclusive)));
            }

            [Test]
            public void assure_new_config_value_reloades_viewModel()
            {
                bool viewModelChanged = false;
                Given(there_are_changesets_in_SourceControl_system).
                    And(Configuration_timespan_entry_is_correctly_setup_for_10_days).
                    And(controller_is_spawned).

                    And("subscribe to ViewModel CollectionChanged", () =>
                    {

                        controller.ViewModel.Data.CollectionChanged +=
                            (o, e) => { viewModelChanged = true; };
                    });

                When("configuration is changed", () =>
                {
                    Configuration_timespan_entry_is_changed_to_5();
                });

                Then("assure data is loaded into the viewModel according to config value", () =>
                {
                    controller.ViewModel.Data.Count.ShouldBe(1);
                    controller.ViewModel.Data.Where(d => d.NumberOfCommits.Equals(3)).Last();
                    changesetRepositoryMock.Verify(r => r.BeginGet(It.IsAny<Specification<Changeset>>()),
                                                   Times.Exactly(2));
                    viewModelChanged.ShouldBeTrue();
                });
            }
        }

        [TestFixture]
        public class When_buttons_are_pressed : Shared
        {
            [Test]
            public void Assure_settings_are_saved_to_the_db_after_save_command_when_there_are_changes_done()
            {
                Given(there_are_changesets_in_SourceControl_system).
                And(controller_is_spawned).
                And(config_is_changed);
                When(save_button_is_pressed);
                Then("", () => configPersisterRepositoryMock.Verify(r => r.Save(It.IsAny<Configuration>()), Times.AtLeastOnce()));
            }

            [Test]
            [Ignore("Not relevant, as we just reuse the old configuration object")]
            public void Assure_that_pressing_the_reloadSettings_button_leads_to_a_get_to_the_nonfigrepository()
            {
                Given(controller_is_spawned);
                When(reload_button_is_pressed);
                Then("",
                    () => configRepositoryMock.Verify(r => r.BeginGet(It.IsAny<ConfigurationByName>()), Times.Exactly(2)));
            }
        }
        [TestFixture]
        public class When_updating_data_from_revision : Shared
        {
            [Test]
            public void Assure_lastRevision_is_set_to_the_highest_revision_number_in_the_changesets()
            {
                Given(there_are_changesets_in_SourceControl_system);
                When(the_controller_is_created);
                Then("", () => viewModel.CurrentRevision.ShouldBe(3));
            }

            [Test]
            public void Assure_revision_are_the_same_after_refresh_without_new_changesets()
            {
                Given(there_are_changesets_in_SourceControl_system).And(controller_is_spawned).And(Configuration_timespan_entry_is_correctly_setup_for_3_days);
                When(the_ViewModel_is_updated);
                Then("", () => viewModel.CurrentRevision.ShouldBe(3));
            }

            [Test]
            public void Assure_older_revisions_are_not_updated_to_the_ViewModel()
            {
                Given(there_are_changesets_in_SourceControl_system).And(Configuration_timespan_entry_is_correctly_setup_for_10_days).And(controller_is_spawned).And(older_changesets_are_added_to_the_system);
                When(the_ViewModel_is_updated);
                Then("The viewModel should not be updated",
                    () => viewModel.Data.Count.ShouldBe(1));
            }

            [Test]
            public void Assure_lastRevision_is_updated_when_updated_with_new_changesets()
            {
                Given(there_are_changesets_in_SourceControl_system).And(controller_is_spawned).And(Configuration_timespan_entry_is_set_up_for_1_day);
                When(three_new_changesets_are_made_and_refresh_is_called);
                Then("", () => viewModel.CurrentRevision.ShouldBe(6));
            }

            [Test]
            public void Assure_that_a_change_in_config_causes_a_full_reload_of_the_ViewModel()
            {
                Given(there_are_changesets_in_SourceControl_system).And(controller_is_spawned).And(Configuration_timespan_entry_is_correctly_setup_for_3_days);
                When(Configuration_timespan_entry_is_changed_to_2);
                Then("", () => Then("The viewModel should be updated",
                                    () => viewModel.Data.Count.ShouldBe(1)));
            }

            [Test]
            public void Assure_that_new_changesets_gives_the_right_amount_of_datapoints()
            {
                Given(there_are_changesets_in_SourceControl_system).And(controller_is_spawned);
                When(three_new_changesets_are_made_and_refresh_is_called);
                Then("The viewModel should contain 3 datapoint",
                                    () => viewModel.Data.Count.ShouldBe(3));
            }

            [Test]
            public void Assure_that_there_are_two_datapoints_drawn_when_timespan_is_two_and_project_is_at_least_two_days_old()
            {
                Given(there_are_only_four_changesets_the_last_two_weeks).And(controller_is_spawned).And(Configuration_timespan_entry_is_correctly_setup_for_2_days);
                When("the viewModel is loaded");
                Then("The viewModel should contain three datapoints",
                                   () => viewModel.Data.Count.ShouldBe(3));
                // timespan is currently such that two days = start the day before yesterday
                // So there should be three datapoints, one for each day including today
            }

            [Test]
            public void Assure_that_new_data_on_the_same_day_does_not_increase_datapoints()
            {
                Given(there_are_changesets_in_SourceControl_system).And(controller_is_spawned);
                When(three_new_changesets_are_made_today_and_refresh_is_called);
                Then("there should still only be one datapoint",
                     () =>
                     {
                         viewModel.Data.Count.ShouldBe(1);
                         viewModel.Data.First().NumberOfCommits.ShouldBe(6);
                     });
            }

            [Test]
            public void Assure_that_the_first_datapoint_is_not_zero_if_it_has_commits()
            {
                Given(there_are_changesets_in_SourceControl_system).And(Configuration_date_entry_is_correctly_setup_for_today);
                When(the_controller_is_created);
                Then("there should be one datapoint with three commits", () =>
                             {
                                 viewModel.Data.Count.ShouldBe(2);
                                 viewModel.Data.Where(c => c.Date.Equals(DateTime.Today)).SingleOrDefault().NumberOfCommits.ShouldBe(3);
                             });
            }

            [Test]
            public void Assure_that_the_first_datapoint_after_changing_config_is_not_zero()
            {
                Given(there_are_only_four_changesets_the_last_two_weeks).And(controller_is_spawned);
                When(Configuration_date_entry_is_correctly_setup_for_six_days_ago);
                Then("there should be one commit in the datapoint 6 days ago",
                    () => viewModel.Data.Where(c => c.Date.Equals(DateTime.Today.AddDays(-6)))
                                   .SingleOrDefault().NumberOfCommits.ShouldBe(1));
            }

            [Test]
            public void Assure_that_today_recieves_a_zero_point_if_it_has_no_commits()
            {
                Given(one_changeset_one_day_ago).
                    And(controller_is_spawned);
                When(Configuration_timespan_entry_is_changed_to_2);
                Then("", () =>
                             {
                                 viewModel.Data.Count.ShouldBe(2);

                                 viewModel.Data.Where(c => c.Date.Equals(DateTime.Today)).
                                     SingleOrDefault().NumberOfCommits.ShouldBe(0);
                             });
            }

            [Test]
            public void Assure_that_one_commit_six_days_ago_and_six_day_span_gives_correct_points()
            {
                Given(one_changeset_six_days_ago).
                    And(controller_is_spawned);
                When(Configuration_timespan_entry_is_changed_to_6);
                Then("there should be one at 6 days ago, and zeros until today", () =>
                             {
                                 viewModel.Data.Count.ShouldBe(7);

                                 for (int i = 0; i < 6; i++)
                                 {
                                     viewModel.Data.Where(c => c.Date.Equals(DateTime.Today.AddDays(-i))).
                                         SingleOrDefault().NumberOfCommits.ShouldBe(0);
                                 }

                                 viewModel.Data.Where(c => c.Date.Equals(DateTime.Today.AddDays(-6)))
                                   .SingleOrDefault().NumberOfCommits.ShouldBe(1);
                             });
            }

        }

        [TestFixture]
        public class When_notifying : Shared
        {
            [Test]
            public void Assure_loadingNotifier_is_shown_while_loading()
            {
                Given(there_are_changesets_in_SourceControl_system).And(controller_is_spawned);
                When(the_ViewModel_is_updated);
                Then("the loadingNotifyer should be shown",
                () => loadingNotifierMock.Verify(l => l.ShowInView(It.IsAny<string>()), Times.AtLeastOnce()));
            }

            [Test]
            public void Assure_loadingNotifier_is_hidden_after_loading()
            {
                Given(there_are_changesets_in_SourceControl_system);
                When(the_controller_is_created);
                Then("", () =>
                {
                    loadingNotifierMock.Verify(l => l.HideInView(), Times.AtLeastOnce());
                    controller.ViewModel.IsLoading.ShouldBe(false);
                });
            }

            [Test]
            public void Assure_loadingNotifier_is_shown_when_save_is_pressed()
            {
                Given(controller_is_spawned);
                When(save_button_is_pressed);
                Then("the loadingNotifier should be shown",
                    () => loadingNotifierMock.Verify(l => l.ShowInSettingsView(It.IsAny<string>()), Times.AtLeastOnce()));
            }

            [Test]
            public void Assure_loadingNotifier_is_hidden_again_some_time_after_save_is_pressed()
            {
                Given(controller_is_spawned);
                When(save_button_is_pressed);
                Then("", () =>
                {
                    loadingNotifierMock.Verify(l => l.HideInSettingsView(), Times.AtLeastOnce());
                    controller.ViewModel.IsSaving.ShouldBeFalse();
                });
            }

            [Test]
            [Ignore("Not the controllers responsibility to load settings when started anymore")]
            public void Assure_progressbar_is_shown_while_loading_settings()
            {
                Given(Setup_configPersister_NOT_to_raise_save_completed);
                When(the_controller_is_created);
                Then(() =>
                {
                    loadingNotifierMock.Verify(l => l.ShowInBothViews(It.IsAny<string>()), Times.AtLeastOnce());
                    controller.ViewModel.IsLoadingConfig.ShouldBe(true);

                });
            }

            [Test]
            [Ignore("Not the controllers responsibility to load settings when started anymore")]
            public void Assure_progressbar_is_hidden_after_loading_settings()
            {
                Given(controller_is_spawned);

                When(settingsViewModel.ReloadSettings.Execute);

                Then(() =>
                {
                    loadingNotifierMock.Verify(l => l.HideInBothViews(), Times.AtLeastOnce());
                    controller.ViewModel.IsLoadingConfig.ShouldBe(false);
                });
            }

        }

        public class Shared : ScenarioClass
        {
            protected static Mock<IAsyncRepository<Changeset>> changesetRepositoryMock;
            protected static CommitStatisticsController controller;
            protected static int changesetRepositoryGetThreadId;
            protected static Mock<ITimer> ITimerMock;
            protected static Mock<IInvokeBackgroundWorker<IEnumerable<Changeset>>> backgroundWorkerInvokerMock;
            protected static Mock<IPersistDomainModelsAsync<Configuration>> configPersisterRepositoryMock =
                new Mock<IPersistDomainModelsAsync<Configuration>>();
            protected static Mock<IAsyncRepository<Configuration>> configRepositoryMock =
                new Mock<IAsyncRepository<Configuration>>();
            protected static Mock<IProgressbar> loadingNotifierMock = new Mock<IProgressbar>();
            protected static CommitStatisticsSettingsViewModel settingsViewModel = new CommitStatisticsSettingsViewModel();
            protected static BindableViewModel<CommitStatisticsForDate> viewModel = new BindableViewModel<CommitStatisticsForDate>();
            protected static Mock<IWidget> widgetMock = new Mock<IWidget>();


            private static void SetupConfigRepositoryMock(int timespan)
            {
                var configuration = new Configuration("Commit Statistics");
                configuration.NewSetting("CommitTimespanDays", timespan.ToString());
                configuration.NewSetting("isUsingTimespan", true.ToString());
                var configList = new List<Configuration> { configuration };

                widgetMock.SetupGet(w => w.Configuration).Returns(configuration);
                UpdateConfiguration();


                //configRepositoryMock.Setup(
                //    r => r.BeginGet(It.IsAny<ConfigurationByName>())).Raises(
                //    t => t.GetCompleted += null, new GetCompletedEventArgs<Configuration>(configList, null));
            }
            private static void SetupConfigRepositoryMockItSinceDate(DateTime date)
            {
                var configuration = new Configuration("Commit Statistics");
                configuration.NewSetting("SinceDate", date.ToString(new CultureInfo("en-US")));
                configuration.NewSetting("IsUsingDate", true.ToString());
                var configList = new List<Configuration> { configuration };

                widgetMock.SetupGet(w => w.Configuration).Returns(configuration);
                UpdateConfiguration();

                //configRepositoryMock.Setup(
                //    r => r.BeginGet(It.IsAny<ConfigurationByName>())).Raises(
                //    t => t.GetCompleted += null, new GetCompletedEventArgs<Configuration>(configList, null));
            }
            protected Context new_configPersister =
                () => configPersisterRepositoryMock = new Mock<IPersistDomainModelsAsync<Configuration>>();
            protected Context Configuration_entry_does_not_exist = () =>
            {
                //var configlist = new List<Configuration>();
                //configRepositoryMock.Setup(
                //     r => r.BeginGet(It.IsAny<ConfigurationByName>())).Raises(
                //     t => t.GetCompleted += null, new GetCompletedEventArgs<Configuration>(configlist, null));
            };

            protected Context Configuration_date_entry_is_correctly_setup_for_today = () =>
            {
                SetupConfigRepositoryMockItSinceDate(DateTime.Today);
            };

            protected When Configuration_date_entry_is_correctly_setup_for_six_days_ago = () =>
            {
                SetupConfigRepositoryMockItSinceDate(DateTime.Today.AddDays(-6));
            };

            protected Context Configuration_timespan_entry_is_correctly_setup_for_14_days = () =>
            {
                SetupConfigRepositoryMock(14);
            };

            protected Context Configuration_timespan_entry_is_correctly_setup_for_10_days = () =>
            {
                SetupConfigRepositoryMock(10);
            };

            protected Context Configuration_timespan_entry_is_correctly_setup_for_3_days = () =>
            {
                SetupConfigRepositoryMock(3);
            };

            protected Context Configuration_timespan_entry_is_correctly_setup_for_2_days = () =>
            {
                SetupConfigRepositoryMock(2);
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
                changesetRepositoryMock = new Mock<IAsyncRepository<Changeset>>();

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

                ChangesetRepositoryContains(changesets);
            };

            protected Context one_changeset_one_day_ago = () =>
            {
                changesetRepositoryMock = new Mock<IAsyncRepository<Changeset>>();

                var changesets = new List<Changeset>();
                changesets.Add(new Changeset()
                {
                    Revision = 1,
                    Time = DateTime.Today.AddDays(-1),
                    Comment = "Repository created",
                    Author = new Author()
                    {
                        Username = "goeran"
                    }
                });

                ChangesetRepositoryContains(changesets);
            };

            protected Context one_changeset_six_days_ago = () =>
            {
                changesetRepositoryMock = new Mock<IAsyncRepository<Changeset>>();

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
                ChangesetRepositoryContains(changesets);
            };

            protected Context Setup_configPersister_NOT_to_raise_save_completed =
                () =>
                {
                    configRepositoryMock.Setup(r => r.BeginGet(It.IsAny<ConfigurationByName>()));
                };

            protected Context older_changesets_are_added_to_the_system = () =>
            {
                changesetRepositoryMock = new Mock<IAsyncRepository<Changeset>>();

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
                    Revision = 0,
                    Time = DateTime.Today,
                    Comment = "Added unit testing framework",
                    Author = new Author()
                    {
                        Username = "dagolap"
                    }
                });

                ChangesetRepositoryContains(changesets);
            };

            /*protected Context Changeset_repository_is_empty_and_slow = () =>
            {
                changesetRepositoryMock = new Mock<IRepository<Changeset>>();
                changesetRepositoryMock.Setup(r => r.Get(It.IsAny<AllChangesetsSpecification>())).
                    Returns(() =>
                    {
                        Thread.Sleep(3000);
                        return new List<Changeset>();
                    });
            };*/

            protected Context controller_is_spawned = CreateController;

            protected Context there_are_only_four_changesets_the_last_two_weeks = () =>
            {
                changesetRepositoryMock = new Mock<IAsyncRepository<Changeset>>();

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
                changesets.Add(new Changeset()
                {
                    Revision = 4,
                    Time = DateTime.Today.AddDays(-13),
                    Comment = "Added unit testing framework",
                    Author = new Author()
                    {
                        Username = "dagolap"
                    }
                });

                ChangesetRepositoryContains(changesets);
            };

            protected Context config_is_changed = () =>
            {
                settingsViewModel.CommitTimespanDays++;
            };


            protected When the_controller_is_created = CreateController;
            protected When the_ViewModel_is_updated = () => NotifyToRefresh(1);
            protected When save_button_is_pressed = () => settingsViewModel.SaveSettings.ExecuteDelegate();
            protected When reload_button_is_pressed = () => settingsViewModel.ReloadSettings.ExecuteDelegate();
            protected When save_button_is_pressed_three_times = () =>
            {
                for (int i = 0; i < 3; i++)
                {
                    settingsViewModel.SaveSettings.ExecuteDelegate();
                }
            };
            protected When three_new_changesets_are_made_and_refresh_is_called = () =>
            {
                var changesets = new List<Changeset>();
                changesets.Add(new Changeset()
                {
                    Revision = 4,
                    Time = DateTime.Today.AddDays(2),
                    Comment = "Repository created2",
                    Author = new Author()
                    {
                        Username = "goeran"
                    }
                });
                changesets.Add(new Changeset()
                {
                    Revision = 5,
                    Time = DateTime.Today.AddDays(2),
                    Comment = "Added build script2",
                    Author = new Author()
                    {
                        Username = "goeran"
                    }
                });
                changesets.Add(new Changeset()
                {
                    Revision = 6,
                    Time = DateTime.Today.AddDays(2),
                    Comment = "Added unit testing framework2",
                    Author = new Author()
                    {
                        Username = "dagolap"
                    }
                });

                ChangesetRepositoryContains(changesets);
                NotifyToRefresh(1);
            };

            protected When three_new_changesets_are_made_today_and_refresh_is_called = () =>
            {
                var changesets = new List<Changeset>();
                changesets.Add(new Changeset()
                {
                    Revision = 4,
                    Time = DateTime.Today,
                    Comment = "Repository created2",
                    Author = new Author()
                    {
                        Username = "goeran"
                    }
                });
                changesets.Add(new Changeset()
                {
                    Revision = 5,
                    Time = DateTime.Today,
                    Comment = "Added build script2",
                    Author = new Author()
                    {
                        Username = "goeran"
                    }
                });
                changesets.Add(new Changeset()
                {
                    Revision = 6,
                    Time = DateTime.Today,
                    Comment = "Added unit testing framework2",
                    Author = new Author()
                    {
                        Username = "dagolap"
                    }
                });

                ChangesetRepositoryContains(changesets);
                NotifyToRefresh(1);
            };

            protected When Configuration_timespan_entry_is_changed_to_2 = () => SetupConfigRepositoryMock(2);
            protected When Configuration_timespan_entry_is_changed_to_6 = () => SetupConfigRepositoryMock(6);

            protected Context Configuration_has_been_updated = () => UpdateConfiguration();
            protected When Configuration_is_updated = () => UpdateConfiguration();

            protected static void UpdateConfiguration()
            {
                widgetMock.Raise(w => w.ConfigurationChanged += null, EventArgs.Empty);
            }

            protected static void CreateController()
            {
                backgroundWorkerInvokerMock = new Mock<IInvokeBackgroundWorker<IEnumerable<Changeset>>>();
                ITimerMock = new Mock<ITimer>();
                viewModel = new BindableViewModel<CommitStatisticsForDate>();
                controller = new CommitStatisticsController(viewModel,
                                                            settingsViewModel,
                                                            changesetRepositoryMock.Object,
                                                            new NoBackgroundWorkerInvocation<IEnumerable<Changeset>>(),
                                                            ITimerMock.Object,
                                                            new NoUIInvokation(),
                                                            configRepositoryMock.Object,
                                                            configPersisterRepositoryMock.Object,
                                                            new Logger(new LogEntryMockPersister()),
                                                            loadingNotifierMock.Object,
                                                            widgetMock.Object
                                                            );
            }

            protected static void ChangesetRepositoryContains(IEnumerable<Changeset> changesets)
            {
                changesetRepositoryMock.Setup(r => r.BeginGet(It.IsAny<Specification<Changeset>>())).
                    Callback((Specification<Changeset> specs) =>
                    {
                        changesetRepositoryMock.Raise(e => e.GetCompleted += null,
                            new GetCompletedEventArgs<Changeset>(changesets.Where(c => specs.IsSatisfiedBy(c)),
                                                   specs));
                    });

                changesetRepositoryMock.Setup(r => r.BeginGet(It.IsAny<AllChangesetsSpecification>())).
                    Raises(e => e.GetCompleted += null,
                           new GetCompletedEventArgs<Changeset>(changesets, new AllChangesetsSpecification()));
            }

            private static void NotifyToRefresh(int times)
            {
                for (int i = 0; i < times; i++)
                {
                    ITimerMock.Raise(n => n.Elapsed += null, new EventArgs());
                }
            }

            [SetUp]
            public void SetUp()
            {
                Scenario("");
                controller = null;
                viewModel = new BindableViewModel<CommitStatisticsForDate>();
                settingsViewModel = new CommitStatisticsSettingsViewModel();
                changesetRepositoryMock = new Mock<IAsyncRepository<Changeset>>();
                configRepositoryMock = new Mock<IAsyncRepository<Configuration>>();
                configPersisterRepositoryMock = new Mock<IPersistDomainModelsAsync<Configuration>>();
                loadingNotifierMock = new Mock<IProgressbar>();
                ITimerMock = new Mock<ITimer>();
                configRepositoryMock.Setup(
                     r => r.BeginGet(It.IsAny<ConfigurationByName>())).Raises(
                     t => t.GetCompleted += null, new GetCompletedEventArgs<Configuration>(new List<Configuration>(), null));
                configPersisterRepositoryMock.Setup(
                         r => r.Save(It.IsAny<Configuration>())).Raises(
                         t => t.SaveCompleted += null, new SaveCompletedEventArgs());

                widgetMock.SetupGet(w => w.Configuration).Returns(CommitStatisticsController.CreateDefaultConfig());
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
        }

        public class LogEntryMockPersister : IPersistDomainModelsAsync<LogEntry>
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

            public event EventHandler<SaveCompletedEventArgs> SaveCompleted;
        }
    }
}