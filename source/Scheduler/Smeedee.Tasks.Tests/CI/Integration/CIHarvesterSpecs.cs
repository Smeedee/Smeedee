//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Smeedee.DomainModel.CI;
//using Smeedee.DomainModel.Framework;
//using Smeedee.Task.CI;
//using Smeedee.TaskTests.CI.CITaskSpecs;

//using Moq;

//using NUnit.Framework;

//using TinyBDD.Dsl.GivenWhenThen;
//using TinyBDD.Specification.NUnit;
//using Smeedee.Plugin.Database;
//using NHibernate;
//using System.IO;


//namespace Smeedee.Task.CITests.Integration.CITaskSpecs
//{
//    public class SharedForIntegration : Shared
//    {
//        protected static CITask Task;
//        protected static List<CIServer> sourceData;
//        protected static ISession dbSession;

//        protected Context CITask_is_created = () =>
//        {
//            var databaseFile = "SmeedeeDB_test.db";

//            if (File.Exists(databaseFile))
//                File.Delete(databaseFile);

//            var sessionFactory = NHibernateFactory.AssembleSessionFactory(databaseFile);
//            dbSession = sessionFactory.OpenSession();
            
//            sourceRepositoryMock = new Mock<IRepository<CIServer>>();
//            Task = new CITask(sourceRepositoryMock.Object, new GenericDatabaseRepository<CIServer>(sessionFactory));
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

//        protected Context Task_already_been_dispatched = () =>
//        {
//            Task.Execute();
//        };

//        protected When Task_is_dispatched = () =>
//        {
//            Task.Execute();
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
//                scenario.Given(CITask_is_created).
//                    And(soure_contains_data);
//                scenario.When(Task_is_dispatched);
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
//                scenario.Given(CITask_is_created).
//                    And(soure_contains_data).
//                    And(source_contains_a_Project);

//                scenario.When(Task_is_dispatched);
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
//                Given_Task_already_been_dispatched(scenario);

//                scenario.When(Task_is_dispatched);
//                scenario.Then("assure existing Servers are not added as new rows in database table", () =>
//                    dbSession.CreateCriteria(typeof(CIServer)).List().Count.ShouldBe(1));
//            });
//        }

//        private void Given_Task_already_been_dispatched(Semantics scenario)
//        {
//            scenario.Given(CITask_is_created).
//                And(soure_contains_data).
//                And(source_contains_a_Project).
//                And(Task_already_been_dispatched);
//        }

//        [Test]
//        public void Assure_existing_Projects_are_not_added_as_new_rows_in_database_table()
//        {
//            Scenario.StartNew(this, scenario =>
//            {
//                Given_Task_already_been_dispatched(scenario);

//                scenario.When(Task_is_dispatched);
//                scenario.Then("assure existing Projects are not added as new rows in database table", () =>
//                    dbSession.CreateCriteria(typeof(CIProject)).List().Count.ShouldBe(1));
//            });
//        }
//    }
//}
