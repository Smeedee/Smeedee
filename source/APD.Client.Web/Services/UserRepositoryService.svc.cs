using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;
using APD.DomainModel.Framework;
using APD.DomainModel.Framework.Logging;
using APD.DomainModel.Users;
using APD.Integration.Database.DomainModel.Repositories;


namespace APD.Client.Web.Services
{
    [ServiceContract(Namespace = "")]
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
                ILog logger = new DatabaseLogger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }

            return result;
        }

        [OperationContract]
        public void Save(Userdb userdb)
        {
            try
            {
                userRepository.Save(userdb);
            }
            catch (Exception exception)
            {
                ILog logger = new DatabaseLogger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }
        }
    }
}
