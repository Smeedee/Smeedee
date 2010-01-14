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
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;

using APD.DomainModel.Framework.Logging;
using APD.DomainModel.SourceControl;
using APD.Integration.Database.DomainModel.Repositories;
using APD.DomainModel.Framework;


namespace APD.Client.Web.Services
{
    [ServiceContract(Namespace = "http://agileprojectdashboard.org")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ChangesetRepositoryService
    {
        //private const string REPOSITORY_URL = "https://192.168.1.14:8443/svn/CPMonitor";
        private const string REPOSITORY_URL = "https://agileprojectdashboard.org:8443/svn/CPMonitor";
        //private const string REPOSITORY_URL = "svn://outland.online.ntnu.no/home/dagolap/jallarepo";
        private readonly GenericDatabaseRepository<Changeset> repository;

        public ChangesetRepositoryService()
        {
            repository = new ChangesetDatabaseRepository();
        }
        
        [OperationContract]
        [ServiceKnownType(typeof (Specification<Changeset>))]
        [ServiceKnownType(typeof (AllChangesetsSpecification))]
        [ServiceKnownType(typeof (ChangesetsForUserSpecification))]
        [ServiceKnownType(typeof (ChangesetsAfterRevisionSpecification))]
        public IEnumerable<Changeset> Get(Specification<Changeset> specification)
        {
            IEnumerable<Changeset> result = new List<Changeset>();
            try
            {
                result = repository.Get(specification);
            }
            catch (Exception exception)
            {
                ILog logger = new DatabaseLogger(new GenericDatabaseRepository<LogEntry>());
                logger.WriteEntry(new ErrorLogEntry(this.GetType().ToString(), exception.ToString()));
            }

            return result;
        }
    }
}