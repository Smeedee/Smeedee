using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using Smeedee.DomainModel.NoSql;
using Smeedee.Integration.Database.DomainModel.Repositories;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using Newtonsoft.Json.Linq;

namespace Smeedee.Integration.Tests.Database.DomainModel.Repositories
{
    public class SqliteNoSqlDatabaseRepositorySpecs
    {
        [TestFixture][Category("IntegrationTest")]
        public class When_Save_and_get : Shared
        {
            [Test]
            public void Then_assure_new_Database_record_is_created()
            {
                Scenario.StartNew(this, scenario =>
                {
                    scenario.Given(Database_exists).
                        And(Repository_is_created);

                    scenario.When("a new NoSqlDatabase is saved", () =>
                    {
                        var noSqlDb = new NoSqlDatabase {Name = "Webshop"};
                        var orders = noSqlDb.GetCollection("orders");
                        orders.Insert(Document.Parse("{ OrderId: 1, CustomerName: 'Gøran Hansen'}"));

                        noSqlDb.GetCollection("orders.pr.day");
                        var log = noSqlDb.GetCollection("log");
                        log.Insert(Document.Parse("{ Message: 'wrote something to the log'}"));

                        var noSqlDb2 = new NoSqlDatabase {Name = "VCS"};
                        var vcsLog = noSqlDb2.GetCollection("log");
                        vcsLog.Insert(Document.Parse("{ Message: 'this is another log:)'}"));
                        repository.Save(noSqlDb);
                        repository.Save(noSqlDb2);

                        records = repository.Get(All.ItemsOf<NoSqlDatabase>());
                    });

                    scenario.Then("assure a new record is created in the NoSqlDatabase table", () =>
                    {
                        records.Count().ShouldBe(2);
                    });

                    scenario.Then("assure properties are persisted", () =>
                    {
                        var webshop = records.ElementAt(0);
                        webshop.Name.ShouldBe("Webshop");

                        var vcs = records.ElementAt(1);
                        vcs.Name.ShouldBe("VCS");
                    });

                    scenario.Then("assure Collections are persisted", () =>
                    {
                        var webshop = records.ElementAt(0);

                        var orders = webshop.GetCollection("orders");
                        orders.ShouldNotBeNull();
                        webshop.Collections.ContainsKey("orders.pr.day").ShouldBeTrue();
                        webshop.Collections.ContainsKey("log").ShouldBeTrue();

                        var vcs = records.ElementAt(1);
                        vcs.Collections.ContainsKey("log").ShouldBeTrue();
                    });

                    scenario.Then("assure Documents are persisted", () =>
                    {
                        var webshopDb = records.ElementAt(0);
                        var orders = webshopDb.GetCollection("orders").Documents;
                        orders.Count.ShouldBe(1);
                        orders.First()["OrderId"].Value<int>().ShouldBe(1);
                        orders.First()["CustomerName"].Value<string>().ShouldBe("Gøran Hansen");
                    });
                });
            }
        }

        [TestFixture][Category("IntegrationTest")]
        public class When_Save : Shared
        {
            [Test]
            public void Then_assure_child_objects_are_deleted_if_removed_from_Collections()
            {
                Scenario.StartNew(this, scenario =>
                {
                    scenario.Given(Database_exists).
                        And(Repository_is_created).
                        And("a NoSqlDatabse exists", () =>
                        {
                            var db = new NoSqlDatabase() {Name = "Webshop"};
                            var orders = db.GetCollection("orders");
                            orders.Insert(Document.FromObject(new { OrderId = 1}));
                            orders.Insert(Document.FromObject(new { OrderId = 2}));

                            repository.Save(db);
                        });

                    scenario.When("remove a document (child obj) from collection", () =>
                    {
                        var db =
                            repository.Get(new LinqSpecification<NoSqlDatabase>(d => d.Name == "Webshop")).
                                SingleOrDefault();
                        var orders = db.GetCollection("orders");
                        orders.Delete(orders.Documents.First());

                        repository.Save(db);
                    });

                    scenario.Then("assure document is removed from collection", () =>
                    {
                        var db = repository.Get(new LinqSpecification<NoSqlDatabase>(d => d.Name == "Webshop")).SingleOrDefault();
                        var orders = db.GetCollection("orders");
                        orders.Documents.Count.ShouldBe(1);
                        orders.Documents.First()["OrderId"].Value<int>().ShouldBe(2);
                    });
                });
            }
        }

        public class Shared : Smeedee.IntegrationTests.Database.DomainModel.Repositories.Shared
        {
            protected static SqliteNoSqlDatabaseRepository repository;
            protected static IEnumerable<NoSqlDatabase> records;

            protected Context Repository_is_created = () =>
            {
                repository = new SqliteNoSqlDatabaseRepository(sessionFactory);
            };
        }
    }
}
