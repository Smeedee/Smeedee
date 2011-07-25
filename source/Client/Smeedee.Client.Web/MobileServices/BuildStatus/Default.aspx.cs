using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Smeedee.Client.Web.Framework;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Users;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Client.Web.MobileServices.BuildStatus
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var userdb = new UserdbDatabaseRepository().Get(new DefaultUserdbSpecification()).FirstOrDefault();
            var currentBuilds = GetAllCurrentBuilds();
            var allUsers = (userdb == null) ? new List<User>() : userdb.Users;

            var formattedBuilds = currentBuilds.Select(
                build => new[]
                            {
                                build.Project.ProjectName, 
                                GetNameFromUserList(build.Trigger.InvokedBy, allUsers), 
                                ConvertBuildStatusToMobileFormat(build.Status), 
                                build.FinishedTime.ToString("yyyyMMddHHmmss")
                            }).ToList();

            Response.Write(Csv.ToCsv(formattedBuilds));
        }

        private static string GetNameFromUserList(string invokedBy, IEnumerable<User> allUsers)
        {
            var userData = allUsers.Where(u => u.Username == invokedBy);
            return (userData.Count() > 0) ? userData.First().Firstname + " " + userData.First().Surname : invokedBy;
        }

        private static IEnumerable<Build> GetAllCurrentBuilds()
        {
            var buildRepo = new CIServerDatabaseRepository();
            var ciServers = buildRepo.Get(new AllSpecification<CIServer>());
            var projects = new List<CIProject>();

            foreach (var server in ciServers)
            {
                projects.AddRange(server.Projects);
            }

            return projects.Select(p => p.LatestBuild);
        }

        private static string ConvertBuildStatusToMobileFormat(DomainModel.CI.BuildStatus status)
        {
            switch (status)
            {
                case DomainModel.CI.BuildStatus.FinishedSuccefully:
                    return "Working";
                case DomainModel.CI.BuildStatus.FinishedWithFailure:
                    return "Broken";
            }
            return "Unknown";
        }
    }
}