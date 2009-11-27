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
using APD.Client.Web.Serialization;
using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;
using APD.Integration.Database.DomainModel.Repositories;
using NHibernate;


namespace APD.Client.Web.Services
{
    [ServiceContract(Namespace = "http://agileprojectdashboard.org")]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [UseReferenceTrackingSerializer]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ProjectInfoRepositoryService
    {
        private IRepository<ProjectInfoServer> repository;

        private static readonly string databasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "smeedeeDB.db");

        private static ISessionFactory sessionfactory = NHibernateFactory.AssembleSessionFactory(databasePath);

        public ProjectInfoRepositoryService()
        {
            repository = new GenericDatabaseRepository<ProjectInfoServer>(sessionfactory);
        }

        [OperationContract]
        [UseReferenceTrackingSerializer]
        [ServiceKnownType(typeof(Task))]
        [ServiceKnownType(typeof(Iteration))]
        [ServiceKnownType(typeof(Specification<ProjectInfoServer>))]
        [ServiceKnownType(typeof(AllSpecification<ProjectInfoServer>))]
        public IEnumerable<ProjectInfoServer> Get(Specification<ProjectInfoServer> specification)
        {
            IEnumerable<ProjectInfoServer> projectInfoServer = repository.Get(specification);
            var piServers = new List<ProjectInfoServer>();
            piServers.AddRange(projectInfoServer);

            return piServers;
        }
    }
}