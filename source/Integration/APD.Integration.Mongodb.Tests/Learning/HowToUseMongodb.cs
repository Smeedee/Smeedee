using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using Newtonsoft.Json;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;

namespace APD.Integration.Mongodb.Tests.Learning
{
    [TestFixture]
    public class HowToUseMongodb
    {
        private Mongo dbConnection;
        private Database smeedeeDb;

        [SetUp]
        public void Setup()
        {
            dbConnection = new Mongo();
            dbConnection.Connect();

            smeedeeDb = dbConnection.getDB("SmeedeeDb");
        }

        [TearDown]
        public void TearDown()
        {
            dbConnection.Disconnect();
        }

        [Test]
        public void How_to_conntect_to_a_mongodb()
        {

            dbConnection.Host.ShouldBe("localhost");
            dbConnection.Port.ShouldBe(27017);
        }

        [Test]
        public void How_to_create_a_document()
        {
            var collection = smeedeeDb["changesets"];

            var changeset = new Document();
            changeset["revision"] = 1;
            changeset["comment"] = "hello world";
            changeset["timestamp"] = DateTime.Now;

            collection.Insert(changeset);

            dbConnection.Disconnect();

            var newConnection = new Mongo();
            newConnection.Connect();
            var newSmeedeeDb = newConnection.getDB("SmeedeeDb");
            var newCollection = newSmeedeeDb["changesets"];

            var foundDoc = newCollection.FindOne(changeset);

            foundDoc["revision"].ShouldBe(changeset["revision"]);
        }

        [Test]
        public void How_to_use_JSON_to_deserialize_obj()
        {
            var changesets = smeedeeDb["changesets"];

            var changeset = new Document();
            changeset["Revision"] = 10100;
            changeset["Comment"] = "Refactored a controller";
            changeset["Timestamp"] = DateTime.Now;

            changesets.Insert(changeset);

            //Console.WriteLine(changeset.ToString());
            //Changeset persistedChangeset = JsonConvert.
            //    DeserializeObject<Changeset>(changeset.ToString());
        }

        [Test]
        public void How_to_save_an_obj_using_JSON_serialization()
        {
            var changeset = new Changeset();
            changeset.Revision = 10100;
            changeset.Message = "Refactored a controller";
            changeset.Timestamp = DateTime.Now;
            
            
            //var s = JsonConvert.SerializeObject()
        }

        public class Changeset
        {
            public long Revision { get; set; }
            public string Message { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
