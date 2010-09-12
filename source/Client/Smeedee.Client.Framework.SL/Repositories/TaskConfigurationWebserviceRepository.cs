using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Smeedee.Client.Framework.SL.TaskConfigurationRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;

namespace Smeedee.Client.Framework.SL.Repositories
{
    public class TaskConfigurationWebserviceRepository : IRepository<TaskConfiguration>, IPersistDomainModels<TaskConfiguration>, IDeleteDomainModels<TaskConfiguration>
    {
        private TaskConfigurationRepositoryWebserviceClient client;
        private ManualResetEvent resetEvent;
        private Exception invocationException;
        private IEnumerable<TaskConfiguration> result;

        public TaskConfigurationWebserviceRepository()
        {
            client = new TaskConfigurationRepositoryWebserviceClient();

            resetEvent = new ManualResetEvent(false);

            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            client.GetCompleted += new EventHandler<GetCompletedEventArgs>(Client_GetCompleted);
        }

        public IEnumerable<TaskConfiguration> Get(Specification<TaskConfiguration> specification)
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
        public void Save(TaskConfiguration taskConfig)
        {
            client.SaveAsync(new List<TaskConfiguration> {taskConfig});
        }

        public void Save(IEnumerable<TaskConfiguration> taskConfigs)
        {
            client.SaveAsync(taskConfigs.ToList());
        }

        public void Delete(Specification<TaskConfiguration> specification)
        {
            client.DeleteAsync(specification);
        }
    }
}
