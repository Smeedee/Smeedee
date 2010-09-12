using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.TeamPicture;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "http://smeedee.org")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceKnownType(typeof(CurrentTeamPictureSpecification))]
    public class TeamPictureRepositoryWebservice
    {
        [OperationContract]
        public IEnumerable<TeamPicture> Get(Specification<TeamPicture> specification)
        {
            TeamPictureDatabaseRepository repo = new TeamPictureDatabaseRepository(DefaultSessionFactory.Instance);
            IEnumerable<TeamPicture> result = new List<TeamPicture>();
            try
            {
                result = repo.Get(specification);
            }
            catch (Exception exception)
            {
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }

            return result;
        }

        [OperationContract]
        public void Save(IEnumerable<TeamPicture> teamPictures)
        {
            TeamPictureDatabaseRepository repo = new TeamPictureDatabaseRepository(DefaultSessionFactory.Instance);
            try
            {
                repo.Save(teamPictures);
            }
            catch (Exception exception)
            {
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }
        }
    }
}
