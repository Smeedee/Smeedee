using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using Smeedee.DomainModel.CI;

namespace Smeedee.Integration.CI.TeamCity.DomainModel.Repositories
{
    public class TeamCityXmlParser
    {
        private IFetchTeamCityXml _xmlFetcher;

        public TeamCityXmlParser(IFetchTeamCityXml xmlFetcher)
        {
            _xmlFetcher = xmlFetcher;
        }

        public IList<CIServer> GetTeamCityProjects()
        {
            var doc = new XmlDocument();
            doc.LoadXml(_xmlFetcher.GetTeamCityProjects());

            var result = from XmlElement e in doc.SelectNodes("//projects/project")
                   select new CIServer(e.GetAttribute("name"), e.GetAttribute("href"));

            return result.ToList();

        }

        public IList<CIProject> GetTeamCityBuildConfigurations(string url)
        {
            var doc = new XmlDocument();
            doc.LoadXml(_xmlFetcher.GetTeamCityBuildConfigurations(url));

            var result = from XmlElement e in doc.SelectNodes("//project/buildTypes/buildType")
                   select new CIProject(e.GetAttribute("name")){SystemId = e.GetAttribute("id")};

            return result.ToList();
        }


        public IList<Build> GetTeamCityBuilds(string buildConfigHref)
        {
            var doc = new XmlDocument();
            var builds = new List<Build>();
            doc.LoadXml(_xmlFetcher.GetTeamCityBuilds(buildConfigHref));
            
            foreach (XmlElement e in doc.SelectNodes("//builds/build"))
            {
                var buildDates = GetBuildDates(e.GetAttribute("id"));
                builds.Add(new Build()
                               {
                                   Status = GetBuildStatusFromStatusString(e.GetAttribute("status")),
                                   StartTime = GetDateTimeFromTeamCity(buildDates["start-date"]),
                                   FinishedTime = GetDateTimeFromTeamCity(buildDates["end-date"]),
                                   SystemId = e.GetAttribute("id"),
                                   Trigger = new Trigger()
                                   {
                                       Cause = "TeamCity",
                                       InvokedBy = GetInvoker(e.GetAttribute("id"))
                                   }
                               });
            }
            return builds;
        }

        private Dictionary<string, string> GetBuildDates(string buildId)
        {
            var buildInfo = new Dictionary<string, string>();
            var doc = new XmlDocument();
            doc.LoadXml(_xmlFetcher.GetBuildInfo(buildId));

            buildInfo.Add("start-date", (from XmlElement e in doc.SelectNodes("//build/startDate")
                    select e.InnerText).First());

            buildInfo.Add("end-date", (from XmlElement e in doc.SelectNodes("//build/finishDate")
                    select e.InnerText).First());

            return buildInfo;
        }

        private string GetInvoker(string buildId)
        {
            var doc = new XmlDocument();
            doc.LoadXml(_xmlFetcher.GetChanges(buildId));

            var changes = from XmlElement e in doc.SelectNodes("//changes")
                          select Int32.Parse(e.GetAttribute("count"));

            if (changes.First() > 0)
            {
                XmlElement e = (XmlElement) doc.SelectNodes("//changes/change")[0];
                return GetAuthor(_xmlFetcher.GetChangesetInfo(e.GetAttribute("href")));
            }
            
            return "System";
        }

        private string GetAuthor(string changesetInfo)
        {
            var doc = new XmlDocument();
            doc.LoadXml(changesetInfo);

            return ((XmlElement) doc.SelectNodes("//change")[0]).GetAttribute("username");
        }


        private BuildStatus GetBuildStatusFromStatusString(string statusString)
        {
            switch (statusString)
            {
                case "SUCCESS":
                    return BuildStatus.FinishedSuccefully;
                case "FAILURE":
                    return BuildStatus.FinishedWithFailure;
                case "ERROR":
                    return BuildStatus.FinishedWithFailure;
                default:
                    return BuildStatus.Unknown;
            }
        }

        private static DateTime GetDateTimeFromTeamCity(string dateString)
        {
            var timeFormat = "yyyyMMddTHHmmsszzz";
            var provider = CultureInfo.InvariantCulture;

            return DateTime.ParseExact(dateString, timeFormat, provider);
        }
    }
}