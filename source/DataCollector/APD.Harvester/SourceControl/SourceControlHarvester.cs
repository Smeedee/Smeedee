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

using System.Collections.Generic;
using System.Linq;
using APD.DomainModel.Framework;
using APD.DomainModel.SourceControl;
using APD.Harvester.Framework;
using APD.DomainModel.Config;
using APD.Harvester.Framework.Factories;


namespace APD.Harvester.SourceControl
{
    
    public class SourceControlHarvester : AbstractHarvester
    {
        private const int FIRST_CHANGESET_REVISION_ID = 0;
        private const string VCS_CONFIG_NAME = "vcs"; //VCS == Version Control System

        private readonly IRepository<Changeset> changesetDbRepository;
        private readonly IRepository<Configuration> configRepository;
        private readonly IPersistDomainModels<Changeset> databasePersister;
        private readonly IAssembleRepository<Changeset> csRepositoryFactory;

        public override string Name
        {
            get { return "Source Control Harvester"; }
        }

        public SourceControlHarvester(IRepository<Changeset> changesetDbRepository, 
                                      IRepository<Configuration> configRepository,
                                      IPersistDomainModels<Changeset> databasePersister,
                                      IAssembleRepository<Changeset> csRepositoryFactory)
        {
            this.changesetDbRepository = changesetDbRepository;
            this.configRepository = configRepository;
            this.databasePersister = databasePersister;
            this.csRepositoryFactory = csRepositoryFactory;
        }

        public override void DispatchDataHarvesting()
        {
            IEnumerable<Changeset> allSavedChangesets = changesetDbRepository.Get(new AllChangesetsSpecification());

            long latestSavedRevision = FIRST_CHANGESET_REVISION_ID;
            if (allSavedChangesets.Count() > 0)
            {
                latestSavedRevision = allSavedChangesets.First().Revision;
            }

            var vcsConfiguration = configRepository.Get(new ConfigurationByName(VCS_CONFIG_NAME)).SingleOrDefault();
            
            if (vcsConfiguration != null)
            {
                var changesetRepository = csRepositoryFactory.Assemble(vcsConfiguration);

                IEnumerable<Changeset> allNewChangesets = changesetRepository.Get(
                    new ChangesetsAfterRevisionSpecification(latestSavedRevision)
                    );

                foreach (Changeset changeset in allNewChangesets)
                {
                    databasePersister.Save(changeset);
                }
            }
            else
                throw new HarvesterConfigurationException("Version Control System not configured");
        }
    }
}
