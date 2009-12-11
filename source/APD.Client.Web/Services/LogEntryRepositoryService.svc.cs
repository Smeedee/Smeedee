using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;

using APD.DomainModel.Framework;
using APD.DomainModel.Framework.Logging;
using APD.Integration.Database.DomainModel.Repositories;


namespace APD.Client.Web.Services
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceKnownType(typeof(LogEntry))]
    [ServiceKnownType(typeof(ErrorLogEntry))]
    [ServiceKnownType(typeof(WarningLogEntry))]
    [ServiceKnownType(typeof(InfoLogEntry))]
    [ServiceKnownType(typeof(Specification<LogEntry>))]
    [ServiceKnownType(typeof(AllSpecification<LogEntry>))]
    public class LogEntryRepositoryService
    {

        LogEntryDatabaseRepository repo = new LogEntryDatabaseRepository();

        [OperationContract]
        public void Log(LogEntry logEntry)
        {
            repo.Save(logEntry);
        }

        [OperationContract]
        public IEnumerable<LogEntry> Get(Specification<LogEntry> specification)
        {
            // TODO: Try to fix this webservice so the select call can be removed. The problem is that the client side 
            // fails to deserialize the LogEntry hierarchy even with the knowntype attribute set
            return repo.Get(specification)
                .Select(le => new LogEntry()
                {
                    Message = le.Message, 
                    Severity = le.Severity, 
                    Source = le.Source, 
                    TimeStamp = le.TimeStamp
                });
        }
    }
}
