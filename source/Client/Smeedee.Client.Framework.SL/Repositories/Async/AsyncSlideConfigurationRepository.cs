using System;
using System.Collections.Generic;
using System.Linq;
using Smeedee.Client.Framework.SL.SlideConfigurationService;
using Smeedee.DomainModel.Framework;
using SlideConfiguration = Smeedee.DomainModel.Config.SlideConfig.SlideConfiguration;

namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncSlideConfigurationRepository : IAsyncRepository<SlideConfiguration>, IPersistDomainModelsAsync<SlideConfiguration>, IDeleteDomainModelsAsync<SlideConfiguration>
    {
        private SlideConfigurationRepositoryServiceClient client;

        public AsyncSlideConfigurationRepository()
        {
           client = new SlideConfigurationRepositoryServiceClient();
            client.Endpoint.Address = WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);

            client.GetCompleted += new EventHandler<GetCompletedEventArgs>(client_GetCompleted);
            client.DeleteCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(client_DeleteCompleted);
            client.SaveCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(client_SaveCompleted);
            client.SaveAllCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(client_SaveCompleted);
        }

        void client_SaveCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if(SaveCompleted != null)
            {
                SaveCompleted(this, new SaveCompletedEventArgs(e.Error));
            }
        }

        void client_DeleteCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if(DeleteCompleted != null)
                DeleteCompleted(this, new DeleteCompletedEventArgs<SlideConfiguration>(e.Error, e.UserState as Specification<SlideConfiguration>));
        }

        void client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if(GetCompleted != null)
            {
                var specificationUsed = e.UserState as Specification<SlideConfiguration>;

                GetCompletedEventArgs<SlideConfiguration> eventArgs = null;
                if (e.Error != null)
                    eventArgs = new GetCompletedEventArgs<SlideConfiguration>(specificationUsed, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<SlideConfiguration>(e.Result, specificationUsed);
                
                GetCompleted(this, eventArgs);
            }
        }


        public void BeginGet(Specification<SlideConfiguration> specification)
        {
            client.GetAsync(specification, specification);
        }

        public event EventHandler<GetCompletedEventArgs<SlideConfiguration>> GetCompleted;

        public void Save(SlideConfiguration domainModel)
        {
            client.SaveAsync(domainModel);
        }

        public void Save(IEnumerable<SlideConfiguration> domainModels)
        {
            client.SaveAllAsync(domainModels.ToList());
        }

        public event EventHandler<SaveCompletedEventArgs> SaveCompleted;

        public void Delete(Specification<SlideConfiguration> specification)
        {
            client.DeleteAsync(specification, specification);
        }

        public event EventHandler<DeleteCompletedEventArgs<SlideConfiguration>> DeleteCompleted;
    }
}
