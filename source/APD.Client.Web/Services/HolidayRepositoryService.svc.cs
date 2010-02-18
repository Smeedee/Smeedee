using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;

using APD.DomainModel.Framework;
using APD.DomainModel.Framework.Logging;
using APD.DomainModel.Holidays;
using APD.Integration.Database.DomainModel.Repositories;


namespace APD.Client.Web.Services
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceKnownType(typeof(AllSpecification<Holiday>))]
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
                ILog logger = new DatabaseLogger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
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
                ILog logger = new DatabaseLogger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }
        }

    }
}
