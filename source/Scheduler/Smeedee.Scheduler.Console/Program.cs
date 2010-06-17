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
using NHibernate;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Integration.CI.CruiseControl.DomainModel.Repositories;
using Smeedee.Integration.Database.DomainModel.Repositories;
using Smeedee.Tasks.CI;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.ProjectInfo;
using Smeedee.Tasks.SourceControl;


namespace Smeedee.Scheduler.Console
{
    class Program
    {
        private static readonly string DATABASE_FILE =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "smeedeeDB.db");

        static void Main(string[] args)
        {
            System.Console.WriteLine("Application started -  Esc to quit");
            
            ISessionFactory sesFact = null;

            sesFact = NHibernateFactory.AssembleSessionFactory(DATABASE_FILE);

            ILog consoleLogger = new ConsoleLogger();
            consoleLogger.VerbosityLevel = 2;
            ILog databaseLogger = new Logger(new LogEntryDatabaseRepository(sesFact));
            databaseLogger.VerbosityLevel = 1;
            ILog log = new CompositeLogger(consoleLogger, databaseLogger);

            var harvesterScheduler = new Scheduler(log);

            var configRepository = new HardcodedConfigurationRepository();
            var csDatabase = new ChangesetDatabaseRepository(sesFact);

            var repositoryFactory = new ChangesetRepositoryFactory();
            var csHarvester = new SourceControlTask(csDatabase, configRepository, csDatabase, repositoryFactory);

            var ciRep = new CCServerRepository("http://agileprojectdashboard.org/ccnet/", new SocketXMLBuildlogRequester());
            var ciPersister = new CIServerDatabaseRepository(sesFact);
            var ciRepositoryFactory = new CIServerRepositoryFactory();
            var ciHarvester = new CITask(ciRepositoryFactory, ciPersister, configRepository);

            var piRepositoryFactory = new ProjectInfoRepositoryFactory();
            var piPersister = new ProjectInfoServerDatabaseRepository(sesFact);
            var piHarvester = new ProjectInfoTask(piRepositoryFactory, piPersister, configRepository);
            
            harvesterScheduler.RegisterTasks(new List<TaskBase> { csHarvester });

            //new TaskLoader(harvesterScheduler, catalog, log, factory);

            while (System.Console.ReadKey().Key != ConsoleKey.Escape) {}
        }
    }
}
