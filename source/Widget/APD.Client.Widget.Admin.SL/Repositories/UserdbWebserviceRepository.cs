using System;
using System.Collections.Generic;
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

using APD.Client.Framework.SL;
using APD.Client.Framework.SL.UserRepositoryService;
using APD.DomainModel.Framework;
using APD.DomainModel.Users;

using Userdb = APD.DomainModel.Users.Userdb;


namespace APD.Client.Widget.Admin.SL.Repositories
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
                WebServiceEndpointResolver.ResolveDynamicEndpointAddress(serviceClient.Endpoint.Address);

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
            serviceClient.SaveAsync(domainModel);
        }

        public void Save(IEnumerable<Userdb> domainModels)
        {
            
        }

        #endregion
    }
}
