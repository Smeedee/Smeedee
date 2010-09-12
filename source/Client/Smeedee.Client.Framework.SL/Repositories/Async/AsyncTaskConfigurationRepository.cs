using System;
using System.Collections.Generic;
using System.ComponentModel;
using Smeedee.Client.Framework.SL.TaskConfigurationRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;

namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncTaskConfigurationRepository : IAsyncRepository<TaskConfiguration>, IPersistDomainModelsAsync<TaskConfiguration>, IDeleteDomainModelsAsync<TaskConfiguration>
    {
        private readonly TaskConfigurationRepositoryWebserviceClient client;

        public AsyncTaskConfigurationRepository()
        {
            client = new TaskConfigurationRepositoryWebserviceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);

            client.GetCompleted += Client_GetCompleted;
            client.SaveCompleted += Client_SaveCompleted;
            client.DeleteCompleted += Client_DeleteCompleted;
        }

        #region IAsyncRepository<TaskConfiguration> Members

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if (GetCompleted != null)
            {
                var specificationUsed = e.UserState as Specification<TaskConfiguration>;
                GetCompletedEventArgs<TaskConfiguration> eventArgs = null;
                if (e.Error != null)
                    eventArgs = new GetCompletedEventArgs<TaskConfiguration>(specificationUsed, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<TaskConfiguration>(e.Result, specificationUsed);

                GetCompleted(this, eventArgs);
            }
        }


        public void BeginGet(Specification<TaskConfiguration> specification)
        {
            client.GetAsync(specification, specification);
        }

        public event EventHandler<GetCompletedEventArgs<TaskConfiguration>> GetCompleted;

        #endregion

        #region IPersistDomainModelsAsync<TaskConfiguration> Members

        private void Client_SaveCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (SaveCompleted != null)
            {
                var eventArgs = new SaveCompletedEventArgs(e.Error);
                SaveCompleted(this, eventArgs);
            }
        }

        public void Save(TaskConfiguration domainModel)
        {
            var configList = new List<TaskConfiguration> { domainModel };
            client.SaveAsync(configList);
        }

        public void Save(IEnumerable<TaskConfiguration> domainModels)
        {
            var configList = new List<TaskConfiguration>(domainModels);
            client.SaveAsync(configList);
        }

        public event EventHandler<SaveCompletedEventArgs> SaveCompleted;

        #endregion

        #region IDeleteDomainModelsAsync<TaskConfiguration> Members

        private void Client_DeleteCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (DeleteCompleted != null)
            {
                var specificationUsed = e.UserState as Specification<TaskConfiguration>;
                var eventArgs = new DeleteCompletedEventArgs<TaskConfiguration>(e.Error, specificationUsed);
                DeleteCompleted(this, eventArgs);
            }
        }

        public void Delete(Specification<TaskConfiguration> specification)
        {
            client.DeleteAsync(specification, specification);
        }

        public event EventHandler<DeleteCompletedEventArgs<TaskConfiguration>> DeleteCompleted;

        #endregion

    }
}
