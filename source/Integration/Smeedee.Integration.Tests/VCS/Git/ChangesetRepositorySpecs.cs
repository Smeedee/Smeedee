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
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.Integration.VCS.Git.DomainModel;

using GitSharp;
using NUnit.Framework;
using Smeedee.Integration.VCS.Git.DomainModel.Repositories;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using Author = GitSharp.Author;
using System.Linq;

namespace Smeedee.IntegrationTests.VCS.Git
{
    public class GitSharpContext
    {
        protected static Repository _repo;
        protected static string _localDirectory;
        protected static Author _nicolai;

        protected void SetupContext()
        {
            _localDirectory = "C:\\Code\\CodeKataExercises\\";
            _repo = new Repository(_localDirectory);
        }

        protected static void CleanRepository()
        {
            foreach (var filepath in _repo.Index.Entries) _repo.Index.Delete(_localDirectory + filepath);
            _repo.Commit("Repository cleaned.", _nicolai);
        }

        protected long GetLastRevision()
        {
            var latestChangeset = GetLatestChangeset();
            return latestChangeset.Ancestors.Count() + 1;
        }

        protected Commit GetLatestChangeset()
        {
            Commit latest = _repo.Get<Commit>("HEAD");
            return latest;
        }

        protected static string FilepathInLocalDirectory(string filename)
        {
            var path = Path.Combine(_localDirectory, filename);
            return path;
        }

        protected static void CreateFileInLocalDirectory(string filepath)
        {
            using (var sw = new StreamWriter(filepath)) { }
            ;
        }
    }//end GitSharpContext

    public class ChangesetRepositorySpecs : GitSharpContext
    {
        protected static readonly long MAX_NUMBER_OF_REVISIONS_TO_RECEIVE = 50;
        protected static Collection<Commit> changesetsLog;
        protected static GitChangesetRepository repository;
        protected static IEnumerable<Changeset> resultset;
        protected static Commit _head;

        protected When changesets_are_requested = () => { GetAllChangesets(); };
        protected Context the_repository_contain_changesets = () => { };

        protected static void GetAllChangesets()
        {
            resultset = repository.Get(new ChangesetsAfterRevisionSpecification(min_changeset()));
        }

        protected void SetupSharedContext()
        {
            SetupContext();
            //repository = new StaticRepositoryCache<Changeset>(new GitChangesetRepository(_repositoryUrl), 99000);
            repository = new GitChangesetRepository(_localDirectory);   
        }

        protected Collection<Commit> QueryChangesetsLog()
        {
            _head = _repo.Get<Commit>("HEAD");
            if (changesetsLog == null)
            {
                Collection<Commit> log = new Collection<Commit>();

                foreach (Commit c in _head.Ancestors)
                {
                    log.Add(c);
                }

                changesetsLog = log;
            }
            return changesetsLog;
        }

        protected static long? _minChangesetChache;
        protected static long min_changeset()
        {
            if (_minChangesetChache == null)
            {
                _head = _repo.Get<Commit>("HEAD");
                return (_head.Ancestors.Count() + 1) - MAX_NUMBER_OF_REVISIONS_TO_RECEIVE;
            }
            return (long)_minChangesetChache;
        }
    }

    [Ignore]
    [TestFixture][Category("IntegrationTest")]
    public class When_query_all_changesets : ChangesetRepositorySpecs
    {
        #region Setup/Teardown
        [SetUp]
        public void Setup()
        {
            SetupSharedContext();
        }
        #endregion


        [Test]
        public void assure_all_changesets_are_received()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_repository_contain_changesets);
                scenario.When("changesets are requested", GetAllChangesets);
                scenario.Then("assure all changesets are received", () =>
                {
                    resultset.Count().ShouldBe(QueryChangesetsLog().Count());
                    Console.WriteLine("Changelog: ");
                    foreach (Changeset c in resultset)
                    {
                        Console.WriteLine(c.ToString());
                    }
                });
            });
        }

        [Test]
        public void assure_author_is_loaded_into_the_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_repository_contain_changesets);
                scenario.When(changesets_are_requested);
                scenario.Then("assure Author is loaded into all rows in the resultset", () =>
                {
                    IEnumerable<Changeset> q =
                        resultset.Where(c => c.Author != null && c.Author.Username != null);
                    q.Count().ShouldBe(resultset.Count());
                });
            });
        }

        [Test]
        public void assure_log_message_is_loaded_into_the_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_repository_contain_changesets);
                scenario.When(changesets_are_requested);
                scenario.Then("assure Comment is loaded into all rows of the result set", () =>
                {
                    IEnumerable<Changeset> q = resultset.Where(c => c.Comment != null);
                    q.Count().ShouldBe(resultset.Count());
                });
            });
        }

        [Test]
        public void assure_time_is_loaded_into_the_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_repository_contain_changesets);
                scenario.When(changesets_are_requested);
                scenario.Then("assure Time is loaded into all rows in _resultset", () =>
                {
                    foreach (Changeset changeset in resultset)
                        changeset.Time.ShouldNotBe(DateTime.MinValue);
                });
            });
        }

        [Test]
        public void assure_revision_is_loaded_into_the_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_repository_contain_changesets);
                scenario.When(changesets_are_requested);
                scenario.Then("assure revision is loaded into all rows in _resultset", () =>
                {
                    foreach (Changeset changeset in resultset)
                        changeset.Revision.ShouldNotBe(0);
                });
            });
        }
 
    }
    [Ignore]
    [TestFixture][Category("IntegrationTest")]
    public class When_query_a_users_changesets : ChangesetRepositorySpecs
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            SetupSharedContext();
        }



        #endregion

        [Test]
        public void assure_changesets_are_received()
        {
            var users = "unknown|Martin Altin|Christian Jonassen".Split(new[] { '|' }).ToList();
            long minChangest = min_changeset();

            users.ForEach(user =>
            {
                Scenario.StartNew(this, scenario =>
                {
                    scenario.Given(the_repository_contain_changesets);

                    scenario.When("all changesets for '" + user + "' are requested", () =>
                        resultset = repository.Get(new AndExpressionSpecification<Changeset>(
                            new ChangesetsForUserSpecification(user),
                            new ChangesetsAfterRevisionSpecification(minChangest))));

                    scenario.Then("assure all changesets for '" + user + "' are received", () =>
                    {
                        Collection<Commit> changesetsLog = QueryChangesetsLog();
                        IEnumerable<Commit> usersChangetsLog =
                            changesetsLog.Where(l => l.Author.Name == user);
                        resultset.Count().ShouldBe(usersChangetsLog.Count());
                    });
                });
            });
        }

    }
}


