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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.SourceControl;
using GitSharp;
using Author = Smeedee.DomainModel.SourceControl.Author;
using System.Linq;


namespace Smeedee.Integration.VCS.Git.DomainModel
{
    public class GitChangesetRepository : IRepository<Changeset>
    {
        static Repository _gitRepos;

        public GitChangesetRepository(string reposDir)
        {
            _gitRepos = new Repository(reposDir);
            ReposDir = reposDir;
        }

        public static string ReposDir { get; set; }

        public IEnumerable<Changeset> Get(Specification<Changeset> specification)
        {
            var log = new Collection<Commit>();
            var resultset = new List<Changeset>();
            var head = _gitRepos.Get<Commit>("HEAD");

            foreach (Commit ancestor in head.Ancestors)
            {
                log.Add(ancestor);
            }
            int revision = log.Count();

            foreach (var item in log)
            {
                var t = item.CommitDate;
                DateTime time = t.DateTime;

                var changeset = new Changeset()
                {
                    Time = time,
                    Comment = item.Message,
                    Author = new Author(item.Author.Name)
                };
                if (specification.IsSatisfiedBy(changeset))
                {
                    resultset.Add(changeset);
                    --revision;
                }

            }
            return resultset.OrderByDescending(c => c.Revision);
        }

        ~GitChangesetRepository()
        {
            //_gitRepos.Dispose();
        }
    }
}
