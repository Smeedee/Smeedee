using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.NoSql;
using Smeedee.Tests;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using Newtonsoft.Json.Linq;

namespace Smeedee.DomainModel.Tests.NoSql
{
    public class CollectionSpecs
    {
        [TestFixture]
        public class when_spawned
        {
            [Test]
            public void Assure_Documents_is_set()
            {
                var collection = new Collection();
                collection.Documents.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_Insert : Shared
        {
            public override void Before()
            {
                Given(Collection_is_created);
            }

            [Test]
            public void assure_Document_arg_is_validated()
            {
                When("insert");

                Then(() =>
                    this.ShouldThrowException<ArgumentNullException>(() =>
                        collection.Insert(null)));      
            }

            [Test]
            public void assure_Document_is_added_to_Collection()
            {
                When("insert", () =>
                    collection.Insert(document = new Document()));

                Then(() =>
                {
                    collection.Documents.Count.ShouldBe(1);
                    collection.Documents.ShouldHave(document);
                });
            }
        }

        [TestFixture]
        public class When_query_Documents : Shared
        {
            private int numberOfDocuments = 10000;

            public override void Before()
            {
                Given(Collection_is_created);
                And("it contains documents", () =>
                {
                    for (int i = 1; i <= numberOfDocuments; i++)
                    {
                        collection.Insert(Document.Parse("{ OrderId: " + i + ", CustomerName: 'goeran" + i + "' }"));
                    }
                });

                When("query documents");
            }

            [Test]
            public void assure_its_possible_to_filter_out()
            {
                Then(() =>
                {
                    var thenFirstOrders = collection.Documents.Where(d => d["OrderId"].Value<int>() <= 10);
                    thenFirstOrders.Count().ShouldBe(10);

                    var ordersByGoeran = collection.Documents.Where(d => d["CustomerName"].Value<string>().StartsWith("goeran"));
                    ordersByGoeran.Count().ShouldBe(numberOfDocuments);
                });
            }
        }

        [TestFixture]
        public class When_Parse : Shared
        {
            public override void Before()
            {
                When("Parse JSON", () =>
                {
                    collection = Collection.Parse("[{OrderId: 1}, {OrderId:2}]");
                });
            }

            [Test]
            public void assure_Collection_is_returned()
            {
                Then(() => collection.ShouldNotBeNull());
            }

            [Test]
            public void assure_Documents_are_added()
            {
                Then(() => collection.Documents.Count.ShouldNotBe(0));
            }

            [Test]
            public void assure_Documents_contains_data()
            {
                Then(() =>
                {
                    foreach (var doc in collection.Documents)
                    {
                        doc["OrderId"].Value<int>().ShouldNotBe(0);
                    }
                });
            }
        }

        public class Shared : SmeedeeScenarioTestClass
        {
            protected static Collection collection;
            protected static Document document;

            protected Context Collection_is_created = () =>
            {
                collection = new Collection();
            };
        }
    }
}
