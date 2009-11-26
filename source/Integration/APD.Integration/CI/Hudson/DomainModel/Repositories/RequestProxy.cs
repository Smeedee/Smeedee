using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

using APD.Integration.CI.CruiseControl.DomainModel.Repositories;


namespace APD.Integration.CI.Hudson.DomainModel.Repositories
{
    public class RequestProxy : IRequestProxy
    {
        #region Member Variables

        protected JavaScriptSerializer json = null;

        #endregion

        #region Properties

        public string Url { get; private set;  }

        #endregion

        #region Constructors

        public RequestProxy()
        {
            json = new JavaScriptSerializer();
            Url = "http://localhost:8080";
        }

        public RequestProxy(string serverUrl) : this()
        {
            if (String.IsNullOrEmpty(serverUrl))
            {
                throw new HudsonRepositoryException("Invalid server url");
            }
            Url = serverUrl;
        }

        #endregion

        #region Private Methods

        internal string GetJson(string endpoint)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader reader = null;

            try
            {
                request = WebRequest.Create(String.Format("{0}/{1}", Url, endpoint)) as HttpWebRequest;
                //request.Timeout = 20 * 1000;

                if (request == null)
                {
                    return null;
                }

                using (response = request.GetResponse() as HttpWebResponse)
                {
                    if ((response == null) || (!request.HaveResponse))
                    {
                        return null;
                    }

                    reader = new StreamReader(response.GetResponseStream());
                    return reader.ReadToEnd();
                }
            }
            catch (WebException we)
            {
                if (we != null && we.Response != null)
                {
                    Console.WriteLine("The server returned: {0}", (we.Response as HttpWebResponse).StatusDescription);
                }
            }

            return null;
        }

        #endregion
        
        #region IRequestProxy Methods

        public T Execute<T>(string endpoint)
        {
            string result = GetJson(endpoint);

            if (String.IsNullOrEmpty(result))
            {
                return default(T);
            }

            return json.Deserialize<T>(result);
        }

        #endregion
    }
}
