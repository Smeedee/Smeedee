using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Smeedee.Client.Web.Serialization;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceKnownType(typeof(Specification<SlideConfiguration>))]
    [ServiceKnownType(typeof(AllSpecification<SlideConfiguration>))]
    [ServiceKnownType(typeof(SlideConfigurationByIdSpecification))]
    [UseReferenceTrackingSerializer]
    public class SlideConfigurationRepositoryService
    {
        private SlideConfigurationRepository repo;
        private Logger logger;

        public SlideConfigurationRepositoryService()
        {
            logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
            repo = new SlideConfigurationRepository(DefaultSessionFactory.Instance);
        }

        [OperationContract]
        public IEnumerable<SlideConfiguration> Get(Specification<SlideConfiguration> specification)
        {
            IEnumerable<SlideConfiguration> result = new List<SlideConfiguration>();
            try
            {
                result = repo.Get(specification);
            }
            catch (Exception ex)
            {
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), ex.ToString()));
            }


            return result;
        }

        [OperationContract]
        public void Save(SlideConfiguration configuration)
        {
            try
            {
                repo.Save(configuration);
            }
            catch (Exception exception)
            {
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }
        }

        [OperationContract]
        public void SaveAll(IEnumerable<SlideConfiguration> configs)
        {
            try
            {
                repo.Save(configs);
            }
            catch(Exception e)
            {
                logger.WriteEntry(new ErrorLogEntry(GetType().ToString(), e.ToString()));
            }
        }

        [OperationContract]
        public void Delete(Specification<SlideConfiguration> specification)
        {
            try
            {
                repo.Delete(specification);
            }
            catch (Exception exception)
            {
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }
        }
    }
}
