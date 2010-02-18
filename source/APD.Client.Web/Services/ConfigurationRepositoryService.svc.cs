using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;

using APD.DomainModel.Config;
using APD.DomainModel.Framework;
using APD.DomainModel.Framework.Logging;
using APD.Integration.Database.DomainModel.Repositories;

namespace APD.Client.Web.Services
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ConfigurationRepositoryService
    {
        GenericDatabaseRepository<Configuration> configurationRepository;

        public ConfigurationRepositoryService()
        {
            configurationRepository = new ConfigurationDatabaseRepository(DefaultSessionFactory.Instance);
        }

        [OperationContract]
        [ServiceKnownType(typeof(Specification<Configuration>))]
        [ServiceKnownType(typeof(AllSpecification<Configuration>))]
        public IEnumerable<Configuration> Get(Specification<Configuration> specification)
        {
            IEnumerable<Configuration> result = new List<Configuration>();
            try
            {
                result = configurationRepository.Get(specification);
            }
            catch (Exception exception)
            {
                ILog logger = new DatabaseLogger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }

            return result;
        }

        [OperationContract]
        public void Save(IEnumerable<Configuration> configurations)
        {
            try
            {
                configurationRepository.Save(configurations);
            }
            catch (Exception exception)
            {
                ILog logger = new DatabaseLogger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }
        }
    }
}
