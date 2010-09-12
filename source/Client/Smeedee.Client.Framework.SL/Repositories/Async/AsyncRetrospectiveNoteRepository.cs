using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.SL.RetrospectiveNoteRepositoryService;
using Smeedee.DomainModel.Corkboard;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.ProjectInfo;

namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncRetrospectiveNoteRepository : IAsyncRepository<RetrospectiveNote>, IPersistDomainModelsAsync<RetrospectiveNote>, IDeleteDomainModelsAsync<RetrospectiveNote>
    {
        private readonly RetrospectiveNoteRepositoryServiceClient client;

        public AsyncRetrospectiveNoteRepository()
        {
            client = new RetrospectiveNoteRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);

            client.GetCompleted += Client_GetCompleted;
            client.SaveCompleted += Client_SaveCompleted;
            client.DeleteCompleted += Client_DeleteCompleted;
        }

        #region  IAsyncRepository<RetrospectiveNote> Members

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if (GetCompleted != null)
            {
                var specificationUsed = e.UserState as Specification<RetrospectiveNote>;

                GetCompletedEventArgs<RetrospectiveNote> eventArgs = null;
                if (e.Error != null)
                    eventArgs = new GetCompletedEventArgs<RetrospectiveNote>(specificationUsed, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<RetrospectiveNote>(e.Result, specificationUsed);
                
                GetCompleted(this, eventArgs);
            }
        }


        public void BeginGet(Specification<RetrospectiveNote> specification)
        {
            client.GetAsync(specification, specification);
        }

        public event EventHandler<GetCompletedEventArgs<RetrospectiveNote>> GetCompleted;

        #endregion 

        #region IPersistDomainModelsAsync<RetrospectiveNote> Members

        void Client_SaveCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (SaveCompleted != null)
            {
                var eventArgs = new SaveCompletedEventArgs(e.Error);
                SaveCompleted(this, eventArgs);
            }
        }

        public void Save(RetrospectiveNote domainModel)
        {
            var configList = new List<RetrospectiveNote> { domainModel };
            client.SaveAsync(configList);
        }

        public void Save(IEnumerable<RetrospectiveNote> domainModels)
        {
            var configList = new List<RetrospectiveNote>(domainModels);
            client.SaveAsync(configList);
        }

        public event EventHandler<SaveCompletedEventArgs> SaveCompleted;

        #endregion

        #region IDeleteDomainModelsAsync<RetrospectiveNote> Members

        private void Client_DeleteCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if(DeleteCompleted != null)
            {
                var specificationUsed = e.UserState as Specification<RetrospectiveNote>;
                var eventArgs = new DeleteCompletedEventArgs<RetrospectiveNote>(e.Error, specificationUsed);
                DeleteCompleted(this, eventArgs);
            }
        }

        public void Delete(Specification<RetrospectiveNote> specification)
        {
            client.DeleteAsync(specification, specification);
        }

        public event EventHandler<DeleteCompletedEventArgs<RetrospectiveNote>> DeleteCompleted;

        #endregion
    }
}
