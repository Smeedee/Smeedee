using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using APD.DomainModel.Framework;
using APD.Integration.Framework.Atom.DomainModel;
using APD.Integration.Framework.Atom.DomainModel.Factories;
using APD.Integration.Framework.Atom.DomainModel.Repositories;
using APD.Integration.Framework.Atom.DomainModel.Repositories.AtomEntrySpecifications;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;


namespace APD.Plugin.RSS.Tests.AtomEntryRepositorySpecs
{
    public class Shared : ScenarioClass
    {
        private const string ADDRESS_INVALID = @"http://www.thisisnoserver.com/feed.html";
        private const string ADDRESS_MULTIPLE_ENTRIES = @"Atom\TestData\MultipleEntryFeed.xml";
        private const string ADDRESS_NO_ENTRY = @"Atom\TestData\EmptyFeed.xml";
        private const string ADDRESS_SINGLE_ENTRIES = @"Atom\TestData\SingleEntryFeed.xml";

        protected static IEnumerable<TestAtomEntry> entries;
        protected static AtomEntryRepository<TestAtomEntry> repository;
        protected static TestAtomEntry singleEntry;
        protected static Exception thrownException;

        #region Givens

        protected static Context address_is_invalid =
            () => { repository.Address = new Uri(ADDRESS_INVALID, UriKind.Absolute); };

        protected readonly Context repository_has_been_created =
            () =>
            {
                repository = new AtomEntryRepository<TestAtomEntry>(null)
                {
                    EntryFactory = new TestAtomEntryFactory()
                };
            };

        protected readonly Context there_are_multiple_entries =
            () => { repository.Address = new Uri(ADDRESS_MULTIPLE_ENTRIES, UriKind.Relative); };

        protected readonly Context there_are_no_entries =
            () => { repository.Address = new Uri(ADDRESS_NO_ENTRY, UriKind.Relative); };

        protected readonly Context there_is_one_entry =
            () => { repository.Address = new Uri(ADDRESS_SINGLE_ENTRIES, UriKind.Relative); };

        #endregion

        #region Whens

        protected readonly When asking_for_all_rss_entries = () =>
        {
            try
            {
                entries = repository.Get(new AllSpecification<TestAtomEntry>());
            }
            catch (Exception e)
            {
                thrownException = e;
            }
        };

        protected readonly When asking_for_entries_newer_than_middle = () =>
                entries = repository.Get(
                        new AtomEntriesNewerThanSpecification<TestAtomEntry>(
                            new DateTime(2009, 09, 16, 11, 0, 0)));

        protected readonly When asking_for_entries_newer_than_newest =
            () => entries =
                    repository.Get(
                        new AtomEntriesNewerThanSpecification<TestAtomEntry>(
                            new DateTime(2009, 09, 16, 12, 0, 1)));

        protected readonly When asking_for_entries_older_than_oldest =
            () => entries =
                    repository.Get(new AtomEntriesNewerThanSpecification<TestAtomEntry>(
                        new DateTime(2009, 09, 15, 11, 0, 0)));

        #endregion

        #region Thens

        protected readonly Then assert_all_entries_are_returned = () =>
        {
            Assert.AreEqual(3, entries.Count());
            Assert.IsTrue(entries.Contains(TestEntries.Entry1));
            Assert.IsTrue(entries.Contains(TestEntries.Entry2));
            Assert.IsTrue(entries.Contains(TestEntries.Entry3));
        };

        protected readonly Then assert_the_list_of_entries_is_empty =
            () => Assert.AreEqual(0, entries.Count());

        #endregion

        [TearDown]
        protected void RunTests()
        {
            StartScenario();
        }

        #region Nested type: TestEntries

        protected static class TestEntries
        {
            internal static readonly TestAtomEntry Entry1 = new TestAtomEntry
            {
                Title = "Entry 1",
                Author = "Eivind",
                Updated = new DateTime(2009, 09, 15, 12, 0, 0)
            };

            internal static readonly TestAtomEntry Entry2 = new TestAtomEntry
            {
                Title = "Entry 2",
                Author = "Gøran",
                Updated = new DateTime(2009, 09, 16, 12, 0, 0)
            };

            internal static readonly TestAtomEntry Entry3 = new TestAtomEntry
            {
                Title = "Entry 3",
                Author = "Eivind",
                Updated = new DateTime(2009, 09, 16, 11, 0, 0)
            };
        }

