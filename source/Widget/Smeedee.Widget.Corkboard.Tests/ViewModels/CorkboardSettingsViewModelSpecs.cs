using System;
using System.Linq;
using NUnit.Framework;
using Smeedee.DomainModel.Corkboard;
using Smeedee.Widget.Corkboard.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widget.Corkboard.Tests.ViewModels
{
    [TestFixture]
    public class When_notes_are_added_SettingsView : SettingsViewModelShared
    {
        [Test]
        public void it_should_contain_one_positive_and_one_negative_after_adding_these_when_initially_empty()
        {
            Given("an empty viewSettingsModel");

            When(a_positive_and_a_negative_note_is_added);

            Then("it should contain one positive and one negative note", () =>
            {
                _settingsViewModel.PositiveNotes.Count.ShouldBe(1);
                _settingsViewModel.NegativeNotes.Count.ShouldBe(1);
            });
        }

        [Test]
        public void assure_add_command_is_disabled_when_maximum_number_of_allowed_notes_are_reached()
        {
            Given(a_settingsViewModel_with_one_short_of_the_maximal_number_of_positive_and_negative_notes);

            When(a_positive_and_a_negative_note_is_added);

            Then("the add command should be disabled", () =>
            {
                _settingsViewModel.AddPositiveNote.CanExecute(null).ShouldBeFalse();
                _settingsViewModel.AddNegativeNote.CanExecute(null).ShouldBeFalse();
            });
        }

        [Test]
        public void assure_add_command_is_disabled_while_loading()
        {
            Given("an empty settingsViewModel");
            When(the_controller_is_loading_data);
            Then("add buttons should be disabled", () =>
            {
                _settingsViewModel.AddNegativeNote.CanExecute().ShouldBeFalse();
                _settingsViewModel.AddPositiveNote.CanExecute().ShouldBeFalse();
            });
        }

        [Test]
        public void assure_save_command_is_disabled_while_loading()
        {
            Given("an empty settingsViewModel");
            When(the_controller_is_loading_data);
            Then("save button should be disabled", () =>
            {
                _settingsViewModel.Save.CanExecute().ShouldBeFalse();
            });
        }

        [Test]
        public void assure_add_command_is_disabled_when_new_note_is_added()
        {
            Given(a_settingsViewModel_with_two_positive_and_two_negative_notes);

            When(a_positive_and_negative_note_with_empty_description_is_added);

            Then("the add command should be disabled", () =>
            {
                _settingsViewModel.AddPositiveNote.CanExecute(null).ShouldBeFalse();
                _settingsViewModel.AddNegativeNote.CanExecute(null).ShouldBeFalse();
            });
        }

        [Test]
        public void assure_adding_a_note_sets_hasChanges_to_true()
        {
            Given("an enpty settingsViewModel").And(HasChanges_is_false);

            When(a_positive_and_a_negative_note_is_added);

            Then(HasChanges_should_be_true);
        }
    }

    [TestFixture]
    public class When_notes_are_deleted_SettingsView : SettingsViewModelShared
    {
        [Test]
        public void assure_it_is_removed_from_the_SettingsViewModel()
        {
            Given(a_settingsViewModel_with_two_positive_and_two_negative_notes);

            When(a_positive_and_negative_note_is_deleted);

            Then("this note should be removed from the ViewModel", () =>
            {
                _settingsViewModel.PositiveNotes.Select(t => t.Description).ShouldNotContain("first positive");
                _settingsViewModel.NegativeNotes.Select(t => t.Description).ShouldNotContain("second negative");
            });
        }

        [Test]
        public void assure_deleting_a_note_sets_HasChanges_to_true()
        {
            Given(a_settingsViewModel_with_two_positive_and_two_negative_notes).And(HasChanges_is_false); 

            When(a_positive_and_negative_note_is_deleted);

            Then(HasChanges_should_be_true);
        }
    }

    [TestFixture]
    public class When_notes_are_edited_SettingsView : SettingsViewModelShared
    {
        [Test]
        public void assure_notes_are_replaced()
        {
            Given(a_settingsViewModel_with_two_positive_and_two_negative_notes);

            When(a_positive_and_negative_note_is_edited);

            Then("then these should be replaced", () =>
            {
                _settingsViewModel.PositiveNotes.Select(t => t.Description).ShouldNotContain("first positive");
                _settingsViewModel.PositiveNotes.Select(t => t.Description).ShouldContain("edited positive note");
                _settingsViewModel.NegativeNotes.Select(t => t.Description).ShouldNotContain("second negative");
                _settingsViewModel.NegativeNotes.Select(t => t.Description).ShouldContain("edited negative note");
            });
        }

        [Test]
        public void assure_HasChanges_is_set_to_true_when_note_is_edited()
        {
            Given(a_settingsViewModel_with_two_positive_and_two_negative_notes).And(HasChanges_is_false);

            When(a_positive_and_negative_note_is_edited);

            Then(HasChanges_should_be_true);
        }
    }

    [TestFixture]
    public class When_note_asks_to_change_position_SettingsView : SettingsViewModelShared
    {

        [Test]
        public void assure_note_are_moved_up_correctly()
        {
            Given(a_settingsViewModel_with_four_positive_and_four_negative_notes);

            When("a move up event is recieved", () => _settingsViewModel.PositiveNotes[2].MoveUp.ExecuteDelegate());

            Then("place [1] and [2] should be switched", () =>
            {
                var descriptions = _settingsViewModel.PositiveNotes.Select(t => t.Description).ToArray();
                descriptions[0].ShouldBe("0");
                descriptions[1].ShouldBe("2");
                descriptions[2].ShouldBe("1");
                descriptions[3].ShouldBe("3");
            });
        }

        [Test]
        public void assure_note_are_moved_down_correctly()
        {
            Given(a_settingsViewModel_with_four_positive_and_four_negative_notes);

            When("a move down event is recieved", () => _settingsViewModel.NegativeNotes[1].MoveDown.ExecuteDelegate());

            Then("place [1] and [2] should be switched", () =>
            {
                var descriptions = _settingsViewModel.NegativeNotes.Select(t => t.Description).ToArray();
                descriptions[0].ShouldBe("0");
                descriptions[1].ShouldBe("2");
                descriptions[2].ShouldBe("1");
                descriptions[3].ShouldBe("3");
            });
        }

        [Test]
        public void assure_nothing_happens_when_moving_up_from_the_first_position()
        {
            Given(a_settingsViewModel_with_four_positive_and_four_negative_notes);

            When("a move up event is recieved", () => _settingsViewModel.NegativeNotes[0].MoveUp.ExecuteDelegate());

            Then("everything should stay put", () =>
            {
                var descriptions = _settingsViewModel.NegativeNotes.Select(t => t.Description).ToArray();
                descriptions[0].ShouldBe("0");
                descriptions[1].ShouldBe("1");
                descriptions[2].ShouldBe("2");
                descriptions[3].ShouldBe("3");
            });
        }

        [Test]
        public void assure_nothing_happens_when_moving_down_from_the_last_position()
        {
            Given(a_settingsViewModel_with_four_positive_and_four_negative_notes);

            When("a move down event is recieved", () => _settingsViewModel.PositiveNotes[3].MoveDown.ExecuteDelegate());

            Then("everything should stay put", () =>
            {
                var descriptions = _settingsViewModel.NegativeNotes.Select(t => t.Description).ToArray();
                descriptions[0].ShouldBe("0");
                descriptions[1].ShouldBe("1");
                descriptions[2].ShouldBe("2");
                descriptions[3].ShouldBe("3");
            });
        }

        [Test]
        public void assure_nothing_happens_with_only_one_element_in_the_list()
        {
            Given(a_settingsViewModel_with_one_positive_and_one_negative_note);

            When("a move down event is recieved", () => _settingsViewModel.PositiveNotes[0].MoveDown.ExecuteDelegate());

            Then("nothing should happen", () => _settingsViewModel.PositiveNotes[0].Description.ShouldBe("0"));
        }

        [Test]
        public void assure_moving_a_note_sets_HasChanges_to_true()
        {
            Given(a_settingsViewModel_with_two_positive_and_two_negative_notes).And(HasChanges_is_false); 

            When("a move up event is recieved", () => _settingsViewModel.PositiveNotes[1].MoveUp.ExecuteDelegate());

            Then(HasChanges_should_be_true);
        }
    }


    [TestFixture]
    public class SettingsViewModelShared : ScenarioClass
    {
        protected static CorkboardSettingsViewModel _settingsViewModel;
        private const int MAXIMUM_NUMBER_OF_NOTES = CorkboardSettingsViewModel.MAXIMUM_NUMBER_OF_NOTES;

        [SetUp]
        public void SetUp()
        {
            Scenario("");
            _settingsViewModel = new CorkboardSettingsViewModel();
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }

        protected Context a_settingsViewModel_with_two_positive_and_two_negative_notes = () =>
        {
            _settingsViewModel.AddNote(new NoteViewModel { Description = "first positive", Type = NoteType.Positive });
            _settingsViewModel.AddNote(new NoteViewModel { Description = "second positive", Type = NoteType.Positive });
            _settingsViewModel.AddNote(new NoteViewModel { Description = "first negative", Type = NoteType.Negative });
            _settingsViewModel.AddNote(new NoteViewModel { Description = "second negative", Type = NoteType.Negative });
        };

        protected Context HasChanges_is_false = () => _settingsViewModel.HasChanges = false;

        protected Context a_settingsViewModel_with_one_short_of_the_maximal_number_of_positive_and_negative_notes = () =>
        {
            for (int i = 1; i < MAXIMUM_NUMBER_OF_NOTES; i++)
            {
                _settingsViewModel.AddNote(new NoteViewModel { Description = i.ToString(), Type = NoteType.Positive });
                _settingsViewModel.AddNote(new NoteViewModel { Description = i.ToString(), Type = NoteType.Negative });
            }
        };

        protected Context a_settingsViewModel_with_four_positive_and_four_negative_notes = () =>
        {
            for (int i = 0; i < 4; i++)
            {
                _settingsViewModel.AddNote(new NoteViewModel { Description = i.ToString(), Type = NoteType.Positive });
                _settingsViewModel.AddNote(new NoteViewModel { Description = i.ToString(), Type = NoteType.Negative });
            }
        };

        protected Context a_settingsViewModel_with_one_positive_and_one_negative_note = () =>
        {
            _settingsViewModel.AddNote(new NoteViewModel { Description = "0", Type = NoteType.Positive });
            _settingsViewModel.AddNote(new NoteViewModel { Description = "0", Type = NoteType.Negative });
        };

        protected When the_controller_is_loading_data = () =>
        {
            _settingsViewModel.IsLoading = true;
        };

        protected When a_positive_and_a_negative_note_is_added = () =>
        {
            _settingsViewModel.AddNote(new NoteViewModel { Description = "first positive", Type = NoteType.Positive });
            _settingsViewModel.AddNote(new NoteViewModel { Description = "first negative", Type = NoteType.Negative });
        };

        protected When a_positive_and_a_negative_note_is_added_with_equal_description_to_existing_notes = () =>
        {
            _settingsViewModel.AddNote(new NoteViewModel { Description = "first positive", Type = NoteType.Positive });
            _settingsViewModel.AddNote(new NoteViewModel { Description = "first negative", Type = NoteType.Negative });
        };

        protected When a_positive_and_negative_note_with_empty_description_is_added = () =>
        {
            _settingsViewModel.OnAddPositiveNote();
            _settingsViewModel.OnAddNegativeNote();
        };

        protected When a_positive_and_negative_note_is_edited = () =>
        {
            _settingsViewModel.PositiveNotes.First().Description = "edited positive note";
            _settingsViewModel.NegativeNotes.Last().Description = "edited negative note";
        };

        protected When a_positive_and_negative_note_is_deleted = () =>
        {
            _settingsViewModel.PositiveNotes.First().Delete.ExecuteDelegate();
            _settingsViewModel.NegativeNotes.Last().Delete.ExecuteDelegate();
        };

        protected Then HasChanges_should_be_true = () =>
        {
            _settingsViewModel.HasChanges.ShouldBeTrue();
        };


    }
}
