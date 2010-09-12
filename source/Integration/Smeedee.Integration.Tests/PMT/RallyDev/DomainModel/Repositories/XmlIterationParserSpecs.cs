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
using System.Collections.Generic;

using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Integration.PMT.RallyDev.DomainModel.Repositories;

using NUnit.Framework;
using Smeedee.Integration.Tests.PMT.RallyDev.DomainModel.Repositories;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;

namespace Smeedee.IntegrationTests.PMT.RallyDev.XmlIterationParserSpecs
{
    public class Shared
    {
        protected static XmlIterationParser XmlIterationParser;
        public static string XmlData;

        protected Context IterationXml_is_valid = () =>
        {
            XmlData = Resource.Iteration;
        };
        
        protected Context TasksForIterationXml_is_valid = () =>
        {
            XmlData = Resource.TasksForIteration;
        };

        protected Context the_XmlIterationParser_is_created = () => { XmlIterationParser_is_created(); };

        protected static void XmlIterationParser_is_created()
        {
            XmlIterationParser = new XmlIterationParser(XmlData);
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_XmlIterationParserSpecs_is_spawned : Shared
    {
        [Test]
        public void should_have_default_constructor()
        {
            XmlIterationParser = new XmlIterationParser();
            XmlIterationParser.ShouldNotBeNull();
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_parsing_xml : Shared
    {
        [Test]
        public void should_parse_TaskReferencesForIteration()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(TasksForIterationXml_is_valid)
                    .And(the_XmlIterationParser_is_created);
                scenario.When("the task references for an iteration is to be parsed");
                scenario.Then("a list of references for the tasks in the iteration is returned", () =>
                {
                    List<string> taskReferencesForIteration = XmlIterationParser.ParseTaskReferencesForIteration();
                    taskReferencesForIteration.Count.ShouldBe(9);
                    taskReferencesForIteration[0].ShouldBe("https://community.rallydev.com/slm/webservice/1.13/task/290873986");
                });
            });
        }

        [Test]
        public void should_parse_Iteration()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(IterationXml_is_valid)
                    .And(the_XmlIterationParser_is_created);
                scenario.When("an iteration is to be parsed");
                scenario.Then("a parsed iteration is returned", () =>
                {
                    Iteration iteration = XmlIterationParser.ParseIteration();
                    iteration.Name.ShouldBe("Iteration 0");
                    iteration.StartDate.Kind.ShouldBe(DateTimeKind.Local);
                    iteration.StartDate.ShouldBe(DateTime.Parse("2009-01-23T00:00:00.000Z"));
                    iteration.EndDate.Kind.ShouldBe(DateTimeKind.Local);
                    iteration.EndDate.ShouldBe(DateTime.Parse("2009-01-23T23:59:59.000Z"));
                    iteration.SystemId.ShouldBe("290873918");
                });
            });
        }
    }
}