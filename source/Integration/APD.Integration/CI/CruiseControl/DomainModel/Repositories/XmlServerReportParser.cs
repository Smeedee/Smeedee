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
using System.IO;
using System.Xml;
using APD.DomainModel.CI;
using APD.Integration.Properties;


namespace APD.Integration.CI.CruiseControl.DomainModel.Repositories
{
    public class XmlServerReportParser
    {
        private static Dictionary<string, Build> buildCache = new Dictionary<string, Build>();
        private XmlDocument xmlDocument;

        public IEnumerable<CIProject> Parse(string inputXmlString)
        {
            LoadXml(inputXmlString);
            ValidateXML(xmlDocument);

            IEnumerable<CIProject> allProjectsInInputString = ExtractAllProjectElements();
            return allProjectsInInputString;
        }

        private void LoadXml(string inputXmlData) {
            xmlDocument = new XmlDocument();

            try
            {
                xmlDocument.LoadXml(inputXmlData);
            }
            catch (XmlException ex)
            {
                throw new CruiseControlRepositoryException("XML-data is malformed.", ex);
            }
        }

        private void ValidateXML(XmlDocument inputXmlDocument)
        {
            inputXmlDocument.Schemas.Add("", new XmlTextReader(new StringReader(Resources.XmlServerReport)));
            inputXmlDocument.Validate(
                (o, e) =>
                {
                    throw new CruiseControlRepositoryException(
                        "Invalid XML data. Does not validate against the schema", e.Exception);
                });
            inputXmlDocument.Schemas = null;
        }

        private IEnumerable<CIProject> ExtractAllProjectElements() {
            var projects = new List<CIProject>();
            var projectElements = xmlDocument["CruiseControl"]["Projects"].ChildNodes;

            foreach(XmlElement projectElement in projectElements)
            {
                CIProject CIProject = ExtractProjectInfoFromXmlElement(projectElement);
                CIProject.SystemId = CIProject.ProjectName;
                projects.Add(CIProject);
            }

            return projects;
        }

        private CIProject ExtractProjectInfoFromXmlElement(XmlElement projectElement)
        {
            var projectInfo = new CIProject(projectElement.GetAttribute("name"));

            Build latestBuild = ExtractBuildFromXmlElement(projectElement, projectInfo.ProjectName );

            projectInfo.AddBuild(latestBuild);
            LinkProjectBuildsToProject(projectInfo);

            return projectInfo;
        }

        private void LinkProjectBuildsToProject(CIProject project)
        {
            foreach (Build build in project.Builds)
            {
                build.Project = project;
            }
        }

        private Build ExtractBuildFromXmlElement(XmlElement projectElement, string projectName) 
        {
            var build = new Build();

            build.Status = GetStatus(projectElement);

            bool lastParseBuildIsCached = buildCache.ContainsKey(projectName);
            if( build.Status == BuildStatus.Building && lastParseBuildIsCached )
            {
                Build lastBuild = buildCache[projectName];
                bool lastBuildStatusWasBuilding = lastBuild.Status == BuildStatus.Building;

                build.StartTime = lastBuildStatusWasBuilding
                                      ? lastBuild.StartTime
                                      : GetBuildStartTime(projectElement);
            }
            else
            {
                build.StartTime = GetBuildStartTime(projectElement);                
            }

            build.SystemId = GenerateBuildSystemId(build);
            
            buildCache[projectName] = build;
            return build;
        }

        private string GenerateBuildSystemId(Build build)
        {
            return build.StartTime.Ticks.ToString();
        }

        private DateTime GetBuildStartTime(XmlElement projectElement)
        {
            // CruiseControl does not expose the build's starttime in the XmlServerReport
            // If the project is currently building, the starttime will be approximated to now
            bool isBuilding = projectElement.GetAttribute("activity") == "Building";
            if (isBuilding)
            {
                return DateTime.Now;
            }
            else
            {
                string lastBuildTimeString = projectElement.GetAttribute("lastBuildTime");
                var startBuildTime = DateTime.Parse(lastBuildTimeString, System.Globalization.CultureInfo.InvariantCulture);
                return startBuildTime;
            }
        }

        private BuildStatus GetStatus(XmlElement projectElement)
        {
            bool isBuilding = projectElement.GetAttribute("activity") == "Building";
            string lastBuildStatus = projectElement.GetAttribute("lastBuildStatus");

            if (isBuilding)
            {
                return BuildStatus.Building;
            }
            else
            {
                switch (lastBuildStatus)
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
        }
    }
}