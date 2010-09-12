using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                                   StartTime = GetDateTimeFromHudson(buildInformation["start-date"]),
                                   FinishedTime =
                                       GetDateTimeFromHudson(buildInformation["start-date"],
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

        private static DateTime GetDateTimeFromHudson(string buildDate, string addMilliseconds)
        {
            return GetDateTimeFromHudson(buildDate).AddMilliseconds(Int32.Parse(addMilliseconds));
        }

        private static DateTime GetDateTimeFromHudson(string buildDate)
        {
            var buildDateArray = buildDate.Split('_');
            var yearArray = buildDateArray[0].Split('-');
            var year = Int32.Parse(yearArray[0]);
            var month = Int32.Parse(yearArray[1]);
            var day = Int32.Parse(yearArray[2]);

            var timeArray = buildDateArray[1].Split('-');
            var hour = Int32.Parse(timeArray[0]);
            var minute = Int32.Parse(timeArray[1]);
            var second = Int32.Parse(timeArray[2]);

            var dateTime = new DateTime(year, month, day, hour, minute, second);
            return dateTime;
        }

        private static BuildStatus GetBuildStatusFromStatusString(string getAttribute)
        {
            if (getAttribute=="SUCCESS")
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
