using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Smeedee.DomainModel.Corkboard;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;
using Smeedee.Widgets.Corkboard.ViewModel;

namespace Smeedee.Widgets.Tests.Corkboard.ViewModels
{
    [TestFixture]
    public class When_notes_are_added : ViewModelShared
    {
        [Test]
        public void it_should_contain_one_positive_and_one_negative_after_adding_these_when_initially_empty()
        {
            Given("an empty viewSettingsModel");

            When(a_positive_and_a_negative_note_is_added);

            Then("it should contain one positive and one negative note", () =>
            {
                _viewModel.PositiveNotes.Count.ShouldBe(1);
                _viewModel.NegativeNotes.Count.ShouldBe(1);
            });
        }

        [Test]
        public void assure_no_duplicate_notes_are_added()
        {
            Given(a_viewModel_with_two_positive_and_two_negative_notes);

            When(a_positive_and_a_negative_note_is_added_with_equal_description_to_existing_notes);

            Then("the note counts should be the same", () =>
            {
                _viewModel.PositiveNotes.Count().ShouldBe(2);
                _viewModel.NegativeNotes.Count().ShouldBe(2);
            });
        }
    }

    [TestFixture]
    public class ViewModelShared : ScenarioClass
    {
        protected static CorkboardViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            Scenario("");
            _viewModel = new CorkboardViewModel();
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
        
        protected Context a_viewModel_with_two_positive_and_two_negative_notes = () =>
        {
            _viewModel.AddNote(new NoteViewModel { Description = "first positive", Type = NoteType.Positive });
            _viewModel.AddNote(new NoteViewModel { Description = "second positive", Type = NoteType.Positive });
            _viewModel.AddNote(new NoteViewModel { Description = "first negative", Type = NoteType.Negative });
            _viewModel.AddNote(new NoteViewModel { Description = "second negative", Type = NoteType.Negative });
        };

        protected When a_positive_and_a_negative_note_is_added = () =>
        {
            _viewModel.AddNote(new NoteViewModel {Description = "first positive", Type = NoteType.Positive});
            _viewModel.AddNote(new NoteViewModel {Description = "first negative", Type = NoteType.Negative});
        };

        protected When a_positive_and_a_negative_note_is_added_with_equal_description_to_existing_notes = () =>
        {
            _viewModel.AddNote(new NoteViewModel { Description = "first positive", Type = NoteType.Positive });
            _viewModel.AddNote(new NoteViewModel { Description = "first negative", Type = NoteType.Negative});
        };
    }

    public static class TestExtensions
    {
        public static void ShouldNotContain(this IEnumerable<string> collection, string targetString)
        {
            var tmp = collection.Where(t => t == targetString);
            Assert.AreEqual(0, tmp.Count());
        }
    }
}

