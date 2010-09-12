using System;
using System.Collections.Generic;
using System.Threading;
using Smeedee.Client.Framework.SL.TaskDefinitionRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskDefinition;

namespace Smeedee.Client.Framework.SL.Repositories
{
    public class TaskDefinitionWebserviceRepository : IRepository<TaskDefinition>
    {
        private TaskDefinitionRepositoryServiceClient client;
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private List<TaskDefinition> taskDefinitions;
        private Exception invocationException;

        public TaskDefinitionWebserviceRepository()
        {
            taskDefinitions = new List<TaskDefinition>();

            client = new TaskDefinitionRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            client.GetCompleted += Client_GetCompleted;
        }

        public IEnumerable<TaskDefinition> Get(Specification<TaskDefinition> specification)
        {
            client.GetAsync(specification);
            resetEvent.Reset();
            resetEvent.WaitOne();

            if (invocationException != null)
                throw invocationException;
 
            return taskDefinitions;
        }

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                taskDefinitions = e.Result;
            }

            invocationException = e.Error;
            resetEvent.Set();
        }
    }
}