using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Services
{

    [ServiceContract(Namespace = "http://smeedee.org")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceKnownType(typeof(AllSpecification<TaskConfiguration>))]
    [ServiceKnownType(typeof(Specification<TaskConfiguration>))]
    public class TaskConfigurationRepositoryWebservice
    {
        private TaskConfigurationDatabaseRepository repo;
        private readonly ILog logger;

        public TaskConfigurationRepositoryWebservice()
        {
            repo = new TaskConfigurationDatabaseRepository(DefaultSessionFactory.Instance);
            logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
        }

        [OperationContract]
        public IEnumerable<TaskConfiguration> Get(Specification<TaskConfiguration> specification)
        {
            IEnumerable<TaskConfiguration> result = new List<TaskConfiguration>();

            try
            {
                result = repo.Get(specification);
            }
            catch (Exception exception)
            {
                logger.WriteEntry(new ErrorLogEntry(GetType().ToString(), exception.ToString()));
            }
            return result.ToList();
        }

        [OperationContract]
        public void Save(IEnumerable<TaskConfiguration> taskConfigs)
        {
            try
            {
                repo.Save(taskConfigs);
            }
            catch (Exception exception)
            {
                logger.WriteEntry(new ErrorLogEntry(GetType().ToString(), exception.ToString()));
            }
        }

        [OperationContract]
        public void Delete(Specification<TaskConfiguration> specification)
        {
            try
            {
                repo.Delete(specification);
            }
            catch (Exception exception)
            {
                logger.WriteEntry(new ErrorLogEntry(GetType().ToString(), exception.ToString()));
            }
        }
    }
}
