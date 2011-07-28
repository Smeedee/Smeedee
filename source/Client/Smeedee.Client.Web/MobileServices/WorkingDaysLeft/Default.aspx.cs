using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Smeedee.Client.Web.Framework;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Holidays;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Framework;
using Smeedee.Integration.Database.DomainModel.Repositories;
using Smeedee.Widget.ProjectInfo;

namespace Smeedee.Client.Web.MobileServices.WorkingDaysLeft
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly MobileServicesAuthenticator authenticator = new MobileServicesAuthenticator();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!authenticator.IsApiKeyValid(Request.QueryString["apiKey"] ?? "")) return;

            try
            {
                Response.Write(CreateOutput());
            } catch (Exception ex)
            {
                Response.Write("Failed to retrieve sprint end date:" + ex);
            }
        }

        private string CreateOutput()
        {
            var today = DateTime.Now.Date;
            var config = GetConfiguration();
            var endDate = GetEndDate(config);
            var workingDaysLeft = CalcWorkingDaysLeft(config, today, endDate);

            var output = new List<string[]> { new[] { workingDaysLeft.ToString(), endDate.ToString() } };
            return Csv.ToCsv(output);
        }
        
        private Configuration GetConfiguration()
        {
            var spec = new LinqSpecification<Configuration>(c => c.Name == WorkingDaysLeftController.CONFIG_NAME);
            return new ConfigurationDatabaseRepository().Get(spec).FirstOrDefault();
        }

        private DateTime GetEndDate(Configuration config)
        {
            var isManuallyConfigured = config.GetSetting(WorkingDaysLeftController.SETTING_IS_MANUALLY_CONFIGURED);
            return bool.Parse(isManuallyConfigured.Value)
                       ? GetEndDateFromConfig(config)
                       : GetEndDateFromProjectInfo();
        }

        private static DateTime GetEndDateFromProjectInfo()
        {
            var projectInfoServers = new ProjectInfoServerDatabaseRepository().Get(new AllSpecification<ProjectInfoServer>());
            var projectInfo = projectInfoServers.FirstOrDefault();
            return projectInfo.Projects.FirstOrDefault().CurrentIteration.EndDate;
        }

        private DateTime GetEndDateFromConfig(Configuration config)
        {
            var endDate = config.GetSetting(WorkingDaysLeftController.SETTING_END_DATE).Value;
            return DateTime.Parse(endDate, CultureInfo.InvariantCulture);
        }


        private int CalcWorkingDaysLeft(Configuration config, DateTime today, DateTime endDate)
        {
            int workingDaysLeft;
            if (endDate < today)
            {
                workingDaysLeft = (endDate - today).Days;
            }
            else
            {
                var holidays = GetHolidays(config, today, endDate);
                workingDaysLeft = new Iteration(today, endDate).CalculateWorkingdaysLeft(today, holidays).Days;
            }
            return workingDaysLeft;
        }
        
        private IEnumerable<Holiday> GetHolidays(Configuration config, DateTime today, DateTime? endDate)
        {
            var nonWorkingDays = GetNonWorkingDays(config);
            if (endDate == null) return null;
            var spec = new HolidaySpecification()
                           {
                               EndDate = endDate ?? today,
                               StartDate = today,
                               NonWorkingDaysOfWeek = nonWorkingDays.ToList()
                           };
            return new HolidayDatabaseRepository().Get(spec);
        }

        private IEnumerable<DayOfWeek> GetNonWorkingDays(Configuration config)
        {
            var nonWorkingDays = config.GetSetting(WorkingDaysLeftController.SETTING_NON_WORK_DAYS).Vals;
            return nonWorkingDays.Select(str =>
            {
                DayOfWeek day;
                DayOfWeek.TryParse(str, out day);
                return day;
            });
        }
    }
}