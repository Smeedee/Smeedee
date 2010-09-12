using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;

using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "http://smeedee.org")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ConfigurationRepositoryService
    {
        GenericDatabaseRepository<Configuration> configurationRepository;

        public ConfigurationRepositoryService()
        {
            configurationRepository = new ConfigurationDatabaseRepository(DefaultSessionFactory.Instance);
        }

        [OperationContract]
        [ServiceKnownType(typeof(ConfigurationByName))]
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
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
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
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }
        }
    }
}
