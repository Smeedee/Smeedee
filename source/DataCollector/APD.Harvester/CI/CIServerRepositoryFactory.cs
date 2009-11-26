using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.DomainModel.Framework;
using APD.Harvester.SourceControl.Factories;
using APD.DomainModel.CI;
using APD.DomainModel.Config;
using APD.Integration.CI.CruiseControl.DomainModel.Repositories;
using APD.Integration.CI.TFSBuild.DomainModel.Repositories;
using System.Net;


namespace APD.Harvester.CI
{
    public class CIServerRepositoryFactory : IAssembleRepository<CIServer>
    {
        private const string URL_SETTING_NAME = "url";
        private const string USERNAME_SETTING_NAME = "username";
        private const string PASSWORD_SETTING_NAME = "password";
        private const string PROVIDER_SETTING_NAME = "provider";
        private const string PROJECT_SETTING_NAME = "project";
        private const string TFS = "tfs";

        #region IAssembleRepository<CIServer> Members

        public IRepository<CIServer> Assemble(Configuration configuration)
        {
            if (configuration.GetSetting(PROVIDER_SETTING_NAME).Value.Equals(TFS))
                return new TFSCIServerRepository(configuration.GetSetting(URL_SETTING_NAME).Value,
                    configuration.GetSetting(PROJECT_SETTING_NAME).Value, 
                    new NetworkCredential()
                    {
                        UserName = configuration.GetSetting(USERNAME_SETTING_NAME).Value,
                        Password = configuration.GetSetting(PASSWORD_SETTING_NAME).Value
                    });
            else
                return new CCServerRepository(configuration.GetSetting(URL_SETTING_NAME).Value);
        }

        #endregion
    }
}
