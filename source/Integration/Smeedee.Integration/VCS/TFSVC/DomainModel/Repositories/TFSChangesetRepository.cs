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
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Smeedee.DomainModel.SourceControl;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Changeset=Smeedee.DomainModel.SourceControl.Changeset;
using MsChangeset = Microsoft.TeamFoundation.VersionControl.Client.Changeset;
using Smeedee.DomainModel.Framework;
using System;


namespace Smeedee.Integration.VCS.TFSVC.DomainModel.Repositories
{
    public class TFSChangesetRepository : IRepository<Changeset>
    {
        private TfsVcQueryOptimizer queryOptimizer;
        private TfsTeamProjectCollection tfsClient;
        private string repository;

        public TFSChangesetRepository(string tfsServerUrl, string repository, ICredentials userCredentials)
        {
            this.repository = repository;
            tfsClient = new TfsTeamProjectCollection(new Uri(tfsServerUrl), userCredentials);
            tfsClient.Authenticate();

            queryOptimizer = new TfsVcQueryOptimizer();
        }

        #region IChangesetRepository Members

        public IEnumerable<Changeset> Get(Specification<Changeset> specification)
        {
            List<Changeset> resultset;
            var vcs = tfsClient.GetService<VersionControlServer>();
            var query = new QueryModel();

            queryOptimizer.Optimize(specification, query);

            IEnumerable changesets = vcs.QueryHistory(repository,
                                                      query.RevisionTo, query.DeletionId,
                                                      query.RecursionType, query.Author.Username,
                                                      query.RevisionFrom, query.RevisionTo,
                                                      query.MaxRevisions, query.IncludeChanges,
                                                      query.SlotMode);

            resultset = PostFilterRevisions(changesets, specification);
            return resultset.OrderByDescending(c => c.Revision);
        }

        private static string StripWindowsUsername(string username)
        {
            return username.Substring(username.LastIndexOf('\\') + 1);
        }

        private static List<Changeset> PostFilterRevisions(IEnumerable changesets, Specification<Changeset> specification)
        {
            var satisfyingChangesets = new List<Changeset>();

            foreach (MsChangeset item in changesets)
            {
                var changeset = new Changeset()
                                {
                                    Revision = item.ChangesetId,
                                    Time = item.CreationDate,
                                    Comment = item.Comment,
                                    Author = new Author()
                                             {
                                                 // Strip usernames from windows' domain\user to just user.
                                                 Username = StripWindowsUsername(item.Committer)
                                             }
                                };

                if (specification.IsSatisfiedBy(changeset))
                {
                    satisfyingChangesets.Add(changeset);
                }
            }
            return satisfyingChangesets;
        }

        #endregion
    }
}