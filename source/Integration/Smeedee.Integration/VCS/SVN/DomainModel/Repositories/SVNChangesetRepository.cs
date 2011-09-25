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
using System.Linq;

using Smeedee.DomainModel.SourceControl;

using SharpSvn;
using Smeedee.DomainModel.Framework;

using SharpSvn.Security;


namespace Smeedee.Integration.VCS.SVN.DomainModel.Repositories
{
    public class SVNChangesetRepository : IRepository<Changeset>
    {
        private string repositoryUrl;
        private SvnClient svnClient;
        private SvnQueryOptimizer queryOptimizer;

        public SVNChangesetRepository(string repositoryUrl) : 
            this(repositoryUrl, "guest", "")
        {
            
        }

        public SVNChangesetRepository(string repositoryUrl, string username, string password)
        {
            this.repositoryUrl = repositoryUrl;
            var credentials = new System.Net.NetworkCredential(username, password);

            svnClient = new SvnClient();
            svnClient.Authentication.Clear();
            svnClient.Authentication.DefaultCredentials = credentials;
            svnClient.Authentication.SslServerTrustHandlers += AcceptCerticate;

            queryOptimizer = new SvnQueryOptimizer();
        }

        private void AcceptCerticate(object sender, SvnSslServerTrustEventArgs e)
        {
            e.AcceptedFailures = e.Failures;
            e.Save = true; // Save acceptance to authentication store
        }

        #region IChangesetRepository Members

        public IEnumerable<Changeset> Get(Specification<Changeset> specification)
        {
            var resultset = new List<Changeset>();

            Collection<SvnLogEventArgs> log;

            SvnLogArgs svnLogArgs = queryOptimizer.Optimize(specification);
            //TODO Catch this!
            svnClient.GetLog(new Uri(repositoryUrl), svnLogArgs, out log);

            Changeset changeset;
            foreach (var item in log)
            {
                changeset = new Changeset()
                {
                    Revision = item.Revision,
                    Time = item.Time,
                    Comment = item.LogMessage,
                    Author = new Author()
                    {
                        Username = item.Author
                    }
                };

                if (specification.IsSatisfiedBy(changeset))
                {       
                    resultset.Add(changeset);
                }
            }

            return resultset.OrderByDescending(c => c.Revision);
        }

        #endregion
    }
}