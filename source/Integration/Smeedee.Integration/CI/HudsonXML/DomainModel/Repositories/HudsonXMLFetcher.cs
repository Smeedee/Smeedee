using System;
using System.Net;
using Smeedee.Integration.CI.Hudson_XML.Helper_classes;
using Smeedee.Integration.CI.HudsonXML.Helper_classes;

namespace Smeedee.Integration.CI.HudsonXML.DomainModel.Repositories
{
    public class HudsonXmlFetcher : IFetchHudsonXml
    {
        private readonly string serverUrl;
        private readonly WebClient client;
        private URLGenerator generator;

        private const string API_POSTFIX = "/api/xml/";
        
        public HudsonXmlFetcher(string serverUrl, NetworkCredential credentials)
        {
            this.serverUrl = serverUrl;
            client = new WebClient {Credentials = credentials};
            generator = new URLGenerator(serverUrl);
        }

        public string GetProjects()
        {
            return generator.viewAll();
        }

        public string GetBuilds(string projectName)
        {
            var url = generator.xmlApi(projectName);
            try
            {
                string result = client.DownloadString(url);
                return result;
            }
            catch (WebException exception)
            {
                var newException = new HudsonXmlException("Unable to get builds for project: " + projectName);
                throw newException;

            }
        }

        public string GetBuild(string projectName, string buildId)
        {
            var url = generator.urlForBuild(projectName, buildId);
            return client.DownloadString(url);
        }

        public string GetUrl()
        {
            return serverUrl;
        }
    }

    public class HudsonXmlException : Exception
    {
        public HudsonXmlException(string s)
        {
            
        }
    }
}
