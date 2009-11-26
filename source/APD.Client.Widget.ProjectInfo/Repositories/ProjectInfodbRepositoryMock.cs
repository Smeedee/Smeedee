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

#endregion, 

using System;
using System.Collections.Generic;
using APD.DomainModel.ProjectInfo;
using APD.DomainModel.ProjectInfo.Repositories;


namespace APD.Client.Widget.ProjectInfo.Repositories
{
    public class ProjectInfodbRepositoryMock : IProjectRepository
    {
        public IEnumerable<ProjectInfoServer> Get()
        {
            var server = new ProjectInfoServer("MockServer", "http://mock.com");

            DateTime startDate = new DateTime(2009, 7, 28);
            DateTime endDate = new DateTime(2009, 8, 16, 23, 59, 59);

            var currentProject = new Project("Agile Project Dashboard");
            Iteration iteration = new Iteration
                                  {
                                      StartDate = startDate,
                                      EndDate = endDate
                                  };
            currentProject.AddIteration(iteration);
            server.AddProject(currentProject);

            return new List<ProjectInfoServer> {server};
        }
    }
}