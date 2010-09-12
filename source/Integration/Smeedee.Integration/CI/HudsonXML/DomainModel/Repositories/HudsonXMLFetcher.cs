using System.Diagnostics;
using System.Net;
using Smeedee.Integration.CI.Hudson_XML.Helper_classes;

namespace Smeedee.Integration.CI.HudsonXML.DomainModel.Repositories
{
    public class HudsonXmlFetcher : IFetchHudsonXml
    {
        private readonly string serverUrl;
        private readonly WebClient client;

        private const string API_POSTFIX = "/api/xml/";
        
        public HudsonXmlFetcher(string serverUrl, NetworkCredential credentials)
        {
            this.serverUrl = serverUrl;
            client = new WebClient {Credentials = credentials};
        }

        public string GetProjects()
        {
            return client.DownloadString(URL.Combine(serverUrl, "view/All", API_POSTFIX));
        }

        public string GetBuilds(string projectName)
        {
            var url = URL.Combine(GetFullProjectUrl(projectName), API_POSTFIX);
            try
            {
                string result = client.DownloadString(url);
                return result;
            }
            catch (WebException exception)
            {
                Debug.WriteLine(url + " gave me " + exception);
            }
            return client.DownloadString(url);
        }

        public string GetBuild(string projectName, string buildId)
        {
            var url = URL.Combine(GetFullProjectUrl(projectName), buildId, API_POSTFIX);
            return client.DownloadString(url);
        }

        public string GetUrl()
        {
            return serverUrl;
        }

        private string GetFullProjectUrl(string projectName)
        {
            return URL.Combine(serverUrl, "job", projectName);
        }
    }
}
