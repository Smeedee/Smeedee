using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.SL.CIRepositoryService;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncCIServerRepository :  IAsyncRepository<CIServer>, IPersistDomainModelsAsync<CIServer>
    {
        private readonly CIRepositoryServiceClient client;

        public AsyncCIServerRepository()
        {
            client = new CIRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            
            client.GetCompleted += Client_GetCompleted;
            client.SaveCompleted += Client_SaveCompleted;
            
        }

        #region IAsyncRepository<CIServer> Members

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if(GetCompleted != null)
            {
                var specificationUsed = e.UserState as Specification<CIServer>;

                GetCompletedEventArgs<CIServer> eventArgs = null;
                if (e.Error != null)
                    eventArgs = new GetCompletedEventArgs<CIServer>(specificationUsed, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<CIServer>(e.Result, specificationUsed);
                
                GetCompleted(this, eventArgs);

            }
        }

        public void BeginGet(Specification<CIServer> specification)
        {
            client.GetAsync(specification, specification);
        }

        public event EventHandler<GetCompletedEventArgs<CIServer>> GetCompleted;

        #endregion

        #region IPersistDomainModelsAsync<CIServer> Members

        private void Client_SaveCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (SaveCompleted != null)
            {
                var eventArgs = new SaveCompletedEventArgs(e.Error);
                SaveCompleted(this, eventArgs);
            }
        }
        
        public void Save(CIServer domainModel)
        {
            var CIServerList = new List<CIServer> {domainModel};
            client.SaveAsync(CIServerList);
        }

        public void Save(IEnumerable<CIServer> domainModels)
        {
            var CIServerList = new List<CIServer>(domainModels);
            client.SaveAsync(CIServerList);
        }

        public event EventHandler<SaveCompletedEventArgs> SaveCompleted;

        #endregion
    }
}
