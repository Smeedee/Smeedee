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
    public class WebSnapshotWebserviceRepository : IRepository<WebSnapshot>//, IPersistDomainModels<WebSnapshot>
    {
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private IEnumerable<WebSnapshot> getResult;
        private WebSnapshotRepositoryServiceClient serviceClient;

        private Exception invocationException;

        public WebSnapshotWebserviceRepository()
        {
            getResult = new List<WebSnapshot>();

            serviceClient = new WebSnapshotRepositoryServiceClient();
            serviceClient.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(serviceClient.Endpoint.Address);

            serviceClient.GetCompleted += (sender, eventArgs) =>
                                       {
                                           invocationException = eventArgs.Error;
                                           getResult = eventArgs.Result;
                                           resetEvent.Set();
                                       };
        }
        
        public IEnumerable<WebSnapshot> Get(Specification<WebSnapshot> specification)
        {
            serviceClient.GetAsync(specification);
            resetEvent.Reset();
            resetEvent.WaitOne();

            if (invocationException != null)
            {
                throw invocationException;
            }

            return getResult;
        }

        //public void Save(WebSnapshot domainModel)
        //{
        //    serviceClient.SaveAsync(new List<WebSnapshot> {domainModel});
        //}

        //public void Save(IEnumerable<WebSnapshot> domainModels)
        //{
        //    serviceClient.SaveAsync(domainModels.ToList());
        //}
    }
}