        #endregion
    }

    [TestFixture]
    public class Cannot_connect_to_server : Shared
    {
        [Test]
        public void Assert_exception_is_thrown()
        {
            Given(repository_has_been_created).And(address_is_invalid);
            When(asking_for_all_rss_entries);
            Then("assert exception is thrown", 
                () => Assert.IsInstanceOfType(typeof(AtomFeedRepositoryException), thrownException));
        }
    }

    [TestFixture]
    public class No_entries_exist : Shared
    {
        [Test]
        public void Assert_get_all_returns_empty_list()
        {
            Given(repository_has_been_created).And(there_are_no_entries);
            When(asking_for_all_rss_entries);
            Then(assert_the_list_of_entries_is_empty);
        }

        [Test]
        public void Assert_get_newer_than_date_returns_empty_list()
        {
            Given(repository_has_been_created).And(there_are_no_entries);
            When(asking_for_entries_older_than_oldest);
            Then("assert the right entries are returned",
                 () => Assert.AreEqual(0, entries.Count()));
        }
    }

    [TestFixture]
    public class One_entry_exists : Shared
    {
        [Test]
        public void Assert_get_all_returns_entry()
        {
            Given(repository_has_been_created).And(there_is_one_entry);
            When(asking_for_all_rss_entries);
            Then("assert the entry is returned", () =>
            {
                Assert.AreEqual(1, entries.Count());
                Assert.AreEqual(TestEntries.Entry1, entries.FirstOrDefault());
            });
        }
    }

    [TestFixture]
    public class Multiple_entries_exist : Shared
    {
        [Test]
        public void Assert_get_all_returns_all_entries()
        {
            Given(repository_has_been_created).And(there_are_multiple_entries);
            When(asking_for_all_rss_entries);
            Then(assert_all_entries_are_returned);
        }

        [Test]
        [Ignore("object has no 'updated' property, so list cannot be ordered by date")]
        public void Assert_get_all_returns_newest_entry_first()
        {
            Given(repository_has_been_created).And(there_are_multiple_entries);
            When(asking_for_all_rss_entries);
            Then("assert first entry in list is the newest",
                 () => Assert.AreEqual(TestEntries.Entry2, entries.FirstOrDefault()));
        }

        [Test]
        public void Assert_get_newer_than_middle_date_returns_right_entries()
        {
            Given(repository_has_been_created).And(there_are_multiple_entries);
            When(asking_for_entries_newer_than_middle);
            Then("assert the right entries are returned", () =>
            {
                Assert.AreEqual(2, entries.Count());
                Assert.IsTrue(entries.Contains(TestEntries.Entry2));
                Assert.IsTrue(entries.Contains(TestEntries.Entry3));
            });
        }

        [Test]
        public void Assert_get_newer_than_newest_returns_empty_list()
        {
            Given(repository_has_been_created).And(there_are_multiple_entries);
            When(asking_for_entries_newer_than_newest);
            Then(assert_the_list_of_entries_is_empty);
        }

        [Test]
        public void Assert_get_older_than_oldest_returns_all_entries()
        {
            Given(repository_has_been_created).And(there_are_multiple_entries);
            When(asking_for_entries_older_than_oldest);
            Then(assert_all_entries_are_returned);
        }
    }

    #region Dummy classes

    public class TestAtomEntry : AtomEntry
    {
        public override string ToString()
        {
            return string.Format("Atom entry: [{0}, {1}, {2}]", Title, Author, Updated);
        }

        public bool Equals(TestAtomEntry other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.Title, Title) && other.Updated.Equals(Updated) && Equals(other.Author, Author);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(TestAtomEntry))
            {
                return false;
            }
            return Equals((TestAtomEntry)obj);
        }
    }

    internal class TestAtomEntryFactory : AtomEntryFactory<TestAtomEntry>
    {
        public TestAtomEntry Assemble(XElement xmlEntry)
        {
            XNamespace atomNs = "http://www.w3.org/2005/Atom";

            return new TestAtomEntry
            {
                Author = xmlEntry.Element(atomNs + "author").Element(atomNs + "name").Value,
                Title = xmlEntry.Element(atomNs + "title").Value,
                Updated = Convert.ToDateTime(xmlEntry.Element(atomNs + "updated").Value)
            };
        }
    }

    #endregion
}