using APD.DomainModel.Framework;
using APD.DomainModel.SourceControl;
using APD.Harvester.SourceControl.Factories;
using APD.Integration.VCS.Git.DomainModel;
using APD.Integration.VCS.SVN.DomainModel.Repositories;
using APD.Integration.VCS.TFSVC.DomainModel.Repositories;
using APD.DomainModel.Config;
using System.Net;


namespace APD.Harvester.SourceControl
{
    public class ChangesetRepositoryFactory : IAssembleRepository<Changeset>
    {
        private const string URL_SETTING_NAME = "url";
        private const string USERNAME_SETTING_NAME = "username";
        private const string PASSWORD_SETTING_NAME = "password";
        private const string PROVIDER_SETTING_NAME = "provider";
        private const string PROJECT_SETTING_NAME = "project";
        private const string TFS = "tfs";
        private const string GIT = "git";

        public IRepository<Changeset> Assemble(Configuration configuration)
        {
            if (configuration.GetSetting(PROVIDER_SETTING_NAME).Value.Equals(TFS))
                return new TFSChangesetRepository(configuration.GetSetting(URL_SETTING_NAME).Value,
                    configuration.GetSetting(PROJECT_SETTING_NAME).Value,
                    new NetworkCredential()
                    {
                        UserName = configuration.GetSetting(USERNAME_SETTING_NAME).Value,
                        Password = configuration.GetSetting(PASSWORD_SETTING_NAME).Value
                    });
            if (configuration.GetSetting(PROVIDER_SETTING_NAME).Value.Equals(GIT))
            {
                return new GitChangesetRepository(configuration.GetSetting(URL_SETTING_NAME).Value);
            }
            
            return new SVNChangesetRepository(configuration.GetSetting(URL_SETTING_NAME).Value,
                    configuration.GetSetting(USERNAME_SETTING_NAME).Value,
                    configuration.GetSetting(PASSWORD_SETTING_NAME).Value);
        }
    }
}
