using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Tasks.SourceControl;

namespace Smeedee.Tasks.Tests.SourceControl
{
    
    public class TFSChangesetHarvesterTaskSpecs
    {
        [TestFixture]
        public class when_spawning
        {

//            [Test]
//            public void Assure_projectName_has_correct_prefix_when_no_prefix_is_set()
//            {
//                var config = new TaskConfiguration();
//                config.Entries.Add(new TaskConfigurationEntry { Name = TFSChangesetHarvesterTask.USERNAME_SETTING_NAME, Value = "test", PersistableType = "System.String" });
//                config.Entries.Add(new TaskConfigurationEntry { Name = TFSChangesetHarvesterTask.PASSWORD_SETTING_NAME, Value = "test", PersistableType = "System.String" });
//                config.Entries.Add(new TaskConfigurationEntry { Name = TFSChangesetHarvesterTask.SOURCECONTROL_SERVER_NAME, Value = "test", PersistableType = "System.String" });
//                config.Entries.Add(new TaskConfigurationEntry { Name = TFSChangesetHarvesterTask.URL_SETTING_NAME, Value = "test", PersistableType = "System.String" });
//                config.Entries.Add(new TaskConfigurationEntry { Name = TFSChangesetHarvesterTask.PROJECT_SETTING_NAME, Value = "test", PersistableType = "System.String" });
//
//                var harvester = new TFSChangesetHarvesterTask(
//                    new Mock<IRepository<Changeset>>().Object,
//                    new Mock<IPersistDomainModels<Changeset>>().Object,
//                    config);
//                
//                Assert.IsTrue(harvester.ProjectSettingValue.Contains(TFSChangesetHarvesterTask.REQUIRED_PROJECT_NAME_PREFIX));
//            }
//
//            [Test]
//            public void Assure_projectName_has_correct_prefix_when_correct_prefix_is_set()
//            {
//                var config = new TaskConfiguration();
//                config.Entries.Add(new TaskConfigurationEntry { Name = TFSChangesetHarvesterTask.USERNAME_SETTING_NAME, Value = "test", PersistableType = "System.String" });
//                config.Entries.Add(new TaskConfigurationEntry { Name = TFSChangesetHarvesterTask.PASSWORD_SETTING_NAME, Value = "test", PersistableType = "System.String" });
//                config.Entries.Add(new TaskConfigurationEntry { Name = TFSChangesetHarvesterTask.SOURCECONTROL_SERVER_NAME, Value = "test", PersistableType = "System.String" });
//                config.Entries.Add(new TaskConfigurationEntry { Name = TFSChangesetHarvesterTask.URL_SETTING_NAME, Value = "test", PersistableType = "System.String" });
//                config.Entries.Add(new TaskConfigurationEntry { Name = TFSChangesetHarvesterTask.PROJECT_SETTING_NAME, Value = "$\\test", PersistableType = "System.String" });
//
//                var harvester = new TFSChangesetHarvesterTask(
//                    new Mock<IRepository<Changeset>>().Object,
//                    new Mock<IPersistDomainModels<Changeset>>().Object,
//                    config);
//
//                Assert.IsTrue(harvester.ProjectSettingValue.Contains(TFSChangesetHarvesterTask.REQUIRED_PROJECT_NAME_PREFIX));
//            }
        }
    }
}
