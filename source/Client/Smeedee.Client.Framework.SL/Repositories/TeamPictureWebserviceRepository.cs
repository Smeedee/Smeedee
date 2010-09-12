using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Smeedee.Client.Framework.SL.TeamPictureService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TeamPicture;

namespace Smeedee.Client.Framework.SL.Repositories
{
    public class TeamPictureWebserviceRepository : IRepository<TeamPicture>, IPersistDomainModels<TeamPicture>
    {
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private List<TeamPicture> TeamPictures = new List<TeamPicture>();
        private TeamPictureRepositoryWebserviceClient client;

        private Exception invocationException;

        public TeamPictureWebserviceRepository()
        {
            TeamPictures = new List<TeamPicture>();

            client = new TeamPictureRepositoryWebserviceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            client.GetCompleted += new EventHandler<GetCompletedEventArgs>(Client_GetCompleted);
        }

        #region ICIProjectRepository Members

        public IEnumerable<TeamPicture> Get(Specification<TeamPicture> specification)
        {
            client.GetAsync(specification);
            resetEvent.Reset();
            resetEvent.WaitOne();

            if (invocationException != null)
                throw invocationException;

            var servers = TeamPictures;
            return servers;
        }

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {

            var resultingTeamPictures = e.Result;

            if (resultingTeamPictures != null)
            {
                this.TeamPictures = resultingTeamPictures;
            }

            invocationException = e.Error;

            resetEvent.Set();
        }

        #endregion

        #region IPersistDomainModels<TeamPicture> Members

        public void Save(TeamPicture domainModel)
        {
            client.SaveAsync(new List<TeamPicture> {domainModel});
        }

        public void Save(IEnumerable<TeamPicture> domainModels)
        {
            client.SaveAsync(domainModels.ToList());
        }

        #endregion

    }
}
