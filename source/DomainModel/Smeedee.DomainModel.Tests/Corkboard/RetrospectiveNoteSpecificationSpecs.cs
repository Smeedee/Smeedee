using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.Corkboard;
using Smeedee.DomainModel.SourceControl;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModel.Tests.Corkboard
{
    [TestFixture]
    public class When_Retrospective_Note_by_description_specification_is_used
    {
        private RetrospectiveNoteByDescriptionSpecification specification;
        private RetrospectiveNote retrospectiveNote;

        [Test]
        public void Assure_it_should_be_satisfied_by_an_equal_description() 
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("A retrospective note with description 'Det gikk bra' is created", () =>
                    retrospectiveNote = new RetrospectiveNote { Description = "Det gikk bra" });
                scenario.When("A retrospective note by description specification is used with the description 'Det gikk bra'", () =>
                    specification = new RetrospectiveNoteByDescriptionSpecification("Det gikk bra"));
                scenario.Then("The specification should be true for the retrospective note", () =>
                    specification.IsSatisfiedBy(retrospectiveNote).ShouldBeTrue());
            });
        }

        [Test]
        public void Assure_it_should_not_be_satisfied_by_an_unequal_description()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("A retrospective note with description 'Det gikk bra' is created", () =>
                    retrospectiveNote = new RetrospectiveNote { Description = "Det gikk bra" });
                scenario.When("A retrospective note by description specification is used with the description 'tull'", () =>
                    specification = new RetrospectiveNoteByDescriptionSpecification("tull"));
                scenario.Then("The specification should be false for the retrospective ntoe", () =>
                    specification.IsSatisfiedBy(retrospectiveNote).ShouldBeFalse());
            });
        }
    }

    [TestFixture]
    public class When_RetrospectivePositiveNoteSpecification_is_used
    {
        private RetrospectivePositiveNoteSpecification specification;

        private RetrospectiveNote note;

        [Test]
        public void Assure_positive_specification_is_satisfied_by_positive_note()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("A positive retrospective note is created", () =>
                   note = new RetrospectiveNote { Type = NoteType.Positive });
                scenario.When("A retrospective positive note specification is used", () =>
                    specification = new RetrospectivePositiveNoteSpecification());
                scenario.Then("The specification should be satisfied by the note", () =>
                    specification.IsSatisfiedBy(note).ShouldBeTrue());
            });
        }

        [Test]
        public void Assure_positive_specification_is_not_satisfied_by_negative_note()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("Negative retrospective note is created", () =>
                    note = new RetrospectiveNote { Type = NoteType.Negative });
                scenario.When("A retrospective positive note specification is used", () =>
                    specification = new RetrospectivePositiveNoteSpecification());
                scenario.Then("The specification should not be satisfied by the note", () =>
                    specification.IsSatisfiedBy(note).ShouldBeFalse());
            });
        }
    }

    [TestFixture]
    public class When_RetrospectiveNegativeNoteSpecification_is_used
    {
        private RetrospectiveNote note;

        private RetrospectiveNegativeNoteSpecification specification;

        [Test]
        public void Assure_negative_specification_is_satisfied_by_negative_note()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("A negative retrospective note is created", () =>
                   note = new RetrospectiveNote { Type = NoteType.Negative });
                scenario.When("A retrospective negative note specification is used", () =>
                    specification = new RetrospectiveNegativeNoteSpecification());
                scenario.Then("The specification should be satisfied by the note", () =>
                    specification.IsSatisfiedBy(note).ShouldBeTrue());
            });
        }

        [Test]
        public void Assure_negative_specification_is_not_satisfied_by_positive_note()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given("A positive retrospective note is created", () =>
                   note = new RetrospectiveNote { Type = NoteType.Positive });
                scenario.When("A retrospective negative note specification is used", () =>
                    specification = new RetrospectiveNegativeNoteSpecification());
                scenario.Then("The specification should not be satisfied by the note", () =>
                    specification.IsSatisfiedBy(note).ShouldBeFalse());
            });
        }
    }
}
