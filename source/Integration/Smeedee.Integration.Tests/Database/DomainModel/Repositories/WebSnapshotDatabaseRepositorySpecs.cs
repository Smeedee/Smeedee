using System.Collections.Generic;
using System.IO;
using NHibernate;
using NUnit.Framework;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Users;
using Smeedee.DomainModel.WebSnapshot;
using Smeedee.Integration.Database.DomainModel.Repositories;
using Smeedee.IntegrationTests.Database.DomainModel.Repositories;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Integration.Tests.Database.DomainModel.Repositories
{
    public class WebSnapshotDatabaseRepositoryShared : WebSnapshotShared
    {
        protected static WebSnapshot webSnapshot1 = new WebSnapshot
                                                        {
                                                            Name = "WebSnapshot 1",
                                                            PictureFilePath = "url",
                                                            PictureHeight = 100,
                                                            PictureWidth = 100,
                                                            CropHeight = 40,
                                                            CropWidth = 40,
                                                            CropCoordinateX = 20,
                                                            CropCoordinateY = 20
                                                        };

        protected static WebSnapshotDatabaseRepository repository;

        protected Context a_WebSnapshotDatabaseRepository_is_created = () =>
                                                                           {
                                                                               repository = new WebSnapshotDatabaseRepository(sessionFactory);
                                                                           };

        protected Context there_is_a_snapshot_in_the_database = () =>
                                                                    {
                                                                        repository.Save(webSnapshot1);
                                                                    };


    }

    [TestFixture][Category("IntegrationTest")]
    public class when_saved : WebSnapshotDatabaseRepositoryShared
    {
        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        }

        [Test]
        public void should_be_created()
        {
            Given(a_WebSnapshotDatabaseRepository_is_created);
            When("instance is accessed");
            Then("it is not null", () => repository.ShouldNotBeNull());
        }

        [Test]
        public void should_implement_IRepository()
        {
            Given(a_WebSnapshotDatabaseRepository_is_created);
            When("instance is accessed");
            Then("it implements IChangesetRepository", () =>
                                                       repository.ShouldBeInstanceOfType<IRepository<WebSnapshot>>());
        }

    }

    [TestFixture][Category("IntegrationTest")]
    public class When_saving : WebSnapshotDatabaseRepositoryShared
    {
        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
            repository = new WebSnapshotDatabaseRepository(sessionFactory);
        }

        [Test]
        public void Assure_websnapshot_is_saved()
        {
            Given(a_WebSnapshotDatabaseRepository_is_created);
            When("websnapshot is saved", () =>
                                         repository.Save(webSnapshot1));
            Then("it is stored in database", () =>
                                                 {
                                                     ISessionFactory newSessionFactory =
                                                         NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);

                                                     var dbResults =
                                                         DatabaseRetriever.GetDatamodelFromDatabase<WebSnapshot>(
                                                             newSessionFactory);

                                                     dbResults[0].Name.ShouldBe(webSnapshot1.Name);
                                                     dbResults[0].PictureHeight.ShouldBe(webSnapshot1.PictureHeight);
                                                     dbResults.Count.ShouldBe(1);
                                                 });
        }
        
    }




    public class WebSnapshotShared : ScenarioClass
    {
        protected static ISessionFactory sessionFactory;

        protected static string DATABASE_TEST_FILE = "smeedeeDB_testing.db";
        protected const string TESTPROJECTNAME_TWO = "This is my project";
        protected const string TESTPROJECTNAME_ONE = "Testproject";

        protected WebSnapshotShared()
        {
            RecreateSessionFactory();
        }
        
        protected static void RecreateSessionFactory()
        {
            sessionFactory = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);
        }

        protected static void DeleteDatabaseIfExists()
        {
            File.Delete(DATABASE_TEST_FILE);
        }

        protected Context Database_exists = () =>
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        };

        protected Context Database_contains_user_data = () =>
        {
            using (var session = sessionFactory.OpenSession())
            {
                var userdb = new Userdb("default");

                var users = new List<User>();
                users.Add(new User
                {
                    Email = "mail@goeran.no",
                    ImageUrl = "http://goeran.no/avatar.jpg",
                    Firstname = "Gøran",
                    Surname = "Hansen",
                    Username = "goeran"
                });

                users.Add(new User
                {
                    Email = "jonas@follesoe.no",
                    ImageUrl = "http://jonas.follesoe.no/avatar.jpg",
                    Firstname = "Jonas Follesø",
                    Username = "jonas"
                });

                userdb.Users = users;
                session.Save(userdb);
                session.Flush();
            }
        };

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }


}
