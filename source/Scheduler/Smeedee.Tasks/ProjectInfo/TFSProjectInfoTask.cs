using System;
using System.Collections.Generic;
using System.Linq;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Framework;
using Smeedee.Integration.PMT.ScrumForTFS.DomainModel.Repositories;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.Factories;
using Smeedee.Tasks.Framework.TaskAttributes;

namespace Smeedee.Tasks.ProjectInfo
{
    [Task("TFS Project Info Harvester",
                         Author = "Smeedee Team",
                         Description = "Retrieves information from Team Foundation's Work Item functionality. Used to populate Smeedee's database with project related data such as burndown data and sprint dates.",
                         Version = 1,
                         Webpage = "http://smeedee.org")]
    [TaskSetting(1, TFSProjectInfoSettingsConstants.USERNAME_SETTING_NAME, typeof(string), "")]
    [TaskSetting(2, TFSProjectInfoSettingsConstants.PASSWORD_SETTING_NAME, typeof(string), "")]
    [TaskSetting(3, TFSProjectInfoSettingsConstants.URL_SETTING_NAME, typeof(Uri), "")]
    [TaskSetting(4, TFSProjectInfoSettingsConstants.PROJECT_SETTING_NAME, typeof(string), "")]
    [TaskSetting(5, TFSProjectInfoSettingsConstants.PROVIDER_SETTING_NAME, typeof(string), TFSProjectInfoSettingsConstants.SCRUM_FOR_TFS)]
    public class TFSProjectInfoTask : TaskBase
    {

        private readonly IPersistDomainModels<ProjectInfoServer> databasePersister;
        private readonly IAssembleRepositoryForTasks<ProjectInfoServer> repositoryFactory;
        private readonly TaskConfiguration config;

        public override string Name
        {
            get { return "TFS Project Information Harvester"; }
        }

        public TFSProjectInfoTask(IPersistDomainModels<ProjectInfoServer> databasePersister,
                                  TaskConfiguration config)
        {
            Guard.Requires<ArgumentNullException>(databasePersister != null);
            Guard.Requires<ArgumentNullException>(config != null);
            Guard.Requires<TaskConfigurationException>(config.Entries.Count() >= 5);

            this.databasePersister = databasePersister;
            this.config = config;
            Interval = TimeSpan.FromMilliseconds(config.DispatchInterval);
        }

        public override void Execute()
        {
            var sourceRepository = Assemble(config);
            var data = sourceRepository.Get(new AllSpecification<ProjectInfoServer>());

            databasePersister.Save(data);
        }

        protected virtual IRepository<ProjectInfoServer> Assemble(TaskConfiguration config)
        {
            var provider = (string)config.ReadEntryValue(TFSProjectInfoSettingsConstants.PROVIDER_SETTING_NAME);
            if (provider != TFSProjectInfoSettingsConstants.SCRUM_FOR_TFS)
                throw new ArgumentException("Unsupported Project Info Provider.");

            var configDictionary = new Dictionary<String, String>();
            configDictionary["tfswi-remaining-field"] = "Work Remaining (Scrum v3)";
            configDictionary["tfswi-estimated-field"] = "Estimated Effort (Scrum v3)";

            var serverUrl = (string)config.ReadEntryValue(TFSProjectInfoSettingsConstants.URL_SETTING_NAME);
            var projectName = (string)config.ReadEntryValue(TFSProjectInfoSettingsConstants.PROJECT_SETTING_NAME);
            var username = (string)config.ReadEntryValue(TFSProjectInfoSettingsConstants.USERNAME_SETTING_NAME);
            var password = (string)config.ReadEntryValue(TFSProjectInfoSettingsConstants.PASSWORD_SETTING_NAME);

            return new ScrumForTFSRepository(serverUrl, projectName, username, password, configDictionary);
        }
    }
}