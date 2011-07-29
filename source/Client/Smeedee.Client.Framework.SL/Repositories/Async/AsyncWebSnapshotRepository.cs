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
using Smeedee.Client.Framework.SL.WebSnapshotRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.WebSnapshot;

namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncWebSnapshotRepository : IAsyncRepository<WebSnapshot>
    {

        private readonly WebSnapshotRepositoryServiceClient client;
        public AsyncWebSnapshotRepository()
        {
            client = new WebSnapshotRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);

            client.GetCompleted += Client_GetCompleted;
        }

        public void BeginGet(Specification<WebSnapshot> specification)
        {
            client.GetAsync();
        }
        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if (GetCompleted != null)
            {
                //var specificationUsed = e.UserState as Specification<WebSnapshot>;
                GetCompletedEventArgs<WebSnapshot> eventArgs = null;
                if (e.Error != null)
                    eventArgs = new GetCompletedEventArgs<WebSnapshot>(null, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<WebSnapshot>(e.Result, null);

                GetCompleted(this, eventArgs);
            }
        }
        public event EventHandler<GetCompletedEventArgs<WebSnapshot>> GetCompleted;
    }
}
