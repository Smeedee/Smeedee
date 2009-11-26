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
using System.IO;
using System.Xml;
using APD.DomainModel.ProjectInfo;


namespace APD.Plugin.ProjectInfo.DomainModel.Repositories
{
    public class XmlCountryHolidaysParser
    {
        private XmlDocument xmlDocument;

        public XmlCountryHolidaysParser()
        {
            LoadXml(Resource.NorwegianHolidays);
            ValidateXml();
        }

        public XmlCountryHolidaysParser(string countryHolidaysXml)
        {
            LoadXml(countryHolidaysXml);
            ValidateXml();
        }

        private void LoadXml(string inputXmlData)
        {
            xmlDocument = new XmlDocument();

            try
            {
                xmlDocument.LoadXml(inputXmlData);
            }
            catch (XmlException ex)
            {
                throw new HolidayXmlMalformedException("XML-data is malformed.", ex);
            }
        }

        private void ValidateXml()
        {
            xmlDocument.Schemas.Add("", new XmlTextReader(new StringReader(Resource.XmlCountryHolidays)));
            xmlDocument.Validate((o, e) =>
                {
                    throw new HolidayXmlMalformedException(
                        "Invalid XML data. Is not valid in regards with the schema", e.Exception);
                });
        }

        public HolidayProvider Parse()
        {
            HolidayProvider allHolidaysInInputString = ExtractHolidayProvider();
            return allHolidaysInInputString;
        }

        private HolidayProvider ExtractHolidayProvider()
        {
            HolidayProvider norwegianHolidayProvider = new HolidayProvider();
            var holidayElements = xmlDocument["ProjectInfo"].ChildNodes;

            foreach (XmlElement holidayElement in holidayElements)
            {
                DateTime holidayDate = ExtractHolidayFromXmlElement(holidayElement);
                norwegianHolidayProvider.Holidays.Add(holidayDate);
            }

            return norwegianHolidayProvider;
        }

        private static DateTime ExtractHolidayFromXmlElement(XmlElement holidayElement)
        {
            int year = Convert.ToInt16(holidayElement.GetAttribute("date").Substring(0, 4));
            int month = Convert.ToInt16(holidayElement.GetAttribute("date").Substring(5, 2));
            int day = Convert.ToInt16(holidayElement.GetAttribute("date").Substring(8, 2));
            
            return new DateTime(year, month, day);
        }
    }
}