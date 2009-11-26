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
using System.Net;
using APD.DomainModel.CI;
using APD.DomainModel.Framework;
using APD.Integration.Framework.Utils;


namespace APD.Integration.CI.CruiseControl.DomainModel.Repositories
{
    public class CCServerRepository : IRepository<CIServer>
    {
        private readonly string ccNetDashboardUrl;
        private XmlServerReportParser projectParser;

        private const string XML_BUILD_LOG_FILENAME = "XmlBuildLog.xml";
        private const string LATEST_BUILD_FILENAME = "ViewLatestBuildReport.aspx";
        private const string BUILD_REPORT_FILENAME = "ViewBuildReport.aspx";
        private const string PROJECT_URL_PREFIX = "server/local/project/";
        private const string SERVER_FARM_REPORT_FILENAME = "XmlServerReport.aspx";

        private IHTTPRequest httpRequester;

        public CCServerRepository(string ccNetDashboardUrl) : 
            this(ccNetDashboardUrl, new SocketXMLBuildlogRequester())
        {
            
        }

        public CCServerRepository(string ccNetDashboardUrl, IHTTPRequest httpRequester)
        {
            this.ccNetDashboardUrl = TextUtilities.EnsureTrailingSlash(ccNetDashboardUrl);
            projectParser = new XmlServerReportParser();

            this.httpRequester = httpRequester;
        }

        private string GetBuildLogData(string projectName)
        {
            try
            {
                return GetBuildLogDataFromWebserver(projectName);
            }
            catch (Exception ex)
            {
                throw new CruiseControlRepositoryException(
                    "There was an error getting data from the cruise control server. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// A specialized version of the <c>APD.Utilities.NetUtilities.DownloadContentAsString()</c> 
        /// method to cope with redirection on the CC.NET webpage.
        /// </summary>
        private string GetBuildLogDataFromWebserver(string projectName)
        {
            string landingPageUrl = BuildProjectUrl(projectName) + LATEST_BUILD_FILENAME;
            HttpWebResponse landingPageResponse = NetUtilities.CreateRequestAndGetResponse(landingPageUrl);
        
            string buildLogXmlUrl = landingPageResponse.ResponseUri.ToString().Replace(BUILD_REPORT_FILENAME,
                                                                                       XML_BUILD_LOG_FILENAME);
            landingPageResponse.Close();
            return GetPartialXmlBuildLog(buildLogXmlUrl);
        }

        public string GetPartialXmlBuildLog(string buildLogXmlUrl) 
        {
            return httpRequester.Request(buildLogXmlUrl, 80);
        }

        private string BuildProjectUrl(string projectName)
        {
            return TextUtilities.EnsureTrailingSlash(ccNetDashboardUrl + PROJECT_URL_PREFIX + projectName);
        }

        private string GetProjectsXML()
        {
            try
            {
                return NetUtilities.DownloadContentAsString(ccNetDashboardUrl + SERVER_FARM_REPORT_FILENAME);
            }
            catch (Exception ex)
            {
                throw new CruiseControlRepositoryException(
                    "There was an error getting data from the cruise control server. See inner exception for details.",
                    ex);
            }
        }

        private IEnumerable<CIProject> GetProjects()
        {
            string xmlProjectData = GetProjectsXML();
            IEnumerable<CIProject> projects = projectParser.Parse(xmlProjectData);
            return projects;
        }

        private Build GetLatestBuildFromBuildLog(string projectName)
        {
            string buildLog = GetBuildLogData(projectName);
            var parser = new XmlBuildLogParser();
            Build latestBuild = parser.Parse(buildLog);
            return latestBuild;
        }

        //public IEnumerable<CIProject> Get(BuildSpecification specification)
        //{
        //    IEnumerable<CIProject> projects = GetProjects();
        //    foreach (var project in projects)
        //    {
        //        if (project.LatestBuild.Status == BuildStatus.FinishedSuccefully ||
        //            project.LatestBuild.Status == BuildStatus.FinishedWithFailure)
        //        {
        //            Build latestBuild = GetLatestBuildFromBuildLog(project.Name);
        //            project.LatestBuild.FinishedTime = latestBuild.FinishedTime;
        //            project.LatestBuild.Trigger = latestBuild.Trigger;
        //        }
        //    }


        //    return projects;
        //}

        public IEnumerable<CIServer> Get(Specification<CIServer> specification)
        {
            IEnumerable<CIProject> projects = GetProjects();
            foreach (var project in projects)
            {
                if (project.LatestBuild.Status == BuildStatus.FinishedSuccefully ||
                    project.LatestBuild.Status == BuildStatus.FinishedWithFailure)
                {
                    Build latestBuild = GetLatestBuildFromBuildLog(project.ProjectName);
                    project.LatestBuild.FinishedTime = latestBuild.FinishedTime;
                    project.LatestBuild.Trigger = latestBuild.Trigger;
                }
            }

            var ciServer = new CIServer
            {
                Name = "CC.Net",
                Url = ccNetDashboardUrl
            };
            foreach (CIProject project in projects)
            {
                ciServer.AddProject(project);
            }
            return new List<CIServer> {ciServer};
        }
    }
}