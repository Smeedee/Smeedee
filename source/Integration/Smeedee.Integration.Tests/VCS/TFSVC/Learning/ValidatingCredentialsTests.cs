using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.TeamFoundation;

using NUnit.Framework;
using Microsoft.TeamFoundation.Client;
using System.Net;

using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.VCS.TFSVC.Learning
{
    [TestFixture][Category("IntegrationTest")]
    [Ignore("TFS Server is down")]
    public class ValidatingCredentialsTests
    {
        protected static string serverUrl = "http://80.203.160.221:8080/tfs";
        private TeamFoundationServer tfsClient;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void How_to_check_if_Credentials_are_valid()
        {
            TryCredentials("gronn", "genser").ShouldBeFalse();
            TryCredentials("smeedee", "dlog4321.").ShouldBeTrue();
        }

        private bool TryCredentials(string username, string password)
        {
            tfsClient = new TeamFoundationServer(serverUrl, new NetworkCredential(username, password));
            try
            {
                tfsClient.Authenticate();
                return true;
            }
            catch (TeamFoundationServerUnauthorizedException ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

    }
}
