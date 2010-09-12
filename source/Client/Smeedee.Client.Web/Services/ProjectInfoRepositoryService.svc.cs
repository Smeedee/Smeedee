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
using System.ServiceModel;
using System.ServiceModel.Activation;
using Smeedee.Client.Web.Serialization;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Integration.Database.DomainModel.Repositories;


namespace Smeedee.Client.Web.Services
{
    [ServiceContract(Namespace = "http://smeedee.org")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [UseReferenceTrackingSerializer]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ProjectInfoRepositoryService
    {
        private IRepository<ProjectInfoServer> repository;
        
        public ProjectInfoRepositoryService()
        {
            repository = new ProjectInfoServerDatabaseRepository(DefaultSessionFactory.Instance);
        }

        [OperationContract]
        [UseReferenceTrackingSerializer]
        [ServiceKnownType(typeof(Task))]
        [ServiceKnownType(typeof(Iteration))]
        [ServiceKnownType(typeof(Specification<ProjectInfoServer>))]
        [ServiceKnownType(typeof(ProjectInfoServerByName))]
        [ServiceKnownType(typeof(ProjectInfoServerByUrl))]
        [ServiceKnownType(typeof(AllSpecification<ProjectInfoServer>))]
        public IEnumerable<ProjectInfoServer> Get(Specification<ProjectInfoServer> specification)
        {
            IEnumerable<ProjectInfoServer> result = new List<ProjectInfoServer>();
            try
            {
                result = repository.Get(specification);
            }
            catch (Exception exception)
            {
                ILog logger = new Logger(new LogEntryDatabaseRepository(DefaultSessionFactory.Instance));
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }

            return result;
        }
    }
}