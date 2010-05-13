


using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

using APD.Client.Framework.SL.CIRepositoryService;
using APD.DomainModel.CI;
using APD.DomainModel.Framework;

using CIServer = APD.DomainModel.CI.CIServer;


namespace APD.Client.Framework.SL.Repositories
{
    public class CIProjectRepository : IRepository<APD.DomainModel.CI.CIServer>
    {
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private List<CIServer> ciServers;
        private CIRepositoryServiceClient client;

        private Exception invocationException;

        public CIProjectRepository()
        {
            ciServers = new List<CIServer>();

            client = new CIRepositoryServiceClient();
            client.Endpoint.Address =
                WebServiceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            client.GetCompleted += new EventHandler<GetCompletedEventArgs>(Client_GetCompleted);
        }

        #region ICIProjectRepository Members

        public IEnumerable<APD.DomainModel.CI.CIServer> Get(Specification<APD.DomainModel.CI.CIServer> specification)
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
            if (qServers != null && qServers.Count() > 0)
            {
                ciServers = qServers;
            }

            invocationException = e.Error;

            resetEvent.Set();
        }

        #endregion
    }
}
