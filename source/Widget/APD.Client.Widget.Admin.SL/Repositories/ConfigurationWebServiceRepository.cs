using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using APD.Client.Framework.SL;
using APD.DomainModel.Config;
using APD.DomainModel.Framework;
using System.Collections.Generic;
using APD.Client.Widget.Admin.SL.ConfigurationRepositoryService;
using System.Threading;

namespace APD.Client.Widget.Admin.SL.Repositories
{
    public class ConfigurationWebServiceRepository : IRepository<Configuration>, IPersistDomainModels<Configuration>
    {
        private ConfigurationRepositoryServiceClient serviceClient;
        private ManualResetEvent resetEvent;
        private IEnumerable<Configuration> getResult;

        private Exception invocationException;

        public ConfigurationWebServiceRepository()
        {
            resetEvent = new ManualResetEvent(false);

            serviceClient = new ConfigurationRepositoryServiceClient();
            serviceClient.Endpoint.Address =
                WebServiceEndpointResolver.ResolveDynamicEndpointAddress(serviceClient.Endpoint.Address);

            serviceClient.GetCompleted += (sender, eventArgs) =>
            {
                invocationException = eventArgs.Error;
                getResult = eventArgs.Result;
                resetEvent.Set();
            };

        }

        public IEnumerable<Configuration> Get(Specification<Configuration> specification)
        {
            serviceClient.GetAsync(specification);
            resetEvent.Reset();
            resetEvent.WaitOne();

            if (invocationException != null)
                throw invocationException;

            return getResult;

        }

        #region IPersistDomainModels<Userdb> Members

        public void Save(Configuration domainModel)
        {
            var data = new List<Configuration>();
            data.Add(domainModel);
            serviceClient.SaveAsync(data);
        }

        public void Save(IEnumerable<Configuration> domainModels)
        {
            serviceClient.SaveAsync(domainModels.ToList());
        }

        #endregion
    }
}
