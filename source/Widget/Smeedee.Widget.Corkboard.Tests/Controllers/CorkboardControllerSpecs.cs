using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Corkboard;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Widget.Corkboard.Controllers;
using Smeedee.Widget.Corkboard.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widget.Corkboard.Tests.Controllers 
{
    [TestFixture]
    public class When_notified_to_refresh : Shared
    {
        [Test]
        public void assure_it_updates_the_ViewModel_with_the_data_from_the_repository()
        {
            Given(the_repository_contains_one_positive_and_one_negative_note);

            When(the_controller_is_notified_to_refresh);

            Then("the ViewModel should contain one positive and one negative note", () =>
            {
                _viewModel.PositiveNotes.Count.ShouldBe(1);
                _viewModel.NegativeNotes.Count.ShouldBe(1);
            });
        }

        [Test]
        public void assure_it_updates_the_SettingsViewModel_with_the_data_from_the_repository()
        {
            Given(the_repository_contains_one_positive_and_one_negative_note);

            When(the_controller_is_notified_to_refresh);

            Then("the ViewModel should contain one positive and one negative note", () =>
            {
                _settingsViewModel.PositiveNotes.Count.ShouldBe(1);
                _settingsViewModel.NegativeNotes.Count.ShouldBe(1);
            });
        }

        [Test]
        public void assure_it_sets_HasChanges_to_false()
        {
            Given(HasChanges_is_true);

            When(the_controller_is_notified_to_refresh);

            Then(HasChanges_should_be_false);
        }
    }

    [TestFixture]
    public class When_the_controllers_loads_data_from_the_repository : Shared
    {
        [Test]
        public void assure_filter_function_filter_notes_on_id()
        {
            Given(the_repository_contains_two_positive_notes_with_correct_id_and_2_with_another_id);

            When(the_controller_is_notified_to_refresh);

            Then("Assure the viewModel has 2 notes and the settingsViewModel has 2 notes", () =>
            {
                _viewModel.PositiveNotes.Count.ShouldBe(2);
                _settingsViewModel.PositiveNotes.Count.ShouldBe(2);
            });
        }

        [Test]
        public void mirror_notes_in_the_repository_into_the_viewModel_and_settingsViewModel()
        {
            Given(the_repository_contains_one_positive_and_one_negative_note);

            When(the_controller_is_notified_to_refresh);

            Then("Assure the viewModel has 2 notes and the settingsViewModel has 2 notes", () =>
            {
                _viewModel.PositiveNotes.Count.ShouldBe(1);
                _viewModel.NegativeNotes.Count.ShouldBe(1);
                _settingsViewModel.PositiveNotes.Count.ShouldBe(1);
                _settingsViewModel.NegativeNotes.Count.ShouldBe(1);
            });
        }

        [Test]
        public void the_controller_should_not_load_any_data_into_the_viewModel_or_the_settingsViewModel_if_repository_is_empty()
        {
            Given(the_repository_is_empty);

            When(the_controller_is_notified_to_refresh);

            Then("Assure the viewModel and the settingsViewModel is empty", () =>
                {
                    (_viewModel.NegativeNotes.Count + _viewModel.PositiveNotes.Count).ShouldBe(0);
                    (_viewModel.NegativeNotes.Count + _viewModel.PositiveNotes.Count).ShouldBe(0);
                });
        }

        [Test]
        public void any_notes_in_the_viewModels_and_not_in_the_repository_should_be_removed()
        {
            Given(the_repository_contains_one_positive_and_one_negative_note).And(
                a_SettingsViewModel_containing_two_positive_and_two_negative_notes_with_distinct_description).And(a_ViewModel_containing_two_positive_and_two_negative_notes_with_distinct_description);

            When(the_controller_is_notified_to_refresh);

            Then("The viewModels should only contain 2 notes", () =>
                {
                    _settingsViewModel.PositiveNotes.Count.ShouldBe(1);
                    _settingsViewModel.NegativeNotes.Count.ShouldBe(1);

                    _viewModel.NegativeNotes.Count.ShouldBe(1);
                    _viewModel.PositiveNotes.Count.ShouldBe(1);
                });

        }

        [Test]
        public void adding_notes_from_repo_already_in_the_viewModel_should_not_increase_number_of_notes_in_viewModels()
        {
            Given(the_repository_contains_three_positive_and_three_negative_notes).And(
                the_viewModels_contains_the_same_notes);

            When(the_controller_is_notified_to_refresh);

            Then("The viewModels should contain three positive and three negative notes", () =>
                {
                    _settingsViewModel.PositiveNotes.Count.ShouldBe(3);
                    _settingsViewModel.NegativeNotes.Count.ShouldBe(3);

                    _viewModel.NegativeNotes.Count.ShouldBe(3);
                    _viewModel.PositiveNotes.Count.ShouldBe(3);
                });
        }

        [Test]
        public void the_order_of_the_notes_should_be_preserved()
        {
            Given(the_repository_contains_three_positive_and_three_negative_notes).And("The viewModels are empty");

            When(the_controller_is_notified_to_refresh);

            Then("The viewModels should contain three posetive and three negative notes in the correct order", () =>
                {
                    for(int i = 0; i < 3; i++){
                        _viewModel.PositiveNotes[i].Description.ShouldBe("This is positive " + i);
                        _viewModel.NegativeNotes[i].Description.ShouldBe("This is negative " + i);
                        _settingsViewModel.PositiveNotes[i].Description.ShouldBe("This is positive " + i);
                        _settingsViewModel.NegativeNotes[i].Description.ShouldBe("This is negative " + i);
                    }
                });
        }

        [Test]
        public void and_the_load_fails_the_error_should_be_logged()
        {
            Given(a_faulty_repository);

            When(the_controller_is_notified_to_refresh);

            Then("Assure the controller logs the exception thrown", () =>
                _mockLogger.Verify(l => l.WriteEntry(It.IsAny<ErrorLogEntry>())));
        }
    }

    [TestFixture]
    public class When_start_is_called : Shared
    {
        [Test]
        public void assure_the_refreshnotifier_is_started()
        {
            Given("a controller");

            When(Start_is_called);

            Then("the refreshnotifier should be started", () =>
                _refreshNotifierMock.Verify(n => n.Start(It.IsAny<int>()), Times.Exactly(1)));
        }
    }

    [TestFixture]
    public class When_FlippedBackFromSettingsView_is_called : Shared
    {
        [Test]
        public void assure_the_refreshnotifier_is_started()
        {
            Given("a controller");

            When(FlippedBackFromSettingsView_is_called);

            Then("the refreshnotifier should be started", () =>
                _refreshNotifierMock.Verify(n => n.Start(It.IsAny<int>()), Times.Exactly(1)));
        }

        [Test]
        public void assure_the_changes_is_not_saved_to_the_repository()
        {
            Given(a_SettingsViewModel_containing_two_positive_and_two_negative_notes_with_distinct_description);

            When(FlippedBackFromSettingsView_is_called);

            Then("the repository mock should not be asked to save", () =>
                _repositoryPersistMock.Verify(n => n.Save(It.IsAny<IEnumerable<RetrospectiveNote>>()), Times.Exactly(0)));
        }

        [Test]
        public void assure_unsaved_changes_are_lost_when_notes_has_been_added()
        {
            Given(a_SettingsViewModel_containing_two_positive_and_two_negative_notes_with_distinct_description).And(
                "an empty ViewModel");

            When(FlippedBackFromSettingsView_is_called);

            Then("The SettingsViewModel should be empty", () =>
                {
                    _settingsViewModel.PositiveNotes.Count.ShouldBe(0);
                    _settingsViewModel.NegativeNotes.Count.ShouldBe(0);
                });
        }

        [Test]
        public void assure_unsaved_changes_are_lost_when_notes_are_deleted()
        {
            Given(a_ViewModel_containing_two_positive_and_two_negative_notes_with_distinct_description).And(
                a_SettingsViewModel_containing_a_proper_subset_of_those_notes);

            When(FlippedBackFromSettingsView_is_called);

            Then("The settingsViewModel should contain two posetive and two negative notes", () =>
                {
                    _settingsViewModel.NegativeNotes.Count.ShouldBe(2);
                    _settingsViewModel.PositiveNotes.Count.ShouldBe(2);
                });
        }

        [Test]
        public void assure_HasChanges_is_set_to_false()
        {
            Given(HasChanges_is_true);

            When(FlippedBackFromSettingsView_is_called);

            Then(HasChanges_should_be_false);
        }
    }

    [TestFixture]
    public class When_stop_is_called : Shared
    {
        [Test]
        public void Assure_the_refreshnotifier_is_stopped()
        {
            Given("a controller");

            When(Stop_is_called);

            Then("the refreshnotifier should be stopped", () =>
                _refreshNotifierMock.Verify(n => n.Stop(), Times.Exactly(1)));
        }
    }

    [TestFixture]
    public class When_Save_is_executed : Shared
    {
        [Test]
        public void Assure_Saving_is_started_when_Save_is_executed_when_HasChanges_is_true()
        {
            Given(HasChanges_is_true).
                And(a_SettingsViewModel_containing_two_positive_and_two_negative_notes_with_distinct_description);

            When(Save_is_executed);

            Then("Delete should be called",
                 () =>
                 _repositoryPersistMock.Verify(t => t.Save(It.IsAny<IEnumerable<RetrospectiveNote>>()), Times.Exactly(1)));
        }

        [Test]
        public void assure_Save_is_ignored_when_HasChanges_is_false()
        {
            Given("HasChanges is false");

            When(Save_is_executed);

            Then("Save should not be called",
                 () =>
                 _repositoryPersistMock.Verify(t => t.Save(It.IsAny<IEnumerable<RetrospectiveNote>>()), Times.Exactly(0)));
        }

        [Test]
        public void Assure_delete_is_called_when_saving_0_notes()
        {
            Given(HasChanges_is_true).
                And(viewModel_has_no_notes);

            When(Save_is_executed);

            Then("Save should be called",
                 () =>
                 _repositoryDeleteMock.Verify(t => t.Delete(It.IsAny<Specification<RetrospectiveNote>>()), Times.Exactly(1)));
        }

        [Test]
        public void assure_ViewModel_is_updated_from_SettingsViewModel_when_saving_and_notes_has_been_added()
        {
            Given(a_SettingsViewModel_containing_two_positive_and_two_negative_notes_with_distinct_description).And(
                a_ViewModel_containing_a_proper_subset_of_those_notes);

            When(Save_is_executed);

            Then("The viewModel should contain two positive an two negative notes", () =>
                {
                    _viewModel.NegativeNotes.Count.ShouldBe(2);
                    _viewModel.PositiveNotes.Count.ShouldBe(2);
                });
        }

        [Test]
        public void assure_the_viewModel_is_updated_from_the_settingsViewModel_when_saving_and_notes_has_been_deleted()
        {
            Given(a_ViewModel_containing_two_positive_and_two_negative_notes_with_distinct_description).And(
                a_SettingsViewModel_containing_a_proper_subset_of_those_notes);

            When(Save_is_executed);

            Then("The viewModel should countain one positive and one negative note", () =>
                {
                    _viewModel.NegativeNotes.Count.ShouldBe(1);
                    _viewModel.PositiveNotes.Count.ShouldBe(1);
                });
        }
    }

    [TestFixture]
    public class When_ReloadSettings_is_executed :Shared
    {
        [Test]
        public void assure_Get_is_called_when_Save_is_executed_when_HasChanges_is_true()
        {
            Given(HasChanges_is_true);

            When("ReloadSettings is executed", () => _settingsViewModel.ReloadSettings.Execute(null));

            Then("Get should be called",
                 () =>
                 _repositoryMock.Verify(t => t.Get(It.IsAny<Specification<RetrospectiveNote>>()), Times.Exactly(2)));
        }

        [Test]
        public void assure_ReloadSettings_is_ignored_when_HasChanges_is_false()
        {
            Given("HasChanges is false");

            When("ReloadSettings is executed", () => _settingsViewModel.ReloadSettings.Execute(null));

            Then("Get should not be called", () => _repositoryMock.Verify(t => t.Get(It.IsAny<Specification<RetrospectiveNote>>()), Times.Exactly(1)));
        }

        [Test]
        public void assure_an_empty_board_can_be_reloaded_after_adding_a_negative_note_without_saving()
        {
            Given("an empty settingsviewmodel").
                And(viewModel_is_saved).
                And(a_negative_note_is_added);

            When(reload_settings_is_executed);

            Then("Negative note count in viewmodel should be 0", () =>
                _viewModel.NegativeNotes.Count.ShouldBe(0))
                .And("Negative note count in settingsviewmodel should be 0", () =>
                _settingsViewModel.PositiveNotes.Count.ShouldBe(0));
        }

        [Test]
        public void assure_an_empty_board_can_be_reloaded_after_adding_a_positive_note_without_saving()
        {
            Given("an empty settings viewmodel").
                And(viewModel_is_saved).
                And(a_positive_note_is_added);

            When(reload_settings_is_executed);

            Then("Positive note count in viewmodel should be 0", () =>
                _viewModel.PositiveNotes.Count.ShouldBe(0)).
                And("Positive note count in settingsview should be null", () => 
                _settingsViewModel.PositiveNotes.Count.ShouldBe(0));
        }

        [Test]
        public void assure_reload_dont_remove_saved_notes_text_in_corkboard_notes()
        {
            var note1 = new NoteViewModel {Description = "1", Type = NoteType.Positive};
            var note2 = new NoteViewModel {Description = "2", Type = NoteType.Positive};

            Given("A viewModel with some notes", () =>
                {
                    _settingsViewModel.AddNote(note1);
                    _settingsViewModel.AddNote(note2);
                }).And("the content of the notes is changed to an empty string", () =>
                {
                    note1.Description = "";
                    note2.Description = "";
                }).And(viewModel_is_saved);

            When(reload_settings_is_executed);

            Then("Assure the notes have gotten their original content back", () =>
            {
                _settingsViewModel.PositiveNotes[0].Description.ShouldBe("");
                _settingsViewModel.PositiveNotes[1].Description.ShouldBe("");
            });
        }

    }

    [TestFixture]
    public class When_loading_and_saving_data_and_settings : Shared
    {
        [Test]
        public void assure_progressbar_is_shown_while_loading_data()
        {
            Given("A controller");
            When(the_controller_is_notified_to_refresh);
            Then("assure loading notifier has been shown in view", () =>
                loadingScreenHasBeenShownInView.ShouldBeTrue());
        }

        [Test]
        public void assure_progressbar_is_hidden_after_loading_data()
        {
            Given("A controller");
            When(the_controller_is_notified_to_refresh);
            Then("assure loading loading notifier hide has been called", () =>
            {
                loadingScreenHasBeenHiddenFromView.ShouldBeTrue();
                loadingScreenHasBeenHiddenFromSettingsView.ShouldBeTrue();
                _viewModel.IsLoading.ShouldBe(false);
            });
        }

        [Test]
        public void assure_progressbar_is_shown_while_saving_data()
        {
            Given("A controller");
            When(() => _controller.Save());
            Then(() =>
            {
                loadingScreenHasBeenShownInView.ShouldBeTrue();
                _viewModel.IsSaving.ShouldBe(true);
            });
        }

        [Test]
        public void assure_progressbar_is_hidden_after_saving_data()
        {
            Given(a_SettingsViewModel_with_two_positive_notes_having_equal_description).
                And(persisterMock_setup_to_return_savecomplete);
            When(() => _controller.Save());
            Then(() =>
            {
                _loadingNotifierMock.Verify(l => l.HideInSettingsView(), Times.AtLeastOnce());
                _viewModel.IsSaving.ShouldBe(false);
            });
        }
    }

    [TestFixture]
    public class When_the_ViewModel_is_persisted_to_the_repository : Shared
    {
        [Test]
        public void assure_it_gives_only_one_note_to_the_repository_when_two_notes_of_the_same_type_have_equal_description()
        {
            Given(a_SettingsViewModel_with_two_positive_notes_having_equal_description)
                .And(the_repository_mock_callbacks_all_arguments_given);

            When(the_ViewModel_is_persisted);

            Then("only one note should be given to the repository", () => calledBackNotes.Count().ShouldBe(1));
        }

        [Test]
        public void assure_it_gives_only_one_note_to_the_repository_when_two_notes_of_different_type_have_equal_description()
        {
            Given(a_SettingsViewModel_with_one_positive_and_one_negative_note_with_equal_description)
                .And(the_repository_mock_callbacks_all_arguments_given);

            When(the_ViewModel_is_persisted);

            Then("only one note should be given to the repository", () => calledBackNotes.Count().ShouldBe(1));
        }

        [Test]
        public void assure_the_entire_ViewModel_is_persisted()
        {
            Given(a_SettingsViewModel_containing_two_positive_and_two_negative_notes_with_distinct_description)
                .And(the_repository_mock_callbacks_all_arguments_given);

            When(the_ViewModel_is_persisted);

            Then("all these notes should be given to the repository", () =>
            {
                var positiveNotes = calledBackNotes.Where(t => t.Type == NoteType.Positive).Select(t => t.Description);
                var negativeNotes = calledBackNotes.Where(t => t.Type == NoteType.Negative).Select(t => t.Description);

                positiveNotes.ShouldContain("0");
                positiveNotes.ShouldContain("1");
                negativeNotes.ShouldContain("2");
                negativeNotes.ShouldContain("3");
            });
        }

        [Test]
        public void assure_the_viewModel_is_updated_from_the_settingsViewModel()
        {
            Given(a_SettingsViewModel_containing_two_positive_and_two_negative_notes_with_distinct_description);

            When(the_ViewModel_is_persisted);

            Then("the viewModel should have the same notes", () =>
                {
                    var positiveNotes = _viewModel.PositiveNotes.Where(t => t.Type == NoteType.Positive).Select(t => t.Description);
                    var negativeNotes = _viewModel.NegativeNotes.Where(t => t.Type == NoteType.Negative).Select(t => t.Description);

                    positiveNotes.ShouldContain("0");
                    positiveNotes.ShouldContain("1");
                    negativeNotes.ShouldContain("2");
                    negativeNotes.ShouldContain("3");
                });
        }

        [Test]
        public void assure_HasChanges_is_set_to_false_when_persisting()
        {
            Given(HasChanges_is_true);

            When(the_ViewModel_is_persisted);

            Then(HasChanges_should_be_false);
        }
    }
    
    [TestFixture]
    public class Shared : ScenarioClass
    {
        protected static CorkboardController _controller;
        protected static CorkboardViewModel _viewModel;
        protected static CorkboardSettingsViewModel _settingsViewModel;
        protected static IEnumerable<RetrospectiveNote> calledBackNotes;

        protected static Mock<IPersistDomainModelsAsync<RetrospectiveNote>> _repositoryPersistMock;
        protected static Mock<IDeleteDomainModelsAsync<RetrospectiveNote>> _repositoryDeleteMock;
        protected static Mock<IRepository<RetrospectiveNote>> _repositoryMock;
        protected static Mock <ILog> _mockLogger;
        protected static Mock<ITimer> _refreshNotifierMock;
        protected static Mock<IProgressbar> _loadingNotifierMock;

        protected static bool loadingScreenHasBeenShownInView = false;
        protected static bool loadingScreenHasBeenShownInSettingsView = false;
        protected static bool loadingScreenHasBeenHiddenFromView = false;
        protected static bool loadingScreenHasBeenHiddenFromSettingsView = false;

        protected static Configuration _standardConfig;
        

        [SetUp]
        public void SetUp()
        {
            Scenario("");
            CreateConfig();
            CreateViewModel();
            CreateSettingsViewModel(); 
            CreateMocks();
            CreateController();
        }

        private void CreateConfig()
        {
            _standardConfig = new Configuration();
            _standardConfig.NewSetting("IsDefault", "Yes");
        }

        private void CreateSettingsViewModel()
        {
            _settingsViewModel = new CorkboardSettingsViewModel();
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }

        protected Context the_repository_contains_two_positive_notes_with_correct_id_and_2_with_another_id = () =>
            _repositoryMock
                .Setup(r => r.Get(It.IsAny<AllSpecification<RetrospectiveNote>>()))
                    .Returns(MockWithTwoCorrectIdAndTwoWrong());

        protected Context the_repository_contains_one_positive_and_one_negative_note = () =>
            _repositoryMock
                .Setup(r => r.Get(It.IsAny<AllSpecification<RetrospectiveNote>>()))
                    .Returns(MockRetrospectiveNotes(1));

        protected Context the_repository_contains_three_positive_and_three_negative_notes = () =>
            _repositoryMock.Setup(r => r.Get(It.IsAny<AllSpecification<RetrospectiveNote>>())).Returns(MockRetrospectiveNotes(3));

        protected Context the_repository_mock_callbacks_all_arguments_given = () =>
             _repositoryPersistMock
                .Setup(t => t.Save(It.IsAny<IEnumerable<RetrospectiveNote>>()))
                    .Callback((IEnumerable<RetrospectiveNote> t) => calledBackNotes = t);

        protected Context HasChanges_is_true = () => _settingsViewModel.HasChanges = true;

        private static IEnumerable<RetrospectiveNote> MockRetrospectiveNotes(int numberOfNotes)
        {
            var notes = new RetrospectiveNote[numberOfNotes*2];

            for (int i = 0; i < numberOfNotes; i++ )
            {
                notes[i] = new RetrospectiveNote {Description = "This is positive " + i, Type = NoteType.Positive, Id = _standardConfig.Id.ToString()};
                notes[numberOfNotes + i] = new RetrospectiveNote { Description = "This is negative " + i, Type = NoteType.Negative, Id = _standardConfig.Id.ToString() };
            }

            return notes;
        }

        private static IEnumerable<RetrospectiveNote> MockWithTwoCorrectIdAndTwoWrong()
        {
            var notes = new RetrospectiveNote[4];

            for (int i = 0; i < 2; i++)
            {
                notes[i] = new RetrospectiveNote {Description = "Correct ID" + i, Type = NoteType.Positive, Id = _standardConfig.Id.ToString()};
                notes[2 + i] = new RetrospectiveNote {Description = "Wrong ID " + i, Type = NoteType.Positive, Id = new Guid().ToString()};
            }

            return notes;
        }

        protected Context the_repository_is_empty = () =>
            _repositoryMock.Setup(r => r.Get(It.IsAny<AllSpecification<RetrospectiveNote>>()))
                    .Returns(new List<RetrospectiveNote>());

        protected Context a_faulty_repository = () =>
            _repositoryMock
                .Setup(r => r.Get(It.IsAny<AllSpecification<RetrospectiveNote>>()))
                    .Throws(new Exception());

        protected Context viewModel_has_no_notes = () =>
        {
            _settingsViewModel.PositiveNotes.Clear();
            _settingsViewModel.NegativeNotes.Clear();
            _viewModel.PositiveNotes.Clear();
            _viewModel.NegativeNotes.Clear();
        };
                                                   
                        

        protected Context a_SettingsViewModel_with_two_positive_notes_having_equal_description = () =>
        {
            _settingsViewModel.AddNote(new NoteViewModel {Description = "equal"});
            _settingsViewModel.AddNote(new NoteViewModel());
            _settingsViewModel.PositiveNotes.Last().Description = "equal";
        };

        protected Context a_SettingsViewModel_with_one_positive_and_one_negative_note_with_equal_description = () =>
        {
            _settingsViewModel.AddNote(new NoteViewModel { Description = "equal" });
            _settingsViewModel.AddNote(new NoteViewModel {Type = NoteType.Negative});
            _settingsViewModel.NegativeNotes.Last().Description = "equal";
        };

        protected Context a_SettingsViewModel_containing_two_positive_and_two_negative_notes_with_distinct_description = () =>
        {
            CreateSettingsViewModelWithPositiveNotes("0", "1");
            CreateSettingsViewModelWithNegativeNotes("2", "3");
        };

        protected Context the_viewModels_contains_the_same_notes = () =>
        {
            foreach (var note in MockRetrospectiveNotes(3))
            {
                _settingsViewModel.AddNote(new NoteViewModel { Description = note.Description, Type = note.Type });
                _viewModel.AddNote(new NoteViewModel { Description = note.Description, Type = note.Type });
            }
        };

        protected Context a_ViewModel_containing_two_positive_and_two_negative_notes_with_distinct_description = () =>
        {
            CreateViewModelWithPositiveNotes("0", "1");
            CreateViewModelWithNegativeNotes("2", "3");
        };

        protected Context a_SettingsViewModel_containing_a_proper_subset_of_those_notes = () =>
        {
            CreateSettingsViewModelWithPositiveNotes("0");
            CreateSettingsViewModelWithNegativeNotes("2");
        };

        protected Context a_ViewModel_containing_a_proper_subset_of_those_notes = () =>
        {
            CreateViewModelWithPositiveNotes("0");
            CreateViewModelWithNegativeNotes("2");
        };

        protected Context a_positive_note_is_added = () => CreateSettingsViewModelWithPositiveNotes("positive");

        protected Context a_negative_note_is_added = () => CreateSettingsViewModelWithNegativeNotes("negative");

        protected Context add_negative_note_is_executed = () => _settingsViewModel.AddNegativeNote.Execute(null);
            
        protected Context add_positive_note_is_executed = () => _settingsViewModel.AddPositiveNote.Execute(null);

        protected Context persisterMock_setup_to_return_savecomplete = () =>
            _repositoryPersistMock.Setup(r => r.Save(It.IsAny<IEnumerable<RetrospectiveNote>>()))
                .Raises(t => t.SaveCompleted += null, new SaveCompletedEventArgs());

        private static void CreateSettingsViewModelWithPositiveNotes(params string[] description)
        {
            foreach (string t in description)
                _settingsViewModel.AddNote(new NoteViewModel { Description = t, Type = NoteType.Positive });
        }

        private static void CreateSettingsViewModelWithNegativeNotes(params string[] description)
        {
            foreach (string t in description)
                _settingsViewModel.AddNote(new NoteViewModel { Description = t, Type = NoteType.Negative });
        }

        private static void CreateViewModelWithPositiveNotes(params string[] description)
        {
            foreach (string t in description)
                _viewModel.AddNote(new NoteViewModel { Description = t, Type = NoteType.Positive });
        }

        private static void CreateViewModelWithNegativeNotes(params string[] description)
        {
            foreach (string t in description)
                _viewModel.AddNote(new NoteViewModel { Description = t, Type = NoteType.Negative });
        }

        protected When the_controller_is_notified_to_refresh = () => 
            _refreshNotifierMock.Raise(t => t.Elapsed += null, new EventArgs());

        protected When Start_is_called = () => _controller.Start(It.IsAny<int>());

        protected When FlippedBackFromSettingsView_is_called = () => _controller.FlippedBackFromSettingsView(It.IsAny<int>());

        protected When Stop_is_called = () => _controller.Stop();

        protected When the_ViewModel_is_persisted = () => _controller.Save();

        protected When Save_is_executed = () => _settingsViewModel.Save.Execute(null);

        protected Context viewModel_is_saved = () => _settingsViewModel.Save.Execute(null);

        protected When reload_settings_is_executed = () => _settingsViewModel.ReloadSettings.Execute(null);

        protected Then HasChanges_should_be_false = () => _settingsViewModel.HasChanges.ShouldBeFalse();
        
        private static void CreateViewModel()
        {
            _viewModel = new CorkboardViewModel();
        }

        private static void CreateMocks()
        {
            _repositoryMock = new Mock<IRepository<RetrospectiveNote>>();
            _repositoryPersistMock = new Mock<IPersistDomainModelsAsync<RetrospectiveNote>>();
            _repositoryDeleteMock = new Mock<IDeleteDomainModelsAsync<RetrospectiveNote>>();
//            _repositoryPersistMock.Setup(r => r.Save(It.IsAny<RetrospectiveNote>())).Raises(t => t.SaveCompleted += null);
            _refreshNotifierMock = new Mock<ITimer>();
            _mockLogger = new Mock<ILog>();
            _loadingNotifierMock = new Mock<IProgressbar>();

            SetFlagsWhenLoadingScreenIsShownOrHidden();
        }

        private static void SetFlagsWhenLoadingScreenIsShownOrHidden()
        {
            _loadingNotifierMock.Setup(m => m.ShowInBothViews(It.IsAny<string>())).Callback(
                () =>
                    {
                        loadingScreenHasBeenShownInView = true;
                        loadingScreenHasBeenShownInSettingsView = true;
                    });
            _loadingNotifierMock.Setup(m => m.ShowInSettingsView(It.IsAny<string>())).Callback(
                () => loadingScreenHasBeenShownInSettingsView = true);
            _loadingNotifierMock.Setup(m => m.ShowInView(It.IsAny<string>())).Callback(
                () => loadingScreenHasBeenShownInView = true );

            _loadingNotifierMock.Setup(m => m.HideInBothViews()).Callback(
                () =>
                    {
                        loadingScreenHasBeenHiddenFromView = true;
                        loadingScreenHasBeenHiddenFromSettingsView = true;
                    });
            _loadingNotifierMock.Setup(m => m.HideInSettingsView()).Callback(
                () => loadingScreenHasBeenHiddenFromSettingsView = true);
            _loadingNotifierMock.Setup(m => m.HideInView()).Callback(
                () => loadingScreenHasBeenHiddenFromView = true );
        }

        protected static void CreateController()
        {
            _controller = new CorkboardController(
                _viewModel,
                _settingsViewModel,
                _repositoryMock.Object,
                _repositoryPersistMock.Object,
                _repositoryDeleteMock.Object,
                _refreshNotifierMock.Object,
                new NoUIInvokation(),
                new NoBackgroundWorkerInvocation<IEnumerable<RetrospectiveNote>>(),
                _mockLogger.Object,
                _loadingNotifierMock.Object,
                _standardConfig);
        }
    }  
}