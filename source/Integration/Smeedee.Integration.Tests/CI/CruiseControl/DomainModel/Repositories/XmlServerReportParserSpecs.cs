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

using NUnit.Framework;
using Smeedee.DomainModel.CI;
using Smeedee.Integration.CI.CruiseControl.DomainModel.Repositories;
using Smeedee.Integration.Tests.CI.CruiseControl;
using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.CI.CruiseControl.DomainModel.Repositories.XmlServerReportParserSpecs
{
    public class Shared
    {
        protected XmlServerReportParser parser;
        public Shared()
        {
            parser = new XmlServerReportParser();
        }
    }

    [TestFixture]
    public class when_parsing_xml_data_is_malformed : Shared
    {
        [Test]
        [ExpectedException(typeof(CruiseControlRepositoryException))]
        public void should_fail_with_CruiseControlRepositoryException()
        {
            IEnumerable<CIProject> result = parser.Parse(Resource.MalformedXml);
        }
    }

    [TestFixture]
    public class when_parsing_xml_data_is_invalid : Shared
    {
        [Test]
        [ExpectedException(typeof(CruiseControlRepositoryException))]
        public void should_fail_with_CruiseControlRepositoryException()
        {
            IEnumerable<CIProject> result = parser.Parse(Resource.InvalidXml);
        }
    }

    [TestFixture]
    public class whem_parsing_valid_xml : Shared
    {
        [Test]
        public void should_return_list_with_successful_last_builds()
        {
            IEnumerable<CIProject> result = parser.Parse(Resource.Success);
            foreach (var project in result)
            {
                project.ProjectName.ShouldNotBeNull();
                project.LatestBuild.ShouldNotBeNull();
                project.LatestBuild.Status.ShouldBe(BuildStatus.FinishedSuccefully);
                Assert.IsTrue(project.LatestBuild.StartTime > new DateTime(0));
            }
        }

        [Test]
        public void should_return_list_with_failed_last_builds()
        {
            IEnumerable<CIProject> result = parser.Parse(Resource.BuildFailed);
            foreach (var project in result)
            {
                project.ProjectName.ShouldNotBeNull();
                project.LatestBuild.ShouldNotBeNull();
                project.LatestBuild.Status.ShouldBe(BuildStatus.FinishedWithFailure);
                Assert.IsTrue(project.LatestBuild.StartTime > new DateTime(0));
            }
        }

        [Test]
        public void should_return_list_with_building_last_builds()
        {
            IEnumerable<CIProject> result = parser.Parse(Resource.Building);
            foreach (var project in result)
            {
                project.ProjectName.ShouldNotBeNull();
                project.LatestBuild.ShouldNotBeNull();
                project.LatestBuild.Status.ShouldBe(BuildStatus.Building);
                Assert.IsTrue(project.LatestBuild.StartTime > new DateTime(0));
            }
        }

        [Test]
        public void should_return_list_with_unknown_last_builds()
        {
            IEnumerable<CIProject> result = parser.Parse(Resource.InvalidBuildStatus);
            foreach (var project in result)
            {
                project.ProjectName.ShouldNotBeNull();
                project.LatestBuild.ShouldNotBeNull();
                project.LatestBuild.Status.ShouldBe(BuildStatus.Unknown);
                Assert.IsTrue(project.LatestBuild.StartTime > new DateTime(0));
            }
        }
    }

    [TestFixture]
    public class when_building_continues_across_two_parses
    {
        [Test]
        public void build_start_time_should_be_time_of_first_parse()
        {
            XmlServerReportParser parser = new XmlServerReportParser();
            
            var firstParse = parser.Parse(Resource.Building);

            Thread.Sleep(1500);

            var secondParse = parser.Parse(Resource.Building);

            firstParse.First().LatestBuild.StartTime.ShouldBe(secondParse.First().LatestBuild.StartTime);


        }
    }


}