using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;
using APD.DomainModel.Framework;
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
            userRepository = new UserdbDatabaseRepository();
        }

        [OperationContract]
        [ServiceKnownType(typeof(AllSpecification<Userdb>))]
        [ServiceKnownType(typeof(UserdbNameSpecification))]
        public IEnumerable<Userdb> Get(Specification<Userdb> specification)
        {
            return userRepository.Get(specification);
        }

        [OperationContract]
        public void Save(Userdb userdb)
        {
            userRepository.Save(userdb);
        }
    }
}
