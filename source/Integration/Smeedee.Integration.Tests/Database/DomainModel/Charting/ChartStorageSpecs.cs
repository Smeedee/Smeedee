using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.NoSql;
using Smeedee.Integration.Database.DomainModel.Charting;
using Smeedee.DomainModel.Charting;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Integration.Tests.Database.DomainModel.Charting
{
    class ChartStorageSpecs
    {
        [TestFixture]
        public class When_saving_chart : Shared
        {

            [Test]
            [ExpectedException("System.NullReferenceException")]
            public void Assure_that_trying_to_save_chart_without_database_and_collection_throws_exception()
            {
                Given(the_storage_has_been_created).
                    And(an_empty_chart_has_been_created);
                When(saving_chart);
                Then("Expecting an exception");

                Run(); // must run here, because exception will not be caught at the right place otherwise
            }

            [Test]
            public void Assure_that_chart_without_data_is_saved()
            {
                Given(the_storage_has_been_created).
                    And(a_chart_without_data_has_been_created).
                    And(no_database_exists);
                When(saving_chart);
                Then("Expect a NoSqlDatabase-object to be saved", () => noSqlPersistRepositoryMock.Verify(
                    a => a.Save(It.Is<NoSqlDatabase>(d => (d.Name == "database" && d.GetCollection("collection").Documents.Count == 0)) )
                ));
            }

            [Test]
            public void Assure_that_chart_with_data_is_saved()
            {
                Given(the_storage_has_been_created).
                    And(a_chart_with_data_has_been_created).
                    And(no_database_exists);
                When(saving_chart);
                Then(expect_correct_datastructure);
            }


            private Context an_empty_chart_has_been_created = () =>
                chart = new Chart();

            

            private Context a_chart_with_data_has_been_created = () =>
            {
                chart = new Chart("testbase", "testcol");
                var set = new DataSet("testset");
                set.DataPoints.Add(0);
                set.DataPoints.Add(1);
                chart.DataSets.Add(set);
                set = new DataSet("test2");
                set.DataPoints.Add(42);
                chart.DataSets.Add(set);
            };

            private Then expect_correct_datastructure = () =>
                noSqlPersistRepositoryMock.Verify(
                    a => a.Save(
                        It.Is<NoSqlDatabase>(d => ValidateDatastructure(d))));

            private static bool ValidateDatastructure(NoSqlDatabase d)
            {
                var collection = d.GetCollection("testcol");
                var doc0 = collection.Documents[0];
                var doc1 = collection.Documents[1];

                return d.Name == "testbase" &&
                       d.Collections.Count == 1 &&
                       collection.Documents.Count == 2 &&
                       doc0["Name"].Value<string>() == "testset" &&
                       doc1["Name"].Value<string>() == "test2" &&
                       doc0["DataPoints"].Count() == 2 &&
                       doc1["DataPoints"].Count() == 1 &&
                       doc0["DataPoints"][0].Value<int>() == 0 &&
                       doc0["DataPoints"][1].Value<int>() == 1 &&
                       doc1["DataPoints"][0].Value<int>() == 42;
            }

            private Context no_database_exists = () =>
            {
                var list = new List<NoSqlDatabase>();

                noSqlRepositoryMock.Setup(
                    g =>
                    g.BeginGet(
                        It.IsAny<Specification<NoSqlDatabase>>())).
                        Raises(f => f.GetCompleted += null, new GetCompletedEventArgs<NoSqlDatabase>(list, null));
            };
            
        }

        [TestFixture]
        public class When_database_already_exists : Shared
        {
            [Test]            
            public void Assure_that_database_is_not_overwritten()
            {
                Given(the_storage_has_been_created).
                    And(a_chart_without_data_has_been_created).
                    And(the_database_already_exists);
                When(saving_chart);
                Then(validate_that_the_other_collection_is_still_in_database);
            }

            private Context the_database_already_exists = () =>
            {
                var database = new NoSqlDatabase() {Name = "database"};
                var anotherCollection = database.GetCollection("another");
                anotherCollection.Insert(Document.Parse("{Test:1}"));
                var list = new List<NoSqlDatabase> {database};

                noSqlRepositoryMock.Setup(
                    g =>
                    g.BeginGet(
                        It.IsAny<Specification<NoSqlDatabase>>())).
                        Raises(f => f.GetCompleted += null, new GetCompletedEventArgs<NoSqlDatabase>(list, null));
            };

            private Then validate_that_the_other_collection_is_still_in_database = () =>
            {
                noSqlPersistRepositoryMock.Verify( a => a.Save(
                        It.Is<NoSqlDatabase>(d => ValidateDatabase(d))));
            };

            private static bool ValidateDatabase(NoSqlDatabase d)
            {
                var another = d.GetCollection("another");
                return another.Documents.Count == 1;
            }
        }

        public class Shared : ScenarioClass
        {
            protected static ChartStorage storage;
            protected static Chart chart;
            protected static Mock<IPersistDomainModelsAsync<NoSqlDatabase>> noSqlPersistRepositoryMock;
            protected static Mock<IAsyncRepository<NoSqlDatabase>> noSqlRepositoryMock;

            protected Context the_storage_has_been_created = () =>
                storage = new ChartStorage(noSqlRepositoryMock.Object, noSqlPersistRepositoryMock.Object);

            protected Context a_chart_without_data_has_been_created = () =>
                chart = new Chart("database", "collection");

            protected When saving_chart = () =>
                storage.Save(chart);

            protected bool hasRun = false;

            public void Run()
            {
                if (hasRun) return;
                hasRun = true;
                StartScenario();
            }

            [SetUp]
            public void SetUp()
            {
                noSqlPersistRepositoryMock = new Mock<IPersistDomainModelsAsync<NoSqlDatabase>>();
                noSqlRepositoryMock = new Mock<IAsyncRepository<NoSqlDatabase>>();
                storage = null;
                chart = null;
                hasRun = false;
            }

            [TearDown]
            public void TearDown()
            {
                Run();
            }

            public static bool Compare(Chart a, Chart b)
            {
                if (a.Database != b.Database) return false;
                if (a.Collection != b.Collection) return false;
                if (a.DataSets.Count != b.DataSets.Count) return false;
                for (int i=0; i<a.DataSets.Count; i++)
                {
                    if (!CompareDataSet(a.DataSets[i], b.DataSets[i]))
                        return false;
                }
                return true;
            }

            private static bool CompareDataSet(DataSet a, DataSet b)
            {
                if (a.Name != b.Name) return false;
                if (a.DataPoints.Count != b.DataPoints.Count) return false;
                for (int i=0; i<a.DataPoints.Count; i++)
                {
                    if (!CompareDataPoint(a.DataPoints[i], b.DataPoints[i]))
                        return false;
                }
                return true;
            }
            private static bool CompareDataPoint(object a, object b)
            {
                return (a.Equals(b));
            }

        }
    }
}
