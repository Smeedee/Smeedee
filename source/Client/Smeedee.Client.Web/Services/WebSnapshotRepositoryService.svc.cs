using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.WebSnapshot;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "http://smeedee.org")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceKnownType(typeof(WebSnapshotSpecification))]
    public class WebSnapshotRepositoryService
    {

       [OperationContract]
        public IEnumerable<WebSnapshot> Get(Specification<WebSnapshot> specification)
       {
           WebSnapshotDatabaseRepository repo = new WebSnapshotDatabaseRepository(DefaultSessionFactory.Instance);
           IEnumerable<WebSnapshot> result = new List<WebSnapshot>();
           try
           {
               result = repo.Get(specification);
           }
           catch (Exception exception)
           {
               ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
               logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
           }

           return result;
       }

        [OperationContract]
        public void Save(IEnumerable<WebSnapshot> webSnapshots)
        {
           WebSnapshotDatabaseRepository repo = new WebSnapshotDatabaseRepository(DefaultSessionFactory.Instance);
            try
            {
                repo.Save(webSnapshots);
            }
            catch (Exception exception)
            {
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }
        }
    }
}
