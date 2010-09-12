using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using NHibernate;

using NUnit.Framework;
using Smeedee.DomainModel.CI;
using NHibernate.Cfg;

using TinyBDD.Specification.NUnit;
using Smeedee.DomainModel.Framework;
using Smeedee.Integration.Database.DomainModel.Repositories;
using System.IO;

using Environment=NHibernate.Cfg.Environment;


namespace Smeedee.IntegrationTests.Database.DomainModel.Repositories.GenericDatabaseRepositorySpecs
{
    public class Shared 
    {
        protected static ISessionFactory CreateSessionFactory()
        {
            Configuration configuration = new Configuration();
            configuration.Configure();
            configuration.AddAssembly(typeof(CIServer).Assembly);
            return configuration.BuildSessionFactory();
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_spawning : Shared
    {
        [Test]
        public void Assure_it_accept_a_ISessionFactory_as_argument()
        {
            var genericRepository = new CIServerDatabaseRepository(CreateSessionFactory());
        }

        [Test]
        public void Assure_ISessionFactory_arg_is_validated()
        {
            this.ShouldThrowException<ArgumentNullException>(() =>
                new CIServerDatabaseRepository(null), exception =>
                    exception.Message.ShouldBe("Value cannot be null.\r\nParameter name: sessionFactory"));
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_spawned : Shared
    {
        private GenericDatabaseRepository<CIServer> repository;

        [SetUp]
        public void Setup()
        {
            repository = new CIServerDatabaseRepository(CreateSessionFactory());
        }

        [Test]
        public void Assure_its_a_IRepository()
        {
            (repository is IRepository<CIServer>).ShouldBeTrue();
        }

        [Test]
        public void Assure_its_a_IPersistDomainModels()
        {
            (repository is IPersistDomainModels<CIServer>).ShouldBeTrue();
        }
    }
}
