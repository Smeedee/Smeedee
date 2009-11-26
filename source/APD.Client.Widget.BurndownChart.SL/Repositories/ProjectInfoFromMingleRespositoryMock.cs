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
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;


namespace APD.Client.Widget.BurndownChart.SL.Repositories
{
    public class ProjectInfoFromMingleRespositoryMock : IRepository<ProjectInfoServer>
    {
        public IEnumerable<ProjectInfoServer> Get(Specification<ProjectInfoServer> specification)
        {
            var server = new ProjectInfoServer("Mock Mingle Server", "http://minglemock.com");
            var currentProject = new Project("Agile Project Dashboard");

            var iterationStartDate = new DateTime(2009, 7, 6);
            var iterationEndDate = new DateTime(2009, 7, 19);

            var iteration = new Iteration();
            iteration.Name = "Sprint 2";
            iteration.StartDate = iterationStartDate;
            iteration.EndDate = iterationEndDate;

            var task1 = new Task
                        {
                            Status = "In progress",
                            Name = "Create Mingle Plugin",
                            WorkEffortEstimate = 30
                        };
            var task2 = new Task
                        {
                            Status = "In progress",
                            Name = "Controller: create new",
                            WorkEffortEstimate = 25
                        };
            var task3 = new Task
                        {
                            Status = "In progress",
                            Name = "View: Extend to support sound",
                            WorkEffortEstimate = 6
                        };
            var task4 = new Task
                        {
                            Status = "In progress",
                            Name = "Fake",
                            WorkEffortEstimate = 300
                        };

            task1.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(30, iterationStartDate));
            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(25, iterationStartDate));
            task3.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(6, iterationStartDate));
            task4.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(300, iterationStartDate));


            task1.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(25,  new DateTime(2009, 7, 7)));
            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(22,  new DateTime(2009, 7, 7)));
            task3.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(6,   new DateTime(2009, 7, 7)));
            task4.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(280, new DateTime(2009, 7, 7)));

            task1.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(21,  new DateTime(2009, 7, 8)));
            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(20,  new DateTime(2009, 7, 8)));
            task3.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(5,   new DateTime(2009, 7, 8)));
            task4.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(260, new DateTime(2009, 7, 8)));

            task1.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(18,  new DateTime(2009, 7, 9)));
            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(17,  new DateTime(2009, 7, 9)));
            task3.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(4,   new DateTime(2009, 7, 9)));
            task4.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(230, new DateTime(2009, 7, 9)));

            task1.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(15,  new DateTime(2009, 7, 10)));
            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(15,  new DateTime(2009, 7, 10)));
            task3.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(2,   new DateTime(2009, 7, 10)));
            task4.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(210, new DateTime(2009, 7, 10)));

            task1.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(13,  new DateTime(2009, 7, 13)));
            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(12,  new DateTime(2009, 7, 13)));
            task3.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(0,   new DateTime(2009, 7, 13)));
            task4.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(165, new DateTime(2009, 7, 13)));

            task1.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(10,  new DateTime(2009, 7, 14)));
            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(5,   new DateTime(2009, 7, 14)));
            task3.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(0,   new DateTime(2009, 7, 14)));
            task4.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(150, new DateTime(2009, 7, 14)));

            iteration.AddTask(task1);
            iteration.AddTask(task2);
            iteration.AddTask(task3);
            iteration.AddTask(task4);
            currentProject.AddIteration(iteration);
            server.AddProject(currentProject);

            return new List<ProjectInfoServer>(){server};
        }
    }
}