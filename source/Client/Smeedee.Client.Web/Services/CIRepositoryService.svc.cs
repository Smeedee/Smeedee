using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "http://smeedee.org")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CIRepositoryService
    {
         private IRepository<CIServer> repository;

        public CIRepositoryService()
        {
            repository = new CIServerDatabaseRepository(DefaultSessionFactory.Instance);
        }

        [OperationContract]
        [ServiceKnownType(typeof (AllSpecification<CIServer>))]
        public IEnumerable<CIServer> Get(Specification<CIServer> specification)
        {
            IEnumerable<CIServer> result = new List<CIServer>();
            try
            {
                result = repository.Get(specification);
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
