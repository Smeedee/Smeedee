using System.Collections.Generic;
using System.Web.Script.Serialization;

using Smeedee.Integration.CI.Hudson.DomainModel.Repositories;


namespace Smeedee.IntegrationTests.CI.Hudson.Tests.DomainModel.Repositories
{
    public class MockRequestProxy : IRequestProxy
    {
        protected JavaScriptSerializer json = null;
        protected Dictionary<string, string> testData = null;

        public MockRequestProxy(Dictionary<string, string> data)
        {
            testData = data;
            json = new JavaScriptSerializer();
        }

        public T Execute<T>(string endPoint)
        {
            if (testData.ContainsKey(endPoint) == false)
            {
                return default(T);
            }
            return json.Deserialize<T>(testData[endPoint]);
        }
    }
}
