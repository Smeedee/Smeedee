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
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://www.smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widget.ProjectInfo.ViewModels
{
    public class WorkingDaysLeftViewModel : AbstractViewModel
    {
        // Strings are public to support testing.
        public const string NO_PROJECTS_STRING_LARGE = "No project with the given name exists in the repository";
        public const string NO_ITERATIONS_STRING_LARGE = "There are no iterations in the given project";
        public const string NO_PROJECTS_STRING = "No projects";
        public const string NO_ITERATIONS_STRING = "No iterations";
        public const string WORKING_DAYS_LEFT_STRING = " working days left";
        public const string WORKING_DAYS_LEFT_SINGULAR_STRING = " working day left";
        public const string DAYS_ON_OVERTIME_STRING = " days on overtime";
        public const string DAYS_ON_OVERTIME_SINGULAR_STRING = " day on overtime";
        public const string DAYS_LEFT_STRING = "Days left: ";
        public const string CONNECTION_ERROR = "Problems with loading the data";
        public const string ERROR_IN_CONFIG = "Configuration is invalid. Set correct end date";

        public bool ProjectsInRepository;
        public bool IterationInProject;
        public bool IsOnOvertime;


        public WorkingDaysLeftViewModel()
        {
            IterationInProject = false;
            ProjectsInRepository = false;
            DaysRemaining = 0;
        }

        private int? daysRemaining;
        public int? DaysRemaining
        {
            // Background for nullable int:
            // ----------------------------------------------------
            // Logic for altering displayed text is complex
            // View is hard to modify
            // Must be handled in the "large" and "small" view...
            // Will be fixed when this domain is given an overhaul!
            get
            {
                if(ProjectsInRepository && IterationInProject && !HasConnectionProblems)
                {
                    return daysRemaining;
                }

                return null;
            }
            set
            {
                if (value != daysRemaining && IterationInProject && ProjectsInRepository)
                {
                    daysRemaining = value;
                    TriggerPropertyChanged<WorkingDaysLeftViewModel>(vm => vm.DaysRemaining);
                }
            }
        }

        public string DaysRemainingTextLarge
        {
            get
            {
                if (HasConnectionProblems)
                {
                    return CONNECTION_ERROR;
                }
                else if(HasConfigError)
                {
                    return ERROR_IN_CONFIG;
                }
                else if (!ProjectsInRepository)
                {
                    return NO_PROJECTS_STRING_LARGE;
                }
                else if (!IterationInProject)
                {
                    return NO_ITERATIONS_STRING_LARGE;
                }
                else if (IsOnOvertime)
                {
                    if (this.DaysRemaining != 1)
                        return DAYS_ON_OVERTIME_STRING;
                    else
                        return DAYS_ON_OVERTIME_SINGULAR_STRING;
                }
                else
                {
                    if (this.DaysRemaining != 1)
                        return WORKING_DAYS_LEFT_STRING;
                    else
                        return WORKING_DAYS_LEFT_SINGULAR_STRING;
                }
            }
        }

        public string DaysRemainingTextSmall
        {
            get
            {
                if(HasConnectionProblems)
                {
                    return ""; // Display no error on the statusbar
                }
                else if(HasConfigError)
                {
                    return "";
                }
                else if (!ProjectsInRepository)
                {
                    return NO_PROJECTS_STRING;
                }
                else if (!IterationInProject)
                {
                    return NO_ITERATIONS_STRING;
                }
                else if (IsOnOvertime)
                {
                    return DAYS_ON_OVERTIME_STRING;
                }
                else
                {
                    return DAYS_LEFT_STRING;
                }
            }
        }

        public bool HasConfigError { get; set; }
    }
}