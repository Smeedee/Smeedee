using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;

using APD.DomainModel.Config;
using APD.DomainModel.Framework;
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
            configurationRepository = new GenericDatabaseRepository<Configuration>();
        }

        [OperationContract]
        [ServiceKnownType(typeof(Specification<Configuration>))]
        [ServiceKnownType(typeof(AllSpecification<Configuration>))]
        public IEnumerable<Configuration> Get(Specification<Configuration> specification)
        {
            return configurationRepository.Get(specification);
        }

        [OperationContract]
        public void Save(IEnumerable<Configuration> configurations)
        {
            configurationRepository.Save(configurations);
        }
    }
}
