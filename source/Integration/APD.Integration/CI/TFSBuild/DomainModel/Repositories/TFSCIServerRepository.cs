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
using System.Net;

using APD.DomainModel.CI;
using APD.DomainModel.Framework;

using MSBuildStatus = Microsoft.TeamFoundation.Build.Client.BuildStatus;


namespace APD.Integration.CI.TFSBuild.DomainModel.Repositories
{
    public class TFSCIServerRepository : IRepository<CIServer>
    {
        private readonly BuildRepository buildRepository;
        private readonly string serverAddress;

        public TFSCIServerRepository(String serverAddress, String projectName, ICredentials credentials)
        {
            this.serverAddress = serverAddress;
            buildRepository = new BuildRepository(serverAddress, projectName, credentials);
        }

        public IEnumerable<CIServer> Get(Specification<CIServer> specification)
        {
            var projects = new List<CIProject>();

            var server = new CIServer()
            {
                Name = "TFS Continuous Integration Server",
                Url = serverAddress
            };

            foreach (var buildDefinition in buildRepository.Definition)
            {
                var project = new CIProject(buildDefinition.Name)
                {
                    SystemId = buildDefinition.Name,
                    Builds = buildRepository.GetBuildsFromDefinition(buildDefinition),
                    Server = server
                };

                projects.Add(project);
            }

            server.Projects = projects;

            return new List<CIServer> {server};
        }
    }
}