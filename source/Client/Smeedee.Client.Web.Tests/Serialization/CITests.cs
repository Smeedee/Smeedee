#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;
using Smeedee.DomainModel.CI;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Client.WebTests.Serialization
{
    [TestFixture][Category("IntegrationTest")]
    public class when_Serializing
    {
        [Test]
        public void assure_object_graph_is_Serialized_correctly()
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(IEnumerable<CIServer>), null, int.MaxValue,false, true, null);
            

            CIServer server = new CIServer() {Name = "Test name", Url = "http://www.smeedee.org"};
            CIProject project = new CIProject(){Server = server, ProjectName = "SmeeDee"};
            Build build = new Build() { SystemId = "123"};
            server.AddProject(project);
            project.AddBuild(build);

            List<CIServer> listOfServers = new List<CIServer>();
            listOfServers.Add(server);

            MemoryStream stream = new MemoryStream();

            serializer.WriteObject(stream, listOfServers);
            stream.Position = 0;

            object deSerialized = serializer.ReadObject(stream);

            var deSerializedServerList = deSerialized as IEnumerable<CIServer>;

            var deSerializedServer = deSerializedServerList.ElementAt(0);
            deSerializedServer.Name.ShouldBe("Test name");
            deSerializedServer.Url.ShouldBe("http://www.smeedee.org");

            deSerializedServer.Projects.Count() .ShouldBe(1);
            deSerializedServer.Projects.ElementAt(0).ProjectName.ShouldBe("SmeeDee");
            deSerializedServer.Projects.ElementAt(0).Server.Name.ShouldBe("Test name");
            deSerializedServer.Projects.ElementAt(0).Server.Url.ShouldBe("http://www.smeedee.org");

            deSerializedServer.Projects.ElementAt(0).Builds.Count().ShouldBe(1);
            deSerializedServer.Projects.ElementAt(0).Builds.ElementAt(0).SystemId.ShouldBe("123");
            deSerializedServer.Projects.ElementAt(0).Builds.ElementAt(0).Project.ProjectName.ShouldBe("SmeeDee");
            deSerializedServer.Projects.ElementAt(0).Builds.ElementAt(0).Project.Server.Name.ShouldBe("Test name");
        }
    }
}
