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
using System.Globalization;
using System.Xml;
using APD.DomainModel.CI;


namespace APD.Integration.CI.CruiseControl.DomainModel.Repositories
{
    public class XmlBuildLogParser
    {
        private XmlDocument xmlDocument;

        public Build Parse(string inputXmlData)
        {
            ValidateAndLoadXml(inputXmlData);

            try
            {
                var build = new Build();

                XmlElement integrationProperties = xmlDocument["cruisecontrol"]["integrationProperties"];

                string buildTrigger = integrationProperties["CCNetBuildCondition"].InnerText;
                string buildStatus = integrationProperties["CCNetIntegrationStatus"].InnerText;

                build.Trigger = GetTrigger(buildTrigger);

                build.StartTime = GetStartTime();
                TimeSpan duration = GetBuildDuration();
                build.FinishedTime = build.StartTime + duration;
                build.Status = ParseStatus(buildStatus);

                return build;
            }
            catch (Exception ex)
            {
                throw new CruiseControlRepositoryException(
                    "There was an error parsing the XML-file. " +
                    "Please refer to the inner exception for details.", ex);
            }
        }

        private BuildStatus ParseStatus(string buildStatus)
        {
            switch (buildStatus)
            {
                case "Success":
                    return BuildStatus.FinishedSuccefully;

                case "Failure":
                case "Exception":
                    return BuildStatus.FinishedWithFailure;

                default:
                    return BuildStatus.Unknown;
            }
        }

        private DateTime ParseDate(string date)
        {
            return DateTime.Parse(date.Replace("-", "."), CultureInfo.CurrentCulture);
        }

        private TimeSpan ParseTimeSpan(string timespan)
        {
            return TimeSpan.Parse(timespan);
        }

        private Trigger GetTrigger(string buildTrigger)
        {
            switch (buildTrigger)
            {
                case "IfModificationExists":
                    //TODO: refactor?
                    var users = xmlDocument["cruisecontrol"]["integrationProperties"]["CCNetModifyingUsers"];

                    return new CodeModifiedTrigger(users.ChildNodes[0].InnerText);

                case "ForceBuild":
                    return new EventTrigger("Forced Build");

                default:
                    return new UnknownTrigger();
            }
        }

        private DateTime GetStartTime()
        {
            string startTimeString = xmlDocument["cruisecontrol"]["build"].GetAttribute("date");

            return ParseDate(startTimeString);
        }

        private TimeSpan GetBuildDuration()
        {
            string durationString = xmlDocument["cruisecontrol"]["build"].GetAttribute("buildtime");

            return ParseTimeSpan(durationString);
        }

        private void ValidateAndLoadXml(string inputXmlData)
        {
            xmlDocument = new XmlDocument();

            try
            {
                // Validating is performed on load.
                xmlDocument.LoadXml(inputXmlData);
            }
            catch (XmlException ex)
            {
                throw new CruiseControlRepositoryException(
                    "Input data was not well formed.", ex);
            }
        }
    }
}