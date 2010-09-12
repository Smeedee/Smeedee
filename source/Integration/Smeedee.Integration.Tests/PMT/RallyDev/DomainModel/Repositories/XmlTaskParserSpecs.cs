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
using System.Globalization;
using System.Threading;

using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Integration.PMT.RallyDev.DomainModel.Repositories;

using NUnit.Framework;
using Smeedee.Integration.Tests.PMT.RallyDev.DomainModel.Repositories;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;


namespace Smeedee.IntegrationTests.PMT.RallyDev.XmlTaskParserSpecs
{
    public class Shared
    {
        protected static XmlTaskParser XmlTaskParser;
        public static string XmlData;

        protected Context TaskXml_is_valid = () =>
        {
            XmlData = Resource.Task;
        };

        protected Context TaskWithDataXml_is_valid = () =>
        {
            XmlData = Resource.TaskWithData;
        };

        protected Context RevistionHistory_is_valid = () =>
        {
            XmlData = Resource.RevisionHistory;
        };

        protected Context the_XmlTaskParser_is_created = () => { XmlTaskParser_is_created(); };

        protected static void XmlTaskParser_is_created()
        {
            XmlTaskParser = new XmlTaskParser(XmlData);
        }

        protected Context the_current_ui_culture_is_norwegian = () =>
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nb-NO");
        };

        protected Context the_current_ui_culture_is_us_english = () =>
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        };
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_XmlTaskParser_is_spawned :Shared
    {
        [Test]
        public void should_have_default_constructor()
        {
            XmlTaskParser = new XmlTaskParser();
            XmlTaskParser.ShouldNotBeNull();
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_parsing_xml : Shared
    {
        [Test]
        public void should_parse_Task_with_data()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(TaskWithDataXml_is_valid)
                    .And(the_XmlTaskParser_is_created);
                scenario.When("a task is parsed");
                scenario.Then("a task is returned", () =>
                {
                    Task task = XmlTaskParser.ParseTaskWithData();
                    task.Name.ShouldBe("Update User Stories");
                    task.WorkEffortEstimate.ShouldBe(1);
                    task.Status.ShouldBe("Defined");
                    task.SystemId.ShouldBe("290873986");
                });
            });
        }

        [Test]
        public void should_parse_Task_with_norwegian_ui_culture()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(TaskWithDataXml_is_valid)
                    .And(the_XmlTaskParser_is_created)
                    .And(the_current_ui_culture_is_norwegian);
                scenario.When("a task is parsed");
                scenario.Then("a task is returned", () =>
                {
                    Task task = XmlTaskParser.ParseTaskWithData();
                    task.Name.ShouldBe("Update User Stories");
                    task.WorkEffortEstimate.ShouldBe(1);
                    task.Status.ShouldBe("Defined");
                    task.SystemId.ShouldBe("290873986");
                });
            });
        }

        [Test]
        public void should_parse_Task_with_us_english_ui_culture()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(TaskWithDataXml_is_valid)
                    .And(the_XmlTaskParser_is_created)
                    .And(the_current_ui_culture_is_us_english);
                scenario.When("a task is parsed");
                scenario.Then("a task is returned", () =>
                {
                    Task task = XmlTaskParser.ParseTaskWithData();
                    task.Name.ShouldBe("Update User Stories");
                    task.WorkEffortEstimate.ShouldBe(1);
                    task.Status.ShouldBe("Defined");
                    task.SystemId.ShouldBe("290873986");
                });
            });
        }

        [Test]
        public void should_parse_revisionhistory_reference()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(TaskWithDataXml_is_valid)
                    .And(the_XmlTaskParser_is_created);
                scenario.When("the revision history reference is to be parsed");
                scenario.Then("the revision history reference is returned", ()=>
                {
                    string historyReference = XmlTaskParser.ParseRevisionHistoryReference();
                    historyReference.ShouldBe("https://community.rallydev.com/slm/webservice/1.13/revisionhistory/290873987");
                });
            });
        }

        [Test]
        public void should_parse_WorkEffortHistory_from_RevisionHistory()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(RevistionHistory_is_valid)
                    .And(the_XmlTaskParser_is_created);
                scenario.When("the revision history is to be parsed");
                scenario.Then("the WorkEffortHistoryItems for the task is returned", ()=>
                {
                    List<WorkEffortHistoryItem> workEffortHistoryItems = XmlTaskParser.ParseWorkEffortHistoryFromRevisionHistory();
                    workEffortHistoryItems.Count.ShouldBe(2);
                    
                    workEffortHistoryItems.Sort();

                    workEffortHistoryItems[0].TimeStampForUpdate.Kind.ShouldBe(DateTimeKind.Local);
                    workEffortHistoryItems[0].TimeStampForUpdate.ShouldBe(DateTime.Parse("2009-07-29T14:25:44.000Z"));
                    workEffortHistoryItems[0].RemainingWorkEffort.ShouldBe(5);
                    
                    
                    workEffortHistoryItems[1].TimeStampForUpdate.Kind.ShouldBe(DateTimeKind.Local);
                    workEffortHistoryItems[1].TimeStampForUpdate.ShouldBe(DateTime.Parse("2009-07-30T14:25:44.000Z"));
                    workEffortHistoryItems[1].RemainingWorkEffort.ShouldBe(0);
                });
            });
        }

        [Test]
        public void should_parse_WorkEffortHistoryItem()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(TaskWithDataXml_is_valid)
                    .And(the_XmlTaskParser_is_created);
                scenario.When("a WorkEffortHistoryItem for a task is parsed");
                scenario.Then("a WorkEffortHistoryItem is returned", ()=>
                {
                    WorkEffortHistoryItem workEffortHistoryItem = XmlTaskParser.ParseWorkEffortHistoryItem();
                    workEffortHistoryItem.RemainingWorkEffort.ShouldBe(1);
                    workEffortHistoryItem.TimeStampForUpdate.ShouldNotBeNull();
                });
            });
        }
    }
}