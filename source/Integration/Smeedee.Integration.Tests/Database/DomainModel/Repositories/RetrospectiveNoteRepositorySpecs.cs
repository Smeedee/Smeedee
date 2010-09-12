using System;
using System.Collections.Generic;
using System.Linq;

using Smeedee.DomainModel.Corkboard;
using Smeedee.DomainModel.Framework;
using Smeedee.Integration.Database.DomainModel.Repositories;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.Database.DomainModel.Repositories.RetrospectiveNoteRepositorySpecs
{
    

    [TestFixture][Category("IntegrationTest")]
    public class When_fetching_RetrospectiveNotes_from_an_empty_repository : RetrospectiveNoteRepositoryShared
    {
        [Test]
        public void Assure_there_are_no_rows_in_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_exists)
                        .And(Repository_is_created);

                scenario.When("Get is called", () =>
                              resultSet = repository.Get(new AllSpecification<RetrospectiveNote>()));

                scenario.Then("Assure result set is empty", () =>
                              resultSet.Count().ShouldBe(0));
            });
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_fetching_RetrospectiveNotes_from_a_repository_containing_data : RetrospectiveNoteRepositoryShared
    {
        [Test]
        public void Assure_there_are_some_rows_in_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_exists)
                        .And(Repository_is_created)
                        .And(Database_contains_RetrospectiveNote_data);

                scenario.When("Get is called", () =>
                              resultSet = repository.Get(new AllSpecification<RetrospectiveNote>()));

                scenario.Then("Assure result set is not empty", () =>
                              resultSet.Count().ShouldNotBeSameAs(0));
            });  
        }

        [Test]
        public void Assure_note_with_null_description_returns_the_empty_string()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_exists)
                        .And(Repository_is_created)
                        .And(Database_contains_a_RetrospectiveNote_with_null_description);

                scenario.When("Get is called", () =>
                              resultSet = repository.Get(new AllSpecification<RetrospectiveNote>()));

                scenario.Then("Assure description note is the empty string", () =>
                              resultSet.First().Description.ShouldBe(string.Empty));
            });
        }
    }

    [TestFixture][Category("IntegrationTest")][Category("IntegrationTest")]
    public class When_deleting_files_from_an_repository_containing_data : RetrospectiveNoteRepositoryShared
    {
        [Test]
        public void Assure_the_number_of_notes_in_the_repository_has_decreased() 
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_exists)
                        .And(Repository_is_created)
                        .And(Database_contains_RetrospectiveNote_data);
                    
                scenario.When("Delete is called", () =>
                        repository.Delete(new RetrospectiveNoteByDescriptionSpecification("test note")));

                scenario.Then("Assure that the repository size has decreased by one", () =>
                        repository.Get(new AllSpecification<RetrospectiveNote>())
                        .Count().ShouldBe(0));
            });
        }

        [Test]
        public void Assure_that_only_objects_that_exists_in_the_database_can_be_deleted()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_exists)
                        .And(Repository_is_created)
                        .And(Database_contains_RetrospectiveNote_data);

                scenario.When("Delete is called", () =>
                        repository.Delete(new RetrospectiveNoteByDescriptionSpecification("non existing description")));

                scenario.Then("Assure that the repository size has not decreased by one", () =>
                        repository.Get(new AllSpecification<RetrospectiveNote>())
                        .Count().ShouldBe(1));
            });
        }

        [Test]
        public void Assure_that_a_retrospective_note_with_an_empty_string_description_can_be_deleted()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_exists)
                        .And(Repository_is_created)
                        .And(Database_contains_RetrospectiveNote_data)
                        .And(Database_contains_a_RetrospectiveNote_with_empty_string_description);

                scenario.When("Delete is called", () => 
                        repository.Delete(new RetrospectiveNoteByDescriptionSpecification("")));

                scenario.Then("Assure that the repository size has decreased by one", () =>
                        repository.Get(new AllSpecification<RetrospectiveNote>())
                        .Count().ShouldBe(1));
            });
        }

        [Test]
        public void Assure_that_calling_delete_with_an_allspecificiaton_parameter_clears_the_repository()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_exists)
                        .And(Repository_is_created)
                        .And(Database_contains_RetrospectiveNote_data)
                        .And(Database_contains_a_RetrospectiveNote_with_null_description);

                scenario.When("Delete(Allspecification<RetrospectiveNote> is called", () =>
                        repository.Delete(new AllSpecification<RetrospectiveNote>()));

                scenario.Then("Assure that the repository is empty", () =>
                        repository.Get(new AllSpecification<RetrospectiveNote>()).Count().ShouldBe(0));
            });
        }        
    }

    [TestFixture][Category("IntegrationTest")]
    [Category("IntegrationTest")]
    public class When_saving_to_repository : RetrospectiveNoteRepositoryShared
    {
        [Test]
        public void assure_it_clears_database_for_notes_with_same_id_before_saving_enumerables()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_exists)
                    .And(Repository_is_created)
                    .And(Database_contains_4_notes_with_same_id_and_3_with_other_id);

                scenario.When("a list of RetrospectiveNotes are saved", () =>
                    repository.Save(new List<RetrospectiveNote> { new RetrospectiveNote { Description = "Something different" , Id = id1}}));

                scenario.Then("it should contain the new note and the 3 with other id", () => 
                    repository.Get(new AllSpecification<RetrospectiveNote>()).Count().ShouldBe(4));
            });       
        }

        [Test]
        [ExpectedException(typeof(NHibernate.NonUniqueObjectException))]
        public void assure_database_throws_an_exception_when_given_duplicate_data()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_exists)
                    .And(Repository_is_created)
                    .And(Database_contains_many_RetrospectiveNotes);

                scenario.When(a_list_of_retrospectiveNotes_containing_duplicate_data_is_saved);

                scenario.Then("it should throw a NonUniqueObjectException");
                   
            }); 
        }

        [Test]
        public void assure_database_rollbacks_to_previous_version_when_given_duplicate_data()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_exists)
                    .And(Repository_is_created)
                    .And(Database_contains_many_RetrospectiveNotes);

                scenario.When(a_list_of_retrospectiveNotes_containing_duplicate_data_is_saved_and_exception_is_ignored);

                scenario.Then("it should contain its old data", () =>
                {
                    var notes = repository.Get(new AllSpecification<RetrospectiveNote>()).Select(t => t.Description).ToArray();

                    for (int i = 0; i < 8; i++)
                    {
                        notes[i].ShouldBe(i.ToString());
                    }
                });
            }); 
        }
    }

    public class RetrospectiveNoteRepositoryShared : Shared
    {
        protected static RetrospectiveNoteDatabaseRepository repository;
        protected IEnumerable<RetrospectiveNote> resultSet;

        protected static string id1;
        protected static string id2;

        protected Context Repository_is_created = () =>
        {
            repository = new RetrospectiveNoteDatabaseRepository(sessionFactory);
        };

        protected Context Database_contains_RetrospectiveNote_data = () =>
        {
            var note = new RetrospectiveNote { Description = "test note" };
            repository.Save(note);
        };

        protected Context Database_contains_many_RetrospectiveNotes = () =>
        {
            var notes = new List<RetrospectiveNote>();

            for (int i = 0; i < 8; i++)
                notes.Add(new RetrospectiveNote {Description = i.ToString()});

            repository.Save(notes);
        };

        protected Context Database_contains_4_notes_with_same_id_and_3_with_other_id = () =>
        {
            var notes = new List<RetrospectiveNote>();

            for (int i = 0; i < 7; i++)
                if(i < 4)
                    notes.Add(new RetrospectiveNote { Description = i.ToString(), Id = id1 });
                else
                    notes.Add(new RetrospectiveNote { Description = i.ToString(), Id = id2 });

            repository.Save(notes);

        };
        protected Context Database_contains_a_RetrospectiveNote_with_null_description = () =>
        {
            var note = new RetrospectiveNote { Description = null };
            repository.Save(note);
        };

        protected Context Database_contains_a_RetrospectiveNote_with_empty_string_description = () =>
        {
            var note = new RetrospectiveNote { Description = "" };
            repository.Save(note);
        };

        protected When a_list_of_retrospectiveNotes_containing_duplicate_data_is_saved = () =>
        {
            var notes = new List<RetrospectiveNote>();
            for (int i = 0; i < 8; i++)
                notes.Add(new RetrospectiveNote {Description = "3", Id="3"});

            repository.Save(notes);
        };

        protected When a_list_of_retrospectiveNotes_containing_duplicate_data_is_saved_and_exception_is_ignored = () =>
        {
            var notes = new List<RetrospectiveNote>();
            for (int i = 0; i < 8; i++)
                notes.Add(new RetrospectiveNote { Description = "3" });
            
            try
            {
                repository.Save(notes);
            }
            catch(NHibernate.NonUniqueObjectException e)
            {
                //ignore it;
            } 
        };

        [SetUp]
        public void Setup()
        {
            id1 = new Guid(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 6, 7 }).ToString();
            id2 = new Guid().ToString();
        }


    }
}