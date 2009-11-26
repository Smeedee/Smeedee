using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Harvester.FileImport;
using Moq;
using APD.Harvester.Framework.Services;
using APD.Harvester.FileImport.Services;
using APD.Harvester.FileImport.Factories;

namespace APD.HarvesterTests.FileImport.FileImportHarvesterSpecs
{
    public class Shared : ScenarioClass
    {
        protected static Mock<IManageFileSystems> fileSystemServiceMock = new Mock<IManageFileSystems>();
        protected static Mock<IKnowTheFileImportQueue> fileImportQueueService = new Mock<IKnowTheFileImportQueue>();
        protected static Mock<IAssembleFileProcessors> fileProcessorsFactoryMock =
            new Mock<IAssembleFileProcessors>();
        private static FileImportHarvester harvester;
        protected static readonly string fileImportQueueDirectory = @"FileImport\Queue";
        protected static readonly string fileImportUnrecognizedDirectory = @"FileImport\Unrecognized";
        protected static readonly string fileImportCompletedDirectory = @"FileImport\Completed";

        protected Context FileImportQueue_service_is_created = () =>
        {
            fileImportQueueService = new Mock<IKnowTheFileImportQueue>();
            fileImportQueueService.Setup(s => s.GetCompletedDirPath()).Returns(fileImportCompletedDirectory);
            fileImportQueueService.Setup(s => s.GetDirectoryPath()).Returns(fileImportQueueDirectory);
            fileImportQueueService.Setup(s => s.GetUnrecognizedDirPath()).Returns(fileImportUnrecognizedDirectory);
        };

        protected Context FileSystem_service_is_created = () =>
        {
            fileSystemServiceMock = new Mock<IManageFileSystems>();
        };

        protected Context Harvester_is_created = () =>
        {
            harvester = new FileImportHarvester(fileSystemServiceMock.Object,
                fileImportQueueService.Object,
                fileProcessorsFactoryMock.Object);
        };

        protected Context FileImport_Queue_contains_recognized_files = () =>
        {
            var files = new List<string>();
            files.Add(string.Format(@"{0}\{1}", fileImportQueueDirectory, "users.userdb"));
            fileSystemServiceMock.Setup(s => s.GetFiles(fileImportQueueDirectory)).Returns(files);
        };

        protected Context FileImport_Queue_contains_unrecognized_files = () =>
        {
            var files = new List<string>();
            files.Add(string.Format(@"{0}\{1}", fileImportQueueDirectory, "file1.blah"));
            fileSystemServiceMock.Setup(s => s.GetFiles(fileImportQueueDirectory)).Returns(files);
        };

        protected GivenSemantics dependencies_are_created()
        {
            return Given(FileSystem_service_is_created).
                And(FileImportQueue_service_is_created);
        }

        protected When harvest = () =>
        {
            harvester.DispatchDataHarvesting();
        };

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }

    [TestFixture]
    public class When_spawing : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("When spawning FileImportHarvester");
            Given("Harvester is not created");
            When("spawning");
        }

        [Test]
        public void Assure_FileSystem_service_obj_is_validated()
        {
            Then("assure FileSystem service obj is validated", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new FileImportHarvester(null, null, null), ex => { }));
        }

        [Test]
        public void Assure_FileImportQueue_service_obj_is_validated()
        {
            Then("assure FileImportQueue service obj is validated", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new FileImportHarvester(fileSystemServiceMock.Object, null, null), ex => { }));
        }

        [Test]
        public void Assure_FileProcessorFactory_obj_is_validated()
        {
            Then("assure FileProcessorFactory obj is validated", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new FileImportHarvester(fileSystemServiceMock.Object, fileImportQueueService.Object, null), ex => { }));
        }
    }

    [TestFixture]
    public class When_dispatched : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("When FileImportHarvester is harvest");
            Given(dependencies_are_created()).
                And(Harvester_is_created);
            When(harvest);
        }

        [Test]
        public void Assure_it_get_the_FileImport_Queue_directory_path()
        {
            Then("assure it get the FileImport Queue directory path", () =>
                fileImportQueueService.Verify(s => s.GetDirectoryPath(), Times.Once()));
        }


        [Test]
        public void Assure_it_get_the_FileImport_Unrecognized_directory_path()
        {
            Then("assure it get the FileImport Unrecognized directory path", () =>
                fileImportQueueService.Verify(s => s.GetUnrecognizedDirPath(), Times.Once()));
        }

        [Test]
        public void Assure_it_check_for_new_files_in_the_FileImport_Queue_directory()
        {
            Then("assure it checks for new files in the FileImport Queue directory", () =>
                fileSystemServiceMock.Verify(s => s.GetFiles(fileImportQueueDirectory), Times.Once()));
        }
    }

    [TestFixture]
    public class There_are_recognized_files_in_the_FileImport_Queue : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("There are recognized files in the FileImport Queue");
            Given(dependencies_are_created()).
                And(Harvester_is_created).
                And(FileImport_Queue_contains_recognized_files);
            When(harvest);
        }

        [Test]
        public void Assure_files_are_processed()
        {
            Then("assure files are processed");
        }

        [Test]
        public void Assure_processed_files_are_moved_to_the_Completed_directory()
        {
            Then("assure processed files are moved to the Completed directory", () =>
                fileSystemServiceMock.Verify(s => s.Move(It.IsAny<string>(), It.Is<string>(str => str.StartsWith(fileImportCompletedDirectory)))));
        }

    }

    [TestFixture]
    public class There_are_unrecognized_files_in_the_FileImport_Queue : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("There are unrecognized files in the FileImport Queue");
            Given(dependencies_are_created()).
                And(Harvester_is_created).
                And(FileImport_Queue_contains_unrecognized_files);
            When(harvest);
        }

        [Test]
        public void Assure_files_are_moved_to_Unrecognized_directory()
        {
            Then("assure files are moved to Unrecognized directory", () =>
                fileSystemServiceMock.Verify(s => s.Move(It.IsAny<string>(), It.Is<string>(str => str.StartsWith(fileImportUnrecognizedDirectory)))));
        }

    }

    [TestFixture]
    public class When_file_processing_fails : Shared
    {
        
    }
}
