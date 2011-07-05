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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.Tests;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.Users;
using Smeedee.Tests;
using Smeedee.Widget.SourceControl.Controllers;
using Smeedee.Widget.SourceControl.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widget.SourceControl.Tests.Controllers
{
   
    [TestFixture]
    public class When_controller_is_spawned : Shared
    {
        [Test]
        public void Assure_no_exceptions_with_empty_database()
        {
            Given("No database");
            When(the_controller_is_created);
            Then("assure no exceptions are caught",
                () => loggerMock.Verify(r => r.WriteEntry(It.IsAny<ErrorLogEntry>()), Times.Never()));
        }

        [Test]
        public void Should_load_changeset_data_into_the_ViewModel()
        {
                Given(there_are_changesets_in_sourcecontrol).
                    And(user_info_exist_in_userdb);

                When(the_controller_is_created);

                Then("changeset data should be loaded into the ViewModel", () =>
                {
                    //viewModel.Message.ShouldBe("Added hello world method");
                    //viewModel.Date.ShouldBe(new DateTime(1986, 5, 20));
                    viewModel.Changesets.Count.ShouldNotBe(0);
                    viewModel.Changesets.First().Message.ShouldBe("Added hello world method");
                    viewModel.Changesets.First().Date.ShouldBe(new DateTime(1986, 5, 20).ToLocalTime());
                });
        }

        [Test]
        public void Should_load_userinfo_data_into_the_ViewModel()
        {
            Given(there_are_changesets_in_sourcecontrol).
                    And(user_info_exist_in_userdb);

                When(the_controller_is_created);

                Then("user info data should be loaded into the ViewModel", () =>
                {
                    Person developer = viewModel.Changesets.First().Developer;
                    developer.Name.ShouldBe("Gøran Hansen");
                    developer.Email.ShouldBe("mail@goeran.no");
                    developer.ImageUrl.ShouldBe("http://goeran.no/avatar.jpg");
                });
        }

        [Test]
        public void Should_query_Changeset_repository_for_newest_changeset()
        {
            Given(there_are_changesets_in_sourcecontrol)
                    .And(user_info_exist_in_userdb);

                When(the_controller_is_created);

                Then(assure_it_queried_Changeset_Repository_for_the_newest_changeset);
        }

        [Test]
        public void Should_query_userdb_repository_for_userinfo()
        {
                Given(there_are_changesets_in_sourcecontrol).
                    And(user_info_exist_in_userdb);

                When(the_controller_is_created);
    
                Then("it should fetch user info",() => 
                    userRepositoryMock.Verify(r => r.Get(It.IsAny<AllSpecification<User>>()), Times.Once()));
        }

        [Test]
        public void Should_query_config_repository_for_initial_settings()
        {
            Given(there_are_config_settings_in_db);
            When(the_controller_is_created);
            Then("the settings should be fetched", () => 
                configRepositoryMock.Verify(r => r.Get(It.IsAny<ConfigurationByName>()), Times.AtLeastOnce()));
        }
    }

    [TestFixture]
    public class When_notified_to_refresh : Shared
    {

        [Test]
        public void Should_query_Changeset_repository_for_newest_changeset()
        {
            
                Given(there_are_changesets_in_sourcecontrol).
                    And(user_info_exist_in_userdb).
                    And(the_controller_has_been_created);

                When("notified to refresh", () => ITimerMock.Raise(n => n.Elapsed += null,new EventArgs()));

                Then("assure it queried Changeset Repository for the newest changeset", 
                    () => repositoryMock.Verify(r => r.Get(It.IsAny<Specification<Changeset>>()),Times.Exactly(2)));
        }

        [Test]
        public void Assure_refresh_does_not_throw_exception()
        {
            Given(the_controller_has_been_created);
            When("notified to refresh", () => ITimerMock.Raise(n => n.Elapsed += null,new EventArgs()));
            Then("assure no exceptions are caught", 
                () => loggerMock.Verify(r => r.WriteEntry(It.IsAny<ErrorLogEntry>()), Times.Never()));
        }
        [Test]
        public void Assure_five_refresh_does_not_throw_exception()
        {
            Given(the_controller_has_been_created);
            When("notified to refresh", () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    ITimerMock.Raise(n => n.Elapsed += null, new EventArgs());
                }

            });
            Then("assure no exceptions are caught",
                () => loggerMock.Verify(r => r.WriteEntry(It.IsAny<ErrorLogEntry>()), Times.Never()));
        }

        [Test]
        public void Assure_db_with_nonrelevant_config_does_not_throw_exception()
        {
            Given(the_controller_has_been_created).And(db_with_nonrelevant_configdata);
            When("notified to refresh", () => ITimerMock.Raise(n => n.Elapsed += null, new EventArgs()));
            Then("assure no exceptions are caught",
                () => loggerMock.Verify(r => r.WriteEntry(It.IsAny<ErrorLogEntry>()), Times.Never()));
        }
    }

    [TestFixture]
    public class When_controller_is_spawned_and_Changeset_repository_impl_is_invalid : Shared
    {
        private Context Changeset_repository_return_a_null_reference_value = () =>
        {
            repositoryMock = new Mock<IRepository<Changeset>>();
            repositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(
                new Object() as IEnumerable<Changeset>);
        };

        [Test]
        [Ignore]
        public void
            Assure_ImplementationViolationException_is_thrown_if_Changeset_repository_return_a_null_reference()
        {
            
                Given("there are no changesets in sourcecontrol").
                    And(Changeset_repository_return_a_null_reference_value).
                    And(user_info_exist_in_userdb);

                When("the controller is created");

                Then("assure ImplementationViolationException is thrown",
                              () => this.ShouldThrowException<Exception>(
                                        () => the_controller_is_created(),
                                        exception =>
                                        exception.Message.ShouldBe(
                                            "Violation of IChangesetRepository. Does not accept a null reference as a return value.")));
        }
    }

    [TestFixture]
    public class When_controller_is_spawned_and_there_are_no_changesets_in_sourceControl : Shared
    {
        private Context There_are_no_changesets_in_sourceControl = () =>
        {
            repositoryMock = new Mock<IRepository<Changeset>>();
            var changesets = new List<Changeset>();
            repositoryMock.Setup(r => r.Get(new AllChangesetsSpecification())).Returns(changesets);
        };

        [Test]
        public void assure_that_changesetsToDisplay_gets_its_value_default_value()
        {
            Given(the_controller_has_been_created);
            When("always");
            Then(
                () => viewModel.NumberOfCommits.ShouldBe(viewModel.NumberOfCommits));
        }

        [Test]
        public void Assure_userinfo_is_not_loaded_into_the_ViewModel()
        {
            Given(There_are_no_changesets_in_sourceControl).
                    And(user_info_exist_in_userdb);

                When(the_controller_is_created);

                Then("assure userinfo is not loaded into the viewModel",
                              () => { viewModel.Changesets.Count.ShouldBe(0); });
        }
    }

    [TestFixture]
    public class When_loading_data : Shared
    {
        // TODO: Move to new tests in the Framework
        [Test]
        public void Assure_state_in_ViewModel_is_set_to_Loading() {}

        //[Test]
        //public void Assure_DisplayLoadingAnimationCmdPublisher_is_notified()
        //{
        //    Start
        //    {
        //        Given(there_are_changesets_in_sourcecontrol).
        //            And(user_info_exist_in_userdb).
        //            And(controller_is_created);

        //        When("loading data");

        //        Then("assure DisplayLoadingAnimationCmdPublisher is notified", () =>
        //            displayLoadingAnimationCmdPublisherMock.Verify(cp => cp.Notify(), Times.Once()));
        //    });
        //}

        //[Test]
        //public void Assure_HideLoadingAnimatioCmdPublisher_is_notified()
        //{
        //    Start
        //    {
        //        Given(there_are_changesets_in_sourcecontrol).
        //            And(user_info_exist_in_userdb).
        //            And(controller_is_created);

        //        When("loading data");

        //        Then("assure HideLoadingAnimationCmdPublisher is notified", () =>
        //            hideLoadingAnimationCmdPublisherMock.Verify(cp => cp.Notify(), Times.Once()));
        //    });
        //}
    }

    [TestFixture]
    public class When_data_is_loaded : Shared
    {

        [Test]
        public void Assure_only_new_changesets_are_added_to_viewModel()
        {
            Given(there_are_changesets_in_sourcecontrol).
                    And(user_info_exist_in_userdb).
                    And(the_controller_has_been_created).
                    And("and data is loaded");

                When("data is reloaded from repository");


                Then("assure only new changesets are added to the viewmodel", () =>
                {
                    ObservableCollection<ChangesetViewModel> oldChangesets = viewModel.Changesets;
                    var oldFirstChangeset = oldChangesets.First();

                    var changesets = new List<Changeset>();
                    changesets.Add(new Changeset() //this changeset already exists in db.
                    {
                        Revision = 201222,
                        Comment = "Added hello world method",
                        Time = new DateTime(1986, 5, 20),
                        Author = new Author()
                        {
                            Username = "goeran"
                        }
                    });
                    changesets.Add(new Changeset()
                    {
                        Revision = 201223,
                        Comment = "New thing with comments",
                        Time = new DateTime(1986, 5, 23),
                        Author = new Author()
                        {
                            Username = "jask"
                        }
                    });
                    repositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(changesets);

                    ITimerMock.Raise(n => n.Elapsed += null, new EventArgs());

                    viewModel.Changesets.Count.ShouldBe(2);
                    oldFirstChangeset.ShouldBeSameAs(viewModel.Changesets.Last());
                });
        }

        [Test]
        public void Assure_state_in_ViewModel_is_set_to_Ready()
        {
            Given(there_are_changesets_in_sourcecontrol).
                    And(user_info_exist_in_userdb).
                    And(the_controller_has_been_created);

                When("data is loaded");

                Then("assure state in ViewModel is changed to Ready",
                              () => viewModel.IsLoading.ShouldBeFalse());
        }

        [Test]
        public void Assure_the_number_of_changesets_loaded_into_the_viewModel_equals_NumberOfCommits()
        {
            Given(there_are_10_changesets_in_Changesets)
                .And(the_controller_has_been_created)
                .And("the number of commits to show is set to 4", () => viewModel.NumberOfCommits = 4)
                .And("this number of commits is loaded into the view model", () =>
                    ITimerMock.Raise(t => t.Elapsed += null, new EventArgs()));

            When("the number of commits is increased", () =>
            {
                viewModel.NumberOfCommits = 9;
                viewModel.SaveSettings.ExecuteDelegate();
            });

            Then("the number of displayed commits should have been increased", () =>
            {
                viewModel.Changesets.Count.ShouldBe(9);
            });
        }

    }

    [TestFixture]
    public class When_settings_are_changed : Shared
    {

        [Test]
        public void Assure_that_the_amount_of_changesets_equals_NumberOfCommitts()
        {
            Given(the_controller_has_been_created).And(there_are_10_changesets_in_Changesets);

            When(numberOfCommitts_is_changed_to_five);

            Then(() => controller.ViewModel.Changesets.Count.ShouldBe(5));
        }

        [Test]
        public void Assure_DbSettingsHasChanged_method_returns_true_when_Db_settings_are_unequal_to_the_viewModels_settings()
        {
            Given(the_controller_has_been_created);

            When("viewmodel has different settings from db", () => SetConfigOnViewModel(GenerateSettings(11, true)));

            Then(() => controller.DbSettingsIsChanged(GenerateSettings(11, false)).ShouldBeTrue());
        }

        [Test]
        public void Assure_DbSettingsHasChanged_method_returns_false_when_Db_settings_are_equal_to_the_viewModels_settings()
        {
            Given(the_controller_has_been_created);

            When("viewmodel has same settings as db", () => SetConfigOnViewModel(GenerateSettings(10, true)));

            Then(() => controller.DbSettingsIsChanged(GenerateSettings(10, true)).ShouldBeFalse());
        }

        private static void SetConfigOnViewModel(Configuration configExample)
        {
            controller.ViewModel.NumberOfCommits = int.Parse(configExample.GetSetting("numberOfCommits").Value);
            controller.ViewModel.BlinkWhenNoComment = bool.Parse(configExample.GetSetting("blinkOnBlankComment").Value);
        }
    }
    
    [TestFixture]
    public class When_settings_are_loaded_and_saved_to_db : Shared
    {

        [Test]
        public void Assure_that_a_change_in_db_settings_lead_to_changes_in_ViewModel()
        {
            Given(the_controller_has_been_created).And(there_are_config_settings_in_db);

            When(refresh_is_triggered);

            Then(() => controller.ViewModel.NumberOfCommits.ShouldBe(40));
        }

        [Test]
        public void Assure_settings_are_saved_to_the_db_after_save_command_when_changes_are_done()
        {
            Given(the_controller_has_been_created).And(settings_is_changed_in_viewmodel);

            When(save_button_is_pressed);

            Then(() => configPersisterMock.Verify(r => r.Save(It.IsAny<Configuration>()), Times.Once()));
        }

        [Test]
        public void Assure_that_change_in_color_settings_in_db_lead_to_changes_in_ViewModel()
        {
            Given(the_controller_has_been_created).And(the_keyword_fix_is_bound_to_green_in_settings_db);

            When(refresh_is_triggered);

            Then(() =>
            {
                controller.ViewModel.KeywordList.Count.ShouldBe(1);
                controller.ViewModel.KeywordList[0].Keyword.ShouldBe("fix");
                controller.ViewModel.KeywordList[0].ColorName.ShouldBe("green");
            });

        }
    }

    [TestFixture]
    public class When_reload_settings_is_pressed : Shared
    {
        [Test]
        public void Assure_nothing_happens_when_no_changes_are_made()
        {
            Given(the_controller_has_been_created);

            When(reload_settings_button_is_pressed);

            Then("nothing is changed",
                () =>
                {
                    viewModel.BlinkWhenNoComment.ShouldBeFalse();
                    viewModel.KeywordList.ShouldBeEmpty();
                    viewModel.NumberOfCommits.ShouldBe(8);
                }
            );
        }

        [Test]
        public void Assure_number_of_commits_is_reset_if_changed_and_not_saved()
        {
            Given(the_controller_has_been_created).
                And(number_of_commits_has_been_changed);

            When(reload_settings_button_is_pressed);

            Then("numberOfCommits should be back to default 8", 
                () => viewModel.NumberOfCommits.ShouldBe(8));
        }

        [Test]
        public void Assure_number_of_commits_is_saved_and_reloades_correctly()
        {
            Given(the_controller_has_been_created).
                And(number_of_commits_has_been_changed).
                And(save_button_has_been_pressed);

            When(reload_settings_button_is_pressed);

            Then("numberOfCommits should be saved and not changed when reload is pressed",
                 () => viewModel.NumberOfCommits.ShouldBe(42));
        }

        [Test]
        public void Assure_blink_when_no_comment_is_reset_if_changed_and_not_saved()
        {
            Given(the_controller_has_been_created).
                And(blink_when_no_comment_has_been_changed);

            When(reload_settings_button_is_pressed);

            Then("blinkWhenNoComment should be back to default false", 
                () => viewModel.BlinkWhenNoComment.ShouldBeFalse());
        }

        [Test]
        public void Assure_blink_when_no_comment_is_saved_and_reloades_correctly()
        {
            Given(the_controller_has_been_created).
                And(blink_when_no_comment_has_been_changed).
                And(save_button_has_been_pressed);

            When(reload_settings_button_is_pressed);

            Then("blinkWhenNoComment should be saved and not changed when reload is pressed",
                 () => viewModel.BlinkWhenNoComment.ShouldBeTrue());
        }

        [Test]
        public void Assure_keywordlist_is_reset_if_changed_and_not_saved()
        {
            Given(the_controller_has_been_created).
                And(fix_green_binding_has_been_added_to_keywordlist);

            When(reload_settings_button_is_pressed);

            Then("keywordlist should be empty", 
                () => viewModel.KeywordList.ShouldBeEmpty());
        }

        [Test]
        public void Assure_keywordcolorpair_is_saved_and_reloades_correctly()
        {
            Given(the_controller_has_been_created).
                And(fix_green_binding_has_been_added_to_keywordlist).
                And(save_button_has_been_pressed);

            When(reload_settings_button_is_pressed);

            Then(keywordlist_contains_fix_green_binding);
        }

        [Test]
        public void Assure_two_keywordcolorpair_is_saved_and_reloades_correctly()
        {
            Given(the_controller_has_been_created).
                And(fix_green_binding_has_been_added_to_keywordlist).
                And(gul_yellow_binding_has_been_added_to_keywordlist).
                And(save_button_has_been_pressed);

            When(reload_settings_button_is_pressed);

            Then(keywordlist_contains_fix_green_and_gul_yellow_binding);
        }

        [Test]
        public void Assure_two_keywordcolorpair_is_saved_and_reloades_correctly_if_the_first_one_is_deleted_but_already_saved()
        {
            Given(the_controller_has_been_created).
                And(fix_green_binding_has_been_added_to_keywordlist).
                And(gul_yellow_binding_has_been_added_to_keywordlist).
                And(save_button_has_been_pressed).
                And(fix_green_binding_has_been_deleted);

            When(reload_settings_button_is_pressed);

            Then(keywordlist_contains_fix_green_and_gul_yellow_binding);
        }

        [Test]
        public void Assure_that_keywordcolorpair_is_reloaded_correctly()
        {
            Given(the_controller_has_been_created).
                And(fix_green_binding_has_been_added_to_keywordlist).
                And(save_button_has_been_pressed).
                And(the_keyword_fix_has_been_changed);

            When(reload_settings_button_is_pressed);

            Then("the_keyword_should_be_fix_again", 
                () => viewModel.KeywordList[0].Keyword.ShouldBe("fix"));


        }

        [Test]
        public void Assure_that_keywordcolorpair_is_removed_when_keyword_is_set_to_an_empty_string()
        {
            Given(the_controller_has_been_created).
                And(fix_green_binding_has_been_added_to_keywordlist);

            When(fix_keyword_is_set_to_an_empty_string);

            Then("keyword list should be empty", 
                () => viewModel.KeywordList.Count.ShouldBe(0));
        }


        private When reload_settings_button_is_pressed = () => viewModel.ReloadSettings.ExecuteDelegate();
        private Context number_of_commits_has_been_changed = () => viewModel.NumberOfCommits = 42;
        private Context save_button_has_been_pressed = () => viewModel.SaveSettings.ExecuteDelegate();
        private Context blink_when_no_comment_has_been_changed = () => viewModel.BlinkWhenNoComment = true;
        
        private Context fix_green_binding_has_been_added_to_keywordlist = 
            () => viewModel.KeywordList.Add(new KeywordColorPairViewModel { Keyword = "fix", ColorName = "green", KeywordChanged = controller.KeywordChangedHandler});
        protected Context gul_yellow_binding_has_been_added_to_keywordlist =
            () => viewModel.KeywordList.Add(new KeywordColorPairViewModel { Keyword = "gul", ColorName = "yellow", KeywordChanged = controller.KeywordChangedHandler});

        private Context fix_green_binding_has_been_deleted = () => viewModel.KeywordList.RemoveAt(0);
        private Context the_keyword_fix_has_been_changed = () => viewModel.KeywordList[0].Keyword = "change";


        private When fix_keyword_is_set_to_an_empty_string = () => viewModel.KeywordList[0].Keyword = "";

        private Then keywordlist_contains_fix_green_binding = () =>
        {
            var pair = viewModel.KeywordList[0];
            pair.Keyword.ShouldBe("fix");
            pair.ColorName.ShouldBe("green");
        };
        private Then keywordlist_contains_fix_green_and_gul_yellow_binding = () =>
        {

            var greenPair = viewModel.KeywordList[0];
            greenPair.Keyword.ShouldBe("fix");
            greenPair.ColorName.ShouldBe("green");

            var yellowPair = viewModel.KeywordList[1];
            yellowPair.Keyword.ShouldBe("gul");
            yellowPair.ColorName.ShouldBe("yellow");

        };
    }




    [TestFixture]
    public class Notify_when_loading : Shared
    {
        [Test]
        public void Assure_loadingNotifyer_is_shown_while_loading()
        {
            Given(the_controller_has_been_created);
            When(refresh_is_triggered);
            Then("the loadingNotifyer should be shown",
                () => loadingNotifierMock.Verify(l => l.ShowInView(It.IsAny<string>()), Times.AtLeastOnce()));
        }

        [Test]
        public void Assure_loadingNotifier_is_hidden_after_loading()
        {
            Given("No controller spawned");
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
            Given(the_controller_has_been_created);
            When(save_button_is_pressed);
            Then("the loadingNotifier should be shown",
                () => loadingNotifierMock.Verify(l => l.ShowInBothViews(It.IsAny<string>()), Times.AtLeastOnce()));
        }

        [Test]
        [Ignore]
        public void Assure_loadingNotifier_is_hidden_again_some_time_after_save_is_pressed()
        {
            Given(the_controller_has_been_created);
            When(save_button_is_pressed);
            Then("", () =>
            {
                loadingNotifierMock.Verify(l => l.HideInBothViews(), Times.AtLeastOnce());
                viewModel.IsSaving.ShouldBeFalse();
            });
        }

    }

    [TestFixture]
    public class When_configuring_keyword_color_bindings : Shared
    {
        [Test]
        public void Assure_that_clicking_Add_word_and_color_button_inserts_a_new_binding()
        {
            Given(the_controller_has_been_created);
            When(add_word_and_color_button_is_pressed);
            Then("there should be one item in KeywordList", () => viewModel.KeywordList.Count().ShouldBe(1));
        }

        [Test]
        public void Assure_that_clicking_Add_word_and_color_button_twice_inserts_two_bindings()
        {
            Given(the_controller_has_been_created);
            When("add_word_and_color_is_pressed_twice", () =>
                {
                    add_word_and_color_button_is_pressed();
                    add_word_and_color_button_is_pressed();
                });
            Then("there should be two items in KeywordList", () => viewModel.KeywordList.Count().ShouldBe(2));
        }

        [Test]
        public void Assure_that_removing_keyword_from_a_binding_removes_that_binding()
        {
            Given(the_keyword_fix_is_bound_to_green_in_settings_db).And(the_controller_has_been_created);
            When(the_keyword_green_is_replaced_with_an_empty_string);
            Then("there should not be items in KeywordList", () => viewModel.KeywordList.Count.ShouldBe(0));
        }                                                                                               
    }

    [TestFixture]
    public class When_checking_comment_for_key_words : Shared
    {
        private Context there_is_one_changeset_in_sourcecontrol_containing_the_word_fix = () =>
        {
            var changesets = new List<Changeset>();

            changesets.Add(
                new Changeset
                {
                    Revision = 201222,
                    Comment = "Added fix to hello world method ",
                    Time = new DateTime(1986, 5, 20),
                    Author = new Author { Username = "goeran" }
                }
            );
            repositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(changesets);
        };

        private Context there_is_one_changeset_in_sourcecontrol_containing_the_word_Fix_with_capital_f = () =>
        {
            var changesets = new List<Changeset>();

            changesets.Add(
                new Changeset
                {
                    Revision = 201222,
                    Comment = "Added Fix to hello world method ",
                    Time = new DateTime(1986, 5, 20),
                    Author = new Author { Username = "goeran" }
                }
            );
            repositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(changesets);
        };

 
        

        [Test]
        public void Assure_that_changeset_has_default_color_when_no_word_is_found()
        {
            Given(there_are_changesets_in_sourcecontrol).
                And(the_controller_has_been_created);
            When("there are no key words");
            Then("the changeset viewModel should have default colors", () => viewModel.Changesets[0].BackgroundColor.ShouldBe(ChangesetViewModel.DEFAULT_BACKGROUND_COLOR));
        }

        [Test]
        public void Assure_that_color_is_updated_when_changeset_has_keyword()
        {
            Given(there_is_one_changeset_in_sourcecontrol_containing_the_word_fix).
                And(the_keyword_fix_is_bound_to_green_in_settings_db).
                And(the_controller_has_been_created);
            When("the keyword fix exists in comment");
            Then("the changeset viewModel should have have green colors", () => viewModel.Changesets[0].BackgroundColor.ShouldBe("GreenGradientBrush"));
        }

        [Test]
        public void Assure_that_keywords_are_case_insensitive()
        {
            Given(there_is_one_changeset_in_sourcecontrol_containing_the_word_Fix_with_capital_f).
               And(the_keyword_fix_is_bound_to_green_in_settings_db).
               And(the_controller_has_been_created);
            When("the keyword Fix exists in comment");
            Then("the changeset viewModel should have have green colors", () => viewModel.Changesets[0].BackgroundColor.ShouldBe("GreenGradientBrush"));
        }

    }


    public class Shared : ScenarioClass
    {
        protected static PropertyChangedRecorder changeRecorder;
        protected static LatestCommitsController controller;
        protected static Mock<ITimer> ITimerMock;
        protected static Mock<IRepository<Changeset>> repositoryMock;
        protected static Mock<IRepository<User>> userRepositoryMock;
        protected static Mock<IRepository<Configuration>> configRepositoryMock;
        protected static Mock<IPersistDomainModelsAsync<Configuration>> configPersisterMock;
        protected static Mock<IProgressbar> loadingNotifierMock;
        protected static Mock<ILog> loggerMock;
        protected static LatestCommitsViewModel viewModel;

        protected Context the_controller_has_been_created = CreateController;

        public Shared()
        {
            ViewModelBootstrapperForTests.Initialize();
        }

        protected Context there_are_changesets_in_sourcecontrol = () =>
        {
            var changesets = new List<Changeset>
            {
                new Changeset
                {
                    Revision = 201222,
                    Comment = "Added hello world method",
                    Time = new DateTime(1986, 5, 20),
                    Author = new Author { Username = "goeran" }
                }
            };
            repositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(changesets);
        };
        protected Context there_are_10_changesets_in_Changesets = () =>
        {
            var changesets = new List<Changeset>();

            for (int i = 0; i < 10; i++)
            {
                changesets.Add(
                    new Changeset
                    {
                        Revision = 201222 + i,
                        Comment = "Added hello world method ",
                        Time = new DateTime(1986, 5, 20),
                        Author = new Author { Username = "goeran" }
                    }
                );
            }
            //Forces the changesets to load the "natural" way since the test cant access viewModel.Changesets
            repositoryMock.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(changesets);
            ITimerMock.Raise(n => n.Elapsed += null, new EventArgs());
        };

        protected Context user_info_exist_in_userdb = () =>
        {
            var users = new List<User>();
            users.Add(new User()
            {
                Firstname = "Gøran",
                Surname = "Hansen",
                Username = "goeran",
                ImageUrl = "http://goeran.no/avatar.jpg",
                Email = "mail@goeran.no"
            });

            users.Add(new User()
            {
                Firstname = "Jonas",
                Surname = "Ask",
                Username = "jask",
                ImageUrl = "http://goeran.no/haldis.jpg",
                Email = "jask@trondheim.com"
            });

            userRepositoryMock.Setup(r => r.Get(It.Is<UserByUsername>(s => s.Username.Equals("goeran")))).Returns(
                new List<User>() { users.First() });

            userRepositoryMock.Setup(r => r.Get(It.Is<UserByUsername>(s => s.Username.Equals("jask")))).Returns(
                new List<User>() { users.Last() });

            userRepositoryMock.Setup(r => r.Get(It.IsAny<AllSpecification<User>>())).Returns(users);
        };

        protected Context there_are_config_settings_in_db = () =>
        {
            var configs = new List<Configuration>();
            var config = GenerateSettings(40, false);
            configs.Add(config);
            configRepositoryMock.Setup(r => r.Get(It.IsAny<ConfigurationByName>())).Returns(configs);
        };

        protected Context the_keyword_fix_is_bound_to_green_in_settings_db = () =>
        {
            var configs = new List<Configuration>();
            var config = GenerateSettings(40, false);
            config.NewSetting("commentKeywords", new[] { "fix", "green" });
            configs.Add(config);
            configRepositoryMock.Setup(r => r.Get(It.IsAny<ConfigurationByName>())).Returns(configs);
        };

        protected Context db_with_nonrelevant_configdata = () =>
        {
            var configs = new List<Configuration>();
            var config = new Configuration("DummyConfiguration");
            configs.Add(config);
            configRepositoryMock.Setup(r => r.Get(It.IsAny<AllSpecification<Configuration>>())).Returns(configs);
        };

        protected Context settings_is_changed_in_viewmodel = () =>
        {
            viewModel.NumberOfCommits = 32;
            viewModel.BlinkWhenNoComment = false;
        };

        protected When save_button_is_pressed = () => viewModel.SaveSettings.ExecuteDelegate();

        protected When add_word_and_color_button_is_pressed = () => viewModel.AddWordAndColorSettings.ExecuteDelegate();

        protected When the_controller_is_created = CreateController;

        protected When numberOfCommitts_is_changed_to_five = () =>
        {
            controller.ViewModel.NumberOfCommits = 5;
            controller.ViewModel.SaveSettings.ExecuteDelegate();
        };
        protected When refresh_is_triggered = () => ITimerMock.Raise(n => n.Elapsed += null, new EventArgs());

        protected When the_keyword_green_is_replaced_with_an_empty_string = () =>
        {
            controller.ViewModel.KeywordList[0].Keyword = "";
            controller.ViewModel.SaveSettings.ExecuteDelegate();
        };

        protected Then assure_it_queried_Changeset_Repository_for_the_newest_changeset =
            () => repositoryMock.Verify(r => r.Get(It.IsAny<Specification<Changeset>>()));

        protected static Configuration GenerateSettings(int numOfCommits, bool blinkIsChecked)
        {
            return GenerateSettings(numOfCommits, blinkIsChecked, null);
        }

        protected static Configuration GenerateSettings(int numOfCommits, bool blinkIsChecked, string[] keywordColors)
        {
            var config = new Configuration("CheckInNotification");
            config.NewSetting("numberOfCommits", new[] { numOfCommits.ToString() });
            config.NewSetting("blinkOnBlankComment", new[] { blinkIsChecked.ToString() });
            if (keywordColors != null)
                config.NewSetting("commentKeywords", keywordColors);
            else
                config.NewSetting("commentKeywords", new string[0]);
    
            return config;
        }

        private static void CreateController()
        {
            ITimerMock = new Mock<ITimer>();
            controller = new LatestCommitsController(new LatestCommitsViewModel(new Client.Framework.ViewModel.Widget()),
                                                           new NoUIInvokation(),
                                                           ITimerMock.Object,
                                                           repositoryMock.Object,
                                                           userRepositoryMock.Object,
                                                           configRepositoryMock.Object,
                                                           configPersisterMock.Object,
                                                           new NoBackgroundWorkerInvocation<IEnumerable<Changeset>>(),
                                                           loggerMock.Object,
                                                           loadingNotifierMock.Object);
            viewModel = controller.ViewModel;
            changeRecorder = new PropertyChangedRecorder(controller.ViewModel);
            //Thread.Sleep(1500);
        }

        [SetUp]
        public void SetUp()
        {
            Scenario("");
            repositoryMock = new Mock<IRepository<Changeset>>();
            userRepositoryMock = new Mock<IRepository<User>>();
            configRepositoryMock = new Mock<IRepository<Configuration>>();
            configPersisterMock = new Mock<IPersistDomainModelsAsync<Configuration>>();
            loadingNotifierMock = new Mock<IProgressbar>();
            loggerMock = new Mock<ILog>();
            ITimerMock = new Mock<ITimer>();
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }

    internal class LogEntryMockPersister : IPersistDomainModelsAsync<LogEntry>
    {
        public void Save(LogEntry domainModel) { }
        public void Save(IEnumerable<LogEntry> domainModels) { }
        public event EventHandler<SaveCompletedEventArgs> SaveCompleted;
    }
}