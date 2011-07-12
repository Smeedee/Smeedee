using System;
using Ninject;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Corkboard;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Holidays;
using Smeedee.DomainModel.NoSql;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.DomainModel.TeamPicture;
using Smeedee.DomainModel.Users;
using Smeedee.Integration.Database.DomainModel.Charting;
using Smeedee.Integration.Database.DomainModel.Repositories;

namespace Smeedee.Scheduler
{
    public class IocContainerForScheduler
    {
        private IKernel kernel;

        public IocContainerForScheduler()
        {
            kernel = new StandardKernel();

            kernel.Bind<IRepository<LogEntry>>().To<LogEntryDatabaseRepository>();
            kernel.Bind<IRepository<Configuration>>().To<ConfigurationDatabaseRepository>();
            kernel.Bind<IRepository<TaskConfiguration>>().To<TaskConfigurationDatabaseRepository>();
            kernel.Bind<IRepository<Userdb>>().To<UserdbDatabaseRepository>();
            kernel.Bind<IRepository<Changeset>>().To<ChangesetDatabaseRepository>();
            kernel.Bind<IRepository<CIServer>>().To<CIServerDatabaseRepository>();
            kernel.Bind<IRepository<Holiday>>().To<HolidayDatabaseRepository>();
            kernel.Bind<IRepository<ProjectInfoServer>>().To<ProjectInfoServerDatabaseRepository>();
            kernel.Bind<IRepository<RetrospectiveNote>>().To<RetrospectiveNoteDatabaseRepository>();
            kernel.Bind<IRepository<TeamPicture>>().To<TeamPictureDatabaseRepository>();
            kernel.Bind<IRepository<NoSqlDatabase>>().To<SqliteNoSqlDatabaseRepository>();

            kernel.Bind<IPersistDomainModels<LogEntry>>().To<LogEntryDatabaseRepository>();
            kernel.Bind<IPersistDomainModels<Configuration>>().To<ConfigurationDatabaseRepository>();
            kernel.Bind<IPersistDomainModels<Userdb>>().To<UserdbDatabaseRepository>();
            kernel.Bind<IPersistDomainModels<TaskConfiguration>>().To<TaskConfigurationDatabaseRepository>();
            kernel.Bind<IPersistDomainModels<Changeset>>().To<ChangesetDatabaseRepository>();
            kernel.Bind<IPersistDomainModels<CIServer>>().To<CIServerDatabaseRepository>();
            kernel.Bind<IPersistDomainModels<Holiday>>().To<HolidayDatabaseRepository>();
            kernel.Bind<IPersistDomainModels<ProjectInfoServer>>().To<ProjectInfoServerDatabaseRepository>();
            kernel.Bind<IPersistDomainModels<RetrospectiveNote>>().To<RetrospectiveNoteDatabaseRepository>();
            kernel.Bind<IPersistDomainModels<TeamPicture>>().To<TeamPictureDatabaseRepository>();

            kernel.Bind<IChartStorage>().To<ChartStorage>();

            kernel.Bind<ILog>().To<Logger>();
        }


        public void BindToConstant<T>(T instance)
        {
            kernel.Unbind<T>();
            kernel.Bind<T>().ToConstant(instance).InTransientScope();
        }

        public void BindTo<T>(Type type)
        {
            kernel.Unbind<T>();
            kernel.Bind<T>().To(type).InTransientScope();
        }

        public T Get<T>()
        {
            return kernel.Get<T>();
        }

        
    }
}
