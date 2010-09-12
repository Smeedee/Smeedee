using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Smeedee.Client.Framework.SL.TeamPictureService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TeamPicture;

namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncTeamPictureRepository: IAsyncRepository<TeamPicture>, IPersistDomainModelsAsync<TeamPicture>
    {
        private readonly TeamPictureRepositoryWebserviceClient client;

        public AsyncTeamPictureRepository()
        {
            client = new TeamPictureRepositoryWebserviceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            
            client.GetCompleted += Client_GetCompleted;
            client.SaveCompleted += Client_SaveCompleted;
        }

        #region IAsyncRepository<TeamPicture> Members
        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if( GetCompleted != null )
            {
                var specificationUsed = e.UserState as Specification<TeamPicture>;
                GetCompletedEventArgs<TeamPicture> eventArgs = null;
                if (e.Error != null)
                    eventArgs = new GetCompletedEventArgs<TeamPicture>(specificationUsed, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<TeamPicture>(e.Result, specificationUsed); 
                
                GetCompleted(this, eventArgs);
            }
        }

        public void BeginGet(Specification<TeamPicture> specification)
        {
            client.GetAsync(specification, specification);
        }

        public event EventHandler<GetCompletedEventArgs<TeamPicture>> GetCompleted;

        #endregion

        #region IPersistDomainModelsAsync<TeamPicture> Members
        private void Client_SaveCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (SaveCompleted != null)
            {
                var eventArgs = new SaveCompletedEventArgs(e.Error);
                SaveCompleted(this, eventArgs);
            }
        }

        public void Save(TeamPicture domainModel)
        {
            client.SaveAsync(new List<TeamPicture> {domainModel});
        }

        //Note: Method will produce one SaveCompleted event for each item in collection (modify IPersisterDomainModel with SaveAllComplete event to fix)
        public void Save(IEnumerable<TeamPicture> domainModels)
        {
            client.SaveAsync(domainModels.ToList());
        }

        public event EventHandler<SaveCompletedEventArgs> SaveCompleted;

        #endregion
    }
}
