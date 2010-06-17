using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;

using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Integration.Database.DomainModel.Repositories;


namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "http://smeedee.org")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceKnownType(typeof(LogEntry))]
    [ServiceKnownType(typeof(ErrorLogEntry))]
    [ServiceKnownType(typeof(WarningLogEntry))]
    [ServiceKnownType(typeof(InfoLogEntry))]
    [ServiceKnownType(typeof(Specification<LogEntry>))]
    [ServiceKnownType(typeof(AllSpecification<LogEntry>))]
    public class LogEntryRepositoryService
    {

        LogEntryDatabaseRepository repo = new LogEntryDatabaseRepository(DefaultSessionFactory.Instance);

        [OperationContract]
        public void Log(LogEntry logEntry)
        {
            repo.Save(logEntry);
        }

        [OperationContract]
        public IEnumerable<LogEntry> Get(Specification<LogEntry> specification)
        {
            IEnumerable<LogEntry> result = new List<LogEntry>();
            try
            {
                // TODO: Try to fix this webservice so the select call can be removed. The problem is that the client side 
                // fails to deserialize the LogEntry hierarchy even with the knowntype attribute set
                result = repo.Get(specification)
                    .Select(le => new LogEntry()
                    {
                        Message = le.Message,
                        Severity = le.Severity,
                        Source = le.Source,
                        TimeStamp = le.TimeStamp
                    });
            }
            catch (Exception exception)
            {
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }

            return result;
        }
    }
}
