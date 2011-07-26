using System;
using System.Collections.Generic;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            var today = DateTime.Now.Date;
            var endDate = GetEndDate();
            var holidays = GetHolidays(today, endDate);

            if (endDate == null || holidays == null)
            {
                Response.Write("failure");
                return;
            }

            var iteration = new Iteration(today, endDate ?? today);
            var workingDaysLeft = iteration.CalculateWorkingdaysLeft(today, holidays).Days;

            var output = new List<string[]> {new[] {workingDaysLeft.ToString(), endDate.ToString()}};
            Response.Write(Csv.ToCsv(output));
        }

        private DateTime? GetEndDate()
        {
            try
            {
                var projectInfoServers = new ProjectInfoServerDatabaseRepository().Get(new AllSpecification<ProjectInfoServer>());
                var projectInfo = projectInfoServers.FirstOrDefault();
                return projectInfo.Projects.FirstOrDefault().CurrentIteration.EndDate;
            } catch (NullReferenceException)
            {
                return null;
            }
        }

        private IEnumerable<DayOfWeek> GetNonWorkingDays()
        {
            var spec = new AllSpecification<Configuration>();
            var config = new ConfigurationDatabaseRepository().Get(spec).FirstOrDefault();
            if (config == null) return null;
            var nonWorkingDays = config.GetSetting(WorkingDaysLeftController.SETTING_NON_WORK_DAYS).Vals;
            return nonWorkingDays.Select(str =>
                                             {
                                                 DayOfWeek day;
                                                 DayOfWeek.TryParse(str, out day);
                                                 return day;
                                             });
        }
        
        private IEnumerable<Holiday> GetHolidays(DateTime today, DateTime? endDate)
        {
            var nonWorkingDays = GetNonWorkingDays();
            if (endDate == null) return null;
            var spec = new HolidaySpecification()
                           {
                               EndDate = endDate ?? today,
                               StartDate = today,
                               NonWorkingDaysOfWeek = nonWorkingDays.ToList()
                           };
            return new HolidayDatabaseRepository().Get(spec);
        }

    }
}