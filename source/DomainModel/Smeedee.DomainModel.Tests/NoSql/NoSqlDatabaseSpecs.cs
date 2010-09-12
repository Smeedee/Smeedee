using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.NoSql;
using Smeedee.Tests;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModel.Tests.NoSql
{
    public class NoSqlDatabaseSpecs
    {
        [TestFixture]
        public class When_spawned : Shared
        {
            public override void Before()
            {
                When(spawned);
            }

            [Test]
            public void assure_it_has_Collections()
            {
                Then(() =>
                     database.Collections.ShouldNotBeNull());
            }

            [Test]
            public void assure_it_is_a_DynamicObject()
            {
                Then(() =>
                    (database is DynamicObject).ShouldBeTrue());
            }
        }

        [TestFixture]
        public class When_get_collection : Shared
        {
            public override void Before()
            {
                Given(Database_is_created);
                And("it has a collection", () =>
                    database.Collections.Add("orders", new Collection()));

                When("get collection", () =>
                {
                    collection = dynDatabase.orders;
                });
            }

            [Test]
            public void assure_Collection_is_returned()
            {
                Then(() =>
                    collection.ShouldNotBeNull());
            }
        }

        [TestFixture]
        public class When_get_collection_that_does_not_exists : Shared
        {
            public override void Before()
            {
                Given(Database_is_created);
                And("it doesn't have any collections", () =>
                    database.Collections.Clear());

                When("get collection", () =>
                    collection = dynDatabase.ordersToday);
            }

            [Test]
            public void assure_Collection_is_returned()
            {
                Then(() =>
                     collection.ShouldNotBeNull());
            }

            [Test]
            public void assure_Collection_is_added_to_Database()
            {
                Then(() =>
                {
                    database.Collections.Count.ShouldBe(1);
                    database.Collections["ordersToday"].ShouldNotBeNull();
                });
            }
        }

        public class Shared : SmeedeeScenarioTestClass
        {
            protected static NoSqlDatabase database;
            protected static Collection collection;
            protected static dynamic dynDatabase;

            protected Context Database_is_created = () =>
            {
                database = new NoSqlDatabase();
                collection = null;
                dynDatabase = database;
            };

            protected When spawned = () =>
            {
                database = new NoSqlDatabase();
            };
        }
    }
}
