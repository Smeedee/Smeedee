//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using APD.DomainModel.CI;
//using APD.DomainModel.Framework;
//using APD.Harvester.CI;
//using APD.HarvesterTests.CI.CIHarvesterSpecs;

//using Moq;

//using NUnit.Framework;

//using TinyBDD.Dsl.GivenWhenThen;
//using TinyBDD.Specification.NUnit;
//using APD.Plugin.Database;
//using NHibernate;
//using System.IO;


//namespace APD.Harvester.CITests.Integration.CIHarvesterSpecs
//{
//    public class SharedForIntegration : Shared
//    {
//        protected static CIHarvester harvester;
//        protected static List<CIServer> sourceData;
//        protected static ISession dbSession;

//        protected Context CIHarvester_is_created = () =>
//        {
//            var databaseFile = "SmeedeeDB_test.db";

//            if (File.Exists(databaseFile))
//                File.Delete(databaseFile);

//            var sessionFactory = NHibernateFactory.AssembleSessionFactory(databaseFile);
//            dbSession = sessionFactory.OpenSession();
            
//            sourceRepositoryMock = new Mock<IRepository<CIServer>>();
//            harvester = new CIHarvester(sourceRepositoryMock.Object, new GenericDatabaseRepository<CIServer>(sessionFactory));
//        };

//        protected Context soure_contains_data = () =>
//        {
//            sourceData = new List<CIServer>();
//            sourceData.Add(new CIServer("CC.net", "http://smeedee.org/ccnet"));

//            sourceRepositoryMock.Setup(p => p.Get(It.IsAny<AllSpecification<CIServer>>())).Returns(sourceData);
//        };

//        protected Context source_contains_a_Project = () =>
//        {
//            sourceData.First().AddProject(new CIProject("development baseline")
//            {
//                SystemId = "development baseline"
//            });
//        };

//        protected Context harvester_already_been_dispatched = () =>
//        {
//            harvester.DispatchDataHarvesting();
//        };

//        protected When harvester_is_dispatched = () =>
//        {
//            harvester.DispatchDataHarvesting();
//        };
//    }

//    [TestFixture]
//    public class when_dispatched_for_the_first_time : SharedForIntegration
//    {
//        [Test]
//        public void Assure_Servers_are_persisted_in_database()
//        {
//            Scenario.StartNew(this, scenario =>
//            {
//                scenario.Given(CIHarvester_is_created).
//                    And(soure_contains_data);
//                scenario.When(harvester_is_dispatched);
//                scenario.Then("assure Servers are persisted in database", () =>
//                {
//                    dbSession.CreateCriteria(typeof(CIServer)).List().Count.ShouldBe(1);
//                });
//            });
//        }

//        [Test]
//        public void Assure_Projects_are_persisted_in_database()
//        {
//            Scenario.StartNew(this, scenario =>
//            {
//                scenario.Given(CIHarvester_is_created).
//                    And(soure_contains_data).
//                    And(source_contains_a_Project);

//                scenario.When(harvester_is_dispatched);
//                scenario.Then("assure Projects are persisted in database", () =>
//                    dbSession.CreateCriteria(typeof(CIProject)).List().Count.ShouldBe(1));
//            });
//        }
//    }

//    [TestFixture]
//    public class When_dispatched_for_the_nth_time : SharedForIntegration
//    {
//        [Test]
//        public void Assure_existing_Servers_are_not_added_as_new_rows_in_database_table()
//        {
//            Scenario.StartNew(this, scenario =>
//            {
//                Given_harvester_already_been_dispatched(scenario);

//                scenario.When(harvester_is_dispatched);
//                scenario.Then("assure existing Servers are not added as new rows in database table", () =>
//                    dbSession.CreateCriteria(typeof(CIServer)).List().Count.ShouldBe(1));
//            });
//        }

//        private void Given_harvester_already_been_dispatched(Semantics scenario)
//        {
//            scenario.Given(CIHarvester_is_created).
//                And(soure_contains_data).
//                And(source_contains_a_Project).
//                And(harvester_already_been_dispatched);
//        }

//        [Test]
//        public void Assure_existing_Projects_are_not_added_as_new_rows_in_database_table()
//        {
//            Scenario.StartNew(this, scenario =>
//            {
//                Given_harvester_already_been_dispatched(scenario);

//                scenario.When(harvester_is_dispatched);
//                scenario.Then("assure existing Projects are not added as new rows in database table", () =>
//                    dbSession.CreateCriteria(typeof(CIProject)).List().Count.ShouldBe(1));
//            });
//        }
//    }
//}
