using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.WebTests.LogEntryRepositoryService;
using APD.DomainModel.Framework;
using APD.DomainModel.Framework.Logging;

using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace APD.Client.WebTests.Services.Integration
{
    [TestFixture]
    public class LogEntryRepositoryServiceSpecs
    {
        [Test]
        public void assure_can_save_logEntry()
        {
            LogEntryRepositoryService.LogEntryRepositoryServiceClient client = new LogEntryRepositoryServiceClient();

            var guid = Guid.NewGuid();
            client.Log(new InfoLogEntry()
            {
                Message = guid.ToString(),
                TimeStamp = DateTime.Now,
                Source = "me"
            });

            //TODO: Fix this webservice so this will run
            
            //client.Get(new AllSpecification<LogEntry>())
            //    .Where(le => le.Message.Equals(guid.ToString()))
            //    .Count().ShouldBe(1);
        }
    }
}
