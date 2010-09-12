using System;
using System.Collections.Generic;
using System.ComponentModel;
using Smeedee.Client.Framework.SL.HolidayRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Holidays;

namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncHolidayRepository : IAsyncRepository<Holiday>, IPersistDomainModelsAsync<Holiday>
    {
        private readonly HolidayRepositoryServiceClient client;

        public AsyncHolidayRepository()
        {
            client = new HolidayRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            
            client.GetCompleted += Client_GetCompleted;
            client.SaveCompleted += Client_SaveCompleted;
        }

        #region IAsyncRepository<Holiday> Members

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if( GetCompleted != null )
            {
                var specificationUsed = e.UserState as Specification<Holiday>;
                GetCompletedEventArgs<Holiday> eventArgs = null;
                if( e.Error != null )
                    eventArgs = new GetCompletedEventArgs<Holiday>(specificationUsed, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<Holiday>(e.Result, specificationUsed);

                GetCompleted(this, eventArgs);
            }
        }

        public void BeginGet(Specification<Holiday> specification)
        {
            client.GetAsync(specification, specification);
        }

        public event EventHandler<GetCompletedEventArgs<Holiday>> GetCompleted;

        #endregion

        #region IPersistDomainModelsAsync<Holiday> Members

        private void Client_SaveCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (SaveCompleted != null)
            {
                var eventArgs = new SaveCompletedEventArgs(e.Error);
                SaveCompleted(this, eventArgs);
            }
        }
        
        public void Save(Holiday domainModel)
        {
            var holidayList = new List<Holiday> {domainModel};
            client.SaveAsync(holidayList);
        }

        public void Save(IEnumerable<Holiday> domainModels)
        {
            var holidayList = new List<Holiday>(domainModels);
            client.SaveAsync(holidayList);
        }

        public event EventHandler<SaveCompletedEventArgs> SaveCompleted;

        #endregion
    }
}
