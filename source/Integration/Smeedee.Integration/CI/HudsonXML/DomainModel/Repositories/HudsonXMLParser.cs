using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;
using Smeedee.DomainModel.CI;

namespace Smeedee.Integration.CI.HudsonXML.DomainModel.Repositories
{
    public class HudsonXmlParser
    {
        private readonly IFetchHudsonXml xmlFetcher;

        public HudsonXmlParser(IFetchHudsonXml xmlFetcher)
        {
            this.xmlFetcher = xmlFetcher;
        }

        public CIServer GetServer()
        {
            return new CIServer("Hudson Server", xmlFetcher.GetUrl());
        }

        public IList<CIProject> GetProjects()
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlFetcher.GetProjects());
            var result = from XmlElement e in doc.SelectNodes("/allView/job/name")
                         select new CIProject(e.InnerXml.Trim());

            return result.ToList();
        }

        public IList<Build> GetBuilds(string projectName)
        {
            var doc = new XmlDocument();
            var builds = new List<Build>();
            doc.LoadXml(xmlFetcher.GetBuilds(projectName));

            foreach (XmlElement e in doc.SelectNodes("/freeStyleProject/build"))
            {
                var buildInformation = GetBuildInformation(projectName, e.SelectSingleNode("//number").InnerXml.Trim());
                builds.Add(new Build()
                               {
                                   Status = GetBuildStatusFromStatusString(buildInformation["status"]),
                                   StartTime = GetDateTimeForBuild(buildInformation["start-date"]),
                                   FinishedTime =
                                       GetDateTimeForBuild(buildInformation["start-date"],
                                                             buildInformation["duration"]),
                                   SystemId = e.GetAttribute("id"),
                                   Trigger = new Trigger()
                                                 {
                                                     Cause = "Hudson",
                                                     InvokedBy = buildInformation["invoker"]
                                                 }
                               });
            }
            return builds;
        }

        private static DateTime GetDateTimeForBuild(string buildDate, string addMilliseconds)
        {
            return GetDateTimeForBuild(buildDate).AddMilliseconds(Int32.Parse(addMilliseconds));
        }

        private static DateTime GetDateTimeForBuild(string buildDate)
        {
            const string timeformat = "yyyy-MM-dd_HH-mm-ss";
            var provider = CultureInfo.InvariantCulture;

            return DateTime.ParseExact(buildDate, timeformat, provider);
        }

        private static BuildStatus GetBuildStatusFromStatusString(string statusString)
        {
            if (statusString=="SUCCESS")
                return BuildStatus.FinishedSuccefully;
            
            return BuildStatus.FinishedWithFailure;
            
        }

        private Dictionary<string, string> GetBuildInformation(string projectName, string buildId)
        {
            var elementPath = new Dictionary<string, string>();
            elementPath.Add("status", "/freeStyleBuild/result");
            elementPath.Add("start-date", "/freeStyleBuild/id");
            elementPath.Add("duration", "/freeStyleBuild/duration");
            elementPath.Add("invoker", "/freeStyleBuild/culprit/fullName");

            var buildInfo = new Dictionary<string, string>();
            var doc = new XmlDocument();
            doc.LoadXml(xmlFetcher.GetBuild(projectName, buildId));

            foreach (KeyValuePair<string, string> pair in elementPath)
            {
                try
                {
                    buildInfo.Add(pair.Key, (from XmlElement e in doc.SelectNodes(pair.Value)
                                             select e.InnerText).First().Trim());
                }catch(InvalidOperationException exception)
                {
                    buildInfo.Add(pair.Key, "Unknown");
                }
            }

            return buildInfo;
        }


        public string GetUrl()
        {
            return xmlFetcher.GetUrl();
        }
    }
}
