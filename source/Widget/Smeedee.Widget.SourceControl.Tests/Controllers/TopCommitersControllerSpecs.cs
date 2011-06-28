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
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.Users;
using Smeedee.Widget.SourceControl.Controllers;
using Smeedee.Widget.SourceControl.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Widget.SourceControlTests.Controllers.TopCommitersControllerSpecs
{
    public class LogEntryMockPersister : IPersistDomainModels<LogEntry>
    {
        public void Save(LogEntry domainModel) { }
        public void Save(IEnumerable<LogEntry> domainModels) { }
    }

    [TestFixture]
    public class Controller_is_spawned : Shared
    {
        [Test]
        public void Assure_configuration_is_created_if_it_does_not_exist()
        {
            Given(there_are_3_users_in_userdb).
            And(configuration_entry_does_not_exist).
            And(there_are_changesets_for_2_users_in_SourceControl_system);

            When(the_Controller_is_created);

            Then("assure Configuration is created if it doesn't exist", () =>
            configPersisterRepositoryMock.Verify(r => r.Save(It.IsAny<Configuration>()), Times.Once()));
        }

        [Test]
        public void Assure_default_values_are_loaded_when_there_is_no_configuration()
        {
            Given(there_are_no_users_in_userdb).
                And(configuration_entry_does_not_exist).
                And(there_are_changesets_for_2_users_in_SourceControl_system);

            When(the_Controller_is_created);

            Then(() =>
            {
                controller.ViewModel.SinceDate.ShouldBe(DateTime.Now.Date);
                controller.ViewModel.TimeSpanInDays.ShouldBe(10);
                controller.ViewModel.IsUsingDate.ShouldBe(true);
                controller.ViewModel.MaxNumOfCommiters.ShouldBe(15);
                controller.ViewModel.AcknowledgeOthers.ShouldBe(true);
            });
        }

        [Test]
        public void Assure_settings_are_loaded_from_repository()
        {
            Given(there_are_3_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(there_are_changesets_for_2_users_in_SourceControl_system);

            When(the_Controller_is_created);

            Then( () =>
            {
                configRepositoryMock.Verify(r => r.BeginGet(It.IsAny<Specification<Configuration>>()), Times.Once());
                controller.ViewModel.TimeSpanInDays.ShouldBe(1);
                controller.ViewModel.SinceDate.ShouldBe(new DateTime(2010,01,01));
                controller.ViewModel.IsUsingDate.ShouldBe(false); 
                controller.ViewModel.MaxNumOfCommiters.ShouldBe(12);
                controller.ViewModel.AcknowledgeOthers.ShouldBe(false);
            });
        }

        [Test]
        public void Assure_actualDate_is_correct_when_using_timespan()
        {
            Given(there_are_no_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(there_are_changesets_for_2_users_in_SourceControl_system);

            When(the_Controller_is_created);

            Then( ()=> controller.ViewModel.ActualDateUsed.Date.ShouldBe(DateTime.Now.Date.AddDays(-1)));
        }

        [Test]
        public void Assure_actualDate_is_correct_when_using_date()
        {
            Given(there_are_no_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_true_numOfCom_1_ackOthers_false).
                And(there_are_changesets_for_2_users_in_SourceControl_system);

            When(the_Controller_is_created);

            Then( () => controller.ViewModel.ActualDateUsed.Date.ShouldBe(new DateTime(2010,01,01)));
        }

        [Test]
        public void Assure_IsUsingDate_and_IsUsingTimespan_have_opposite_values()
        {
            Given(there_are_no_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_true_numOfCom_1_ackOthers_false).
                And(there_are_changesets_for_2_users_in_SourceControl_system);

            When(the_Controller_is_created);

            Then( () => controller.ViewModel.IsUsingDate.ShouldNotBe(controller.ViewModel.IsUsingTimespan));
        }
        
        [Test]
        public void Assure_controller_querys_ChangesetRepository_for_all_changesets()
        {
            Given(there_are_changesets_for_2_users_in_SourceControl_system)
                .And(there_are_3_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false);

            When(the_Controller_is_created);

            Then("assure it query ChangesetRepository for all changesets", () =>
                changesetRepositoryMock.Verify(r =>r.Get(It.IsAny<Specification<Changeset>>()),Times.Once()));
        }

        [Test]
        public void Assure_number_of_commiters_is_less_than_or_equal_to_maxNumOfCommiters_when_ackOthers_is_false()
        {
            Given(there_are_3_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_true_numOfCom_1_ackOthers_false).
                And(there_are_changesets_for_2_users_in_SourceControl_system);

            When(the_Controller_is_created);

            Then(() => controller.ViewModel.Data.Count.ShouldBe(1));
        }

        [Test]
        public void Assure_others_commiter_is_created_when_ackOthers_is_true_and_num_of_commiters_is_larger_than_maxNumOfCom()
        {
            Given(there_are_3_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_true_numOfCom_1_ackOthers_true).
                And(there_are_changesets_for_2_users_in_SourceControl_system);

            When(the_Controller_is_created);

            Then(() =>
                                {
                                    controller.ViewModel.Data.Count.ShouldBe(2);
                                    controller.ViewModel.Data.Where(
                                        d => d.Firstname.Equals("Others")).First().NumberOfCommits.ShouldBe(1);
                                });
        }

        [Test]
        public void Assure_total_number_of_commits_is_calculated_when_controller_is_spawnd()
        {
            Given(there_are_3_users_in_userdb).
                And(config_with_timespan_2_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(there_are_changesets_for_2_users_in_SourceControl_system);

            When(the_Controller_is_created);

            Then(() => controller.ViewModel.NumberOfCommitsShown.ShouldBe(3));
        }

        [Test]
        public void Assure_log_entry_is_created_when_configRepo_throws_an_exception()
        {
            Given(there_are_3_users_in_userdb).
                And(config_repository_throws_an_exception);

            When(the_Controller_is_created);

            Then(() => logger.Verify(l => l.WriteEntry(It.IsAny<ErrorLogEntry>()), Times.AtLeastOnce()));
        }

        [Test]
        public void Assure_log_entry_is_created_when_config_contains_an_invalid_value()
        {
            Given(there_are_3_users_in_userdb).
                And(config_repository_returns_a_invalid_setting);

            When(the_Controller_is_created);

            Then(() => logger.Verify(l => l.WriteEntry(It.IsAny<ErrorLogEntry>()), Times.Once()));
        }
    }

    [TestFixture]
    public class Loading_data : Shared
    {
        [Test]
        public void Assure_data_loading_is_performed_on_the_specified_thread()
        {
            Given(there_are_3_users_in_userdb)
                .And(there_are_changesets_for_2_users_in_SourceControl_system)
                .And(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false)
                .And(the_Controller_is_spawned);

            When("data is loading");

            Then("assure data loading is performed on the supplied thread (this case: current thread)",() =>
                changesetRepositoryGetThreadId.ShouldBe(Thread.CurrentThread.ManagedThreadId));
        }

        [Test]
        public void Assure_the_controller_can_handle_null_author()
        {
                Given(there_are_no_users_in_userdb).
                    And(there_are_changesets_with_null_Author).
                    And(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                    And(the_Controller_is_spawned);

                When("Loading data");

                Then("No Exception should be thrown",() => logger.Verify(l => l.WriteEntry(It.IsAny<ErrorLogEntry>()), Times.Never()));
        }
    }

    [TestFixture]
    public class Data_is_loaded : Shared
    {
        [Test]
        public void Assure_data_is_loaded_into_the_viewModel()
        {
            Given(there_are_changesets_for_2_users_in_SourceControl_system).
                And(there_are_3_users_in_userdb).
                And(config_with_timespan_2_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(the_Controller_is_spawned);

            When("loadData() is run");

            Then("assure data is correctly loaded into the viewModel", () =>
            {
                controller.ViewModel.Data.Count.ShouldBe(2);
                controller.ViewModel.Data.Where(
                    d => d.Username.Equals("goeran")).First().NumberOfCommits.ShouldBe(2);
                controller.ViewModel.Data.Where(
                    d => d.Username.Equals("dagolap")).First().NumberOfCommits.ShouldBe(1);
            });
        }

        [Test]
        public void Assure_the_correct_amount_of_committs_persist_through_updates()
    {
            Given(there_are_changesets_for_2_users_in_SourceControl_system).
                And(there_are_3_users_in_userdb).
                And(config_with_timespan_2_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(the_Controller_is_spawned);

            When(three_loadData_are_run);

                Then("assure data is correctly loaded into the viewModel", () =>
            {
                controller.ViewModel.Data.Count.ShouldBe(2);
                controller.ViewModel.Data.Where(
                    d => d.Username.Equals("goeran")).First().NumberOfCommits.ShouldBe(2);
                controller.ViewModel.Data.Where(
                    d => d.Username.Equals("dagolap")).First().NumberOfCommits.ShouldBe(1);
            });
        }

        [Test]
        public void Should_use_timespan_from_config_when_loading_data()
        {
            Given(there_are_3_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(there_are_changesets_for_2_users_in_SourceControl_system).
                And(the_Controller_is_spawned);

            When("data is loaded");

            Then("the value from the configuration should be used in caluculating top committers", () =>
            {
                controller.ViewModel.Data.Count.ShouldBe(1);
                controller.ViewModel.Data[0].NumberOfCommits.ShouldBe(1);
            });
        }

        [Test]
        public void Assure_IsUsingDate_changes_when_IsUsingTimespan_changes()
        {
            var oldIsUsingDate = false;

            Given(there_are_3_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(there_are_changesets_for_2_users_in_SourceControl_system).
                And(the_Controller_is_spawned).
                And(() => oldIsUsingDate = controller.ViewModel.IsUsingDate );

            When(IsUsingTipespan_changes);

            Then( () => oldIsUsingDate.ShouldNotBe(controller.ViewModel.IsUsingDate));
        }

        [Test]
        public void Assure_IsUsingTimespan_changes_when_IsUsingDate_changes()
        {
            var oldIsUsingTimespan = false;

            Given(there_are_3_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(there_are_changesets_for_2_users_in_SourceControl_system).
                And(the_Controller_is_spawned).
                And(() => oldIsUsingTimespan = controller.ViewModel.IsUsingTimespan);

            When(IsUsingDate_changes);

            Then(() => oldIsUsingTimespan.ShouldNotBe(controller.ViewModel.IsUsingTimespan));
        }

    }

    [TestFixture]
    public class When_notified_to_refresh : Shared
    {
        [Test]
        public void Changing_NumberOfCommits_should_not_update_view_model()
        {
            var viewModelChanged = false;

            Given(there_are_changesets_for_2_users_in_SourceControl_system).
                And(there_are_3_users_in_userdb).
                And(config_with_timespan_2_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(the_Controller_is_spawned).
                And(a_new_changeset_is_committed).
                And("subscribe to ViewModel CollectionChanged", () =>
                    {
                        controller.ViewModel.Data.CollectionChanged += (o, e) => { viewModelChanged = true; };
                    });

            When(the_Controller_is_notified_to_refresh);

            Then("assure new NumberOfCommits is loaded into the viewModel", () =>
                {
                    controller.ViewModel.Data.Count.ShouldBe(2);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("goeran")).First().NumberOfCommits.ShouldBe(3);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("dagolap")).First().NumberOfCommits.ShouldBe(1);
                }).
                And("the CollectionChanged event in ViewModel should not have been fired", () => viewModelChanged.ShouldBeFalse());
        }

        [TestFixture]
        public class When_config_value_is_changed_before_notified_to_refresh : Shared
        {
            [Test]
            public void Assure_timeSpan_changed_will_update_the_ViewModel()
            {
                var timeSpanChanged = config_is_changed_to_1_2010_01_01_false_12_false;
                var viewModelChanged = false;

                Given(there_are_changesets_for_2_users_in_SourceControl_system).
                    And(there_are_3_users_in_userdb).
                    And(config_with_timespan_2_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                    And(the_Controller_is_spawned).
                    And(timeSpanChanged)

                    .And("subscribe to ViewModel CollectionChanged", () =>
                    {
                        controller.ViewModel.Data.CollectionChanged += (o, e) => { viewModelChanged = true; };
                    });

                When(the_Controller_is_notified_to_refresh);

                Then(() =>
                {
                    changesetRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Changeset>>()), Times.Exactly(2));
                    viewModelChanged.ShouldBeTrue();
                });

            }

            [Test]
            public void Assure_date_changed_will_update_the_ViewModel()
            {
                var dateChanged = config_is_changed_to_2_2010_01_02_false_12_false;
                var viewModelChanged = false;

                Given(there_are_changesets_for_2_users_in_SourceControl_system).
                    And(there_are_3_users_in_userdb).
                    And(config_with_timespan_2_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                    And(the_Controller_is_spawned).
                    And(dateChanged)

                    .And("subscribe to ViewModel CollectionChanged", () =>
                    {
                        controller.ViewModel.Data.CollectionChanged += (o, e) => { viewModelChanged = true; };
                    });

                When(the_Controller_is_notified_to_refresh);

                Then(() =>
                {
                    changesetRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Changeset>>()), Times.Exactly(2));
                    viewModelChanged.ShouldBeTrue();
                });

            }

            [Test]
            public void Assure_isUsingDate_changed_will_update_the_ViewModel()
            {
                var isUsingDateChanged = config_is_changed_to_2_2010_01_01_true_12_false;
                var viewModelChanged = false;

                Given(there_are_changesets_for_2_users_in_SourceControl_system).
                    And(there_are_3_users_in_userdb).
                    And(config_with_timespan_2_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                    And(the_Controller_is_spawned).
                    And(isUsingDateChanged)

                    .And("subscribe to ViewModel CollectionChanged", () =>
                    {
                        controller.ViewModel.Data.CollectionChanged += (o, e) => { viewModelChanged = true; };
                    });

                When(the_Controller_is_notified_to_refresh);

                Then(() =>
                {
                    changesetRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Changeset>>()), Times.Exactly(2));
                    viewModelChanged.ShouldBeTrue();
                });

            }

            [Test]
            public void Assure_numOfCommiters_changed_will_update_the_ViewModel()
            {
                var numOfCommitersChanged = config_is_changed_to_2_2010_01_01_false_10_false;
                var viewModelChanged = false;

                Given(there_are_changesets_for_2_users_in_SourceControl_system).
                    And(there_are_3_users_in_userdb).
                    And(config_with_timespan_2_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                    And(the_Controller_is_spawned).
                    And(numOfCommitersChanged)

                    .And("subscribe to ViewModel CollectionChanged", () =>
                    {
                        controller.ViewModel.Data.CollectionChanged += (o, e) => { viewModelChanged = true; };
                    });

                When(the_Controller_is_notified_to_refresh);

                Then(() =>
                {
                    changesetRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Changeset>>()), Times.Exactly(2));
                    viewModelChanged.ShouldBeTrue();
                });

            }

            [Test]
            public void Assure_ackOthers_changed_will_update_the_ViewModel()
            {
                var ackOthersChanged = config_is_changed_to_2_2010_01_01_false_12_true;
                var viewModelChanged = false;

                Given(there_are_changesets_for_2_users_in_SourceControl_system).
                    And(there_are_3_users_in_userdb).
                    And(config_with_timespan_2_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                    And(the_Controller_is_spawned).
                    And(ackOthersChanged)

                    .And("subscribe to ViewModel CollectionChanged", () =>
                    {
                        controller.ViewModel.Data.CollectionChanged += (o, e) => { viewModelChanged = true; };
                    });

                When(the_Controller_is_notified_to_refresh);

                Then(() =>
                {
                    changesetRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Changeset>>()), Times.Exactly(2));
                    viewModelChanged.ShouldBeTrue();
                });

            } 
        }

        [Test]
        public void New_user_in_changesets_should_update_viewModel()
        {
                var viewModelChanged = false;

                Given(there_are_changesets_for_2_users_in_SourceControl_system).
                    And(there_are_3_users_in_userdb).
                    And(config_with_timespan_2_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                    And(the_Controller_is_spawned).
                    And(a_new_changeset_with_a_new_user_is_committed).
                    And("subscribe to ViewModel CollectionChanged", () =>
                        {
                            controller.ViewModel.Data.CollectionChanged += (o, e) => { viewModelChanged = true; };
                        });

                When(the_Controller_is_notified_to_refresh);

                Then("the viewmodel should be updated", () =>
                {
                    controller.ViewModel.Data.Count.ShouldBe(3);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("goeran")).First().NumberOfCommits.ShouldBe(2);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("dagolap")).First().NumberOfCommits.ShouldBe(1);
                    controller.ViewModel.Data.Where(d => d.Username.Equals("heine")).First().NumberOfCommits.ShouldBe(1);
                }).
                    And("the CollectionChanged event in ViewModel should have been fired", () => viewModelChanged.ShouldBeTrue());
        }

        [Test]
        public void Assure_data_is_loaded_again_after_config_updates()
        {
            Given(there_are_changesets_for_2_users_in_SourceControl_system).
                And(there_are_3_users_in_userdb).
                And(config_with_timespan_2_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(the_Controller_is_spawned).
                And(config_is_changed_to_2_2010_01_01_false_12_true);

            When(the_Controller_is_notified_to_refresh);

            Then("the viewmodel should be updated", () =>
            {
                controller.ViewModel.Data.Count.ShouldBe(2);
                controller.ViewModel.Data.Where( d => d.Username.Equals("goeran")).First().NumberOfCommits.ShouldBe(2);
                controller.ViewModel.Data.Where( d => d.Username.Equals("dagolap")).First().NumberOfCommits.ShouldBe(1);
            });
        }

        [Test]
        public void Should_query_repository_for_changesets()
        {
            Given(there_are_changesets_for_2_users_in_SourceControl_system).
                And(there_are_3_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(the_Controller_is_spawned);

            When(the_Controller_is_notified_to_refresh);

            Then("it should query repository for changesets", () =>
                changesetRepositoryMock.Verify(r =>r.Get(It.IsAny<Specification<Changeset>>()), Times.Exactly(2)));
        }

        [Test]
        public void Unchanged_changeset_should_not_update_view_model()
        {
                var viewModelChanged = false;

                Given(there_are_changesets_for_2_users_in_SourceControl_system).
                    And(there_are_3_users_in_userdb).
                    And(config_with_timespan_1_date_2010_01_01_isUsingDate_true_numOfCom_1_ackOthers_true).
                    And(the_Controller_is_spawned).
                    And("subscribe_to_ViewModel_CollectionChanged",() =>
                        {
                            controller.ViewModel.Data.CollectionChanged += (o, e) => { viewModelChanged = true; };
                        });

                When(the_Controller_is_notified_to_refresh);

                Then("the viewmodel should not be updated", () => viewModelChanged.ShouldBeFalse());
        }

        [Test]
        public void Assure_TotalNumberOfCommits_is_updaten_when_notified_to_refresh()
        {
            Given(there_are_3_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_true_numOfCom_1_ackOthers_false).
                And(there_are_changesets_for_2_users_in_SourceControl_system).
                And(the_Controller_is_spawned).
                And(config_is_changed_to_2_2010_01_02_false_12_true);

            When(the_Controller_is_notified_to_refresh);

            Then(() => controller.ViewModel.NumberOfCommitsShown.ShouldBe(3));
        }

       [Test]
       public void After_save_notifyToRefresh_should_not_update_view_model()
       {
            var viewModelChanged = false;

            Given(there_are_changesets_for_2_users_in_SourceControl_system).
                And(there_are_3_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_true_numOfCom_1_ackOthers_true).
                And(the_Controller_is_spawned).
                And(viewModel_data_is_changed_to_2_2010_01_02_false_12_false).
                And(SaveSettings_delegate_has_been_executed).
                And(config_is_changed_to_2_2010_01_02_false_12_false).
                And("subscribe_to_ViewModel_CollectionChanged", () =>
                {
                    controller.ViewModel.Data.CollectionChanged += (o, e) => { viewModelChanged = true; };
                });

            When(the_Controller_is_notified_to_refresh);

            Then("the viewmodel should not be updated", () => { viewModelChanged.ShouldBeFalse(); });
       }

       [Test]
       public void Assure_correct_values_are_kept_in_the_viewModel_after_a_save()
       {
            Given(there_are_changesets_for_2_users_in_SourceControl_system).
                And(there_are_3_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_true_numOfCom_1_ackOthers_true).
                And(the_Controller_is_spawned).
                And(viewModel_data_is_changed_to_2_2010_01_02_false_12_false).
                And(SaveSettings_delegate_has_been_executed).
                And(config_is_changed_to_2_2010_01_02_false_12_false);

            When(the_Controller_is_notified_to_refresh);

            Then("the viewmodel should not be updated",() => 
                {
                    controller.ViewModel.TimeSpanInDays.ShouldBe(2);
                    controller.ViewModel.SinceDate.ShouldBe(new DateTime(2010,01,02));
                    controller.ViewModel.IsUsingDate.ShouldBe(false);
                    controller.ViewModel.MaxNumOfCommiters.ShouldBe(12);
                    controller.ViewModel.AcknowledgeOthers.ShouldBe(false);
                });
       }
    }

    [TestFixture]
    public class When_reload_from_repository_delegate_is_executed : Shared
    {
        [Test]
        public void Assure_Settings_is_loaded_from_repository()
        {
            Given(there_are_3_users_in_userdb).
                And(config_with_timespan_1_date_2010_01_01_isUsingDate_true_numOfCom_1_ackOthers_false).
                And(there_are_changesets_for_2_users_in_SourceControl_system).
                And(the_Controller_is_spawned).
                And(config_is_changed_to_2_2010_01_02_false_12_true);

            When(ReloadFromRepository_delegate_is_executed);

            var dateInRepository = new DateTime(2010, 01, 02);

            Then(() =>
                {
                    controller.ViewModel.TimeSpanInDays.ShouldBe(2);
                    controller.ViewModel.SinceDate.ShouldBe(dateInRepository);
                    controller.ViewModel.IsUsingDate.ShouldBe(false);
                    controller.ViewModel.MaxNumOfCommiters.ShouldBe(12);
                    controller.ViewModel.AcknowledgeOthers.ShouldBe(true);
                });
        }
    }

    [TestFixture]
    public class When_save_delegate_is_executed : Shared
    {
        [Test]
        public void Asssure_save_command_is_executed()
        {
            Given(there_are_3_users_in_userdb)
                .And(mock_is_initialized_to_1_2010_01_01_false_12_false)
                .And(there_are_changesets_for_2_users_in_SourceControl_system)
                .And(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false)
                .And(the_Controller_is_spawned);

            When(SaveSettings_delegate_is_executed);

            Then("the mock should recieve exactly 1 save", () => 
                configPersisterRepositoryMock.Verify(c => c.Save(It.IsAny<Configuration>()), Times.Exactly(1)));
        }

        [Test]
        public void Assure_the_correct_values_are_saved()
        {
            Given(there_are_3_users_in_userdb)
                .And(mock_is_initialized_to_1_2010_01_01_false_12_false)
                .And(there_are_changesets_for_2_users_in_SourceControl_system)
                .And(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false)
                .And(the_Controller_is_spawned);

            When(SaveSettings_delegate_is_executed);

            Then("The mock should get a save with the correct values");
        }
    }

    [TestFixture]
    public class When_loading_and_saving_data_and_settings : Shared
    {

        [Test]
        public void Assure_progressbar_is_shown_while_loading_data()
        {
            Given(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(the_Controller_is_spawned);

            When(the_Controller_is_notified_to_refresh);

            Then("the loadingNotifyer should be shown during spawn and on refresh", () =>
            {
                progressbarMock.Verify(l => l.ShowInView(It.IsAny<string>()), Times.Exactly(2));
                //controller.ViewModel.IsLoading.ShouldBe(true); when async get is implemented
            });
        }

        [Test]
        public void Assure_progressbar_is_hidden_after_loading_data()
        {
            Given(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false);

            When(the_Controller_is_created);

            Then("", () =>
            {
                progressbarMock.Verify(l => l.HideInView(), Times.Once());
                controller.ViewModel.IsLoading.ShouldBe(false);
            });
        }

        [Test]
        public void Assure_progressbar_is_shown_while_saving_settings()
        {
            Given(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(the_Controller_is_spawned);

            When(SaveSettings_delegate_is_executed);

            Then(() =>
            {
                progressbarMock.Verify(l => l.ShowInSettingsView(It.IsAny<string>()), Times.Exactly(1)); 
                controller.ViewModel.IsSaving.ShouldBe(true);
            });
        }

        [Test]
        public void Assure_progressbar_is_hidden_after_saving_settings()
        {
            Given(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(the_Controller_is_spawned).
                And(configPersisterRepositoryMock_setup_to_return_savecomplete).
                And(SaveSettings_delegate_has_been_executed);

            When("allways");

            Then(() =>
            {
                progressbarMock.Verify(l => l.HideInSettingsView(), Times.Exactly(1));
                controller.ViewModel.IsSaving.ShouldBe(false);
            });
        }

        [Test]
        public void Assure_progressbar_is_shown_while_loading_settings()
        {
            Given(configRepository_does_not_return_GetCompleted);

            When(the_Controller_is_created);

            Then(() =>
            {
                progressbarMock.Verify(l => l.ShowInBothViews(It.IsAny<string>()), Times.Exactly(1));
                controller.ViewModel.IsLoadingConfig.ShouldBe(true);
            });
        }

        [Test]
        public void Assure_progressbar_is_hidden_after_loading_settings()
        {
            Given(config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false).
                And(the_Controller_is_spawned).
                And(configPersisterRepositoryMock_setup_to_return_savecomplete);

            When(ReloadFromRepository_delegate_is_executed);

            Then(() =>
            {
                progressbarMock.Verify(l => l.HideInBothViews(), Times.Exactly(3));
                controller.ViewModel.IsLoadingConfig.ShouldBe(false);
            });
        } 
    }

    public class Shared : ScenarioClass
    {
        protected static int changesetRepositoryGetThreadId;
        protected static Mock<IRepository<Changeset>> changesetRepositoryMock = new Mock<IRepository<Changeset>>();
        protected static TopCommitersController controller;
        protected static Mock<ITimer> ITimerMock;
        protected static Mock<IRepository<User>> userRepositoryMock = new Mock<IRepository<User>>();
        protected static Mock<ILog> logger;
        protected static Mock<IPersistDomainModelsAsync<Configuration>> configPersisterRepositoryMock = new Mock<IPersistDomainModelsAsync<Configuration>>();
        protected static Mock<IAsyncRepository<Configuration>> configRepositoryMock = new Mock<IAsyncRepository<Configuration>>();
        protected static Mock<IProgressbar> progressbarMock = new Mock<IProgressbar>();

        private const string SETTINGS_ENTRY_NAME = "TopCommiters";
        private const string DATE_ENTRY_NAME = "SinceDate";
        private const string TIMESPAN_ENTRY_NAME = "TimeSpanInDays";
        private const string IS_USING_DATE_ENTRY_NAME = "IsUsingDate";
        private const string MAX_NUM_OF_COMMITERS_ENTRY_NAME = "MaxNumOfCommiters";
        private const string ACKNOWLEDGE_OTHERS_ENTRY_NAME = "AcknowledgeOthers";

        protected When the_Controller_is_created = CreateController;
        protected Context the_Controller_is_spawned = CreateController;

        protected When SaveSettings_delegate_is_executed = () => controller.ViewModel.SaveSettings.Execute(null);
        protected Context SaveSettings_delegate_has_been_executed = () => controller.ViewModel.SaveSettings.Execute(null);
        protected When ReloadFromRepository_delegate_is_executed = () => controller.ViewModel.ReloadFromRepository.Execute(null);

        protected When IsUsingTipespan_changes = () => controller.ViewModel.IsUsingTimespan ^= true;
        protected When IsUsingDate_changes = () => controller.ViewModel.IsUsingDate ^= true;

        protected When the_Controller_is_notified_to_refresh = () => NotifyToRefresh(1);

        protected When three_loadData_are_run = () => NotifyToRefresh(3);

        protected When timespan_in_ViewModel_is_changed_to_1 = () => controller.ViewModel.TimeSpanInDays = 1;

        protected Context viewModel_data_is_changed_to_2_2010_01_02_false_12_false = () =>
        {
            controller.ViewModel.TimeSpanInDays = 2;
            controller.ViewModel.SinceDate = new DateTime(2010, 01, 02);
            controller.ViewModel.IsUsingDate = false;
            controller.ViewModel.MaxNumOfCommiters = 12;
            controller.ViewModel.AcknowledgeOthers = false;
        };

        protected Context config_repository_throws_an_exception = () =>
        {
            configRepositoryMock.Setup(r => r.BeginGet(It.IsAny<Specification<Configuration>>())).Throws(new Exception());
        };

        protected Context config_repository_returns_a_invalid_setting = () => SetupConfigRepositoryMockWithInvalidValues();

        protected Context date_is_changed_to_2010_06_30 = () => controller.ViewModel.SinceDate = new DateTime(2010, 06, 30);

        protected Context config_with_timespan_1_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false = () =>
        {
            SetupConfigRepositoryMock(1, new DateTime(2010, 01, 01), false, 12, false);
        };

        protected Context config_with_timespan_2_date_2010_01_01_isUsingDate_false_numOfCom_12_ackOthers_false = () =>
        {
            SetupConfigRepositoryMock(2, new DateTime(2010, 01, 01), false, 12, false);
        };

        protected Context config_with_timespan_1_date_2010_01_01_isUsingDate_true_numOfCom_1_ackOthers_false = () =>
        {
            SetupConfigRepositoryMock(1, new DateTime(2010, 01, 01), true, 1, false);
        };

        protected Context config_with_timespan_1_date_2010_01_01_isUsingDate_true_numOfCom_1_ackOthers_true = () =>
        {
            SetupConfigRepositoryMock(1, new DateTime(2010, 01, 01), true, 1, true);
        };

        protected Context config_is_changed_to_1_2010_01_01_false_12_false = () =>
        {
            SetupConfigRepositoryMock(1, new DateTime(2010, 01, 01), false, 12, false);
        };

        protected Context config_is_changed_to_2_2010_01_02_false_12_false = () =>
        {
            SetupConfigRepositoryMock(2, new DateTime(2010, 01, 02), false, 12, false);
        };

        protected Context config_is_changed_to_2_2010_01_01_true_12_false = () =>
        {
            SetupConfigRepositoryMock(2, new DateTime(2010, 01, 01), true, 12, false);
        };

        protected Context config_is_changed_to_2_2010_01_01_false_10_false = () =>
        {
            SetupConfigRepositoryMock(2, new DateTime(2010, 01, 01), false, 10, false);
        };

        protected Context config_is_changed_to_2_2010_01_01_false_12_true = () =>
        {
            SetupConfigRepositoryMock(2, new DateTime(2010, 01, 01), false, 12, true);
        };

        protected Context config_is_changed_to_2_2010_01_02_false_12_true = () =>
        {
            SetupConfigRepositoryMock(2, new DateTime(2010, 01, 02), false, 12, true);
        };
        
        protected Context configuration_entry_does_not_exist = () =>
        {
            var configList = new List<Configuration>();

            configRepositoryMock.Setup(r => r.BeginGet(It.IsAny<Specification<Configuration>>())).
                Raises(t => t.GetCompleted += null, new GetCompletedEventArgs<Configuration>(configList, null));
        };

        protected Context configPersisterRepositoryMock_setup_to_return_savecomplete = () =>
        {
            configPersisterRepositoryMock.Setup(r => r.Save(It.IsAny<Configuration>())).Raises(t => t.SaveCompleted += null, new SaveCompletedEventArgs());
        };

        protected Context configRepository_does_not_return_GetCompleted = () =>
        {
            configRepositoryMock = new Mock<IAsyncRepository<Configuration>>();
        };

        protected Context a_new_changeset_is_committed = () =>
        {
            List<Changeset> changesets = GenerateChangesetData();

            changesets.Add(new Changeset
            {
                Revision = 4,
                Time = DateTime.Now,
                Comment = "Added support for superfeature",
                Author = new Author { Username = "goeran" }
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
                Author = new Author { Username = "heine" }
            });

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).
                Returns((Specification<Changeset> spec) => changesets.Where(c=>spec.IsSatisfiedBy(c))).Callback(
                () => changesetRepositoryGetThreadId = Thread.CurrentThread.ManagedThreadId);
        };

        protected Context there_are_changesets_for_2_users_in_SourceControl_system = () =>
        {
            changesetRepositoryMock = new Mock<IRepository<Changeset>>();

            changesetRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).
                Returns((Specification<Changeset> spec) => GenerateChangesetData().Where(c => spec.IsSatisfiedBy(c))).Callback(
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

        protected Context there_are_3_users_in_userdb = () =>
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


        protected Context mock_is_initialized_to_1_2010_01_01_false_12_false = () => 
            configPersisterRepositoryMock.Setup(p => p.Save(It.IsAny<Configuration>())).Callback((Configuration config) =>
                    {
                        config.ContainsSetting(TIMESPAN_ENTRY_NAME).ShouldBeTrue();
                        config.ContainsSetting(DATE_ENTRY_NAME).ShouldBeTrue();
                        config.ContainsSetting(IS_USING_DATE_ENTRY_NAME).ShouldBeTrue();
                        config.ContainsSetting(MAX_NUM_OF_COMMITERS_ENTRY_NAME).ShouldBeTrue();
                        config.ContainsSetting(ACKNOWLEDGE_OTHERS_ENTRY_NAME).ShouldBeTrue();
                        config.ContainsSetting("").ShouldBeFalse();

                        config.GetSetting(DATE_ENTRY_NAME).Value.ShouldBe("1/1/2010 12:00:00 AM");
                        config.GetSetting(TIMESPAN_ENTRY_NAME).Value.ShouldBe("1");
                        config.GetSetting(IS_USING_DATE_ENTRY_NAME).Value.ShouldBe("False");
                        config.GetSetting(MAX_NUM_OF_COMMITERS_ENTRY_NAME).Value.ShouldBe("12");
                        config.GetSetting(ACKNOWLEDGE_OTHERS_ENTRY_NAME).Value.ShouldBe("False");
                    });

        private static List<Changeset> GenerateChangesetData()
        {
            var changesets = new List<Changeset>();

            changesets.Add(new Changeset
            {
                Revision = 1,
                Time = DateTime.Now.AddDays(-2),
                Comment = "Repository created",
                Author = new Author { Username = "goeran" }
            });
            changesets.Add(new Changeset
            {
                Revision = 2,
                Time = DateTime.Now.AddDays(-1),
                Comment = "Added build script",
                Author = new Author { Username = "goeran" }
            });
            changesets.Add(new Changeset
            {
                Revision = 3,
                Time = DateTime.Now.AddDays(-2),
                Comment = "Added unit testing framework",
                Author = new Author { Username = "dagolap" }
            });

            return changesets;
        }

        private static void SetupConfigRepositoryMock(int timespan, DateTime date, bool isUsingDate, int numberOfCommiters, bool acknowledgeOthers)
        {
            var configuration = new Configuration(SETTINGS_ENTRY_NAME);

            configuration.NewSetting(DATE_ENTRY_NAME, date.ToString(new CultureInfo("en-US")));
            configuration.NewSetting(TIMESPAN_ENTRY_NAME, timespan.ToString());
            configuration.NewSetting(IS_USING_DATE_ENTRY_NAME, isUsingDate.ToString());
            configuration.NewSetting(MAX_NUM_OF_COMMITERS_ENTRY_NAME, numberOfCommiters.ToString());
            configuration.NewSetting(ACKNOWLEDGE_OTHERS_ENTRY_NAME, acknowledgeOthers.ToString());

            var configList = new List<Configuration> { configuration };

            configRepositoryMock.Setup(r => r.BeginGet(It.IsAny<Specification<Configuration>>())).
                Raises(t => t.GetCompleted += null, new GetCompletedEventArgs<Configuration>(configList, null));
        }

        private static void SetupConfigRepositoryMockWithInvalidValues()
        {
            var configuration = new Configuration(SETTINGS_ENTRY_NAME);

            configuration.NewSetting(DATE_ENTRY_NAME, "INVALID DATE");
            configuration.NewSetting(TIMESPAN_ENTRY_NAME, "2");
            configuration.NewSetting(IS_USING_DATE_ENTRY_NAME, "False");
            configuration.NewSetting(MAX_NUM_OF_COMMITERS_ENTRY_NAME, "12");
            configuration.NewSetting(ACKNOWLEDGE_OTHERS_ENTRY_NAME, "False");

            var configList = new List<Configuration> { configuration };

            configRepositoryMock.Setup(r => r.BeginGet(It.IsAny<Specification<Configuration>>())).
                Raises(t => t.GetCompleted += null, new GetCompletedEventArgs<Configuration>(configList, null));
        }

        private static void CreateController()
        {
            ITimerMock = new Mock<ITimer>();

            controller = new TopCommitersController(new BindableViewModel<CodeCommiterViewModel>(),
                                                    changesetRepositoryMock.Object,
                                                    new NoBackgroundWorkerInvocation<IEnumerable<Changeset>>(),
                                                    ITimerMock.Object,
                                                    new NoUIInvokation(),
                                                    configRepositoryMock.Object,
                                                    configPersisterRepositoryMock.Object,
                                                    userRepositoryMock.Object,
                                                    logger.Object,
                                                    progressbarMock.Object
                                                    );
        }

        private static void NotifyToRefresh(int times)
        {
            for (int i = 0; i < times; i++)
            {
                ITimerMock.Raise(n => n.Elapsed += null, new EventArgs());
            }
        }

        [SetUp]
        public void Setup()
        {
            configPersisterRepositoryMock = new Mock<IPersistDomainModelsAsync<Configuration>>();
            configRepositoryMock = new Mock<IAsyncRepository<Configuration>>();
            logger = new Mock<ILog>();
            progressbarMock = new Mock<IProgressbar>();
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }
}