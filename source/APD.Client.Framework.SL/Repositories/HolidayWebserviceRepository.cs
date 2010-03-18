using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using APD.Client.Framework.SL.HolidayRepositoryService;
using APD.DomainModel.Framework;
using APD.DomainModel.Holidays;


namespace APD.Client.Framework.SL.Repositories
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
                WebServiceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
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
            client.SaveAsync(new List<Holiday>() {domainModel});
        }

        public void Save(IEnumerable<Holiday> domainModels)
        {
            client.SaveAsync(domainModels.ToList());
        }

        #endregion
    }
}
