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
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.ProjectInfo;


namespace Smeedee.Integration.PMT.RallyDev.DomainModel.Repositories
{
    public class RallyDevRepository : IRepository<ProjectInfoServer>
    {
        private RallyDevReader reader;

        public RallyDevRepository(string webServiceUrl, string username, string password)
        {
            reader = new RallyDevReader(webServiceUrl, new XmlDownloader(username,password));
        }

        public IEnumerable<ProjectInfoServer> Get(Specification<ProjectInfoServer> specification)
        {
            return reader.Get(new AllSpecification<ProjectInfoServer>());
        }
    }
}