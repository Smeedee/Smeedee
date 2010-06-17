using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Smeedee.Client.Framework.SL.ConfigurationRepositoryService;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Client.Framework.SL.Repositories
{
    public class ConfigurationWebserviceRepository : IRepository<Configuration>, IPersistDomainModels<Configuration>
    {

        private ConfigurationRepositoryServiceClient client;
        private ManualResetEvent resetEvent;

        private Exception invocationException;

        private IEnumerable<Configuration> configurations;

        public ConfigurationWebserviceRepository()
        {

            client = new ConfigurationRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
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
            client.SaveAsync(domainModels.ToList());
        }

        #endregion
    }
}
