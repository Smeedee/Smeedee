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
using Smeedee.DomainModel.SourceControl;
using NHibernate;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using Smeedee.Integration.Database.DomainModel.Repositories;


namespace Smeedee.IntegrationTests.Database.DomainModel.Repositories.GenericDomainModelPersisterSpecs
{
    public class TestPersister : GenericDatabaseRepository<ChangesetServer>
    {
        public TestPersister(ISessionFactory sessionFactory)
            : base(sessionFactory) {}
    }

    [TestFixture][Category("IntegrationTest")]
    public class when_saving : Shared
    {
        [SetUp]
        public void Setup()
        {
            new ChangesetDatabaseRepository(sessionFactory).Delete(new AllChangesetsSpecification());
        }

        [Test]
        public void should_save_domain_model_to_database()
        {
            TestPersister persister = new TestPersister(sessionFactory);
            persister.Save(new ChangesetServer()
                           {
                               Name="Testname",
                               Url = "http://www.testurl.com"

                           });

            ISessionFactory newSessionFactory = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);

            var result = DatabaseRetriever.GetDatamodelFromDatabase<ChangesetServer>(newSessionFactory);

            result.Count.ShouldBe(1);
        }
    }
}