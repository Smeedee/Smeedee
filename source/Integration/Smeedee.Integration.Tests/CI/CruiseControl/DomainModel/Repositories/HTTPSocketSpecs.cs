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
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Net;
using Smeedee.Integration.CI.CruiseControl.DomainModel.Repositories;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.CI.CruiseControl.DomainModel.Repositories
{
    [TestFixture]
    [Ignore]
    public class HTTPSocketSpecs
    {
        private readonly string HOST = "http://agileprojectdashboard.org";
        private readonly string FILE = "/ccnet/server/local/project/Agile+Project+Dashboard+-+Trunk/build/log20090721155557Lbuild.0.2.0.30.xml/XmlBuildLog.xml";
        private readonly int PORT = 80;

        private HTTPSocket httpSocket;

        [SetUp]
        public void Setup()
        {
            httpSocket = new HTTPSocket();  
            httpSocket.Open(HOST,PORT);  
        }
        [TearDown]
        public void Teardown()
        {
            httpSocket.Close();
        }

        [Test]
        public void can_open_connection()
        {
            httpSocket.IsConnected.ShouldBeTrue();
        }

        [Test]
        public void can_close_connection()
        {
            httpSocket.Close();
            httpSocket.IsConnected.ShouldBeFalse();
        }
        [Test]
        public void will_not_connect_to_nonexisting_hosts()
        {
            httpSocket.Close();
            try
            {
                httpSocket.Open("http://www.fakedomainserver1234AQWD.com", 80);
            }
            catch {}
            httpSocket.IsConnected.ShouldBeFalse();
        }

        [Test]
        [ExpectedException(typeof(HTTPSockectCouldNotConnectException))]
        public void assure_throws_exception_on_connect_to_nonexisting_host()
        {
            httpSocket.Close();
            httpSocket.Open("http://www.fakedomainserver1234AQWD.com", 80);
        }

        [Test]
        [ExpectedException(typeof(WebException))]
        public void assure_throws_exception_on_Read_when_not_connected()
        {
            httpSocket.Close();
            httpSocket.Read(10);
        }

        [Test]
        public void assure_returns_http_response()
        {
            string request = String.Format("GET {0} HTTP/1.1\n Host: agileprojectdashboard.org \n\n", FILE);
            httpSocket.Send(request);

            byte[] data = httpSocket.Read(4);

            string message = System.Text.Encoding.ASCII.GetString(data);
            
            message.ShouldBe("HTTP");
        }

        [Test]
        [ExpectedException(typeof(HTTPSockectInvalidReadException))]
        public void assure_throws_exception_on_incorrect_read_requests_1()
        {
            httpSocket.Read(0);
        }

        [Test]
        [ExpectedException(typeof(HTTPSockectInvalidReadException))]
        public void assure_throws_exception_on_incorrect_read_requests_2()
        {
            httpSocket.Read(-3);
        }

    }
}
