using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using APD.DomainModel.Framework;
using APD.Integration.Framework.Atom.DomainModel;
using APD.Integration.Framework.Atom.DomainModel.Factories;
using APD.Integration.Framework.Atom.DomainModel.Repositories;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;


namespace APD.Integration.FrameworkTests.Atom.DomainModel.Repositories.AtomFeedRepositorySpecs
{
    public class Shared : ScenarioClass
    {
        private const string ADDRESS_MULTIPLE_ENTRIES = @"Atom\TestData\MultipleEntryFeed.xml";
        private const string ADDRESS_NO_ENTRY = @"Atom\TestData\EmptyFeed.xml";
        private const string ADDRESS_MULTIPLE_FEEDS = @"Atom\TestData\MultipleFeedsFeed.xml";

        protected static IEnumerable<TestAtomFeed> feeds;
        protected static AtomFeedRepository<TestAtomFeed, AtomEntry> repository;

        #region Givens

        protected Context repository_has_been_created = () =>
        {
            repository = new AtomFeedRepository<TestAtomFeed, AtomEntry>(null)
            {
                FeedFactory = new TestAtomFeedFactory
                {
                    EntryFactory = new TestAtomEntryFactory()
                }
            };
        };

        protected Context there_is_one_feed_with_entries =
            () => repository.Address = new Uri(ADDRESS_MULTIPLE_ENTRIES, UriKind.Relative);

        protected Context there_is_one_feed_with_no_entries =
            () => repository.Address = new Uri(ADDRESS_NO_ENTRY, UriKind.Relative);

        protected Context there_are_multiple_feeds =
            () => repository.Address = new Uri(ADDRESS_MULTIPLE_FEEDS, UriKind.Relative);

        #endregion

        #region Whens

        protected When asking_for_all_feeds =
            () => feeds = repository.Get(new AllSpecification<TestAtomFeed>());

        #endregion

        [TearDown]
        protected void RunTests()
        {
            StartScenario();
        }

        #region Nested type: TestObjects

        protected static class TestObjects
        {
            internal static readonly TestAtomFeed Feed1 = new TestAtomFeed
            {
                Title = "Feed 1",
                Updated = new DateTime(2009, 9, 15, 12, 0, 0),
                Entries = new List<AtomEntry>()
            };

            internal static readonly TestAtomFeed Feed2 = new TestAtomFeed
            {
                Title = "Feed 2",
                Updated = new DateTime(2009, 9, 15, 12, 0, 0),
                Entries = new List<AtomEntry> {new AtomEntry(), new AtomEntry(), new AtomEntry()}
            };

            internal static readonly TestAtomFeed Feed3 = new TestAtomFeed
            {
                Title = "Feed 3",
                Updated = new DateTime(2009, 09, 29, 12, 0, 0)
            };

            internal static readonly TestAtomFeed Feed4 = new TestAtomFeed
            {
                Title = "Feed 4",
                Updated = new DateTime(2009, 09, 29, 13, 0, 0)
            };

            internal static readonly TestAtomFeed Feed5 = new TestAtomFeed
            {
                Title = "Feed 5",
                Updated = new DateTime(2009, 09, 28, 12, 0, 0)
            };
        }

        #endregion
    }

    [TestFixture]
    public class One_feed_with_no_entries_exists : Shared
    {
        [Test]
        public void Assert_get_all_returns_right_feed()
        {
            Given(repository_has_been_created).And(there_is_one_feed_with_no_entries);
            When(asking_for_all_feeds);
            Then("assert one feed with no entries is returned", () =>
            {
                Assert.AreEqual(feeds.Count(), 1);
                Assert.AreEqual(feeds.First(), TestObjects.Feed1);
                Assert.AreEqual(feeds.First().Entries.Count(), 0);
            });
        }
    }

    [TestFixture]
    public class One_feed_with_entries_exists : Shared
    {
        [Test]
        public void Assert_get_all_returns_feed_with_all_entries()
        {
            Given(repository_has_been_created).And(there_is_one_feed_with_entries);
            When(asking_for_all_feeds);
            Then("assert returned feed contains all entries",
                 () => Assert.AreEqual(3, feeds.First().Entries.Count()));
        }

        [Test]
        public void Assert_get_all_returns_right_feed()
        {
            Given(repository_has_been_created).And(there_is_one_feed_with_entries);
            When(asking_for_all_feeds);
            Then("assert right feed is returned", () =>
            {
                Assert.AreEqual(1, feeds.Count());
                Assert.AreEqual(TestObjects.Feed2, feeds.First());
            });
        }
    }

    [TestFixture]
    public class Multiple_feeds_exist : Shared
    {
        [Test]
        [Ignore("Wrong assumption: multiple feed tags per feed")]
        public void Assert_get_all_returns_all_feeds()
        {
            Given(repository_has_been_created).And(there_are_multiple_feeds);
            When(asking_for_all_feeds);
            Then("assert all feeds are returned", () =>
            {
                Assert.IsTrue(feeds.Contains(TestObjects.Feed3));
                Assert.IsTrue(feeds.Contains(TestObjects.Feed4));
                Assert.IsTrue(feeds.Contains(TestObjects.Feed5));
            });
        }
    }

    public class TestAtomFeed : AtomFeed<AtomEntry>
    {
        public bool Equals(TestAtomFeed other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.Title, Title) && other.Updated.Equals(Updated) &&
                   Equals(other.Entries.Count(), Entries.Count());
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
            if (obj.GetType() != typeof (TestAtomFeed))
            {
                return false;
            }
            return Equals((TestAtomFeed) obj);
        }

        public override string ToString()
        {
            return string.Format("Atom feed: [{0}, {1}, {2}]", Title, Updated, Entries);
        }
    }

    internal class TestAtomFeedFactory : AtomFeedFactory<TestAtomFeed, AtomEntry>
    {
        #region AtomFeedFactory<TestAtomFeed,AtomEntry> Members

        public AtomEntryFactory<AtomEntry> EntryFactory { get; set; }

        public TestAtomFeed Assemble(XElement xmlEntry)
        {
            XNamespace atomNs = "http://www.w3.org/2005/Atom";

            return new TestAtomFeed
            {
                Title = xmlEntry.Element(atomNs + "title").Value,
                Updated = Convert.ToDateTime(xmlEntry.Element(atomNs + "updated").Value),
                Entries = from entry in xmlEntry.Elements(atomNs + "entry")
                          select EntryFactory.Assemble(entry)
            };
        }

        #endregion
    }

    internal class TestAtomEntryFactory : AtomEntryFactory<AtomEntry>
    {
        #region AtomEntryFactory<AtomEntry> Members

        public AtomEntry Assemble(XElement xmlEntry)
        {
            return new AtomEntry();
        }

        #endregion
    }
}