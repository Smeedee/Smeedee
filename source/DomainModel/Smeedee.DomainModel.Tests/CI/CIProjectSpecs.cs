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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.DomainModel.CI.CIProjectSpecs
{
    public class Shared
    {
        protected static string projectName = "Agile Project Dashboard";
        protected static CIProject testProject;
        protected static CIServer testServer;

        public Shared()
        {
            SharedSetup();

        }
        public static void SharedSetup()
        {
            testServer = new CIServer
                         {
                             Name = "ADP",
                             Url = "http://smeedee.org/ccnet/"
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
    public class When_comparing_persisted_Projects : Shared
    {
        private static List<CIProject> projects;
        protected static CIProject testProject2;
        protected static CIProject testProject3;
        protected static CIProject testProject4;

        protected Context projects_and_servers_are_intialized = () =>
        {
            SharedSetup();

            testProject2 = new CIProject(projectName)
            {
                SystemId = "ProjectNr.2",
                Server = testServer
            };
            testProject3 = new CIProject(projectName)
            {
                SystemId = "ProjectNr.3",
                Server = testServer
            }; 
            testProject4 = new CIProject(projectName)
            {
                SystemId = "ProjectNr.4",
                Server = testServer
            };
        };

        protected Context projects_are_added_to_list = () =>
        {
            projects = new List<CIProject>();
            projects.Add(testProject);
            projects.Add(testProject2);
            projects.Add(testProject3);
            projects.Add(testProject4);
        };
        
        protected Context ciProjects_LatestBuild_have_different_BuildStatuses = () =>
        {
            testProject.AddBuild(new Build { StartTime = DateTime.Now });
            testProject2.AddBuild(new Build { StartTime = DateTime.Now.AddSeconds(1) });
            testProject3.AddBuild(new Build { StartTime = DateTime.Now.AddSeconds(2) });
            testProject4.AddBuild(new Build { StartTime = DateTime.Now.AddSeconds(3) });
            testProject.LatestBuild.Status = BuildStatus.FinishedSuccefully;
            testProject2.LatestBuild.Status = BuildStatus.FinishedWithFailure;
            testProject3.LatestBuild.Status = BuildStatus.Unknown;
            testProject4.LatestBuild.Status = BuildStatus.Building;
        };

        protected Context ciProjects_LatestBuild_have_the_same_BuildStatuses = () =>
        {
            testProject.AddBuild(new Build { StartTime = DateTime.Now });
            testProject2.AddBuild(new Build { StartTime = DateTime.Now.AddSeconds(4) });
            testProject3.AddBuild(new Build { StartTime = DateTime.Now.AddSeconds(2) });
            testProject4.AddBuild(new Build { StartTime = DateTime.Now.AddSeconds(3) });
            testProject.LatestBuild.Status = BuildStatus.Unknown;
            testProject2.LatestBuild.Status = BuildStatus.FinishedWithFailure;
            testProject3.LatestBuild.Status = BuildStatus.Unknown;
            testProject4.LatestBuild.Status = BuildStatus.FinishedWithFailure;
        };
        

        [Test]
        public void Assure_projects_are_sorted_by_status()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(projects_and_servers_are_intialized).
                    And(ciProjects_LatestBuild_have_different_BuildStatuses).
                    And(projects_are_added_to_list);
                scenario.When("CIProjects are compared");
                scenario.Then("CIProjects with LatestBuild.Status FinishedWithFailure " + 
                    "should come first followed by Unknown, Building and FinishedSuccefully", () =>
                {
                    projects.Sort();
                    projects[0].ShouldBe(testProject2);
                    projects[1].ShouldBe(testProject3);
                    projects[2].ShouldBe(testProject4);
                    projects[3].ShouldBe(testProject);
                });
            });
        }

        [Test]
        public void Assure_projects_with_same_LatestBuilds_Status_are_sorted_by_freshness()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(projects_and_servers_are_intialized).
                    And(ciProjects_LatestBuild_have_the_same_BuildStatuses).
                    And(projects_are_added_to_list);
                scenario.When("CIProjects are compared");
                scenario.Then("CIProjects with the same status should be sorted by freshness", () =>
                {
                    projects.Sort();
                    projects[0].ShouldBe(testProject2);
                    projects[1].ShouldBe(testProject4);
                    projects[2].ShouldBe(testProject3);
                    projects[3].ShouldBe(testProject);
                });
            });
        }
        
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