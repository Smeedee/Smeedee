using System;
using System.Collections.Generic;
using System.Threading;
using APD.Client.Framework.SL.ConfigurationRepositoryService;
using APD.DomainModel.Config;
using APD.DomainModel.Framework;

using Configuration = APD.DomainModel.Config.Configuration;


namespace APD.Client.Framework.SL.Repositories
{
    public class ConfigurationRepository : IRepository<Configuration>, IPersistDomainModels<Configuration>
    {

        private ConfigurationRepositoryServiceClient client;
        private ManualResetEvent resetEvent;

        private Exception invocationException;

        private IEnumerable<APD.DomainModel.Config.Configuration> configurations;

        public ConfigurationRepository()
        {

            client = new ConfigurationRepositoryServiceClient();
            client.Endpoint.Address =
                WebServiceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            resetEvent = new ManualResetEvent(false);

            client.GetCompleted += (sender, e) =>
            {
                try
                {
                    if (e.Error != null)
                    {
                        invocationException = e.Error;
                    }
                    configurations = e.Result;
                }
                finally
                {
                    resetEvent.Set();
                }
            };

        }

        #region IRepository<Configuration> Members

        public IEnumerable<Configuration> Get(Specification<Configuration> specification)
        {

            client.GetAsync(specification);
            resetEvent.Reset();
            resetEvent.WaitOne();

            if (invocationException != null)
                throw invocationException;

            return configurations;
        }

        #endregion


        #region IPersistDomainModels<Configuration> Members

        public void Save(Configuration domainModel)
        {
            var configs = new List<Configuration>();
            configs.Add(domainModel);
            client.SaveAsync(configs);
        }

        public void Save(IEnumerable<Configuration> domainModels)
        {
            client.SaveAsync(domainModels as List<Configuration>);
        }

        #endregion
    }
}
