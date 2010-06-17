using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using NHibernate.Cfg;
using Smeedee.DomainModel.CI;
using NHibernate;

using TinyBDD.Specification.NUnit;
using NHibernate.Tool.hbm2ddl;


namespace Smeedee.IntegrationTests.Database.Learning
{
    [TestFixture]
    [Ignore]
    public class NHibernateLearningTests
    {
        private ISessionFactory sessionFactory;
        private ISession session;

        [SetUp]
        public void Setup()
        {
            var configuration = new Configuration();
            configuration.Configure();
            configuration.AddAssembly(typeof(CIServer).Assembly);

            new SchemaExport(configuration).Execute(false, true, false);

            sessionFactory = configuration.BuildSessionFactory();
            session = sessionFactory.OpenSession();
        }

        [Test]
        public void Assure_SessionFactory_is_built()
        {
            sessionFactory.ShouldNotBeNull();
        }

        [Test]
        public void How_to_use_xml_configuration()
        {
            var configuration = new Configuration();
            configuration.Configure();
            configuration.AddAssembly(typeof (CIServer).Assembly);
        }

     
        [Test]
        public void Use_xml_configuration_to_persist_objects()
        {
            var server = new CIServer("cc.net", "http://smeedee.net");
            server.AddProject(new CIProject("development")
            {
                SystemId = "haldis"
            });
            server.AddProject(new CIProject("main")
            {
                SystemId = "Haldis2"
            });
            session.Save(server);
            session.Flush();
        }
    }
}
