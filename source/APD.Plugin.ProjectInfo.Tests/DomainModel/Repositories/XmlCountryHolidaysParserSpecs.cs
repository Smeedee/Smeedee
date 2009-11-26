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

using APD.DomainModel.ProjectInfo;
using APD.Plugin.ProjectInfo.DomainModel.Repositories;

using NUnit.Framework;


namespace APD.Plugin.ProjectInfo.Tests.DomainModel.Repositories
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
                new XmlCountryHolidaysParser(Resource.MalformedXml);
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
                new XmlCountryHolidaysParser(Resource.InvalidXml);
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
        private XmlCountryHolidaysParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new XmlCountryHolidaysParser(Resource.NorwegianHolidays);
        }

        [Test]
        public void Should_return_holidayprovider()
        {
            var testHolidayProvider = parser.Parse();
            Assert.IsNotNull(testHolidayProvider);
            Assert.IsInstanceOfType(typeof(HolidayProvider), testHolidayProvider);
        }
    }
}