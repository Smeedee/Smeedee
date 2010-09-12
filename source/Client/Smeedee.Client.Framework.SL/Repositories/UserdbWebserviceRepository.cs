using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Smeedee.Client.Framework.SL.UserRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Users;

namespace Smeedee.Client.Framework.SL.Repositories
{
    public class UserdbWebserviceRepository : IRepository<Userdb>, IPersistDomainModels<Userdb>
    {
        private UserRepositoryServiceClient serviceClient;
        private ManualResetEvent resetEvent;
        private IEnumerable<Userdb> getResult;

        private Exception invocationException;

        public UserdbWebserviceRepository()
        {
            resetEvent = new ManualResetEvent(false);

            serviceClient = new UserRepositoryServiceClient();
            serviceClient.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(serviceClient.Endpoint.Address);

            serviceClient.GetCompleted += (sender, eventArgs) =>
            {
                invocationException = eventArgs.Error;
                getResult = eventArgs.Result;
                resetEvent.Set();
            };

        }

        public IEnumerable<Userdb> Get(Specification<Userdb> specification)
        {
            serviceClient.GetAsync(specification);
            resetEvent.Reset();
            resetEvent.WaitOne();

            if (invocationException != null)
                throw invocationException;

            return getResult;

        }

        #region IPersistDomainModels<Userdb> Members

        public void Save(Userdb domainModel)
        {
            serviceClient.SaveAsync(new List<Userdb> {domainModel});
        }

        public void Save(IEnumerable<Userdb> domainModels)
        {
            serviceClient.SaveAsync(domainModels.ToList());
        }

        #endregion
    }
}
