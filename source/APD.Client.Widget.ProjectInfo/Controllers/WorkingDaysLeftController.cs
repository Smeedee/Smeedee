#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using APD.Client.Framework;
using APD.Client.Framework.Controllers;
using APD.Client.Widget.ProjectInfo.ViewModels;
using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;
using APD.DomainModel.Config;


namespace APD.Client.Widget.ProjectInfo.Controllers
{
    public class WorkingDaysLeftController : ControllerBase<WorkingDaysLeftViewModel>
    {
        protected override void OnNotifiedToRefresh(object sender, RefreshEventArgs e)
        {
            LoadData();
        }

        private readonly IRepository<ProjectInfoServer> repository;
        private readonly IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> asyncClient;
        private readonly IRepository<Configuration> configRepository;
        private readonly IPersistDomainModels<Configuration> configPersisterRepository;

        public Project CurrentProject { get; private set; }

        public WorkingDaysLeftController(IRepository<ProjectInfoServer> repository,
                                         IRepository<Configuration> configRepository,
                                         IPersistDomainModels<Configuration> configPersisterRepository,
                                         INotifyWhenToRefresh refreshNotifier,
                                         IInvokeUI uiInvoker,
                                         IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> backgroundWorkerInvoker)
            : base(refreshNotifier, uiInvoker)
        {
            this.repository = repository;
            this.asyncClient = backgroundWorkerInvoker;
            this.configRepository = configRepository;
            this.configPersisterRepository = configPersisterRepository;

            LoadData();
        }

        private void LoadData()
        {
            asyncClient.RunAsyncVoid(() =>
            {
                try
                {
                    var allConfigSpec = new AllSpecification<Configuration>();
                    var config = configRepository
                                    .Get(allConfigSpec)
                                    .Where(c => c.Name.Equals(("project-info")))
                                    .SingleOrDefault();

                    string settingValue = LoadSettings(config);

                    if (settingValue == "true")
                    {
                        SetEndDateFromConfig(config);
                    }
                    else if(settingValue == "false")
                    {
                        SetEndDateFromProjectInfo();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[WARNING] : " +
                            "Setting value for project 'use-config-repo' does not have a valid value. Value is " + settingValue);

                        ViewModel.HasConfigError = true;
                    }
                }
                catch (Exception)
                {
                    SetProjectStatusError();
                }

            });
        }

        private void SetEndDateFromProjectInfo()
        {
            var servers = repository.Get(new AllSpecification<ProjectInfoServer>());
            
            if (servers != null && servers.Count() > 0)
            {
                var projects = servers.First().Projects;

                if (projects.Count() == 0)
                {
                    ViewModel.ProjectsInRepository = false;
                }
                else if (projects.Count() > 0)
                {
                    // TODO: This is just temporary - should get the current project based on date and status?
                    CurrentProject = projects.First();
                    SetProjectStatus();
                }
            }
            else
            {
                SetProjectStatusError();
            }
        }

        private void SetEndDateFromConfig(Configuration config)
        {
            SettingsEntry endDateEntry = config.GetSetting("end-date");
            string endDateSetting = endDateEntry.Value;

            try
            {
                var endDate = DateTime.Parse(endDateSetting, new CultureInfo("en-US"));
                SetProjectStatusFromEndDate(endDate);
            }
            catch (FormatException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        private string LoadSettings(Configuration config)
        {
            string settingValue = "false";

            if (config != null)
            {
                SettingsEntry setting = config.GetSetting("use-config-repo");

                if (setting != null)
                {
                    settingValue = setting.Value;
                }
            }
            else
            {
                var newConfig = new Configuration("project-info");
                newConfig.NewSetting("use-config-repo", "false");
                newConfig.NewSetting("end-date", string.Empty);
                configPersisterRepository.Save(newConfig);
            }

            return settingValue;
        }

        private void SetProjectStatusError()
        {
            ViewModel.HasConnectionProblems = true;
            ViewModel.ProjectsInRepository = false;
            ViewModel.IterationInProject = false;
        }

        private void SetProjectStatusFromEndDate(DateTime endDate)
        {
            DateTime startDate = DateTime.Today.AddDays(-1);
            if (endDate < startDate)
            {
                startDate = endDate.AddDays(-1);
            }

            CurrentProject = new Project();
            var iteration = new Iteration(startDate, endDate, new HolidayProvider());
            CurrentProject.Iterations = new List<Iteration>() { iteration };
            SetProjectStatus();
        }

        private void SetProjectStatus()
        {
            if (CurrentProject.CurrentIteration == null)
            {
                SetCurrentProjectHasNoIteration();
            }
            else if (CurrentProject.CurrentIteration.IsOvertime(DateTime.Now))
            {
                SetCurrentProjectIsOnOvertime();
            }
            else
            {
                SetCurrentProjectHasDaysRemaining();
            }
        }

        private void SetCurrentProjectHasNoIteration()
        {
            ViewModel.ProjectsInRepository = true;
            ViewModel.IterationInProject = false;
        }

        private void SetCurrentProjectIsOnOvertime()
        {
            ViewModel.ProjectsInRepository = true;
            ViewModel.IterationInProject = true;
            ViewModel.DaysRemaining = GetDaysRemaining();
            ViewModel.IsOnOvertime = true;
        }

        private void SetCurrentProjectHasDaysRemaining()
        {
            ViewModel.ProjectsInRepository = true;
            ViewModel.IterationInProject = true;
            ViewModel.DaysRemaining = GetDaysRemaining();
        }

        private int GetDaysRemaining()
        {

            if (CurrentProject == null)
                throw new ArgumentNullException("CurrentProject");

            if (CurrentProject.CurrentIteration == null)
                throw new ArgumentNullException("CurrentProject.CurrentIteration");

            return CurrentProject.CurrentIteration.CalculateWorkingdaysLeft(DateTime.Now).Days;

        }
    }
}