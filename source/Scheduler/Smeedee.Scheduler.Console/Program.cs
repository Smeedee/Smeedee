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
using System.IO;
using System.Reflection;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Integration.Database.DomainModel.Repositories;
using Smeedee.Integration.Framework.Utils;
using Smeedee.Scheduler.Services;
using Smeedee.Tasks.Framework;

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

            var sessionFactory = NHibernateFactory.AssembleSessionFactory(DATABASE_FILE);
            ILog consoleLogger = new ConsoleLogger {VerbosityLevel = 2};
            ILog databaseLogger = new Logger(new LogEntryDatabaseRepository(sessionFactory)) {VerbosityLevel = 1};
            ILog log = new CompositeLogger(consoleLogger, databaseLogger);

            #region Unncomment to fill up the database with some task configurations. Used for TESTING ONLY

            //var dbRepository = new TaskConfigurationDatabaseRepository();
            //var hardCodedTaskConfigurations = new HardCodedTaskConfigurationRepository();
            //dbRepository.Save(hardCodedTaskConfigurations.Get(new AllSpecification<TaskConfiguration>()));

            #endregion

            var iocContainer = new IocContainerForScheduler();
            iocContainer.BindToConstant(sessionFactory);
            var taskDirectory = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.FullName;

            var taskScheduler = new Scheduler(iocContainer, taskDirectory, new FileUtilities(), new TimerWithTimestamp(0, 10000), new TimerWithTimestamp(0, 2000), log);
            taskScheduler.Start();

            try
            {
                while (System.Console.ReadKey().Key != ConsoleKey.Escape) { }
            }
            catch (Exception e)
            {
                //TODO: Replace this with some proper logging
                System.Console.WriteLine(e);
            }
        }

    }
}
