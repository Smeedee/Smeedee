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

using System.Collections.Generic;
using System.Threading;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.Integration.Database.DomainModel.Repositories;
using Smeedee.Scheduler;
using Smeedee.Tasks.CI;
using Smeedee.Tasks.Framework;
using Moq;
using NUnit.Framework;
using Smeedee.Tasks.SourceControl;
using TinyBDD.Specification.NUnit;
using NHibernate;


namespace Smeedee.SchedulerTests.DataCollectorThreadingTest
{
    [TestFixture]
    public class DataCollectorThreadingTest
    {
        private static readonly string DATABASE_TEST_FILE = "smeedeeTestDB.db";

        private IRepository<Changeset> changesetRepository;
        private Mock<IRepository<Changeset>> changesetRepositoryMock;
        private Mock<IRepository<Configuration>> configRepositoryMock;

        [SetUp]
        public void Setup()
        {
            
        }

        /// <summary>
        /// This is a bad test. Should use mock objects for the ChangesetRepositories
        /// </summary>
        [Test]
        [Ignore]
        public void Assure_two_harversters_can_use_same_session_factory_and_avoid_concurrency_issues()
        {
            var hasFailed = false;
            var log = new ConsoleLogger();
            var harvesterScheduler = new Scheduler.Scheduler(log);

            harvesterScheduler.OnFailedDispatch += () =>
                hasFailed = true;

            ISessionFactory sesFact = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);

            var csDatabase = new ChangesetDatabaseRepository(sesFact);
            var csPersister = new ChangesetDatabaseRepository(sesFact);
            var repositoryFactory = new ChangesetRepositoryFactory();
            var csHarvester = new SourceControlTask(csDatabase, configRepositoryMock.Object, csPersister, repositoryFactory);

            var ciPersister = new CIServerDatabaseRepository(sesFact);
            var ciRepositoryFactory = new CIServerRepositoryFactory();
            var ciHarvester = new CITask(ciRepositoryFactory, ciPersister, null);


            harvesterScheduler.RegisterTasks(new List<TaskBase> { csHarvester, ciHarvester});

            Thread.Sleep(10000); // Must exist to allow harversters to complete.

            hasFailed.ShouldBe(false);
        }
    }
}
