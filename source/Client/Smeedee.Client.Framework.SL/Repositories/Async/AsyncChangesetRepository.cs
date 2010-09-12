using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.SL.ChangesetRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;

namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncChangesetRepository : IAsyncRepository<Changeset>
    {
        private readonly ChangesetRepositoryServiceClient client;
        public AsyncChangesetRepository()
        {
            client = new ChangesetRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            
            client.GetCompleted += Client_GetCompleted;
        }

        #region  IAsyncRepository<Changeset> Members

        public void BeginGet(Specification<Changeset> specification)
        {
            client.GetAsync(specification, specification);
        }

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if( GetCompleted != null )
            {
                var specificationUsed = e.UserState as Specification<Changeset>;
                GetCompletedEventArgs<Changeset> eventArgs = null;
                if (e.Error != null)
                    eventArgs = new GetCompletedEventArgs<Changeset>(specificationUsed, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<Changeset>(e.Result, specificationUsed);

                GetCompleted(this, eventArgs);
            }
        }

        public event EventHandler<GetCompletedEventArgs<Changeset>> GetCompleted;

        #endregion
    }
}
