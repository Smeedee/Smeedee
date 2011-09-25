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
            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void Assure_projectName_has_correct_prefix()
            {
                var config = new TaskConfiguration();
                config.Entries.Add(new TaskConfigurationEntry() { Name = TFSChangesetHarvesterTask.USERNAME_SETTING_NAME, Value = "test", PersistableType = "System.String"});
                config.Entries.Add(new TaskConfigurationEntry() { Name = TFSChangesetHarvesterTask.PASSWORD_SETTING_NAME, Value = "test", PersistableType = "System.String" });
                config.Entries.Add(new TaskConfigurationEntry() { Name = TFSChangesetHarvesterTask.SOURCECONTROL_SERVER_NAME, Value = "test", PersistableType = "System.String" });
                config.Entries.Add(new TaskConfigurationEntry() { Name = TFSChangesetHarvesterTask.URL_SETTING_NAME, Value = "test", PersistableType = "System.String" });
                config.Entries.Add(new TaskConfigurationEntry() { Name = TFSChangesetHarvesterTask.PROJECT_SETTING_NAME, Value = "test", PersistableType = "System.String" });

                var harvester = new TFSChangesetHarvesterTask(
                    new Mock<IRepository<Changeset>>().Object,
                    new Mock<IPersistDomainModels<Changeset>>().Object,
                    config);

            }
        }
    }
}
