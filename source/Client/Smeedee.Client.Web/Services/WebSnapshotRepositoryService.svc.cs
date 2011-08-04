using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.WebSnapshot;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "http://smeedee.org")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WebSnapshotRepositoryService
    {
        [OperationContract]
        public IEnumerable<WebSnapshot> Get()
        {
            IEnumerable<WebSnapshot> result = new List<WebSnapshot>();
            var repo = new WebSnapshotDatabaseRepository(DefaultSessionFactory.Instance);

            try
            {
                result = repo.Get(new WebSnapshotSpecification());
            }
            catch (Exception exception)
            {
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString())); ;
            }

            return result;
        }
    }
}
