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
using APD.DomainModel.ProjectInfo.Repositories;


namespace APD.Plugin.ProjectInfo.DomainModel.Repositories
{
    public class ProjectInfoServerRepository : IRepository<ProjectInfoServer>
    {
        private IEnumerable<ProjectInfoServer> GetProjects()
        {
            var holidayProviderParser = new XmlCountryHolidaysParser();
            HolidayProvider holidayProvider = holidayProviderParser.Parse();

            Iteration iteration = new Iteration();
            iteration.HolidayProvider = holidayProvider;
            iteration.SystemId = "iteration system ID";

            Project project = new Project();
            project.SystemId = "project system ID";
            project.AddIteration(iteration);

            ProjectInfoServer server = new ProjectInfoServer("server name", "url");
            server.AddProject(project);

            return new List<ProjectInfoServer> {server};
        }

        public IEnumerable<ProjectInfoServer> Get(Specification<ProjectInfoServer> specification)
        {
            // TODO: WTF?! Refactor this _MOCK_!!!
            return GetProjects();
        }
    }
}