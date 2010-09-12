using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Smeedee.Client.Framework.SL.RetrospectiveNoteRepositoryService;
using Smeedee.DomainModel.Corkboard;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Client.Framework.SL.Repositories
{
    public class RetrospectiveNoteWebServiceRepository : IRepository<RetrospectiveNote>, IPersistDomainModels<RetrospectiveNote>, IDeleteDomainModels<RetrospectiveNote>
    {
        private ManualResetEvent resetEvent;
        private RetrospectiveNoteRepositoryServiceClient client;
        private Exception invocationException;
        private List<RetrospectiveNote> result;

        public RetrospectiveNoteWebServiceRepository()
        {
            client = new RetrospectiveNoteRepositoryServiceClient();
            resetEvent = new ManualResetEvent(false);

            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            client.GetCompleted += new EventHandler<GetCompletedEventArgs>(Client_GetCompleted);
        }

        public IEnumerable<RetrospectiveNote> Get(Specification<RetrospectiveNote> specification)
        {
            client.GetAsync(specification);
            resetEvent.Reset();
            resetEvent.WaitOne();

            if (invocationException != null)
                throw invocationException;

            return result;
        }

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                this.result = e.Result;
            }

            invocationException = e.Error;

            resetEvent.Set();
        }

        public void Save(RetrospectiveNote domainModel)
        {
            client.SaveAsync(new List<RetrospectiveNote>() { domainModel });
        }

        public void Save(IEnumerable<RetrospectiveNote> domainModels)
        {
            client.SaveAsync(domainModels.ToList());
        }

        public void Delete(Specification<RetrospectiveNote> specification)
        {
            client.DeleteAsync(specification);
        }
    }
}
