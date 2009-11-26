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
using APD.DomainModel.NHMapping.Entities;
using APD.DomainModel.SourceControl;
using FluentNHibernate.Cfg.Db;

using NHibernate;

using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using APD.Integration.Database.DomainModel.Repositories;


namespace APD.IntegrationTests.Database.DomainModel.Repositories.GenericDomainModelPersisterSpecs
{
    public class TestPersister : GenericDatabaseRepository<Changeset>
    {
        public TestPersister(ISessionFactory sessionFactory)
            : base(sessionFactory) {}
    }

    [TestFixture]
    public class when_saving : Shared
    {
        [SetUp]
        public void Setup()
        {
            new ChangesetPersister(sessionFactory).Delete(new AllChangesetsSpecification());
        }

        [Test]
        public void should_save_domain_model_to_database()
        {
            TestPersister persister = new TestPersister(sessionFactory);
            persister.Save(new Changeset()
                           {
                               Revision = 1067,
                               Author = new Author() {Username = "fardin2"},
                               Comment = "testass333333",
                               Time = DateTime.Now
                           });

            ISessionFactory newSessionFactory = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);

            var result = DatabaseRetriever.GetDatamodelFromDatabase<Changeset>(newSessionFactory);

            result.Count.ShouldBe(1);
        }
    }
}