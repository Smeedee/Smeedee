using System.ComponentModel.Composition;
using System.Linq;
using System.Collections.Generic;
using Smeedee.Client.Framework.SL.UserRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Users;

namespace Smeedee.Client.Framework.SL.Repositories
{
    public class UserWebserviceRepositoryProxy : IRepository<User>
    {
        private UserRepositoryServiceClient serviceClient;
        private static List<User> usersCache = new List<User>();

        public UserWebserviceRepositoryProxy()
        {
            serviceClient = new UserRepositoryServiceClient();
            serviceClient.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(serviceClient.Endpoint.Address);

            serviceClient.GetCompleted += (o, e) =>
            {
                var servers = e.Result;
                if (servers.Count() > 0)
                {
                    var tmpUsersCache = new List<User>();            
                    foreach (var user in servers.First().Users)
                    {
                        tmpUsersCache.Add(user);
                    }
                    usersCache = tmpUsersCache;
                }
            };
        }

        public IEnumerable<User> Get(Specification<User> specification)
        {
            serviceClient.GetAsync(new UserdbNameSpecification("default"));
            var retValue = usersCache.Where(u => specification.IsSatisfiedBy(u)).ToList();
            return retValue;
        }
    }
}
