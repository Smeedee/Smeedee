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
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Smeedee.Integration.CI.CruiseControl.DomainModel.Repositories;
using Moq;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.CI.CruiseControl.DomainModel.Repositories
{
    [TestFixture][Category("IntegrationTest")]
    public class OptimizedHTTPRequestSpecs
    {

        private readonly string URL = "http://agileprojectdashboard.org/ccnet";
        private readonly int PORT = 80;

        private Mock<IHTTPSocket> httpRequestMock;
        private OptimizedHTTPRequest optimizedHTTPRequest;

        [SetUp]
        public void Setup()
        {
            httpRequestMock = new Mock<IHTTPSocket>();
            optimizedHTTPRequest = new OptimizedHTTPRequest(httpRequestMock.Object);
            httpRequestMock.Setup(m => m.Read(It.IsAny<int>())).Returns(GenerateHTTPResponse("FILE CONTENT"));

            httpRequestMock.SetupGet(m => m.IsConnected).Returns(true);
        }

        [Test]
        public void assure_data_is_returned()
        {
            var response = optimizedHTTPRequest.RequestURL(URL, PORT, 4);
            
            response.ShouldBe("FILE");
        }

        [Test]
        public void assure_short_body_content_length_is_handled()
        {
            httpRequestMock.Setup(m => m.Read(It.IsAny<int>())).Returns(GenerateHTTPResponse("FIL"));

            var resonse = optimizedHTTPRequest.RequestURL(URL, PORT, 4);
            
            resonse.ShouldBe("FIL");
        }

        [Test]
        public void assure_httpConnection_is_opened_when_requesting_url()
        {
            optimizedHTTPRequest.RequestURL(URL, PORT, 4);

            httpRequestMock.Verify(m => m.Open(URL,PORT), Times.AtLeastOnce());

        }

        [Test]
        public void assure_connection_is_closed_after_requsting_url()
        {
            optimizedHTTPRequest.RequestURL(URL, PORT, 4);

            httpRequestMock.Verify(m => m.Close(), Times.AtLeastOnce());
        }



        private byte[] GenerateHTTPResponse(String HTTPBody)
        {
            StringBuilder data = new StringBuilder();

            data.Append("HTTP/1.1 200 OK\n");
            data.Append("Date: Tue, 21 Jul 2009 13:12:14 GMT\n");
            data.Append("Server: Microsoft-IIS/6.0\n");
            data.Append("X-Powered-By: ASP.NET\n");
            data.Append("X-AspNet-Version: 2.0.50727\n");
            data.Append("X-CCNet-Version: CruiseControl.NET/1.4.3.4022\n");
            data.Append("Transfer-Encoding: chunked\n");
            data.Append("Last-Modified: Mon, 01 Jan 0001 00:00:00 GMT\n");
            data.Append("ETag: \"NOT AVAILABLE\"\n");
            data.Append("Cache-Control: private, max-age=0\n");
            data.Append("Content-Type: text/xml; charset=utf-8\n");
            data.Append("\n");
            data.Append(HTTPBody);

            return System.Text.Encoding.ASCII.GetBytes(data.ToString());
        }
    }
    [TestFixture][Category("IntegrationTest")]
    public class OpimizedHTTPRequestSpecs_Integration
    {
        private readonly string LONGFILE_URL = "http://smeedee.org/__utility/js/prototype.js";
        private readonly string PERMANENT_URL = "http://smeedee.org/default.aspx";
        private readonly string NONEXISTINGHOST_URL = "http://www.fakedomainserver1234AQWD.com";
        private readonly string NONEXISTINGFILE_URL = "http://www.google.com/SomeDumbFileIKnowsNotThere.dat";
        private readonly int PORT = 80;

        private OptimizedHTTPRequest optimizedHTTPRequest;

        [SetUp]
        public void Setup()
        {
            optimizedHTTPRequest = new OptimizedHTTPRequest( new HTTPSocket());

        }

        [Test]
        public void assure_can_request_file_from_live_server()
        {
            string result = optimizedHTTPRequest.RequestURL(PERMANENT_URL, PORT, 200);

            result.ToLower().Contains("<html").ShouldBeTrue();
        }

        [Test]
        [ExpectedException(typeof(WebException))]
        public void assure_throws_excpetion_when_requsting_nonexisting_hosts()
        {
            optimizedHTTPRequest.RequestURL(NONEXISTINGHOST_URL, 80, 10);
        }

        [Test]
        [ExpectedException(typeof(WebException))]
        public void assure_throws_excpetion_when_requsting_nonexisting_files()
        {
            optimizedHTTPRequest.RequestURL(NONEXISTINGFILE_URL, 80, 10);
        }

        [Test]
        public void assure_fetched_data_is_not_larger_than_specified()
        {
            int length = 1240;
            string result = optimizedHTTPRequest.RequestURL(PERMANENT_URL, PORT, length);

            result.Length.ShouldBeLessThan(length+1);
        }


        [Test]
        [Ignore]
        public void assure_to_handle_heavy_load_with_large_files()
        {
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            const int bytesToFetch = 90000;
            const int numberOfRequests = 50;
            int completedRequests = 0;

            bool[] successfulRequests = new bool[numberOfRequests];

            for (int i = 0; i < numberOfRequests; i++)
            {
                int index = i;
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    try
                    {
                        OptimizedHTTPRequest testRequest = new OptimizedHTTPRequest(new HTTPSocket());

                        string result = testRequest.RequestURL(LONGFILE_URL, 80, bytesToFetch);

                        if (result.Length != bytesToFetch)
                            throw new Exception("only got " + result.Length + " bytes");
                        successfulRequests[index] = true;

                        Console.WriteLine("File request {0} completed, {1} bytes returned", index, bytesToFetch);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("File request {0} FAILED", index);
                        Console.WriteLine(ex);
                    }
                    finally
                    {
                        completedRequests++;
                        if (completedRequests == numberOfRequests)
                            resetEvent.Set();
                    }
                });
            }

            resetEvent.WaitOne();
            successfulRequests.Where(r => r == true).Count().ShouldBe(numberOfRequests);
        }
    }
}
