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

using APD.DomainModel.ProjectInfo;
using APD.Integration.Holidays;
using APD.Plugin.ProjectInfo.DomainModel.Repositories;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.Holidays
{
    [TestFixture]
    public class When_parsing_xml_data_is_malformed
    {
        private string errorMessage = "";

        [Test]
        public void Should_fail_with_HolidayXmlMalformedException()
        {
            try
            {
                new XmlHolidayParser(Resource.MalformedXml);
            }
            catch (HolidayXmlMalformedException hxme)
            {
                errorMessage = hxme.Message;
            }
            Assert.AreSame(errorMessage, "XML-data is malformed.");
        }

        [Test]
        public void Should_fail_with_invalid_xml_data_when_xml_data_is_invalid()
        {
            try
            {
                new XmlHolidayParser(Resource.InvalidXml);
            }
            catch(HolidayXmlMalformedException hxme)
            {
                errorMessage = hxme.Message;
            }
            Assert.AreSame(errorMessage, "Invalid XML data. Is not valid in regards with the schema");
        }
    }
    
    [TestFixture]
    public class When_parsing_valid_xml
    {
        private XmlHolidayParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new XmlHolidayParser(Resource.NorwegianHolidays);
        }

        [Test]
        public void Should_return_list_of_holidays()
        {
            var holidaysList = parser.GenerateHolidays(DateTime.MinValue, DateTime.MaxValue);
            holidaysList.Count().ShouldBe(20);
            holidaysList.Last().Description.ShouldBe("Nyttårsaften");
        }

        [Test]
        public void Holiday_list_shall_only_contain_defined_range()
        {
            var inclusiveStart = new DateTime(2010, 01, 01);
            var inclusiveEnd = new DateTime(2010, 05, 17);

            var holidaysList = parser.GenerateHolidays(inclusiveStart, inclusiveEnd);
            holidaysList.Count().ShouldBe(3);
            holidaysList.Last().Description.ShouldBe("17. Mai");
        }
    }
}