using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;
using Smeedee.DomainModel.Framework.Services;
using Smeedee.Integration.VCS.SVN.DomainModel.Services;
using Smeedee.Integration.VCS.TFSVC.DomainModel.Services;


namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "http://smeedee.org")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class VCSCredentialsService
    {
        private static Dictionary<string, ICheckIfCredentialsIsValid> credentialsCheckers =
            new Dictionary<string, ICheckIfCredentialsIsValid>();

        static VCSCredentialsService()
        {
            credentialsCheckers.Add("svn", new SVNCredentialsChecker());
            credentialsCheckers.Add("tfs", new TFSCredentialsChecker());
        }

        [OperationContract]
        public bool Check(string provider, string url, string username, string password)
        {
            try
            {
                if (credentialsCheckers.ContainsKey(provider))
                    return credentialsCheckers[provider].Check(provider, url, username, password);

                return false;
            }
            catch
            {
                return false;
            }
        }

        // Add more operations here and mark them with [OperationContract]
    }
}
