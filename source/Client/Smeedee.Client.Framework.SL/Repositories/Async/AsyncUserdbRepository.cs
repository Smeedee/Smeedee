using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Smeedee.Client.Framework.SL.UserRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Users;

namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncUserdbRepository   : IAsyncRepository<Userdb>, IPersistDomainModelsAsync<Userdb>
    {
        private readonly UserRepositoryServiceClient client;

        public AsyncUserdbRepository()
        {
            client = new UserRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            
            client.GetCompleted += Client_GetCompleted;
            client.SaveCompleted += Client_SaveCompleted;
        }

        #region IAsyncRepository<Userdb> Members
        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if( GetCompleted != null )
            {
                var specificationUsed = e.UserState as Specification<Userdb>;
                GetCompletedEventArgs<Userdb> eventArgs = null;
                if (e.Error != null)
                    eventArgs = new GetCompletedEventArgs<Userdb>(specificationUsed, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<Userdb>(e.Result, specificationUsed);

                GetCompleted(this, eventArgs);
            }
        }

        public void BeginGet(Specification<Userdb> specification)
        {
            client.GetAsync(specification, specification);
        }

        public event EventHandler<GetCompletedEventArgs<Userdb>> GetCompleted;
        #endregion

        #region IPersistDomainModelsAsync<Userdb> Members
        private void Client_SaveCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (SaveCompleted != null)
            {
                var eventArgs = new SaveCompletedEventArgs(e.Error);
                SaveCompleted(this, eventArgs);
            }
        }

        public void Save(Userdb domainModel)
        {
            client.SaveAsync(new List<Userdb>{domainModel});
        }

        public void Save(IEnumerable<Userdb> domainModels)
        {
            client.SaveAsync(domainModels.ToList());
        }

        public event EventHandler<SaveCompletedEventArgs> SaveCompleted;

        #endregion
    }
}
