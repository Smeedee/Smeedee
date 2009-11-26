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
using System.Linq;
using System.Net;

using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;

using MSBuildStatus = Microsoft.TeamFoundation.Build.Client.BuildStatus;


namespace APD.Integration.CI.TFSBuild.DomainModel.Repositories
{
    public class BuildFetcher
    {
        private readonly TeamFoundationServer tfsServer;
        private readonly IBuildServer buildServer;
        private readonly String projectName;

        public BuildFetcher(String serverAddress, String projectName, ICredentials credentials)
        {
            this.projectName = projectName;
            tfsServer = new TeamFoundationServer(serverAddress, credentials);
            tfsServer.Authenticate();
            buildServer = (IBuildServer) tfsServer.GetService(typeof (IBuildServer));
        }

        public IEnumerable<IBuildDefinition> GetBuildDefinitions()
        {
            return new List<IBuildDefinition>(buildServer.QueryBuildDefinitions(projectName));
        }

        public IEnumerable<IBuildDetail> GetBuildHistory(IBuildDefinition buildDefinition)
        {
            return new List<IBuildDetail>(buildServer.QueryBuilds(buildDefinition).Where(bd => bd.Status != BuildStatus.NotStarted).Reverse());
        }
    }
}