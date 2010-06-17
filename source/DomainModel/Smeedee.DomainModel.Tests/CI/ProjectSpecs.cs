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
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Linq;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace APD.DomainModel.CI.ProjectInfoSpecs
{
    public class Shared
    {
        protected string projectName = "Agile Project Dashboard";
        protected CIProject testProject;
        protected CIServer testServer;

        public Shared()
        {
            SharedSetup();

        }
        public void SharedSetup()
        {
            testServer = new CIServer
                         {
                             Name = "ADP",
                             Url = "http://agileprojectdashboard.org/ccnet/"
                         };

            testProject = new CIProject(projectName)
                          {
                              SystemId = "ProjectNr.1",
                              Server = testServer
                          };
        }
    }


    [TestFixture]
    public class when_spawned : Shared
    {
        [Test]
        public void should_have_project_name()
        {
            testProject.ProjectName.ShouldBe(projectName);
        }

        [Test]
        public void should_have_list_of_builds_property()
        {
            testProject.Builds.ShouldNotBeNull();
        }

        [Test]
        public void latest_build_should_be_null()
        {
            testProject.LatestBuild.ShouldBeNull();
        }
    }

    [TestFixture]
    public class when_adding_builds:Shared
    {
        [SetUp]
        public void Setup()
        {
            SharedSetup();
        }

        [Test]
        public void assure_it_is_possible_to_add_builds()
        {
            var build = new Build();
            testProject.AddBuild(build);
        }

        [Test]
        public void assure_added_builds_appear_in_Builds_list()
        {
            var build = new Build {Project = new CIProject()};
            testProject.AddBuild(build);
            testProject.Builds.First().ShouldBe(build);
        }

        [Test]
        public void assure_owner_project_reference_is_assigned_to_added_builds()
        {
            var build = new Build { Project = new CIProject() };
            testProject.AddBuild(build);
            testProject.Builds.First().Project.ShouldBe(testProject);
        }
    }

    [TestFixture]
    public class when_no_builds_available : Shared
    {
        [Test]
        public void latestbuild_should_be_null()
        {
            testProject.LatestBuild.ShouldBeNull();
        }
    }


    [TestFixture]
    public class when_builds_available : Shared
    {
        [SetUp]
        public void SetUp()
        {
            testProject.AddBuild(new Build {StartTime = DateTime.Now.AddSeconds(2)});
            testProject.AddBuild(new Build {StartTime = DateTime.Now});
            testProject.AddBuild(new Build { StartTime = DateTime.Now.AddYears(44) });
            testProject.AddBuild(new Build {StartTime = DateTime.Now.AddSeconds(1)});
        }

        [Test]
        public void should_have_latest_build_property()
        {
            testProject.LatestBuild.ShouldNotBeNull();
        }

        [Test]
        public void Assure_the_last_build_added_is_the_LatestBuild()
        {
            var latestBuild = testProject.Builds.Last();

            testProject.LatestBuild.StartTime.ShouldBe(latestBuild.StartTime);
        }
    }

    [TestFixture]
    public class When_comparing_persisted_Projects
    {
        [Test]
        public void Assure_rows_are_unique()
        {
            var project = new CIProject("development")
            {
                SystemId = Guid.NewGuid().ToString()
            };

            var project2 = new CIProject("development")
            {
                SystemId = project.SystemId
            };

            project.ShouldBe(project2);

            project2 = new CIProject("development")
            {
                SystemId = Guid.NewGuid().ToString()
            };

            project.ShouldNotBe(project2);
        }
    }
}