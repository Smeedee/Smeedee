using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
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

namespace Smeedee.Client.Framework.SL.Repositories
{
    public class WebSnapshotWebserviceRepository : IRepository<WebSnapshot>, IPersistDomainModels<WebSnapshot>
    {
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private List<WebSnapshot> WebSnapshots;
        private WebSnapshotRepositoryServiceClient client;

        private Exception invocationException;

        public WebSnapshotWebserviceRepository()
        {
            WebSnapshots = new List<WebSnapshot>();

            client = new WebSnapshotRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            client.GetCompleted += client_GetCompleted;
        }

        void client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            var resultingWebSnapshots = e.Result;
            if (resultingWebSnapshots != null)
            {
                this.WebSnapshots = resultingWebSnapshots.ToList();
            }

            invocationException = e.Error;
            resetEvent.Set();
        }
        
        public IEnumerable<WebSnapshot> Get(Specification<WebSnapshot> specification)
        {
            client.GetAsync(specification);
            resetEvent.Reset();
            resetEvent.WaitOne();

            if (invocationException != null)
            {
                throw invocationException;
            }

            var servers = WebSnapshots;
            return servers;
        }

        public void Save(WebSnapshot domainModel)
        {
            client.SaveAsync(new WebSnapshot[] {domainModel});
        }

        public void Save(IEnumerable<WebSnapshot> domainModels)
        {
            client.SaveAsync(domainModels.ToArray());
        }
    }
}
