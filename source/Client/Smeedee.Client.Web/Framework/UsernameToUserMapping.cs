using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Smeedee.DomainModel.Users;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.Framework
{
    public static class UsernameToUserMapping
    {
        public static Dictionary<string, User> Map(IEnumerable<string> usernames)
        {
            var map = new Dictionary<string, User>();
            var userdb = new UserdbDatabaseRepository().Get(new DefaultUserdbSpecification()).FirstOrDefault();
            var users = userdb == null ? new List<User>() : userdb.Users;

            foreach (var username in usernames)
            {
                var match = users.Where(u => u.Username == username || u.Name == username);
                map[username] = match.FirstOrDefault() ?? new User(username) { ImageUrl = "" };
            }
            return map;
        }
    }
}
