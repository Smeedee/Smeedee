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
using System.Linq;
using Smeedee.Client.Framework.SL.UserRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Users;

//Note: Obsolete, webservice needs a Userdb reference, so this will only ask for users in default. Use AsyncUserdbRepository instead!
namespace Smeedee.Client.Framework.SL.Repositories.Async
{
    public class AsyncUserRepository  : IAsyncRepository<User>, IPersistDomainModelsAsync<User>
    {
        private readonly UserRepositoryServiceClient client;
        private Specification<User> specificationCache;


        public AsyncUserRepository()
        {
            client = new UserRepositoryServiceClient();
            client.Endpoint.Address =
                WebserviceEndpointResolver.ResolveDynamicEndpointAddress(client.Endpoint.Address);
            
            client.GetCompleted += Client_GetCompleted;
            client.SaveCompleted += Client_SaveCompleted;
        }
        #region  IAsyncRepository<User> Members

        private void Client_GetCompleted(object sender, GetCompletedEventArgs e)
        {
            if( GetCompleted != null )
            {
                var userdbs = e.Result;
                var tmpUsersCache = new List<User>();

                if (userdbs.Count() > 0)
                {
                    tmpUsersCache.AddRange(userdbs.First().Users);
                }
                var retValue = tmpUsersCache.Where(u => specificationCache.IsSatisfiedBy(u)).ToList();
                var specificationUsed = e.UserState as Specification<User>;

                GetCompletedEventArgs<User> eventArgs = null;
                if (e.Error != null)
                    eventArgs = new GetCompletedEventArgs<User>(specificationUsed, e.Error);
                else
                    eventArgs = new GetCompletedEventArgs<User>(retValue, specificationUsed);

                GetCompleted(this, eventArgs);
            }
        }

        [Obsolete("Doesn't specify userdb")]
        public void BeginGet(Specification<User> specification)
        {
            specificationCache = specification;
            var workAroundSpecification = new UserdbNameSpecification("default");
            client.GetAsync(workAroundSpecification, workAroundSpecification);
        }

        public event EventHandler<GetCompletedEventArgs<User>> GetCompleted;

        #endregion

        #region IPersistDomainModelsAsync<User> Members

        private void Client_SaveCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (SaveCompleted != null)
            {
                var eventArgs = new SaveCompletedEventArgs(e.Error);
                SaveCompleted(this, eventArgs);
            }
        }
        [Obsolete("Doesn't specify userdb. Use IPersistDomainModelsAsync<UserDb> instead!")]
        public void Save(User domainModel)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Doesn't specify userdb")]
        public void Save(IEnumerable<User> domainModels)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<SaveCompletedEventArgs> SaveCompleted;

        #endregion
    }
}
