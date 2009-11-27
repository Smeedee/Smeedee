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
using System.IO;

using APD.DomainModel.Config;
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



namespace APD.DataCollector.Console
{
    class Program
    {
        private static readonly string DATABASE_TEST_FILE =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "smeedeeDB.db");

        static void Main(string[] args)
        {
            System.Console.WriteLine("Application started -  Esc to quit");

            ISessionFactory sesFact = null;

            sesFact = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);

            ILog consoleLogger = new ConsoleLogger();
            consoleLogger.VerbosityLevel = 2;
            ILog databaseLogger = new DatabaseLogger(new GenericDatabaseRepository<LogEntry>(sesFact));
            databaseLogger.VerbosityLevel = 1;
            ILog log = new CompositeLogger(new List<ILog> {consoleLogger});

            var harvesterScheduler = new Scheduler(log);

            var configRepository = new GenericDatabaseRepository<Configuration>();
            var csDatabase = new GenericDatabaseRepository<Changeset>(NHibernateFactory.AssembleSessionFactory(GenericDatabaseRepository<Changeset>.DatabaseFilePath));
            var csPersister = new ChangesetPersister(sesFact);
            var repositoryFactory = new ChangesetRepositoryFactory();
            var csHarvester = new SourceControlHarvester(csDatabase, configRepository, csPersister, repositoryFactory);

            var ciRep = new CCServerRepository("http://agileprojectdashboard.org/ccnet/", new SocketXMLBuildlogRequester());
            var ciPersister = new GenericDatabaseRepository<CIServer>(sesFact);
            var ciRepositoryFactory = new CIServerRepositoryFactory();
            var ciHarvester = new CIHarvester(ciRepositoryFactory, ciPersister, configRepository);

            //var piPersister = new GenericDatabaseRepository<ProjectInfoServer>(sesFact);
            //var piHarvester = new ProjectInfoHarvester(piRep, piPersister);

            harvesterScheduler.RegisterHarvesters(new List<AbstractHarvester> { csHarvester, ciHarvester, /*piHarvester*/ });

            //new HarvesterLoader(harvesterScheduler, catalog, log, factory);

            while (System.Console.ReadKey().Key != ConsoleKey.Escape) {}
        }
    }
}
