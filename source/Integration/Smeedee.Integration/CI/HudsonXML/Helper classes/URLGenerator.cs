using Smeedee.Integration.CI.Hudson_XML.Helper_classes;

namespace Smeedee.Integration.CI.HudsonXML.Helper_classes
{
    public class URLGenerator
    {
        private string serverUrl;

        private const string API_POSTFIX = "/api/xml/";

        public URLGenerator(string serverUrl)
        {
            this.serverUrl = serverUrl;
        }

        public string viewAll()
        {
            return URL.AppendToBaseURL(serverUrl, "view/All", API_POSTFIX);
        }

        public string xmlApi(string projectName)
        {
            return URL.AppendToBaseURL(GetFullProjectUrl(projectName), API_POSTFIX);
        }

        public string urlForBuild(string projectName, string buildId)
        {
            return URL.AppendToBaseURL(GetFullProjectUrl(projectName), buildId, API_POSTFIX);
        }

        private string GetFullProjectUrl(string projectName)
        {
            return URL.AppendToBaseURL(serverUrl, "job", projectName);
        }
    }
}
