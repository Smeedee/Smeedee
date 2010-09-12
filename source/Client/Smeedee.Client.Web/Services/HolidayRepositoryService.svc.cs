using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Holidays;
using Smeedee.Integration.Database.DomainModel.Repositories;


namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "http://smeedee.org")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceKnownType(typeof(AllSpecification<Holiday>))]
    [ServiceKnownType(typeof(HolidaySpecification))]
    [ServiceKnownType(typeof(Specification<Holiday>))]
    public class HolidayRepositoryService
    {
        [OperationContract]
        public IEnumerable<Holiday> Get(Specification<Holiday> specification)
        {
            HolidayDatabaseRepository repo = new HolidayDatabaseRepository(DefaultSessionFactory.Instance);
            IEnumerable<Holiday> result = new List<Holiday>();
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
        public void Save(IEnumerable<Holiday> holidays)
        {
            HolidayDatabaseRepository repo = new HolidayDatabaseRepository(DefaultSessionFactory.Instance);
            try
            {
                repo.Save(holidays);
            }
            catch (Exception exception)
            {
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }
        }
    }
}
