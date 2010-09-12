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
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.Integration.Database.DomainModel.Repositories;

using NHibernate;

using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.Database.DomainModel.Repositories.ChangesetDatabaseRepositorySpecs
{

    public class ChangesetDatabaseRepositoryShared : Shared
    {

        protected static Changeset changeset1 = new Changeset
                                                {
                                                    Author = new Author {Username = "joachim"},
                                                    Comment = "added tests",
                                                    Revision = 1,
                                                    Time = new DateTime(1970, 1, 1)
                                                };

        protected static Changeset changeset2 = new Changeset
                                                {
                                                    Author = new Author {Username = "jask"},
                                                    Comment = "OM NOM NOM!",
                                                    Revision = 2,
                                                    Time = new DateTime(1970, 1, 2)
                                                };
        
        protected static Changeset changeset3 = new Changeset
                                                {
                                                    Author = new Author {Username = "joachim"},
                                                    Comment = "even more changesets!",
                                                    Revision = 3,
                                                    Time = new DateTime(1970, 1, 3)
                                                };

        protected static GenericDatabaseRepository<Changeset> repository;
        protected static ChangesetDatabaseRepository persister;

        protected Context a_ChangesetDatabaseRepository_is_created = () =>
        {
            repository = new ChangesetDatabaseRepository(sessionFactory);
        };

        protected Context there_are_changesets_in_the_database = () =>
        {
            repository.Save(changeset3);
            repository.Save(changeset2);
            repository.Save(changeset1);
        };


    }

    [TestFixture][Category("IntegrationTest")]
    public class When_get : ChangesetDatabaseRepositoryShared
    {
        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        }


        [Test]
        public void assure_changesets_are_ordered_by_revision()
        {
            IEnumerable<Changeset> result = new List<Changeset>();
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ChangesetDatabaseRepository_is_created).
                    And(there_are_changesets_in_the_database);
                scenario.When("all chagesets are requested", () =>
                    result = repository.Get(new AllChangesetsSpecification()));
                scenario.Then("assure changesets are ordered by revision", () =>
                {
                    var expectedResult = result.OrderByDescending(c => c.Revision);
                    for (int i = 0; i < result.Count(); i++)
                    {
                        result.ElementAt(i).Revision.ShouldBe(expectedResult.ElementAt(i).Revision);
                    }
                });
            });
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class when_saved : ChangesetDatabaseRepositoryShared
    {
        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        }

        [Test]
        public void should_be_created()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ChangesetDatabaseRepository_is_created);
                scenario.When("instance is accessed");
                scenario.Then("it is not null", () => repository.ShouldNotBeNull());
            });
        }

        [Test]
        public void should_implement_IRepository()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ChangesetDatabaseRepository_is_created);
                scenario.When("instance is accessed");
                scenario.Then("it implements IChangesetRepository", () => 
                    repository.ShouldBeInstanceOfType<IRepository<Changeset>>());
            });
        }

        [Test]
        public void should_be_able_to_fetch_all_changesets()
        {
            IEnumerable<Changeset> result = new List<Changeset>();
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ChangesetDatabaseRepository_is_created).
                    And(there_are_changesets_in_the_database);
                scenario.When("all chagesets are requested", () =>
                    result = repository.Get(new AllChangesetsSpecification()));
                scenario.Then("all changesets are returned", () =>
                {
                    result.Count().ShouldBe(3);
                });
            });
        }

        [Test]
        public void should_filter_changesets_on_specification()
        {
            int spesificRevision = 1;
            IEnumerable<Changeset> result = new List<Changeset>();
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ChangesetDatabaseRepository_is_created).
                    And(there_are_changesets_in_the_database);
                scenario.When("changesets after a certain revision are requested", () =>
                    result = repository.Get(new ChangesetsAfterRevisionSpecification(spesificRevision)));
                scenario.Then("all matching changesets are returned", () =>
                {
                    result.Count().ShouldBe(2);
                    foreach (Changeset changeset in result)
                    {
                        (changeset.Revision > spesificRevision).ShouldBeTrue();
                    }
                });
            });
        }

    }

    [TestFixture][Category("IntegrationTest")]
    public class When_saving : Shared
    {
        protected const string TEST_USERNAME = "Kim Bjarne me Musa";
        protected const string TEST_COMMENT = "What what in the butt butt æøå norsk test";
        protected const int TEST_REVISION_NUMBER = 1001;
        protected static readonly DateTime TEST_TIME = DateTime.Today;

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        }

        [Test]
        public void Assure_Changeset_is_saved()
        {

            var changesetPersister = new ChangesetDatabaseRepository(sessionFactory);

            var changeset = new Changeset
                            {
                                Author = new Author { Username = TEST_USERNAME },
                                Comment = TEST_COMMENT,
                                Revision = TEST_REVISION_NUMBER,
                                Time = TEST_TIME
                            };

            changesetPersister.Save(changeset);

            ISessionFactory newSessionFactory = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);

            var dbResults = DatabaseRetriever.GetDatamodelFromDatabase<Changeset>(newSessionFactory);

            dbResults[0].Author.Username.ShouldBe(TEST_USERNAME);
            dbResults[0].Comment.ShouldBe(TEST_COMMENT);
            dbResults[0].Revision.ShouldBe(TEST_REVISION_NUMBER);
            dbResults[0].Time.ShouldBe(TEST_TIME);
        }
    }
}
