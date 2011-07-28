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
        private readonly MobileServicesAuthenticator authenticator = new MobileServicesAuthenticator();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!authenticator.IsApiKeyValid(Request.QueryString["apiKey"] ?? "")) return;
            
            var timeSpan = TimeSpan.FromDays(int.Parse(Request.QueryString["days"] ?? "30"));
            var changesets = GetChangsets(timeSpan);
            var topCommitters = CalcTopCommitters(changesets);
            Response.Write(Csv.ToCsv(topCommitters));
        }

        private IEnumerable<string[]> CalcTopCommitters(IEnumerable<Changeset> changesets)
        {
            var counts = CountCommitsPerAuthor(changesets);
            var users = UsernameToUserMapping.Map(counts.Keys);
            return counts.Keys.Select(username => new[] { users[username].Name, counts[username].ToString(), users[username].ImageUrl });
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