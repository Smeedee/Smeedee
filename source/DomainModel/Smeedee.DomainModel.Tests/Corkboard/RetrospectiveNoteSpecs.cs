using System;
using NUnit.Framework;
using Smeedee.DomainModel.Corkboard;
using Smeedee.Tests;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModelTests.Corkboard.RetrospectiveNoteSpecs
{
    [TestFixture]
    public class When_created : SmeedeeScenarioTestClass
    {
        private RetrospectiveNote _note;
        private NoteType _type;
        private string _description;

        [Test]
        public void Assure_the_default_type_of_a_note_is_positive()
        {
            Given("a new retrospective note is created", () =>
                  _note = new RetrospectiveNote());

            When("we fetch the type of the note", () =>
                 _type = _note.Type);

            Then("the note should be positive", () =>
                 _type.ShouldBe(NoteType.Positive));
        }

        [Test]
        public void Assure_note_with_null_description_returns_empty_string()
        {
            Given("a new retrospective note with null description is created", () =>
                  _note = new RetrospectiveNote { Description = null });

            When("we fetch the description", () => 
                _description = _note.Description);

            Then("description should be empty string", () => 
                _description.ShouldBe(string.Empty));
        }
    }
}