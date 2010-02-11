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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;

using APD.DomainModel.Framework.Logging;
using APD.DomainModel.ProjectInfo;
using APD.Harvester.CI;
using APD.Harvester.Framework;
using APD.Harvester.ProjectInfo;
using APD.Harvester.SourceControl;
using APD.Integration.CI.CruiseControl.DomainModel.Repositories;
using APD.Integration.Database.DomainModel.Repositories;
using APD.Integration.PMT.RallyDev.DomainModel.Repositories;
using APD.Integration.VCS.SVN.DomainModel.Repositories;

using APD.DomainModel.CI;
using APD.DomainModel.SourceControl;
using NHibernate;
using APD.DomainModel.Config;



namespace APD.DataCollector.Service
{
    public partial class DataCollectorService : ServiceBase
    {
        private const string SERVICE_DESCRIPTION = "smeedee_dc_service";
        private const string LOG_NAME = "smeedee_dc_Log";

        private string databaseFile;
        public string DatabaseFile
        {
            get
            {
                commonAppPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                return Path.Combine( commonAppPath, databaseFile);
            }
            set { databaseFile = value; }
        }

        private string commonAppPath;
        private ISessionFactory sesFact;

        public DataCollectorService()
        {
            InitializeComponent();

            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            if (!EventLog.SourceExists(SERVICE_DESCRIPTION))
            {
                EventLog.CreateEventSource(SERVICE_DESCRIPTION, LOG_NAME);
                EventLog.WriteEntry("Created smeedee Data Collector Service", EventLogEntryType.Information);
            }
            sesFact = NHibernateFactory.AssembleSessionFactory(DatabaseFile);
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Starting smeedee Data Collector Service", EventLogEntryType.Information);

            if(args.Length < 1 || String.IsNullOrEmpty(args[0]))
            {
                DatabaseFile = "smeedeeDB.db";
            }
            else
            {
                DatabaseFile = args[0];
            }

            EventLog.WriteEntry("Using database file: " + DatabaseFile, EventLogEntryType.Information);

            RunHarvesters();

        }

        protected override void OnStop()
        {
            base.OnStop();

            EventLog.WriteEntry("Stopping smeedee Data Collector Service", EventLogEntryType.Information);
        }

        private void RunHarvesters()
        {
            ILog logger = new DatabaseLogger(new LogEntryDatabaseRepository(sesFact));

            var harvesterScheduler = new Scheduler(logger);

            var configRepository = new ConfigurationDatabaseRepository();
            
            var csDatabase = new ChangesetDatabaseRepository(sesFact);
            var repositoryFactory = new ChangesetRepositoryFactory();
            var csHarvester = new SourceControlHarvester(csDatabase, configRepository, csDatabase, repositoryFactory);

            var ciPersister = new CIServerDatabaseRepository(sesFact);
            var ciRepositoryFactory = new CIServerRepositoryFactory();
            var ciHarvester = new CIHarvester(ciRepositoryFactory, ciPersister, configRepository);

            harvesterScheduler.RegisterHarvesters(new List<AbstractHarvester> { csHarvester, ciHarvester });

            EventLog.WriteEntry("Harvesters started", EventLogEntryType.Information);
        }
    }
}