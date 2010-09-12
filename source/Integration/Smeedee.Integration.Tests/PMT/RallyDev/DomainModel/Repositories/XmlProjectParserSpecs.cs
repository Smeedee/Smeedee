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

using System.Collections.Generic;

using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Integration.PMT.RallyDev.DomainModel.Repositories;
using NUnit.Framework;
using Smeedee.Integration.Tests.PMT.RallyDev.DomainModel.Repositories;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;


namespace Smeedee.IntegrationTests.PMT.RallyDev.XmlProjectParserSpecs
{
    public class Shared
    {
        protected static XmlProjectParser XmlProjectParser;
        public static string XmlData;

        protected Context ProjectXml_is_valid = () =>
        {
            XmlData = Resource.Project;
        };

        protected Context ProjectXml_with_data_is_valid = () =>
        {
            XmlData = Resource.ProjectWithData;
        };

        protected Context IterationsForProjectXml_is_valid = () =>
        {
            XmlData = Resource.IterationsForProject;
        };

        protected Context the_XmlProjectParser_is_created = () => { XmlProjectParser_is_created(); };

        protected static void XmlProjectParser_is_created()
        {
            XmlProjectParser = new XmlProjectParser(XmlData);
        }
    }
    
    [TestFixture][Category("IntegrationTest")]
    public class When_XmlTaskParser_is_spawned :Shared
    {
        [Test]
        public void should_have_default_constructor()
        {
            XmlProjectParser = new XmlProjectParser();
            XmlProjectParser.ShouldNotBeNull();
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_parsing_XML : Shared
    {
        [Test]
        public void should_parse_names_for_projects()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(ProjectXml_is_valid)
                    .And(the_XmlProjectParser_is_created);
                scenario.When("references for all projects are to be parsed");
                scenario.Then("a list of all project names should be returned", ()=>
                {
                    List<string> projects = XmlProjectParser.ParseProjectNames();
                    
                    projects.Count.ShouldBe(1);
                    projects[0].ShouldBe("Continuous Project Monitor");
                });
            });
        }
        
        [Test]
        public void should_parse_Project()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(ProjectXml_is_valid)
                    .And(the_XmlProjectParser_is_created);
                scenario.When("a project is to be parsed");
                scenario.Then("a new project with the same name as the parsed project is returned", () =>
                {
                    Project project = XmlProjectParser.ParseProject();
                    project.Name.ShouldBe("Continuous Project Monitor");
                });
            });
        }

        [Test]
        public void should_parse_project_reference()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(ProjectXml_is_valid)
                    .And(the_XmlProjectParser_is_created);
                scenario.When("a projects reference is to be parsed");
                scenario.Then("the projects reference is returned", ()=>
                {
                    string projectReference = XmlProjectParser.ParseProjectReference();
                    projectReference.ShouldBe("https://community.rallydev.com/slm/webservice/1.13/project/290873262");
                });
            });
        }

        [Test]
        public void should_parse_IterationReferencesForProject()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(IterationsForProjectXml_is_valid)
                    .And(the_XmlProjectParser_is_created);
                scenario.When("iterations for a project is to be parsed");
                scenario.Then("a list of references for the iterations in the project is returned", () =>
                {
                    List<string> iterationReferencesForProject = XmlProjectParser.ParseIterationReferencesForProject();
                    iterationReferencesForProject.Count.ShouldBe(5);
                    iterationReferencesForProject[0].ShouldBe("https://community.rallydev.com/slm/webservice/1.13/iteration/290873918");
                });
            });
        }

        [Test]
        public void should_parse_id_from_reference()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(ProjectXml_with_data_is_valid)
                    .And(the_XmlProjectParser_is_created);
                scenario.When("parsing reference string");
                scenario.Then("the id should be returned if it exist", () =>
                {
                    string projectId = XmlProjectParser.ParseIdFromReference();
                    projectId.ShouldBe("290873262");
                });
            });
        }
    }
}