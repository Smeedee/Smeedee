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


namespace APD.Client.Widget.BurndownChart.Repositories
{
    public class ProjectInfoRepositoryMock : IRepository<ProjectInfoServer>
    {
        public IEnumerable<ProjectInfoServer> Get(Specification<ProjectInfoServer> specification)
        {
            var server = new ProjectInfoServer("Mock Serve", "http://mock.com");

            var currentProject = new Project("Agile Project Dashboard");

            var iterationStartDate = new DateTime(2009, 7, 28);
            var iterationEndDate = new DateTime(2009, 8, 9);

            var iteration = new Iteration();
            iteration.Name = "Sprint 3";
            iteration.StartDate = iterationStartDate;
            iteration.EndDate = iterationEndDate;

            var task1 = new Task();
            var task2 = new Task();
            var task3 = new Task();
            var task4 = new Task();
            var task5 = new Task();
            var task6 = new Task();
            var task7 = new Task();
            var task8 = new Task();
            var task9 = new Task();
            var task10 = new Task();
            var task11 = new Task();
            var task12 = new Task();
            var task13 = new Task();
            var task14 = new Task();
            var task15 = new Task();
            var task16 = new Task();
            var task17 = new Task();
            var task18 = new Task();
            var task19 = new Task();
            var task20 = new Task();

            task1.WorkEffortEstimate = 12;
            task2.WorkEffortEstimate = 8;
            task3.WorkEffortEstimate = 8;
            task4.WorkEffortEstimate = 8;
            task5.WorkEffortEstimate = 8;
            task6.WorkEffortEstimate = 40;
            task7.WorkEffortEstimate = 8;
            task8.WorkEffortEstimate = 20;
            task9.WorkEffortEstimate = 5;
            task10.WorkEffortEstimate = 55;
            task11.WorkEffortEstimate = 20;
            task12.WorkEffortEstimate = 20;
            task13.WorkEffortEstimate = 11;
            task14.WorkEffortEstimate = 10;
            task15.WorkEffortEstimate = 20;
            task16.WorkEffortEstimate = 4;
            task17.WorkEffortEstimate = 3;
            task18.WorkEffortEstimate = 3;
            task19.WorkEffortEstimate = 5;
            task20.WorkEffortEstimate = 12;


            task1.AddWorkEffortHistoryItem (new WorkEffortHistoryItem(0,  new DateTime(2009, 7, 28)));
            task2.AddWorkEffortHistoryItem (new WorkEffortHistoryItem(0,  new DateTime(2009, 7, 28)));
            task3.AddWorkEffortHistoryItem (new WorkEffortHistoryItem(8,  new DateTime(2009, 7, 28)));
            task4.AddWorkEffortHistoryItem (new WorkEffortHistoryItem(8,  new DateTime(2009, 7, 28)));
            task5.AddWorkEffortHistoryItem (new WorkEffortHistoryItem(8,  new DateTime(2009, 7, 28)));
            task6.AddWorkEffortHistoryItem (new WorkEffortHistoryItem(40, new DateTime(2009, 7, 28)));
            task7.AddWorkEffortHistoryItem (new WorkEffortHistoryItem(1,  new DateTime(2009, 7, 28)));
            task8.AddWorkEffortHistoryItem (new WorkEffortHistoryItem(10, new DateTime(2009, 7, 28)));
            task9.AddWorkEffortHistoryItem (new WorkEffortHistoryItem(0,  new DateTime(2009, 7, 28)));
            task10.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(55, new DateTime(2009, 7, 28)));
            task11.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(20, new DateTime(2009, 7, 28)));
            task12.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(20, new DateTime(2009, 7, 28)));
            task13.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(11, new DateTime(2009, 7, 28)));
            task14.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(10, new DateTime(2009, 7, 28)));
            task15.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(0,  new DateTime(2009, 7, 28)));
            task16.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(0,  new DateTime(2009, 7, 28)));
            task17.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(3,  new DateTime(2009, 7, 28)));
            task18.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(3,  new DateTime(2009, 7, 28)));
            task19.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(5,  new DateTime(2009, 7, 28)));
            task20.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(12, new DateTime(2009, 7, 28)));



            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(0,  new DateTime(2009, 7, 30)));
            task7.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(7,  new DateTime(2009, 7, 31)));
            task8.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(10, new DateTime(2009, 7, 30)));
            task9.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(3,  new DateTime(2009, 7, 30)));
            task10.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(50, new DateTime(2009, 7, 29)));
            task11.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(8,  new DateTime(2009, 7, 30)));

            task7.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(1,  new DateTime(2009, 8, 3)));
            task9.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(0,  new DateTime(2009, 7, 31)));
            task10.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(40, new DateTime(2009, 8, 3)));

            iteration.AddTask(task1);
            iteration.AddTask(task2);
            iteration.AddTask(task3);
            iteration.AddTask(task4);
            iteration.AddTask(task5);
            iteration.AddTask(task6);
            iteration.AddTask(task7);
            iteration.AddTask(task8);
            iteration.AddTask(task9);
            iteration.AddTask(task10);
            iteration.AddTask(task11);
            iteration.AddTask(task12);
            iteration.AddTask(task13);
            iteration.AddTask(task14);
            iteration.AddTask(task15);
            iteration.AddTask(task16);
            iteration.AddTask(task17);
            iteration.AddTask(task18);
            iteration.AddTask(task19);
            iteration.AddTask(task20);

            currentProject.AddIteration(iteration);
            //currentProject.CurrentIteration = iteration;   //Should not be required if iterationStartDate < DateTime.Now < iterationEndDate
                                                           //but set in case mock is used in future.
            server.AddProject(currentProject);

            return new List<ProjectInfoServer>() {server};
        }
    }
}