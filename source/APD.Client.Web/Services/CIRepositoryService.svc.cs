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
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using APD.DomainModel.CI;
using APD.DomainModel.Framework;
using APD.DomainModel.Framework.Logging;
using APD.Integration.Database.DomainModel.Repositories;
using APD.Framework;
using APD.Client.Web.Services.Metadata;
using System.Runtime.Serialization;


namespace APD.Client.Web.Services
{
    namespace Metadata
    {
        [DataContract(IsReference = true)]
        public class CIServerMetadata
        {
            
        }

        [DataContract(IsReference = true)]
        public class CIProjectMetadata
        {
            
        }
    }

    [ServiceContract(Namespace = "http://agileprojectdashboard.org")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CIRepositoryService
    {
        private const string CI_URL = "http://agileprojectdashboard.org/ccnet/";
        private IRepository<CIServer> repository;

        public CIRepositoryService()
        {
            repository = new GenericDatabaseRepository<CIServer>();
        }

        static CIRepositoryService()
        {
            RegisterMetadataType<CIProject, CIProjectMetadata>.Register();
        }

        [OperationContract]
        [ServiceKnownType(typeof (AllSpecification<CIServer>))]
        public IEnumerable<CIServer> Get(Specification<CIServer> specification)
        {
            IEnumerable<CIServer> result = new List<CIServer>();
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