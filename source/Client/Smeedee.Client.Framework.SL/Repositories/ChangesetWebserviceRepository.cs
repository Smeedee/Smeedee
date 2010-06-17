using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using Smeedee.Client.Framework.SL.ChangesetRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;

namespace Smeedee.Client.Framework.SL.Repositories
{
    public class ChangesetWebserviceRepository : IRepository<Changeset>
    {
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private List<Changeset> changesets;
        private ChangesetRepositoryServiceClient client;

        private Exception invocationException;

        public ChangesetWebserviceRepository()
        {
            changesets = new List<Changeset>();

            client = new ChangesetRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            client.GetCompleted += new EventHandler<GetCompletedEventArgs>(Client_GetCompleted);
        }

        public IEnumerable<Changeset> Get(Specification<Changeset> specification)
        {
            client.GetAsync(specification);
            resetEvent.Reset();
            resetEvent.WaitOne();

            if (invocationException != null)
                throw invocationException;
 
            return changesets;
        }

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            
            var returnedChangesets = e.Result;
            if (returnedChangesets != null && returnedChangesets.Count > 0)
            {
                changesets = returnedChangesets;
            }

            invocationException = e.Error;

            resetEvent.Set();
        }

    }
}
