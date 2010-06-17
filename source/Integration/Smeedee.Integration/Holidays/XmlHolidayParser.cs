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
using System.IO;
using System.Xml;

using Smeedee.DomainModel.Holidays;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Plugin.ProjectInfo.DomainModel.Repositories;


namespace Smeedee.Integration.Holidays
{
    public class XmlHolidayParser : IGenerateHolidays
    {
        private XmlDocument xmlDocument;

        public XmlHolidayParser()
        {
            LoadXml(Resource.NorwegianHolidays);
            ValidateXml();
        }

        public XmlHolidayParser(string countryHolidaysXml)
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

        public IEnumerable<Holiday> GenerateHolidays(DateTime inclusiveStart, DateTime inclusiveEnd)
        {
            var holidayElements = xmlDocument["Holidays"].ChildNodes;

            List<Holiday> holidays = new List<Holiday>();

            foreach (XmlElement holidayElement in holidayElements)
            {
                DateTime holidayDate = ExtractHolidayFromXmlElement(holidayElement);
                if( holidayDate >= inclusiveStart && holidayDate <= inclusiveEnd )
                {
                    string holidayDescription = ExtractDescriptionFromXmlElement(holidayElement);

                    holidays.Add( new Holiday() {
                                    Date = holidayDate,
                                    Description = holidayDescription
                                });
                }
            }

            return holidays;
        }

        private DateTime ExtractHolidayFromXmlElement(XmlElement holidayElement)
        {
            return XmlConvert.ToDateTime(holidayElement.Attributes["date"].Value, XmlDateTimeSerializationMode.Local);
        }

        private string ExtractDescriptionFromXmlElement(XmlElement holidayElement)
        {
            string description = holidayElement.GetAttribute("description");
            if (string.IsNullOrEmpty(description))
                return string.Empty;
            else
                return description;
        }
    }
}