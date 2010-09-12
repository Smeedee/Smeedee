using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;

namespace Smeedee.DomainModel.Tests.Learning
{
    [TestFixture]
    public class JSONLearningTests
    {
        [Test]
        public void How_to_serialize_a_Dictionary_to_JSON()
        {
            var doc = new Dictionary<string, object>();
            doc.Add("order1", new Order());

            var json = JsonConvert.SerializeObject(doc);

            Console.WriteLine(json);
        }

        [Test]
        public void How_to_load_JSON_into_JObject()
        {
            var doc = JObject.Parse("{ OrderId : 1, CustomerName : 'goeran' }");
            doc["OrderId"].ShouldNotBeNull();
        }

        [Test]
        public void How_to_cast_JObject_to_specific_type()
        {
            var doc = JObject.Parse("{ OrderId : 1, CustomerName : 'goeran' }");
            var order = doc.ToObject<Order>();

            order.ShouldNotBeNull();
        }

        [Test]
        public void How_to_add_data_to_JObject()
        {
            var jObject = new JObject();
            jObject["OrderId"] = 1;
            jObject["Address"] = JObject.FromObject(new Address());

            jObject["OrderId"].ShouldNotBeNull();

            Console.WriteLine(jObject.ToString());
        }

        public class Order
        {
            public int OrderId { get; set; }
            public string CustomerName { get; set; }
        }

        public class Address
        {
            public string StreetName { get; set; }
            public string City { get; set; }
        }
    }

    public static class JObjectExtensions
    {
        public static T ToObject<T>(this JObject obj)
        {
            return JsonConvert.DeserializeObject<T>(obj.ToString());
        }

    }

}
