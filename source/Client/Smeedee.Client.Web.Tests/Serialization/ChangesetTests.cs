using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.SourceControl;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Web.Tests.Serialization
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class when_Serializing_changesets
    {
        [Test]
        public void assure_object_graph_is_Serialized_correctly()
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(IEnumerable<Changeset>), null, int.MaxValue, false, false, null);


            ChangesetServer server = new ChangesetServer() { Name = "Test name", Url = "http://www.smeedee.org" };
            Changeset changeset = new Changeset() { Server = server, Comment = "SmeeDee", Revision = 1001, Author = new Author("tuxbear")};
            Changeset changeset2 = new Changeset() { Server = server, Comment = "SmeeDee2", Revision = 1002, Author = new Author("tuxbear")};
            
            server.Changesets.Add(changeset);
            server.Changesets.Add(changeset2);
            

            MemoryStream stream = new MemoryStream();

            serializer.WriteObject(stream, new [] {changeset, changeset2});
            stream.Position = 0;

            object deSerialized = serializer.ReadObject(stream);

            var changesets = deSerialized as IEnumerable<Changeset>;
            var firstDeserialized = changesets.ElementAt(0);
            firstDeserialized.Revision.ShouldBe(1001);
            var secondDeserialized = changesets.ElementAt(1);
            secondDeserialized.Revision.ShouldBe(1002);
        }
    }
}
