using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Smeedee.Client.Web.Framework;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.Users;
using Smeedee.Integration.Database.DomainModel.Repositories;
namespace Smeedee.Client.Web.MobileServices
{
    public partial class TopCommitters : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            var userdb = new UserdbDatabaseRepository().Get(new DefaultUserdbSpecification()).FirstOrDefault();

            var allUsers = userdb == null ? new List<User>() : userdb.Users;
            var changesets = GetChangsets(TimeSpan.FromDays(30));
            var topCommitters = CalcTopCommitters(changesets, allUsers);
            Response.Write(Csv.ToCsv(topCommitters));
        }

        private IEnumerable<string[]> CalcTopCommitters(IEnumerable<Changeset> changesets, IEnumerable<User> allUsers)
        {
            var counts = CountCommitsPerAuthor(changesets);
            var users = CreateUsernameToUserMap(counts.Keys, allUsers);
            return counts.Keys.Select(username => new[] { users[username].Name, counts[username].ToString(), users[username].ImageUrl });
        }

        private Dictionary<string, User> CreateUsernameToUserMap(IEnumerable<string> usernames, IEnumerable<User> users)
        {
            var map = new Dictionary<string, User>();
            foreach (var username in usernames)
            {
                var match = users.Where(u => u.Username == username);
                map[username] = match.Count() > 0
                                    ? match.First()
                                    : new User(username) {ImageUrl = ""};
            }
            return map;
        }

        private static Dictionary<string, int> CountCommitsPerAuthor(IEnumerable<Changeset> changesets)
        {
            var counts = new Dictionary<string, int>();
            foreach (var changeset in changesets)
            {
                var username = changeset.Author.Username;
                if (!counts.ContainsKey(username))
                    counts[username] = 0;
                counts[username] += 1;
            }
            return counts;
        }

        private static IEnumerable<Changeset> GetChangsets(TimeSpan timePeriod)
        {
            var spec = new LinqSpecification<Changeset>(changeset => DateTime.Now - changeset.Time < timePeriod);
            return new ChangesetDatabaseRepository().Get(spec);
        }
    }
}