﻿#region File header

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
// The project webpage is located at http://www.smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Holidays;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Widget.ProjectInfo.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.ProjectInfo.Controllers
{
    public class WorkingDaysLeftController : ControllerBase<WorkingDaysLeftViewModel>
    {
        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            LoadData();
        }

        private readonly IRepository<ProjectInfoServer> repository;
        private readonly IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> asyncClient;
        private readonly ILog logger;
        private readonly IRepository<Configuration> configRepository;
        private readonly IPersistDomainModels<Configuration> configPersisterRepository;
        private IRepository<Holiday> holidayRepository;

        public Project CurrentProject { get; private set; }

        public WorkingDaysLeftController(WorkingDaysLeftViewModel viewmodel,
                                        IRepository<ProjectInfoServer> repository,
                                         IRepository<Configuration> configRepository,
                                         IRepository<Holiday> holidayRepository, 
                                         IPersistDomainModels<Configuration> configPersisterRepository,
                                         ITimer refreshNotifier,
                                         IUIInvoker uiInvoker,
                                         IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>> backgroundWorkerInvoker,
                                         ILog logger)

            : base(viewmodel, refreshNotifier, uiInvoker)
        {
            this.holidayRepository = holidayRepository;
            this.repository = repository;
            this.asyncClient = backgroundWorkerInvoker;
            this.logger = logger;
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
                        var endDate = GetEndDateFromConfig(config);
                        SetProjectStatusFromEndDate(endDate);
                    }
                    else if(settingValue == "false")
                    {
                        SetEndDateFromProjectInfo();
                    }
                    else
                    {
                        ViewModel.HasConfigError = true;
                        logger.WriteEntry(new WarningLogEntry
                                              {
                                                  Message =
                                                      "Setting value for project 'use-config-repo' does not have a valid value. Value is " +
                                                      settingValue,
                                                  Source = this.GetType().ToString(),
                                                  TimeStamp = DateTime.Now
                                              });

                    }
                }
                catch (Exception exception)
                {
                    SetProjectStatusError();

                    logger.WriteEntry(new ErrorLogEntry()
                    {
                        Message = exception.ToString(),
                        Source = this.GetType().ToString(),
                        TimeStamp = DateTime.Now
                    });
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

        private DateTime GetEndDateFromConfig(Configuration config)
        {
            SettingsEntry endDateEntry = config.GetSetting("end-date");
            string endDateSetting = endDateEntry.Value;
            try
            {
                return DateTime.Parse(endDateSetting, new CultureInfo("en-US"));
            }
            catch (FormatException e)
            {
                logger.WriteEntry(new ErrorLogEntry( this.ToString(), "Error parsing given end-date: " + e ));
                throw;
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
            var iteration = new Iteration(startDate, endDate);
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

            var holidays = holidayRepository.Get(new HolidaySpecification()
            {
                StartDate = CurrentProject.CurrentIteration.StartDate,
                EndDate = CurrentProject.CurrentIteration.EndDate,
                NonWorkingDaysOfWeek = new List<DayOfWeek>
                {
                    DayOfWeek.Saturday,
                    DayOfWeek.Sunday
                }
            } );


            return CurrentProject.CurrentIteration.CalculateWorkingdaysLeft(DateTime.Now, holidays).Days;

        }
    }
}