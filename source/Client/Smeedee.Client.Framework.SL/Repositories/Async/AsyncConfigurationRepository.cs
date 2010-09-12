using System;
using System.Collections.Generic;
using System.ComponentModel;
using Smeedee.Client.Framework.SL.ConfigurationRepositoryService;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncConfigurationRepository : IAsyncRepository<Configuration>, IPersistDomainModelsAsync<Configuration>
    {
        private readonly ConfigurationRepositoryServiceClient client;

        public AsyncConfigurationRepository()
        {
            client = new ConfigurationRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            
            client.GetCompleted += Client_GetCompleted;
            client.SaveCompleted += Client_SaveCompleted;
        }

        #region IAsyncRepository<Configuration> Members

        void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if (GetCompleted != null)
            {
                var specificationUsed = e.UserState as Specification<Configuration>;

                GetCompletedEventArgs<Configuration> eventArgs = null;
                if (e.Error != null)
                    eventArgs = new GetCompletedEventArgs<Configuration>(specificationUsed, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<Configuration>(e.Result, specificationUsed);

                GetCompleted(this, eventArgs);
            }
        }

        public void BeginGet(Specification<Configuration> specification)
        {
            client.GetAsync(specification, specification);
        }

        public event EventHandler<GetCompletedEventArgs<Configuration>> GetCompleted;

        #endregion

        #region IPersistDomainModelsAsync<Configuration> Members

        private void Client_SaveCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (SaveCompleted != null)
            {
                var eventArgs = new SaveCompletedEventArgs(e.Error);
                SaveCompleted(this, eventArgs);
            }
        }

        public void Save(Configuration domainModel)
        {
            var configList = new List<Configuration> {domainModel};
            client.SaveAsync(configList);
        }

        public void Save(IEnumerable<Configuration> domainModels)
        {
            var configList = new List<Configuration>(domainModels);
            client.SaveAsync(configList);
        }

        public event EventHandler<SaveCompletedEventArgs> SaveCompleted;

        #endregion
    }
}
