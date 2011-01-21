using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
    [TaskSetting(3, TFSProjectInfoSettingsConstants.URL_SETTING_NAME, typeof(Uri), "", "Include protocols such as http://\nand port numbers such as :8080")]
    [TaskSetting(4, TFSProjectInfoSettingsConstants.PROJECT_SETTING_NAME, typeof(string), "")]
    [TaskSetting(5, TFSProjectInfoSettingsConstants.REMAINING_SETTING_NAME, typeof(string), TFSProjectInfoSettingsConstants.REMAINING_FIELD, "If you are using a custom template you can edit this value to tell Smeedee what work item field it should retrieve 'time remaining' information for a task from.")]
    [TaskSetting(6, TFSProjectInfoSettingsConstants.ESTIMATED_SETTING_NAME, typeof(string), TFSProjectInfoSettingsConstants.ESTIMATED_FIELD, "If you are using a custom template you can edit this value to tell smeedee what work item field it should retrieve 'estimated time' information for a task from.")]
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
            var remainingField = (string) config.ReadEntryValue(TFSProjectInfoSettingsConstants.REMAINING_SETTING_NAME);
            var estimatedField = (string) config.ReadEntryValue(TFSProjectInfoSettingsConstants.ESTIMATED_SETTING_NAME);

            var configDictionary = new Dictionary<String, String>();
            configDictionary["tfswi-remaining-field"] = (remainingField != "")
                                                            ? remainingField
                                                            : "Work Remaining (Scrum v3)";
            configDictionary["tfswi-estimated-field"] = (estimatedField != "")
                                                            ? estimatedField
                                                            : "Estimated Effort (Scrum v3)";


            var serverUrl = (string)config.ReadEntryValue(TFSProjectInfoSettingsConstants.URL_SETTING_NAME);
            var projectName = (string)config.ReadEntryValue(TFSProjectInfoSettingsConstants.PROJECT_SETTING_NAME);
            var username = (string)config.ReadEntryValue(TFSProjectInfoSettingsConstants.USERNAME_SETTING_NAME);
            var password = (string)config.ReadEntryValue(TFSProjectInfoSettingsConstants.PASSWORD_SETTING_NAME);
			string domain = username.Contains('\\') ? username.Substring(0, username.IndexOf('\\')) : null;
			username = username.Contains('\\') ? username.Substring(username.IndexOf('\\') + 1) : username;
			var cred = string.IsNullOrEmpty(domain) ? new NetworkCredential(username, password) : new NetworkCredential(username, password, domain);


            return new ScrumForTFSRepository(serverUrl, projectName, configDictionary, cred);
        }
    }
}