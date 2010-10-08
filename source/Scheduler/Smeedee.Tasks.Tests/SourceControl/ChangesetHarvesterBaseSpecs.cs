#region File header
// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.SourceControl;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Tasks.Tests.SourceControl
{
    [TestFixture]
    public class ChangesetHarvesterBaseSpecs
    {
        protected static ChangesetHarvesterBase SourceControlTask;
        protected static Mock<IRepository<Changeset>> changesetRepository;
        protected static Mock<IRepository<Changeset>> changesetDbRepository;
        protected static TaskConfiguration config;
        protected static Mock<IPersistDomainModels<Changeset>> database;

        protected static List<Changeset> savedChangesets;


        protected static Changeset oldChangeset = new Changeset
                                                   {
                                                       Author = new Author {Username = "testUser"},
                                                       Comment = "testComment",
                                                       Revision = 10,
                                                       Time = DateTime.Now.AddDays(-14)
                                                   };
        protected static Changeset notAsOldChangeset = new Changeset
        {
            Author = new Author { Username = "testUser" },
            Comment = "This is a comment",
            Revision = 11,
            Time = DateTime.Now.AddDays(-12)
        };

        protected static Changeset newChangeset = new Changeset
        {
            Author = new Author { Username = "testUser" },
            Comment = "A fresh changeset",
            Revision = 18,
            Time = DateTime.Now.AddDays(-1)
        };

        [Test]
        public void should_be_instantiated()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(sourceControlTask_is_created);
                scenario.When("instance is accessed");
                scenario.Then("instance exists", () => SourceControlTask.ShouldNotBeNull());
            });
        }


        [Test]
        public void should_be_instance_of_TaskBase()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(sourceControlTask_is_created);
                scenario.When("instance is accessed");
                scenario.Then("the Task is an instance of TaskBase", () =>
                    SourceControlTask.ShouldBeInstanceOfType<TaskBase>());
            });
        }

        [Test]
        public void assure_changesets_are_saved_even_if_database_is_empty()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(sourceControlTask_is_created).
                    And(no_changesets_exists_in_the_database).
                    And(there_is_a_changeset_in_repository);
                scenario.When(sourceControlTask_is_dispatched);
                scenario.Then("the list of changesets is saved to database", () =>
                {
                    int mocked = changesetRepository.Object.Get(new AllChangesetsSpecification()).Count();
                    savedChangesets.Count.ShouldBe(mocked);
                });
            });
        }


        [Test]
        public void assure_changesets_are_NOT_saved_if_revision_has_not_changed()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(sourceControlTask_is_created).
                    And(changeset_have_the_same_revision_in_repository_and_db);
                scenario.When(sourceControlTask_is_dispatched);
                scenario.Then("the list of changesets is not saved to database", () =>
                {
                    database.Verify(d => d.Save(It.IsAny<Changeset>()), Times.Never());
                });
            });
        }

        [Test]
        public void Assure_changeset_server_is_set_from_task_configuration()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(sourceControlTask_is_created).
                    And(changesets_have_different_revision_in_repository_than_what_is_saved_in_db);
                scenario.When(sourceControlTask_is_dispatched);
                scenario.Then("the changesets have the correct server values from the config", () =>
                {
                    database.Verify(d => d.Save(It.Is<IEnumerable<Changeset>>(cs => cs.All(c => c.Server.Url == changesetServerUrl))));
                });
            });
        }

        [Test]
        public void assure_changesets_are_saved_if_revision_has_changed()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(sourceControlTask_is_created).
                    And(changesets_have_different_revision_in_repository_than_what_is_saved_in_db);
                scenario.When(sourceControlTask_is_dispatched);
                scenario.Then("the list of changesets is saved to database", () =>
                {
                    database.Verify(d => d.Save(It.IsAny<IEnumerable<Changeset>>()), Times.Once());
                });
            });
        }


        [Test]
        public void assure_only_newer_changesets_are_saved()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(sourceControlTask_is_created).
                    And(there_are_several_changesets_in_database).
                    And(there_are_several_changesets_in_repository);
                scenario.When(sourceControlTask_is_dispatched);
                scenario.Then("only changesets with a newer revision than the ones in the db are saved", () =>
                {
                    savedChangesets.Contains(newChangeset).ShouldBeTrue();
                    savedChangesets.Contains(oldChangeset).ShouldBeFalse();
                    savedChangesets.Contains(notAsOldChangeset).ShouldBeFalse();
                });
            });
        }

        #region Test contexts
        
        protected Context sourceControlTask_is_created = () =>
        {
            var changesets = new List<Changeset>();
            changesetServerUrl = "http://www.testurl.com";
            changesetServerName = "My main server for sourcecontrol";

            changesetRepository = new Mock<IRepository<Changeset>>();
            changesetDbRepository = new Mock<IRepository<Changeset>>();
            database = new Mock<IPersistDomainModels<Changeset>>();

            config = new TaskConfiguration();
            config.Entries.Add(new TaskConfigurationEntry()
                                   {Name = ChangesetHarvesterBase.URL_SETTING_NAME, Value = changesetServerUrl, Type = typeof(string)});
            config.Entries.Add(new TaskConfigurationEntry()
                                   {Name = ChangesetHarvesterBase.SOURCECONTROL_SERVER_NAME, Value = changesetServerName, Type = typeof(string)});
            

            changesetRepository.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(changesets);
            changesetDbRepository.Setup(r => r.Get(It.IsAny<Specification<Changeset>>())).Returns(changesets);

            
            savedChangesets = new List<Changeset>();

            database.Setup(d => d.Save(It.IsAny<Changeset>())).Callback(
                        (Changeset changesetToBeSaved) => savedChangesets.Add(changesetToBeSaved));

            database.Setup(d => d.Save(It.IsAny<IEnumerable<Changeset>>())).Callback(
                        (IEnumerable<Changeset> changesetsToBeSaved) => savedChangesets.AddRange(changesetsToBeSaved));

            SourceControlTask = new MockHarvesterTask(changesetDbRepository.Object,
                                                     database.Object,
                                                     changesetRepository.Object,
                                                     config);
        };

        private class MockHarvesterTask : ChangesetHarvesterBase
        {
            private IRepository<Changeset> changesetRepositoryMock;
            public MockHarvesterTask(IRepository<Changeset> changesetDbRepository,
                                IPersistDomainModels<Changeset> databasePersister,
                                IRepository<Changeset> changesetRepositoryMock, 
                                TaskConfiguration config)
                : base(changesetDbRepository, databasePersister, config)
            {
                this.changesetRepositoryMock = changesetRepositoryMock;
            }

            public override void Execute()
            {
                SaveUnsavedChangesets(changesetRepositoryMock);
            }
        }

        private static void LoadChangesetsIntoRepository(Mock<IRepository<Changeset>> repository, List<Changeset> changesets)
        {
            repository.Setup(p => p.Get(It.IsAny<Specification<Changeset>>())).
                Returns((Specification<Changeset> specs) => FilterChangesetsBySpesification(changesets, specs));
        }

        private static IEnumerable<Changeset> FilterChangesetsBySpesification(List<Changeset> changesets, Specification<Changeset> specification)
        {
            return changesets.Where(c => specification.IsSatisfiedBy(c));
        }


        protected Context no_changesets_exists_in_the_database = () =>
            LoadChangesetsIntoRepository(changesetDbRepository, new List<Changeset>());


        protected Context there_is_a_changeset_in_repository = () => 
            LoadChangesetsIntoRepository(changesetRepository, new List<Changeset> { oldChangeset });


        protected Context there_is_a_changeset_in_the_database = () =>
            LoadChangesetsIntoRepository(changesetDbRepository, new List<Changeset> { oldChangeset });


        protected Context there_are_several_changesets_in_database = () =>
                    LoadChangesetsIntoRepository(changesetDbRepository, new List<Changeset> { notAsOldChangeset, oldChangeset});

        protected Context there_are_several_changesets_in_repository = () =>
            LoadChangesetsIntoRepository(changesetRepository, new List<Changeset> { newChangeset, notAsOldChangeset, oldChangeset });


        protected Context duplicate_changesets_in_repository = () => 
            LoadChangesetsIntoRepository(changesetRepository, new List<Changeset> {oldChangeset, oldChangeset});


        protected Context changeset_have_the_same_revision_in_repository_and_db = () =>
        {
            var sharedChangesetList = new List<Changeset> { oldChangeset };
            LoadChangesetsIntoRepository(changesetDbRepository, sharedChangesetList);
            LoadChangesetsIntoRepository(changesetRepository, sharedChangesetList);
        };
       

        protected Context changesets_have_different_revision_in_repository_than_what_is_saved_in_db = () =>
        {
            var oldChangesetList = new List<Changeset> { oldChangeset };
            var newChangesetList = new List<Changeset>
                                   {
                                       new Changeset
                                       {
                                           Author = new Author {Username = "testUser"},
                                           Comment = "testComment",
                                           Revision = 123456789,
                                           Time = DateTime.Now
                                       }
                                   };
            LoadChangesetsIntoRepository(changesetDbRepository, oldChangesetList);
            LoadChangesetsIntoRepository(changesetRepository, newChangesetList);
        };
        #endregion


        protected When sourceControlTask_is_dispatched = () => SourceControlTask.Execute();


        private Context empty_configuration_is_given = () =>
        {
            config = new TaskConfiguration();
        };

        private static string changesetServerUrl;
        private static string changesetServerName;
    }
}
