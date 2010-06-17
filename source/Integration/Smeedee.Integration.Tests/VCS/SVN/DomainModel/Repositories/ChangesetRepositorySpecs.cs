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
using System.Linq;
using Smeedee.DomainModel.SourceControl;
using Smeedee.Integration.VCS.SVN.DomainModel.Repositories;
using Smeedee.IntegrationTests.VCS.SVN.Learning;
using Moq;
using NUnit.Framework;
using SharpSvn;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using Smeedee.DomainModel.Framework;


namespace Smeedee.IntegrationTests.VCS.SVN.DomainModel.Repositories.ChangesetRepositorySpecs
{
    public class Shared : SharpSVNContext
    {
        protected static readonly long MAX_NUMBER_OF_REVISIONS_TO_RECEIVE = 50;
        protected static Collection<SvnLogEventArgs> changesetsLog;
        protected static IRepository<Changeset> repository;
        protected static IEnumerable<Changeset> resultset;

        protected When changesets_are_requested = () => { GetAllChangesets(); };
        protected Context the_repository_contain_changesets = () => { };
            
        protected static void GetAllChangesets()
        {
            resultset = repository.Get(new ChangesetsAfterRevisionSpecification(MinChangeset()));
        }

        protected void SetupSharedContext()
        {
            SetupContext();
            repositoryUrl = "https://agileprojectdashboard.org:8443/svn/CPMonitor";
            //repository = new ChangesetRepository(repositoryUrl);
            repository = new StaticRepositoryCache<Changeset>(new SVNChangesetRepository(repositoryUrl), 99000);
        }

        protected Collection<SvnLogEventArgs> QueryChangesetsLog()
        {
            if (changesetsLog == null)
            {
                Collection<SvnLogEventArgs> log;
                SvnLogArgs svnLogArgs = new SvnLogArgs();
                svnLogArgs.Range = new SvnRevisionRange(MinChangeset() + 1, long.MaxValue);
                svnClient.GetLog(new Uri(repositoryUrl), svnLogArgs, out log);
                changesetsLog = log;
            }

            return changesetsLog;
        }

        protected static long? minChangesetChache;
        protected static long MinChangeset()
        {
            if (minChangesetChache == null)
            {
                SvnInfoEventArgs a;
                svnClient.GetInfo(new Uri(repositoryUrl), out a);
                return a.LastChangeRevision - MAX_NUMBER_OF_REVISIONS_TO_RECEIVE;
            }
            return (long)minChangesetChache;
        }
    }

    [TestFixture]
    public class When_query_all_changesets : Shared
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            SetupSharedContext();
        }

        #endregion

        [Test]
        public void Assure_all_changesets_are_received()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_repository_contain_changesets);

                scenario.When("changesets are requested", () =>
                      GetAllChangesets());

                scenario.Then("assure all changesets are received", () =>
                    resultset.Count().ShouldBe(
                        QueryChangesetsLog().Count()));
            });
        }

        [Test]
        public void Assure_Author_is_loaded_into_the_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_repository_contain_changesets);

                scenario.When(changesets_are_requested);

                scenario.Then("assure Author is loaded into over 50% of the rows in the resultset", () =>
                {
                    IEnumerable<Changeset> q =
                        resultset.Where(c => c.Author != null && c.Author.Username != null);
                    double authorPercent = ( (double) q.Count() / resultset.Count() ) * 100;
                    authorPercent.ShouldBeGreaterThan(50);
                });
            });
        }

        [Test]
        public void Assure_LogMessage_is_loaded_into_the_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_repository_contain_changesets);

                scenario.When(changesets_are_requested);

                scenario.Then("assure Comment is loaded into over 50% of the rows in the resultset", () =>
                {
                    IEnumerable<Changeset> q = resultset.Where(c => c.Comment != null);
                    double logMessagePercent = ( (double) q.Count() / resultset.Count() ) * 100;
                    logMessagePercent.ShouldBeGreaterThan(50);
                });
            });
        }

        [Test]
        public void Assure_Revision_is_loaded_into_the_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_repository_contain_changesets);

                scenario.When(changesets_are_requested);

                scenario.Then("assure Revision is loaded into the resultset", () =>
                {
                    IEnumerable<Changeset> q = resultset.Where(c => c.Revision != 0);
                    double revisionPercent = ( (double) q.Count() / resultset.Count() ) * 100;
                    revisionPercent.ShouldBeGreaterThan(50);
                });
            });
        }

        [Test]
        public void Assure_Time_is_loaded_into_the_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(the_repository_contain_changesets);

                scenario.When(changesets_are_requested);

                scenario.Then("assure Time is loaded into all rows in resultset", () =>
                {
                    foreach (Changeset changeset in resultset)
                        changeset.Time.ShouldNotBe(DateTime.MinValue);
                });
            });
        }
    }

    [TestFixture]
    public class When_query_all_changesets_after_a_given_revision : Shared
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            SetupSharedContext();
        }

        #endregion

        [Test]
        public void Assure_changesets_are_received()
        {
            Scenario.StartNew(this, scenario =>
            {
                long revisionNr = QueryChangesetsLog().Take(QueryChangesetsLog().Count / 2).First().Revision;

                scenario.Given(the_repository_contain_changesets);

                scenario.When("all changesets after revision " + revisionNr + " are requested", () =>
                    resultset = repository.Get(new ChangesetsAfterRevisionSpecification(revisionNr)));

                scenario.Then("assure all changesets from revision " + revisionNr + " are received", () =>
                {
                    resultset.Count().ShouldBe(
                        QueryChangesetsLog().Where(r => r.Revision > revisionNr).Count());
                });
            });
        }
    }

    [TestFixture]
    public class When_query_a_users_changesets : Shared
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            SetupSharedContext();
        }

        #endregion

        [Test]
        public void Assure_changesets_are_received()
        {
            List<string> users = "goeran|Pete|ole_gunnar".Split(new[] {'|'}).ToList();
            long minChangest = MinChangeset();

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
                        Collection<SvnLogEventArgs> changesetsLog = QueryChangesetsLog();
                        IEnumerable<SvnLogEventArgs> usersChangetsLog =
                            changesetsLog.Where(l => l.Author == user);
                        resultset.Count().ShouldBe(usersChangetsLog.Count());
                        Console.WriteLine("\t{0} of {1} changesets received",
                                          usersChangetsLog.Count(),
                                          changesetsLog.Count);
                    });
                });
            });
        }
    }
}