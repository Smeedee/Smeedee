using System;
using APD.Client.Framework.SL.ChangesetRepositoryService;
using APD.DomainModel.Framework;
using APD.DomainModel.SourceControl;

using Changeset = APD.DomainModel.SourceControl.Changeset;


namespace APD.Client.Framework.SL.Repositories
{
    public class AsyncChangesetRepository : IAsyncRepository<Changeset>
    {
        ChangesetRepositoryServiceClient client = new ChangesetRepositoryServiceClient();

        public AsyncChangesetRepository()
        {
            client.Endpoint.Address =
                WebServiceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);

            client.GetCompleted += new EventHandler<GetCompletedEventArgs>(client_GetCompleted);
        }

        void client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            Specification<Changeset> specification = e.UserState as Specification<Changeset>;
            if (GetCompleted != null)
                GetCompleted(this, new GetCompletedEventArgs<Changeset>(e.Result, specification));
        }

        public void BeginGet(Specification<Changeset> specification)
        {
            client.GetAsync(specification, specification);
        }

        public event EventHandler<GetCompletedEventArgs<Changeset>> GetCompleted;
    }
}
