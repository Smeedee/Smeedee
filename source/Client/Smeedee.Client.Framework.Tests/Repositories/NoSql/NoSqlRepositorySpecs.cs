using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Repositories.NoSql;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.NoSql;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Framework.Tests.Repositories.NoSql
{
    public class NoSqlRepositorySpecs
    {
        [TestFixture]
        public class When_there_is_no_databases : Shared
        {
            [Test]
            public void Assure_that_GetDatabases_return_an_empty_collection()
            {
                Given(the_repository_has_been_created).
                    And(there_are_no_databases_in_NoSql);
                When("fetching databases");
                Then("there should be no databases", () => repository.GetDatabases(collection => collection.Documents.Count.ShouldBe(0), ShouldNotGetHere));
            }

            private Context there_are_no_databases_in_NoSql = () => DownloadStringServiceFakeReturns("[]", null);
        }

        [TestFixture]
        public class When_there_is_one_database : Shared
        {
            [Test]
            public void Assure_that_GetDatabases_return_collection_with_one_database()
            {
                Given(the_repository_has_been_created).
                    And(there_is_one_database_in_NoSql);
                When("fetching databases");
                Then("the database should be returned", () => 
                    repository.GetDatabases(database =>
                    {
                        database.Documents.Count.ShouldBe(1);
                        var doc = database.Documents[0];
                        doc["Name"].Value<string>().ShouldBe("TestDatabase");
                    }, ShouldNotGetHere));
            }


            [Test]
            public void Assure_that_database_with_one_collection_returns_correct_data()
            {
                Given(the_repository_has_been_created).
                    And(there_is_one_database_and_one_collection_in_NoSql);
                When("fetching databases");
                Then("the database should contain one collection", () =>
                    repository.GetDatabases(database =>
                    {
                        var doc = database.Documents[0];
                        doc["Collections"][0]["Name"].Value<string>().ShouldBe("TestCollection");
                    }, ShouldNotGetHere));
            }

            private Context there_is_one_database_in_NoSql = () => DownloadStringServiceFakeReturns("[{Name: \"TestDatabase\", Collections: []}]", null);

            
        }

        [TestFixture]
        public class When_there_is_more_than_one_database : Shared
        {
            [Test]
            public void Assure_that_GetDatabases_return_collection_with_all_databases()
            {
                Given(the_repository_has_been_created).
                    And(there_is_two_databases_in_NoSql);
                When("fetching databases");
                Then("the databases should be returned", () =>
                    repository.GetDatabases(database =>
                    {
                        database.Documents.Count.ShouldBe(2);
                    }, ShouldNotGetHere));
            }

            private Context there_is_two_databases_in_NoSql = () =>
                DownloadStringServiceFakeReturns("[{Name: \"TestDatabase\", Collections: []}, {Name: \"TestDatabase2\", Collections: []}]", null);
        }

        [TestFixture]
        public class When_reading_documents : Shared
        {
            [Test]
            public void Assure_that_no_documents_are_returned_when_collection_is_empty()
            {
                Given(the_repository_has_been_created).
                    And(the_collection_is_empty);
                When("fetching documents");
                Then("the collection should be empty", () =>
                    repository.GetDocuments("database", "collection", collection =>
                        collection.Documents.Count.ShouldBe(0), ShouldNotGetHere));
            }

            [Test]
            public void Assure_that_one_document_is_returned_correctly()
            {
                Given(the_repository_has_been_created).
                    And(the_collection_has_one_document);
                When("fetching documents");
                Then("the collection should have one document", () => 
                    repository.GetDocuments("database", "collection", collection =>
                        {
                            collection.Documents.Count.ShouldBe(1);
                            collection.Documents[0]["Test"].Value<int>().ShouldBe(1);
                            collection.Documents[0]["Data"].Value<int>().ShouldBe(2);
                        },
                        ShouldNotGetHere
                    ));
            }

            [Test]
            public void Assure_that_many_documents_is_read_correctly()
            {
                Given(the_repository_has_been_created).
                    And(the_collection_has_two_documents);
                When("fetching documents");
                Then("the collection should have two documents", () =>
                    repository.GetDocuments("database", "collection", collection =>
                        {
                            collection.Documents.Count.ShouldBe(2);
                            collection.Documents[0]["Test"].Value<int>().ShouldBe(1);
                            collection.Documents[1]["Test2"].Value<int>().ShouldBe(3);
                        },
                        ShouldNotGetHere));
            }


            private Context the_collection_is_empty = () =>
                DownloadStringServiceFakeReturns("[]", null);

            private Context the_collection_has_one_document = () =>
                DownloadStringServiceFakeReturns("[{Test: 1, Data: 2}]", null);

            private Context the_collection_has_two_documents = () =>
                DownloadStringServiceFakeReturns("[{Test: 1, Data: 2}, {Test2: 3, Data2: 4}]", null);
        }

        [TestFixture]
        public class When_using_NoSqlRepository_static_methods : Shared
        {
            private static Collection databases;

            protected override void Before()
            {
                base.Before();
                databases = null;
            }

            [Test]
            public void Assure_that_it_it_possible_to_get_databases_as_a_list()
            {
                Given(the_repository_has_been_created).
                    And(there_is_one_database_and_one_collection_in_NoSql).
                And(GetDatabases_has_been_called);
                When("calling GetDatabasesAsList");
                Then("Databases should be converted to string list", () =>
                {
                    var list = NoSqlRepository.GetDatabasesAsList(databases);
                    list.Count.ShouldBe(1);
                    list[0].ShouldBe("TestDatabase");
                });
            }

            [Test]
            public void Assure_that_it_is_possible_to_get_a_collection_in_database_as_a_list()
            {
                Given(the_repository_has_been_created).
                    And(there_is_one_database_and_one_collection_in_NoSql).
                    And(GetDatabases_has_been_called);
                When("calling GetCollectionsInDatabase");
                Then("Collections should be converted to a string list", () =>
                    {
                        var list = NoSqlRepository.GetCollectionsInDatabase("TestDatabase", databases);
                        list.Count.ShouldBe(1);
                        list[0].ShouldBe("TestCollection");
                    });
            }

            [Test]
            public void Assure_that_it_is_possible_to_get_two_collections_in_database_as_a_list()
            {
                Given(the_repository_has_been_created).
                    And(there_is_two_database_and_two_collection_in_NoSql).
                    And(GetDatabases_has_been_called);
                When("calling GetCollectionsInDatabase");
                Then("Collections should be converted to a string list", () =>
                {
                    var list = NoSqlRepository.GetCollectionsInDatabase("TestDatabase", databases);
                    list.Count.ShouldBe(2);
                    list[0].ShouldBe("TestCollection");
                    list[1].ShouldBe("TestCollection2");
                });
            }

            [Test]
            public void Assure_that_it_is_possible_to_get_no_collections_in_database_as_empty_list()
            {
                Given(the_repository_has_been_created).
                    And(there_is_one_database_without_collections_in_NoSql).
                    And(GetDatabases_has_been_called);
                When("calling GetCollectionsInDatabase");
                Then("Collections should be converted to an empty string list", () =>
                {
                    var list = NoSqlRepository.GetCollectionsInDatabase("EmptyDatabase", databases);
                    list.Count.ShouldBe(0);
                });

            }

            private Context GetDatabases_has_been_called = () =>
                repository.GetDatabases(collection =>
                    {
                        databases = collection;
                    },
                    ShouldNotGetHere);

        }

        [TestFixture]
        public class When_exceptions_occur : Shared
        {
            private static int onErrorCalled;

            protected override void Before()
            {
                onErrorCalled = 0;
            }

            [Test]
            public void Then_exception_should_be_forwarded_when_getting_databases()
            {
                Given(string_service_returns_exception).
                    And(the_repository_has_been_created);
                When("fetching databases", () => repository.GetDatabases(ShouldNotGetHere, OnError));
                Then(OnError_should_have_been_called);
            }

            [Test]
            public void Then_exception_should_be_forwarded_when_getting_documents()
            {
                Given(string_service_returns_exception).
                    And(the_repository_has_been_created);
                When("fetching documents", () => repository.GetDocuments("database", "collection", ShouldNotGetHere, OnError));
                Then(OnError_should_have_been_called);
            }

            private Context string_service_returns_exception = () =>
                 DownloadStringServiceFakeReturnsException(new Exception("Test"));

            private Then OnError_should_have_been_called = () =>
                onErrorCalled.ShouldBe(1);

           
            
            private static void OnError(Exception exception)
            {
                exception.Message.ShouldBe("Test");
                onErrorCalled++;
            }
        }

        public class Shared : ScenarioClass
        {
            protected static NoSqlRepository repository;
            protected Context the_repository_has_been_created = () => repository = new NoSqlRepository(downloadStringServiceFake.Object);
            protected Context there_is_one_database_and_one_collection_in_NoSql = () =>
                DownloadStringServiceFakeReturns("[{Name: \"TestDatabase\", Collections: [{Name: \"TestCollection\"}]}]", null);

            protected Context there_is_two_database_and_two_collection_in_NoSql = () =>
                DownloadStringServiceFakeReturns("[{Name: \"EmptyDatabase\", Collections: []}, {Name: \"TestDatabase\", Collections: [{Name: \"TestCollection\"}, {Name: \"TestCollection2\"}]}]", null);

            protected Context there_is_one_database_without_collections_in_NoSql = () =>
                DownloadStringServiceFakeReturns("[{Name: \"EmptyDatabase\", Collections: [] }]", null);

            protected static Mock<IDownloadStringService> downloadStringServiceFake;

            protected virtual void Before()
            {
            }

            [SetUp]
            public void Setup()
            {
                Scenario("");
                repository = null;
                downloadStringServiceFake = new Mock<IDownloadStringService>();
                Before();
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
                
            }


            protected static void DownloadStringServiceFakeReturns(string data, Action whenDownloaded)
            {
                downloadStringServiceFake.Setup(s => s.DownloadAsync(
                    It.IsAny<Uri>(),
                    It.IsAny<Action<string>>(),
                    It.IsAny<Action<Exception>>())).Callback((Uri url, Action<string> callback, Action<Exception> onError) =>
                    {
                        callback(data);

                        if (whenDownloaded != null)
                            whenDownloaded();
                    });
            }


            protected static void DownloadStringServiceFakeReturnsException(Exception exception)
            {
                downloadStringServiceFake.Setup(s => s.DownloadAsync(
                    It.IsAny<Uri>(),
                    It.IsAny<Action<string>>(),
                    It.IsAny<Action<Exception>>())).Callback((Uri url, Action<string> callback, Action<Exception> onError) =>
                    {
                        onError(exception);
                    });
            }

            protected static void ShouldNotGetHere(Collection collection)
            {
                Assert.Fail("Should not get here!");
            }

            protected static void ShouldNotGetHere(Exception exception)
            {
                Assert.Fail("Should not get here!");
            }


        }
    }
}
