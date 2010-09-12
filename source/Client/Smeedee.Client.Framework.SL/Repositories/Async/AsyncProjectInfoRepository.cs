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
using Smeedee.Client.Framework.SL.ProjectInfoRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.ProjectInfo;

namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncProjectInfoRepository : IAsyncRepository<ProjectInfoServer>
    {
        private readonly ProjectInfoRepositoryServiceClient client;

        public AsyncProjectInfoRepository()
        {
            client = new ProjectInfoRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);

            client.GetCompleted += Client_GetCompleted;
        }

        #region IAsyncRepository<ProjectInfoServer> Members

        public void BeginGet(Specification<ProjectInfoServer> specification)
        {
            client.GetAsync(specification, specification);
        }

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if( GetCompleted != null )
            {
                var specificationUsed = e.UserState as Specification<ProjectInfoServer>;

                GetCompletedEventArgs<ProjectInfoServer> eventArgs = null;
                if (e.Error != null)
                    eventArgs = new GetCompletedEventArgs<ProjectInfoServer>(specificationUsed, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<ProjectInfoServer>(e.Result, specificationUsed);
                
                GetCompleted(this, eventArgs);
            }
        }

        public event EventHandler<GetCompletedEventArgs<ProjectInfoServer>> GetCompleted;

        #endregion
    }
}
