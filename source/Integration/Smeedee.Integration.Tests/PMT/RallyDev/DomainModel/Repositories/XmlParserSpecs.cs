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
using System.Xml;

using Smeedee.Integration.PMT.RallyDev.DomainModel.Repositories;

using NUnit.Framework;
using Smeedee.Integration.Tests.PMT.RallyDev.DomainModel.Repositories;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.PMT.RallyDev.XmlParserSpecs
{
    public class Shared
    {
        protected static XmlParser Parser;
        protected static string XmlFile;
        protected static XmlDocument XmlDocument;

        protected Context xml_file_is_malformed = () =>
        {
            XmlFile = Resource.Malformed;
        };

        protected Context xml_file_is_valid = () =>
        {
            XmlFile = Resource.Valid;
        };

        protected Context xml_has_query_results = () =>
        {
            XmlFile = Resource.TasksForIteration;
        };

        protected Context parser_is_created = CreateParser;
       
        protected When the_parser_is_created = CreateParser;

        protected static void CreateParser()
        {
            Parser = new XmlParser(XmlFile);
            XmlDocument = new XmlDocument();
            XmlDocument.LoadXml(XmlFile);
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_spawned : Shared
    {
        [Test]
        public void should_have_default_constructor()
        {
            XmlParser defaultParser = new XmlParser();
            defaultParser.ShouldNotBeNull();
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_loading : Shared
    {
        [Test]
        [ExpectedException(typeof(XmlException))]
        public void should_throw_XmlException_when_xml_is_malformed()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(xml_file_is_malformed)
                    .And(parser_is_created);
                scenario.When("xml is loaded");
                scenario.Then("XmlException should be thrown");
            });
        }

        [Test]
        public void should_load_valid_xml()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(xml_file_is_valid)
                    .And(parser_is_created);
                scenario.When("xml is loaded");
                scenario.Then("XmlException should not be thrown");
            });
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_parsing : Shared
    {
        [Test]
        public void should_get_string_from_node()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(xml_file_is_valid)
                    .And(parser_is_created);
                scenario.When("parsing node for string");
                scenario.Then("node should be parsed correctly", ()=>
                {
                    string nodeValue = Parser.GetStringValueFromNode(XmlDocument["Task"]["Name"]);
                    nodeValue.ShouldBe("Job interview with students");
                    nodeValue = Parser.GetStringValueFromNode(null);

                    nodeValue.ShouldBe("");
                });
            });
        }

        [Test]
        public void should_get_int_from_node()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(xml_file_is_valid)
                    .And(parser_is_created);
                scenario.When("parsing node for int");
                scenario.Then("node should be parsed correctly", () =>
                {
                    int nodeValue = Parser.GetIntValueFromNode(XmlDocument["Task"]["ObjectID"]);
                    nodeValue.ShouldBe(295139730);
                    nodeValue = Parser.GetIntValueFromNode(null);

                    nodeValue.ShouldBe(0);
                });
            });
        }

        [Test]
        public void should_get_date_and_time_from_node()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(xml_file_is_valid)
                    .And(parser_is_created);
                scenario.When("parsing node for date");
                scenario.Then("node should be parsed correctly", () =>
                {
                    DateTime nodeValue = Parser.GetDateFromXmlNode(XmlDocument["Task"]["LastUpdateDate"]);
                    nodeValue.Kind.ShouldBe(DateTimeKind.Local);
                    nodeValue.ShouldBe(DateTime.Parse("2009-01-27T10:14:27.000Z"));
                    
                    nodeValue = Parser.GetDateFromXmlNode(null);

                    nodeValue.ShouldBe(new DateTime());
                });
            });
        }

        [Test]
        public void should_parse_TotalResultCount()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(xml_has_query_results)
                    .And(parser_is_created);
                scenario.When("parsing a query result");
                scenario.Then("the totalt result count should be returned", ()=>
                {
                    int totalResultCount = Parser.GetTotalResultCount();
                    totalResultCount.ShouldBe(9);
                });
            });
        }
    }
}