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
                scenario.Given(There_is_a_configuration_in_the_configRepository).
                    And(All_harvesters_are_instantiated);
                scenario.When("name is checked");
                scenario.Then(All_of_the_tasks_have_overridden_the_default_name);
            });
        }
        
        protected Context All_harvesters_are_instantiated = () =>
        {
            var changesetDbRepo = new Mock<IRepository<Changeset>>().Object;
            var configMock = new Mock<TaskConfiguration>();
            configMock.Setup((c) => c.Entries).Returns(new List<TaskConfigurationEntry> {null, null, null, null,null});
            var config = configMock.Object;
            var gitConfigWithFiveEntries = new TaskConfiguration {Entries = new List<TaskConfigurationEntry>() {null, null, null, null, null,null}};
            var changesetDbPersister = new Mock<IPersistDomainModels<Changeset>>().Object;
            tasks = new ChangesetHarvesterBase[]
            {
                new SVNChangesetHarvesterTask(changesetDbRepo, changesetDbPersister, config),
                new TFSChangesetHarvesterTask(changesetDbRepo, changesetDbPersister, config),
                new GitChangesetHarvesterTask(changesetDbRepo, changesetDbPersister, gitConfigWithFiveEntries)
            };
        };

        protected Context There_is_a_configuration_in_the_configRepository = () =>
        {
            configRepoMock.Setup(c => c.Get(It.IsAny<Specification<Configuration>>())).
                Returns(new List<Configuration> { new Configuration() });
        };

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
