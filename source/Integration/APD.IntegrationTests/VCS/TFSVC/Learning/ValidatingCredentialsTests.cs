using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.TeamFoundation;

using NUnit.Framework;
using Microsoft.TeamFoundation.Client;
using System.Net;

using TinyBDD.Specification.NUnit;


namespace APD.IntegrationTests.VCS.TFSVC.Learning
{
    [TestFixture]
    public class ValidatingCredentialsTests
    {
        protected static string serverUrl = "https://tfs08.codeplex.com";
        private TeamFoundationServer tfsClient;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void How_to_check_if_Credentials_are_valid()
        {
            TryCredentials("gronn", "genser").ShouldBeFalse();
            TryCredentials("smeedee_cp", "haldis").ShouldBeTrue();
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
