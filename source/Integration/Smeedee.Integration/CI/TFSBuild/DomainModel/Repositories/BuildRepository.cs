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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Smeedee.DomainModel.CI;

using Microsoft.TeamFoundation.Build.Client;

using BuildStatus=Smeedee.DomainModel.CI.BuildStatus;
using MSBuildStatus = Microsoft.TeamFoundation.Build.Client.BuildStatus;


namespace Smeedee.Integration.CI.TFSBuild.DomainModel.Repositories
{
    /// <summary>
    /// Not a conventional repository.
    /// </summary>
    public class BuildRepository
    {
        private BuildFetcher fetcher;

        public BuildRepository(String serverAddress, String projectName, ICredentials credentials)
        {
            fetcher = new BuildFetcher(serverAddress, projectName, credentials);
        }

        internal IList<Build> GetBuildsFromDefinition(IBuildDefinition buildDefinition)
        {
            IList<Build> builds = new List<Build>();
            foreach (var buildDetail in fetcher.GetBuildHistory(buildDefinition))
                builds.Add(ConvertBuildDetailToDomainType(buildDetail));
            return builds;
        }

        internal static Build ConvertBuildDetailToDomainType(IBuildDetail buildDetail)
        {
            var build = new Build
            {
                SystemId = buildDetail.BuildNumber,
                FinishedTime = buildDetail.FinishTime,
                StartTime = buildDetail.StartTime,
                Status = ConvertTFSStatusToBuildStatus(buildDetail.Status),
                Trigger = new Trigger
                {
                    // NOTE: TFS has no concept of what triggered a build.
                    Cause = "TFS Build",
                    InvokedBy = StripWindowsUsername(buildDetail.RequestedBy)
                }
            };
            return build;
        }

        internal static BuildStatus ConvertTFSStatusToBuildStatus(MSBuildStatus status)
        {
            switch (status)
            {
                case MSBuildStatus.Failed:
                    return BuildStatus.FinishedWithFailure;
                case MSBuildStatus.Succeeded:
                case MSBuildStatus.PartiallySucceeded:
                    return BuildStatus.FinishedSuccefully;
                case MSBuildStatus.InProgress:
                    return BuildStatus.Building;
                default:
                    return BuildStatus.Unknown;
            }
        }

        // TODO: Refactor out to shared class along with the similar method in TFSVC changeset repository.
        public static string StripWindowsUsername(String username)
        {
            return username.Substring(username.LastIndexOf('\\') + 1);
        }

        internal IEnumerable<IBuildDefinition> Definition
        {
            get { return fetcher.GetBuildDefinitions(); }
        }
    }
}