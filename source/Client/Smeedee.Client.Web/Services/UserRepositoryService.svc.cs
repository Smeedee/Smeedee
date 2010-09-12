using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Users;
using Smeedee.Integration.Database.DomainModel.Repositories;


namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "http://smeedee.org")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class UserRepositoryService
    {
        private UserdbDatabaseRepository userRepository;

        public UserRepositoryService()
        {
            userRepository = new UserdbDatabaseRepository(DefaultSessionFactory.Instance);
        }

        [OperationContract]
        [ServiceKnownType(typeof(AllSpecification<Userdb>))]
        [ServiceKnownType(typeof(UserdbNameSpecification))]
        public IEnumerable<Userdb> Get(Specification<Userdb> specification)
        {
            IEnumerable<Userdb> result = new List<Userdb>();
            try
            {
                result = userRepository.Get(specification);
            }
            catch (Exception exception)
            {
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }

            return result;
        }

        [OperationContract]
        public void Save(IEnumerable<Userdb> userdb)
        {
            try
            {
                userRepository.Save(userdb);
            }
            catch (Exception exception)
            {
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }
        }
    }
}
