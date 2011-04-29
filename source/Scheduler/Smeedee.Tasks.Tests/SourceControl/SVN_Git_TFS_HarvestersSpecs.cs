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
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.SourceControl;
using Smeedee.Tasks.SourceControl.Git;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Tasks.Tests.SourceControl
{  
    [TestFixture]
    class SVN_Git_TFS_HarvestersSpecs
    {
        private static SVNChangesetHarvesterTask task;
        private static Mock<IRepository<Configuration>> configRepoMock = new Mock<IRepository<Configuration>>();
        private static ChangesetHarvesterBase[] tasks;
        
        [Test]
        public void Git_SVN_and_TFS_tasks_should_override_default_name()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(All_harvesters_are_instantiated);
                scenario.When("name is checked");
                scenario.Then(All_of_the_tasks_have_overridden_the_default_name);
            });
        }
        
        protected Context All_harvesters_are_instantiated = () =>
        {
            var changesetDbRepo = new Mock<IRepository<Changeset>>().Object;
            var config = new TaskConfiguration {Entries = CreateTaskConfigurationEntries()};
            var gitConfigWithSevenEntries = new TaskConfiguration {Entries = new List<TaskConfigurationEntry>() {null, null, null, null, null,null, null}};
            var changesetDbPersister = new Mock<IPersistDomainModels<Changeset>>().Object;
            tasks = new ChangesetHarvesterBase[]
            {
                new SVNChangesetHarvesterTask(changesetDbRepo, changesetDbPersister, config),
                new TFSChangesetHarvesterTask(changesetDbRepo, changesetDbPersister, config),
                new GitChangesetHarvesterTask(changesetDbRepo, changesetDbPersister, gitConfigWithSevenEntries)
            };
        };

        private static List<TaskConfigurationEntry> CreateTaskConfigurationEntries()
        {
            return new List<TaskConfigurationEntry>
                       {
                           new TaskConfigurationEntry(){Name = TFSChangesetHarvesterTask.PROJECT_SETTING_NAME, Value = "$\\testProject", Type = typeof(string)},
                           new TaskConfigurationEntry(){Name = TFSChangesetHarvesterTask.USERNAME_SETTING_NAME, Value = "test", Type = typeof(string)},
                           new TaskConfigurationEntry(){Name = TFSChangesetHarvesterTask.PASSWORD_SETTING_NAME, Value = "pass", Type = typeof(string)},
                           new TaskConfigurationEntry(){Name = TFSChangesetHarvesterTask.URL_SETTING_NAME, Value = "http://uri", Type = typeof(string)},
                           new TaskConfigurationEntry(){Name = TFSChangesetHarvesterTask.SOURCECONTROL_SERVER_NAME, Value = "myName", Type = typeof(string)}
                       };
        }

        private class DummyTask : TaskBase
        {
            public override void Execute()
            {
                throw new NotImplementedException();
            }
        }

        protected Then All_of_the_tasks_have_overridden_the_default_name = () =>
        {
            var defaultTask = new DummyTask();
            string defaultName = defaultTask.Name;
            tasks.Where(t => t.Name == defaultName).Count().ShouldBe(0);
        };
    }
}
