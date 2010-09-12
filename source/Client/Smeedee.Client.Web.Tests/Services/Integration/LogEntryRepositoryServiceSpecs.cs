using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Web.Tests.LogEntryRepositoryService;

using NUnit.Framework;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Client.WebTests.Services.Integration.LogEntryRepositoryServiceSpecs
{
    [TestFixture][Category("IntegrationTest")]
    public class LogEntryRepositoryServiceSpecs
    {
        [Test]
        public void assure_can_save_logEntry()
        {
            var client = new LogEntryRepositoryServiceClient();

            var guid = Guid.NewGuid();
            client.Log(new InfoLogEntry()
            {
                Message = guid.ToString(),
                TimeStamp = DateTime.Now,
                Source = "me"
            });

            client.Get(new AllSpecification<LogEntry>())
                .Where(le => le.Message.Equals(guid.ToString()))
                .Count().ShouldBe(1);
        }
    }
}
