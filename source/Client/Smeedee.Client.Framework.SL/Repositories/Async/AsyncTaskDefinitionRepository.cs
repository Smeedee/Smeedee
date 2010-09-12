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
using Smeedee.Client.Framework.SL.TaskDefinitionRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskDefinition;

namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncTaskDefinitionRepository :  IAsyncRepository<TaskDefinition>
    {
        private readonly TaskDefinitionRepositoryServiceClient client;

        public AsyncTaskDefinitionRepository()
        {
            client = new TaskDefinitionRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);

            client.GetCompleted += Client_GetCompleted;
        }

        #region IAsyncRepository<TaskDefinition> Members

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if (GetCompleted != null)
            {
                var specificationUsed = e.UserState as Specification<TaskDefinition>;
                GetCompletedEventArgs<TaskDefinition> eventArgs = null;
                if (e.Error != null)
                    eventArgs = new GetCompletedEventArgs<TaskDefinition>(specificationUsed, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<TaskDefinition>(e.Result, specificationUsed); 
                
                GetCompleted(this, eventArgs);
            }
        }

        public void BeginGet(Specification<TaskDefinition> specification)
        {
            client.GetAsync(specification, specification);
        }

        public event EventHandler<GetCompletedEventArgs<TaskDefinition>> GetCompleted;

        #endregion
    }
}
