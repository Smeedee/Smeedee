using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Smeedee.DomainModel.Framework.Services;

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;


namespace Smeedee.Integration.VCS.TFSVC.DomainModel.Services
{
    public class TFSCredentialsChecker : ICheckIfCredentialsIsValid
    {
        #region ICheckIfCredentialsIsValid Members

        public bool Check(string provider, string url, string username, string password)
        {
            return TryCredentials(url, username, password);
        }

        #endregion

        private bool TryCredentials(string url, string username, string password)
        {
            var tfsClient = new TeamFoundationServer(url, new NetworkCredential(username, password));
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
