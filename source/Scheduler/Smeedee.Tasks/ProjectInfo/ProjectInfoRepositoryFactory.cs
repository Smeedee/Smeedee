using System;
using System.Collections.Generic;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Integration.PMT.ScrumForTFS.DomainModel.Repositories;
using Smeedee.Tasks.Framework.Factories;

namespace Smeedee.Tasks.ProjectInfo
{
    public class ProjectInfoRepositoryFactory : IAssembleRepository<ProjectInfoServer>
    {
        private const string URL_SETTING_NAME = "url";
        private const string USERNAME_SETTING_NAME = "username";
        private const string PASSWORD_SETTING_NAME = "password";
        private const string PROVIDER_SETTING_NAME = "provider";
        private const string PROJECT_SETTING_NAME = "project";
        private const string SCRUM_FOR_TFS = "conchango-tfs";

        public IRepository<ProjectInfoServer> Assemble(Configuration configuration)
        {
            var configDictionary = new Dictionary<String, String>();
            if (configuration.GetSetting(PROVIDER_SETTING_NAME).Value.Equals(SCRUM_FOR_TFS))
            {
                configDictionary["tfswi-remaining-field"] = "Conchango.TeamSystem.Scrum.WorkRemaining";
                configDictionary["tfswi-estimated-field"] = "Conchango.TeamSystem.Scrum.EstimatedEffort";
            }
            else
            {
                throw new ArgumentException("Unsupported Project Info Provider.");
            }

            var serverUrl = configuration.GetSetting(URL_SETTING_NAME).Value;
            var projectName = configuration.GetSetting(PROJECT_SETTING_NAME).Value;
            var username = configuration.GetSetting(USERNAME_SETTING_NAME).Value;
            var password = configuration.GetSetting(PASSWORD_SETTING_NAME).Value;

            return new ScrumForTFSRepository(serverUrl, projectName, username, password, configDictionary);
        }
    }
}
