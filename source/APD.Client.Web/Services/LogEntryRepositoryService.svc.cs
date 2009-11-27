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
            return repo.Get(specification);
        }
    }
}
