using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Smeedee.Client.Framework.SL.HolidayRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Holidays;

namespace Smeedee.Client.Framework.SL.Repositories
{
    public class HolidayWebserviceRepository : IRepository<Holiday>, IPersistDomainModels<Holiday>
    {

        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private List<Holiday> holidays = new List<Holiday>();
        private HolidayRepositoryServiceClient client;

        private Exception invocationException;

        public HolidayWebserviceRepository()
        {
            holidays = new List<Holiday>();

            client = new HolidayRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            client.GetCompleted += new EventHandler<GetCompletedEventArgs>(Client_GetCompleted);
        }

        #region ICIProjectRepository Members

        public IEnumerable<Holiday> Get(Specification<Holiday> specification)
        {
            client.GetAsync(specification);
            resetEvent.Reset();
            resetEvent.WaitOne();

            if (invocationException != null)
                throw invocationException;

            var servers = holidays;
            return servers;
        }

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {

            var resultingHolidays = e.Result;

            if (resultingHolidays != null)
            {
                this.holidays = resultingHolidays;
            }

            invocationException = e.Error;

            resetEvent.Set();
        }

        #endregion

        #region IPersistDomainModels<Holiday> Members

        public void Save(Holiday domainModel)
        {
            client.SaveAsync(new List<Holiday>() { domainModel });
        }

        public void Save(IEnumerable<Holiday> domainModels)
        {
            client.SaveAsync(domainModels.ToList());
        }

        #endregion
    }
}
