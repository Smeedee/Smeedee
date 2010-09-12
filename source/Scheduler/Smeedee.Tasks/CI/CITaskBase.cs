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
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Collections.Generic;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Tasks.Framework;

namespace Smeedee.Tasks.CI
{
    public abstract class CiTaskBase : TaskBase
    {
        protected const string USERNAME_SETTING_NAME = "Username";
        protected const string PASSWORD_SETTING_NAME = "Password";
        protected const string URL_SETTING_NAME = "Url";
        protected const string PROJECT_SETTING_NAME = "Project";

        private IPersistDomainModels<CIServer> databasePersister;
        private TaskConfiguration _configuration;


        public override string Name
        {
            get { return "Continuous Integration Harvester"; }
        }

        protected CiTaskBase(IPersistDomainModels<CIServer> databasePersister,
                           TaskConfiguration configuration)
        {

            if (databasePersister == null)
                throw new ArgumentNullException("databasePersister");

            if (configuration == null)
                throw new ArgumentNullException("configuration");

            this.databasePersister = databasePersister;
            this._configuration = configuration;
        }

        protected void PersistCiServers(IRepository<CIServer> sourceRepository)
        {
            databasePersister.Save(GetCiServers(sourceRepository));
        }

        private static IEnumerable<CIServer> GetCiServers(IRepository<CIServer> sourceRepository)
        {
            return sourceRepository.Get(new AllSpecification<CIServer>());
        }
    }
}