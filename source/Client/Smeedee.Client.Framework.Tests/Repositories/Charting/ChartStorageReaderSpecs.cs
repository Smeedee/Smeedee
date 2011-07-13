using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Repositories.Charting;
using Smeedee.Client.Framework.Repositories.NoSql;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.NoSql;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Framework.Tests.Repositories.Charting
{
    public class ChartStorageReaderSpecs
    {
        
        [TestFixture]
        public class When_refreshing_data_sources : Shared
        {
            private static int refresh_event_raised=0;

            [Test]
            public void Assure_that_refresh_event_is_raised()
            {
                Given(the_storage_has_been_created).
                    And(there_are_no_databases).
                    And(we_are_listening_for_refresh_event);
                When(calling_refresh_datasources);
                Then("event should have signaled that datasources has been refreshed", () =>
                    refresh_event_raised.ShouldBe(1));
            }

            [Test]
            public void Assure_that_no_data_is_returned_when_there_are_no_databases()
            {
                Given(the_storage_has_been_created).
                    And(there_are_no_databases).
                    And(we_are_listening_for_refresh_event);
                When(calling_refresh_datasources);
                Then("storage should return an empty list of databases", () =>
                {
                    storage.GetDatabases().Count.ShouldBe(0);
                }
                );
            }

            [Test]
            public void Assure_that_one_database_is_returned_when_there_is_only_one()
            {
                Given(the_storage_has_been_created).
                    And(there_is_one_database).
                    And(we_are_listening_for_refresh_event);
                When(calling_refresh_datasources);
                Then("storage should return a list with two database", () =>
                {
                    var databases = storage.GetDatabases();
                    databases.Count.ShouldBe(1);
                    databases[0].ShouldBe("TestDatabase");
                });
            }

            [Test]
            public void Assure_that_two_databases_are_returned_when_there_are_two()
            {
                Given(the_storage_has_been_created).
                    And(there_are_two_databases).
                    And(we_are_listening_for_refresh_event);
                When(calling_refresh_datasources);
                Then("storage should return a list with two database", () =>
                {
                    var databases = storage.GetDatabases();
                    databases.Count.ShouldBe(2);
                    databases[0].ShouldBe("TestDatabase");
                    databases[1].ShouldBe("TestDatabase2");
                });                
            }

            [Test]
            public void Assure_that_it_is_possible_to_return_collections_in_a_spesific_database()
            {
                Given(the_storage_has_been_created).
                    And(there_is_a_database_with_collections).
                    And(we_are_listening_for_refresh_event);
                When(calling_refresh_datasources);
                Then("storage should return collections", () =>
                {
                    var collections = storage.GetCollectionsInDatabase("TestDatabase");
                    collections.Count.ShouldBe(2);
                    collections[0].ShouldBe("Test1");
                    collections[1].ShouldBe("Test2");
                });                
                
            }

            private Context there_are_no_databases = () =>
                NoSqlRepositoryGetDatabasesReturns(new Collection());

            private Context there_is_one_database = () =>
                NoSqlRepositoryGetDatabasesReturns(new Collection { Documents = { Document.Parse("{Name:\"TestDatabase\", Collections: []}") } });

            private Context there_are_two_databases = () =>
                NoSqlRepositoryGetDatabasesReturns(new Collection { Documents = { Document.Parse("{Name:\"TestDatabase\", Collections: []}"), Document.Parse("{Name:\"TestDatabase2\", Collections: []}") } });

            private Context there_is_a_database_with_collections = () =>
                NoSqlRepositoryGetDatabasesReturns(new Collection { Documents = { Document.Parse("{Name:\"TestDatabase\", Collections: [ { Name: \"Test1\"}, { Name: \"Test2\" } ]}") } });

            private Context we_are_listening_for_refresh_event = () =>
            {
                refresh_event_raised = 0;
                storage.DatasourcesRefreshed += RefreshEventRaised;
            };


            private When calling_refresh_datasources = () =>
                storage.RefreshDatasources();

                
            private static void RefreshEventRaised(object sender, EventArgs args)
            {
                refresh_event_raised++;
            }
        }



        public class Shared : ScenarioClass
        {
            protected static Mock<INoSqlRepository> noSqlRepositoryMock;

            protected static ChartStorageReader storage;

            protected Context the_storage_has_been_created = () =>
                storage = new ChartStorageReader(noSqlRepositoryMock.Object);

            [SetUp]
            public void SetUp()
            {
                noSqlRepositoryMock = new Mock<INoSqlRepository>();
                storage = null;
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }


            protected static void NoSqlRepositoryGetDatabasesReturns(Collection databases)
            {
                noSqlRepositoryMock.Setup(s => s.GetDatabases(It.IsAny<Action<Collection>>())).
                    Callback((Action<Collection> callback) => callback(databases));
            }
        }
    }
}
