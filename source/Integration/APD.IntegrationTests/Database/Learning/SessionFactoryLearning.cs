using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using APD.DomainModel.NHMapping.Entities;
using APD.DomainModel.SourceControl;
using APD.Integration.Database.DomainModel.Repositories;
using APD.IntegrationTests.Database.DomainModel.Repositories.ChangesetPersisterSpecs;

using FluentNHibernate.Cfg.Db;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;
using APD.IntegrationTests.Database.DomainModel.Repositories;


namespace APD.IntegrationTests.Database.Learning
{
    [TestFixture]
    public class SessionFactoryLearning : Shared
    {
        protected const string DATABASE_TEST_FILE = "TestDB.db";
        [Test]
        public void database_should_not_be_flushed_when_new_sessionfactory_is_created()
        {
            DeleteDatabaseIfExists();

            sessionFactory = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);

            ChangesetPersister persister = new ChangesetPersister(sessionFactory);
            persister.Save(new Changeset()
            {
                Revision = 1067,
                Author = new Author() { Username = "fardin2" },
                Comment = "testass333333",
                Time = DateTime.Now
            });

            sessionFactory = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);
            var changesets = DatabaseRetriever.GetDatamodelFromDatabase<Changeset>(sessionFactory);

            changesets.Count.ShouldBe(1);

        }

        private void DeleteDatabaseIfExists()
        {
            File.Delete(DATABASE_TEST_FILE);
        }
    }
}
