using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Client.Framework.Tests;
using Smeedee.DomainModel.Corkboard;
using Smeedee.Widget.Corkboard.Tests.ViewModels;
using Smeedee.Widget.Corkboard.ViewModels;
using TinyMVVM.Framework;

namespace Smeedee.Widget.Corkboard.Tests.Controllers
{
    [TestFixture]
    class LearningTest
    {
        private CorkboardViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            viewModel = new CorkboardViewModel();
            viewModel.PositiveNotes.Add(new NoteViewModel { Description = "1" });
            viewModel.PositiveNotes.Add(new NoteViewModel { Description = "1" });
            viewModel.PositiveNotes.Add(new NoteViewModel { Description = "2" });
            viewModel.PositiveNotes.Add(new NoteViewModel { Description = "2" });
            viewModel.NegativeNotes.Add(new NoteViewModel { Description = "1", Type = NoteType.Negative });
            viewModel.NegativeNotes.Add(new NoteViewModel { Description = "1", Type = NoteType.Negative });
            viewModel.NegativeNotes.Add(new NoteViewModel { Description = "2", Type = NoteType.Negative });
            viewModel.NegativeNotes.Add(new NoteViewModel { Description = "2", Type = NoteType.Negative });
        }

        [Test]
        public void How_to_select_distinct_items_from_a_collection_based_on_properties()
        {

            var distinctPositiveDescriptions = viewModel.PositiveNotes.Select(t => t.Description).Distinct();
            var distinctNegativeDescriptions = viewModel.NegativeNotes.Select(t => t.Description).Distinct();
            var list = new List<RetrospectiveNote>();

            list.AddRange(distinctPositiveDescriptions.Select(description => new RetrospectiveNote {Description = description, Type = NoteType.Positive}));
            list.AddRange(distinctNegativeDescriptions.Select(description => new RetrospectiveNote {Description = description, Type = NoteType.Negative}));


            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(2, list.Where(t => t.Type == NoteType.Positive).Count());
            Assert.AreEqual(2, list.Where(t => t.Type == NoteType.Negative).Count());
            Assert.AreEqual("1", list.Where(t => t.Type == NoteType.Positive).Select(t => t.Description).ToArray()[0]);
            Assert.AreEqual("2", list.Where(t => t.Type == NoteType.Positive).Select(t => t.Description).ToArray()[1]);
            Assert.AreEqual("1", list.Where(t => t.Type == NoteType.Negative).Select(t => t.Description).ToArray()[0]);
            Assert.AreEqual("2", list.Where(t => t.Type == NoteType.Negative).Select(t => t.Description).ToArray()[1]);

        }

        [Test]
        public void How_to_select_distinct_items_from_a_collection_based_on_properties_using_LINQ()
        {
            var notes = viewModel.PositiveNotes.Union(viewModel.NegativeNotes);
            var list = new List<RetrospectiveNote>();

            var distinctNotes = notes
                .Select(t => new
                                 {
                                     Description = t.Description,
                                     Type = t.Type
                                 }).Distinct();

            list.AddRange(distinctNotes.Select(t => new RetrospectiveNote {Description = t.Description, Type = t.Type}));

            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(2, list.Where(t => t.Type == NoteType.Positive).Count());
            Assert.AreEqual(2, list.Where(t => t.Type == NoteType.Negative).Count());
            Assert.AreEqual("1", list.Where(t => t.Type == NoteType.Positive).Select(t => t.Description).ToArray()[0]);
            Assert.AreEqual("2", list.Where(t => t.Type == NoteType.Positive).Select(t => t.Description).ToArray()[1]);
            Assert.AreEqual("1", list.Where(t => t.Type == NoteType.Negative).Select(t => t.Description).ToArray()[0]);
            Assert.AreEqual("2", list.Where(t => t.Type == NoteType.Negative).Select(t => t.Description).ToArray()[1]);

        }

        [Test]
        public void How_to_select_distinct_items_from_a_collection_based_on_single_property()
        {
            var distinctNotes = new List<NoteViewModel>();

            foreach (var note in viewModel.PositiveNotes.Union(viewModel.NegativeNotes))
            {
                if (!distinctNotes.Select(t => t.Description).Contains(note.Description))
                    distinctNotes.Add(note);
            }

            var list = distinctNotes.Select(t => new RetrospectiveNote { Description = t.Description, Type = t.Type });
    
            Assert.AreEqual(2, list.Count());
            Assert.AreEqual(2, list.Where(t => t.Type == NoteType.Positive).Count());
            Assert.AreEqual(0, list.Where(t => t.Type == NoteType.Negative).Count());
            Assert.AreEqual("1", list.Where(t => t.Type == NoteType.Positive).Select(t => t.Description).ToArray()[0]);
            Assert.AreEqual("2", list.Where(t => t.Type == NoteType.Positive).Select(t => t.Description).ToArray()[1]);

        }

    }
}
