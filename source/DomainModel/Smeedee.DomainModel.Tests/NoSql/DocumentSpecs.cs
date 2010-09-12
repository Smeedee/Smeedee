using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Smeedee.DomainModel.NoSql;
using Smeedee.Tests;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModel.Tests.NoSql
{
    public class DocumentSpecs
    {
        [TestFixture]
        public class When_set_value : Shared
        {
            public override void Before()
            {
                Given(Document_is_created);

                When("set value", () =>
                {
                    document["OrderId"] = 1;
                    document["CustomerName"] = "Gøran Hansen";
                    document["OrderDate"] = goeransBirthday;
                    document["CustomerAddress"] = JObject.FromObject(goeransAddress);
                    document["ContactInfo"] = JArray.FromObject(new List<string>() {"mail@goeran"});
                });
            }   

            [Test]
            public void assure_Value_has_been_set()
            {
                Then(() =>
                {
                    document["OrderId"].ShouldBe(JValue.FromObject(1));
                    document["CustomerName"].ShouldBe(JValue.FromObject("Gøran Hansen"));
                    document["OrderDate"].ShouldBe(JValue.FromObject(goeransBirthday));
                    document["ContactInfo"].ShouldNotBeNull();
                    document["CustomerAddress"].ShouldNotBeNull();
                });
            }
        }

        [TestFixture]
        public class When_convert_Document_to_Object : Shared
        {
            public override void Before()
            {
                Given(Document_is_created);
                And("it contains data", () =>
                {
                    document["OrderId"] = 1;
                    document["CustomerName"] = "Gøran Hansen";
                    document["CustomerAddress"] = JObject.FromObject(goeransAddress);
                });

                When("convet Document to object", () =>
                    obj = document.ToObject<Order>());
            }

            [Test]
            public void assure_an_Object_is_returned()
            {
                Then(() =>
                     obj.ShouldNotBeNull());
            }

            [Test]
            public void assure_data_has_been_set_on_Object()
            {
                Then(() =>
                {
                    obj.ShouldBeInstanceOfType<Order>();
                    var order = obj as Order;
                    order.OrderId.ShouldBe(1);
                    order.CustomerName.ShouldBe("Gøran Hansen");
                    order.CustomerAddress.ShouldNotBeNull();
                    order.CustomerAddress.StreetName.ShouldBe(goeransAddress.StreetName);
                    order.CustomerAddress.City.ShouldBe(goeransAddress.City);
                });
            }
        }

        [TestFixture]
        public class When_Parse_JSON : Shared
        {
            public override void Before()
            {
                When("parse JSON", () =>
                    document = Document.Parse("{ OrderId : 1, CustomerName : 'Gøran Hansen' }"));
            }

            [Test]
            public void assure_Documents_has_values()
            {
                Then(() =>
                {
                    document["OrderId"].Value<int>().ShouldBe(1);
                    document["CustomerName"].Value<string>().ShouldBe("Gøran Hansen");
                });
            }
        }

        [TestFixture]
        public class When_load_from_object : Shared
        {
            public override void Before()
            {
                When("load from object", () =>
                    document = Document.FromObject(new
                    {
                        OrderId = 1, 
                        CustomerName = "Gøran Hansen", 
                        Email = "mail@goeran.no",
                        Address = new
                        {
                            StreetName = "Fossestuvegen 62",
                            City = "Tiller"        
                        }       
                    }));
            }

            [Test]
            public void assure_JSON_string_is_generated()
            {
                Then(() =>
                {
                    document.JSON.ShouldNotBeNull();
                });
            }

            [Test]
            public void assure_JSON_string_contains_data_from_obj()
            {
                Then(() =>
                {
                    document.JSON.Contains("OrderId").ShouldBeTrue();
                    document.JSON.Contains("CustomerName").ShouldBeTrue();
                    document.JSON.Contains("Gøran Hansen").ShouldBeTrue();
                });
            }

            [Test]
            public void assure_Document_contains_data_from_obj()
            {
                Then(() =>
                {
                    document["OrderId"].Value<int>().ShouldBe(1);
                    document["CustomerName"].Value<string>().ShouldBe("Gøran Hansen");
                    document["Address"]["StreetName"].Value<string>().ShouldBe("Fossestuvegen 62");
                    document["Address"]["City"].Value<string>().ShouldBe("Tiller");
                });
            }
        }

        [TestFixture]
        public class When_load_data_from_list_object : Shared
        {
            public override void Before()
            {
                When("load data from List object", () =>
                {
                });
            }

            [Test]
            public void assure_Exception_is_thrown()
            {
                Then(() =>
                {
                    var orders = new List<Order>();
                    orders.Add(new Order() { OrderId = 1, CustomerName = "Gøran" });
                    orders.Add(new Order() { OrderId = 2, CustomerName = "Dag Olav" });

                    this.ShouldThrowException<ArgumentException>(() =>
                        document = Document.FromObject(orders), ex =>
                            ex.Message.ShouldBe("Document doesn't support list objects, must be an plain old object"));
                });
            }
        }

        public class Shared : SmeedeeScenarioTestClass
        {
            protected static Document document;
            protected static object obj;

            protected Context Document_is_created = () =>
            {
                document = new Document();
            };

            protected DateTime goeransBirthday = new DateTime(1981, 9, 1);
            protected Address goeransAddress = new Address()
            {
                StreetName = "Fossestuvegen 62",
                City = "Tiller"
            };
        }

        public class Address
        {
            public string StreetName { get; set; }
            public string City { get; set; }
        }

        public class Order
        {
            public int OrderId { get; set; }
            public string CustomerName { get; set; }
            public Address CustomerAddress { get; set; }
        }
    }

}
