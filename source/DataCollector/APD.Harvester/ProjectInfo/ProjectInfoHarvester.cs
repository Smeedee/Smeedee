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
using System.Linq;
using APD.DomainModel.Config;
using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;
using APD.Harvester.Framework;
using APD.Harvester.SourceControl.Factories;


namespace APD.Harvester.ProjectInfo
{
    public class ProjectInfoHarvester : AbstractHarvester
    {
        private const string PI_CONFIG_NAME = "pi";

        private readonly IPersistDomainModels<ProjectInfoServer> databasePersister;
        private IRepository<Configuration> configurationRepository;
        private IAssembleRepository<ProjectInfoServer> repositoryFactory;

        public override string Name
        {
            get { return "ProjectInfo Harvester"; }
        }

        public override TimeSpan Interval
        {
            get { return new TimeSpan(0, 20, 0); }
        }

        public ProjectInfoHarvester(IAssembleRepository<ProjectInfoServer> repositoryFactory,
                            IPersistDomainModels<ProjectInfoServer> databasePersister,
                            IRepository<Configuration> configurationRepository)
        {
            if (repositoryFactory == null)
                throw new ArgumentNullException("piServerSourceRepository");

            if (databasePersister == null)
                throw new ArgumentNullException("databasePersister");

            if (configurationRepository == null)
                throw new ArgumentNullException("configurationRepository");

            this.databasePersister = databasePersister;
            this.configurationRepository = configurationRepository;
            this.repositoryFactory = repositoryFactory;
        }


        public override void DispatchDataHarvesting()
        {
            var piConfiguration =
                configurationRepository.Get(new ConfigurationByName(PI_CONFIG_NAME)).SingleOrDefault();

            if (piConfiguration != null)
            {
                var sourceRepository = repositoryFactory.Assemble(piConfiguration);
                var data = sourceRepository.Get(new AllSpecification<ProjectInfoServer>());

                databasePersister.Save(data);
            }
            else
                throw new HarvesterConfigurationException("Project Info not configured");
        }
    }
}