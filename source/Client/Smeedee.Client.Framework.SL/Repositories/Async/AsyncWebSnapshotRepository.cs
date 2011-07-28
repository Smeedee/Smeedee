using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AsyncWebSnapshotRepository : IAsyncRepository<WebSnapshot>//, IPersistDomainModelsAsync<WebSnapshot>
    {
        private readonly WebSnapshotRepositoryServiceClient client;

        public AsyncWebSnapshotRepository()
        {
            client = new WebSnapshotRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);

            client.GetCompleted += client_GetCompleted;
            //client.SaveCompleted += client_SaveCompleted;
        }

        void client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if (GetCompleted != null)
            {
                var specificationUsed = e.UserState as Specification<WebSnapshot>;
                GetCompletedEventArgs<WebSnapshot> eventArgs = null;
                if (e.Error != null)
                {
                    eventArgs = new GetCompletedEventArgs<WebSnapshot>(specificationUsed, e.Error);
                }
                else
                {
                    eventArgs = new GetCompletedEventArgs<WebSnapshot>(e.Result, specificationUsed);
                }

                GetCompleted(this, eventArgs);
            }
        }

        //void client_SaveCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //{
        //    if (SaveCompleted != null)
        //    {
        //        var eventArgs = new SaveCompletedEventArgs(e.Error);
        //        SaveCompleted(this, eventArgs);
        //    }
        //}




        public void BeginGet(Specification<WebSnapshot> specification)
        {
            client.GetAsync(specification, specification);
        }

        public event EventHandler<GetCompletedEventArgs<WebSnapshot>> GetCompleted;
        //public void Save(WebSnapshot domainModel)
        //{
        //    client.SaveAsync(new List<WebSnapshot> {domainModel});
        //}

        //public void Save(IEnumerable<WebSnapshot> domainModels)
        //{
        //    client.SaveAsync(domainModels.ToList());
        //}

        //public event EventHandler<SaveCompletedEventArgs> SaveCompleted;
    }
}
