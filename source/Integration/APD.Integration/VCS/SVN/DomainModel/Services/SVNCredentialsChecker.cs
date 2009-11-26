using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using APD.DomainModel.Framework.Services;

using SharpSvn;


namespace APD.Integration.VCS.SVN.DomainModel.Services
{
    public class SVNCredentialsChecker : ICheckIfCredentialsIsValid
    {
        #region ICheckIfCredentialsIsValid Members

        public bool Check(string provider, string url, string username, string password)
        {
            return TryCredentials(url, username, password);
        }

        #endregion

        private bool TryCredentials(string url, string username, string password)
        {
            var client = new SvnClient();
            client.Authentication.Clear();
            client.Authentication.DefaultCredentials = new NetworkCredential(username, password);
            client.Authentication.SslServerTrustHandlers += (o, e) =>
            {
                e.AcceptedFailures = e.Failures;
                e.Save = true;
            };

            try
            {
                Collection<SvnLogEventArgs> logs;
                client.GetLog(new Uri(url), new SvnLogArgs() { Limit = 1 }, out logs);
                return true;
            }
            catch (SvnAuthorizationException ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
