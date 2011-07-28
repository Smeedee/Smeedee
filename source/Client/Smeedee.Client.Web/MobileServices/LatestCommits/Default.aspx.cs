using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Smeedee.Client.Web.Framework;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.MobileServices.LatestCommits
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly MobileServicesAuthenticator authenticator = new MobileServicesAuthenticator();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!authenticator.IsApiKeyValid(Request.QueryString["apiKey"] ?? "")) return;

            var changesets = GetAllChangesets().OrderByDescending(c => c.Revision);
            var revision = long.Parse(Request.QueryString["revision"] ?? "" + (changesets.First().Revision+1));
            var selectedChangesets = changesets.Where(c => c.Revision < revision).Take(10);
            Response.Write(Serialize(selectedChangesets));
        }

        private IEnumerable<Changeset> GetAllChangesets()
        {
            var spec = new AllSpecification<Changeset>();
            return new ChangesetDatabaseRepository().Get(spec);
        }

        private string Serialize(IEnumerable<Changeset> selectedChangesets)
        {
            var usernames = selectedChangesets.Select(c => c.Author.Username).Distinct();
            var users = UsernameToUserMapping.Map(usernames);
            var asStrings = selectedChangesets.Select(c => new[]
            {
                c.Comment, c.Time.ToString("yyyyMMddHHmmss"), c.Author.Username, c.Revision.ToString(), users[c.Author.Username].ImageUrl.ToString()
            });
            return Csv.ToCsv(asStrings);
        }
    }
}