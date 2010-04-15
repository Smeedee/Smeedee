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
using System.Collections.ObjectModel;
using APD.DomainModel.Framework;
using APD.DomainModel.SourceControl;
using APD.Integration.VCS.Git.DomainModel.RepositoryHelpers;

using GitSharp;
using Author = APD.DomainModel.SourceControl.Author;
using System.Linq;

namespace APD.Integration.VCS.Git.DomainModel.Repositories
{
    public class GitChangesetRepository : IRepository<Changeset>
    {
        static Repository gitRepo;
        private static GitChangesetRepositoryHelper helper;

        public GitChangesetRepository(string reposUrl)
        {
            helper = new GitChangesetRepositoryHelper(reposUrl);
            if (!Repository.IsValid(helper.OurRepoPath))
                helper.RunCloneScript();
            gitRepo = new Repository(helper.OurRepoPath);
        }

        public static string ReposDir { get; set; }
        public Specification<Changeset> Specification { get; set; }

        public Collection<Commit> GetCommitLog()
        {
            var log = new Collection<Commit>();
            var head = gitRepo.Get<Commit>("HEAD");

            log.Add(head);

            foreach (Commit ancestor in head.Ancestors)
            {
                log.Add(ancestor);
            }

            return log;
        }
 
        public IEnumerable<Changeset> Get(Specification<Changeset> specification)
        {
            helper.RunPullScript();
            var resultset = new List<Changeset>();
            Collection<Commit> log = GetCommitLog();
            Specification = specification;
            int revision = log.Count();

            foreach (var item in log)
            {
                var changeset = ConvertCommitToChangeset(item, revision);

                if (Specification.IsSatisfiedBy(changeset))
                {
                    resultset.Add(changeset);
                }
                --revision;
            }
            return resultset.OrderByDescending(c => c.Revision);
        }

        public Changeset ConvertCommitToChangeset(Commit item, int revision)
        {
            DateTimeOffset t = item.CommitDate;
            DateTime time = t.DateTime;

            var changeset = new Changeset
            {
                Time = time,
                Comment = item.Message,
                Author = new Author(item.Author.Name),
                Revision = revision
            };

            return changeset;   
        }
    }
}
