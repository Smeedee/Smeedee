using System;
using System.Net;

namespace Smeedee.Integration.CI.TeamCity.DomainModel.Repositories
{
    public class TeamCityXmlFetcher : IFetchTeamCityXml
    {
        private string _serverUrl;
        private WebClient _client;

        private const string PROJECT_LISTING_ADDRESS = "/httpAuth/app/rest/projects";
        private const string BUILD_LISTING_PREFIX = "/httpAuth/app/rest/buildTypes/id:";
        private const string BUILD_LISTING_POSTFIX = "/builds";
        private const string BUILD_INFO_ADDRESS = "/httpAuth/app/rest/builds/id:";
        private const string BUILD_CHANGES_PREFIX = "/httpAuth/app/rest/changes?build=id:";
        private const string BUILD_CHANGES_POSTFIX = "/builds/";
        

        public TeamCityXmlFetcher(string serverUrl, NetworkCredential credentials)
        {
            _serverUrl = serverUrl;
            _client = new WebClient();
            _client.Credentials = credentials;
        }

        public string GetTeamCityProjects()
        {
            return _client.DownloadString(_serverUrl + PROJECT_LISTING_ADDRESS);
        }

        public string GetTeamCityBuildConfigurations(string projectHref)
        {
            return _client.DownloadString(_serverUrl + projectHref);
        }

        public string GetTeamCityBuilds(string buildConfigHref) // bt5
        {
            return _client.DownloadString(_serverUrl + BUILD_LISTING_PREFIX + buildConfigHref + BUILD_LISTING_POSTFIX);
        }

        public string GetBuildInfo(string buildId)
        {
            return _client.DownloadString(_serverUrl + BUILD_INFO_ADDRESS + buildId);
        }

        public string GetChanges(string buildId)
        {
            return _client.DownloadString(_serverUrl + BUILD_CHANGES_PREFIX + buildId);
        }

        public string GetChangesetInfo(string changesetHref)
        {
            return _client.DownloadString(_serverUrl + changesetHref);
        }
    }
}