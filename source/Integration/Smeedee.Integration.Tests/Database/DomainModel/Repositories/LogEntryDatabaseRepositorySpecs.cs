using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Integration.Database.DomainModel.Repositories;

using Moq;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace Smeedee.IntegrationTests.Database.DomainModel.Repositories.LogEntryDatabaseRepositorySpecs
{
    [TestFixture]
    public class when_saving : Shared
    {
        private LogEntryDatabaseRepository logRepo;

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
            logRepo = new LogEntryDatabaseRepository(sessionFactory);
        }


        [Test]
        public void Should_save_all_logEntry_types()
        {
            
            LogEntry errorLog = new ErrorLogEntry("TestFramework", "Error message");
            LogEntry warningLog = new WarningLogEntry("TestFramework", "Warning message");
            LogEntry infoLog = new InfoLogEntry("TestFramework", "Information message");

            logRepo.Save(infoLog);
            logRepo.Save(warningLog);
            logRepo.Save(errorLog);

            logRepo.Get(new AllSpecification<LogEntry>())
                .Count().ShouldBe(3);

        }
    }
}