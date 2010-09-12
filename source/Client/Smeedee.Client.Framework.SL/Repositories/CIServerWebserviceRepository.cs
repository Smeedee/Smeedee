using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Smeedee.Client.Framework.SL.CIRepositoryService;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using CIServer = Smeedee.DomainModel.CI.CIServer;

namespace Smeedee.Client.Framework.SL.Repositories
{
    public class CIServerWebserviceRepository : IRepository<CIServer>, IPersistDomainModels<CIServer>
    {
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private List<CIServer> ciServers;
        private CIRepositoryServiceClient client;

        private Exception invocationException;

        public CIServerWebserviceRepository()
        {
            ciServers = new List<CIServer>();

            client = new CIRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            client.GetCompleted += new EventHandler<GetCompletedEventArgs>(Client_GetCompleted);
        }

        #region ICIProjectRepository Members

        public IEnumerable<CIServer> Get(Specification<CIServer> specification)
        {
            client.GetAsync(new AllSpecification<CIServer>());
            resetEvent.Reset();
            resetEvent.WaitOne();

            if (invocationException != null)
                throw invocationException;

            var servers = ciServers;
            return servers;
        }

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            
            var qServers = e.Result;
            if (qServers != null && qServers.Count > 0)
            {
                ciServers = qServers;
            }

            invocationException = e.Error;

            resetEvent.Set();
        }

        public void Save(CIServer domainModel)
        {
            var CIServers = new List<CIServer>();
            CIServers.Add(domainModel);
            client.SaveAsync(CIServers);
        }

        public void Save(IEnumerable<CIServer> domainModels)
        {
            client.SaveAsync(domainModels.ToList());
        }

        #endregion
    }
}
