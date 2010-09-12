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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.Integration.CI.CruiseControl.DomainModel.Repositories;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.CI.CruiseControl.DomainModel.Repositories.ProjectInfoRepositorySpecs
{
    public class Shared
    {
        protected static string ciUrl = "http://smeedee.org/ccnet/";
        protected CCServerRepository repository = new CCServerRepository(ciUrl, new SocketXMLBuildlogRequester());
    }

    [TestFixture][Category("IntegrationTest")]
    public class when_queried_for_CIServers : Shared
    {
        private IEnumerable<CIServer> ciServers;

        public when_queried_for_CIServers()
        {
            ciServers = repository.Get(new AllSpecification<CIServer>());
        }

        [Test]
        public void assure_to_handle_heavy_load()
        {
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            const int numberOfRequests = 10;
            int completedRequests = 0;

            bool[] successfulRequests = new bool[numberOfRequests];

            for (int i = 0; i < numberOfRequests; i++)
            {
                int index = i;
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    try
                    {
                      //  repository.Get(new LatestBuildSpecification());
                        successfulRequests[index] = true;
                    }
                    finally
                    {
                        Console.WriteLine("Get from repo completed");
                        completedRequests++;
                        if (completedRequests == numberOfRequests)
                            resetEvent.Set();
                    }
                });
            }

            resetEvent.WaitOne();


            successfulRequests.Where(r => r == true).Count().ShouldBe(numberOfRequests);
        }

        [Test]
        public void should_return_all_CIServers()
        {
            Assert.IsTrue(ciServers.Count() > 0);
        }

        [Test]
        [Ignore("We are testing against a broken system. Consider enabling after setting up CCNet properly.")]
        public void all_servers_should_have_projects()
        {
            foreach (var server in ciServers)
            {
                server.Projects.ShouldNotBeNull();
                (server.Projects.Count() > 0).ShouldBeTrue();
            }
        }


        //[Test]
        //public void when_build_is_finished_should_have_valid_duration()
        //{
        //    foreach (var project in projects)
        //    {
        //        if( project.LatestBuild.Status == BuildStatus.FinishedSuccefully ||
        //            project.LatestBuild.Status == BuildStatus.FinishedWithFailure)
        //        {
        //            Assert.IsTrue(project.LatestBuild.Duration > new TimeSpan(0));
        //        }
        //    }
        //}

        [Test]
        [ExpectedException(typeof(CruiseControlRepositoryException))]
        public void should_fail_when_using_invalid_url()
        {
            var bogusRepository = new CCServerRepository("bogus/url", new SocketXMLBuildlogRequester());
            bogusRepository.Get(new AllSpecification<CIServer>());
        }

        [Test]
        [ExpectedException(typeof(CruiseControlRepositoryException))]
        public void should_fail_when_using_non_existing_url()
        {
            var bogusRepository = new CCServerRepository("http://www.this-is-not-a-real-place.ugg", new SocketXMLBuildlogRequester());
            bogusRepository.Get(new AllSpecification<CIServer>());
        }

    }
}
