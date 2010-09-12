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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Integration.Database.DomainModel.Repositories;
using Smeedee.Integration.Framework.Utils;
using Smeedee.Scheduler.Services;
using Smeedee.Tasks.Framework;

namespace Smeedee.Scheduler.Service
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

        public DataCollectorService()
        {

            InitializeComponent();

            if (!EventLog.SourceExists(SERVICE_DESCRIPTION))
            {
                EventLog.CreateEventSource(SERVICE_DESCRIPTION, LOG_NAME);
                EventLog.WriteEntry("Created smeedee Data Collector Service", EventLogEntryType.Information);
            }

            
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Starting smeedee Data Collector Service", EventLogEntryType.Information);

            if (args.Length < 1 || String.IsNullOrEmpty(args[0]))
            {
                DatabaseFile = "smeedeeDB.db";
            }
            else
            {
                DatabaseFile = args[0];
            }

            EventLog.WriteEntry("Using database file: " + DatabaseFile, EventLogEntryType.Information);

            RunTasks();
        }

        protected override void OnStop()
        {
            base.OnStop();

            EventLog.WriteEntry("Stopping smeedee Data Collector Service", EventLogEntryType.Information);
        }

        private void RunTasks()
        {
            var sessionFactory = NHibernateFactory.AssembleSessionFactory(DatabaseFile);
            var logger = new Logger(new LogEntryDatabaseRepository(sessionFactory)) { VerbosityLevel = 1 };
            var iocContaier = new IocContainerForScheduler();
            iocContaier.BindToConstant(sessionFactory);
            var taskDirectory = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.FullName;

            var taskScheduler = new Scheduler(iocContaier, taskDirectory, new FileUtilities(), new TimerWithTimestamp(0, 10000), new TimerWithTimestamp(0, 2000), logger);
            taskScheduler.Start();
        }
    }
}