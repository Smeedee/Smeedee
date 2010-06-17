using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Ninject;
using Ninject.Activation.Caching;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.SL.Repositories;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Holidays;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.Users;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Services.Impl;

namespace Smeedee.Client.Framework
{
    public class ServiceLocatorForSLClient : DefaultServiceLocator  
    {
        private IKernel kernel;

        public ServiceLocatorForSLClient()
        {
            kernel = new StandardKernel();

            kernel.Bind<IModuleLoader>().To<ModuleLoader>();
            kernel.Bind<IUIInvoker>().To<UIInvoker>();
            kernel.Bind<ITimer>().To<StandardTimer>();

            kernel.Bind<IRepository<Changeset>>().To<ChangesetWebserviceRepository>();
            kernel.Bind<IRepository<CIServer>>().To<CIServerWebserviceRepository>();
            kernel.Bind<IRepository<Holiday>>().To<HolidayWebserviceRepository>();
            kernel.Bind<IRepository<LogEntry>>().To<LogEntryWebserviceRepository>();
            kernel.Bind<IRepository<ProjectInfoServer>>().To<ProjectInfoRepository>();
            kernel.Bind<IRepository<Configuration>>().To<ConfigurationWebserviceRepository>();
            kernel.Bind<IRepository<Userdb>>().To<UserdbWebserviceRepository>();
            kernel.Bind<IRepository<User>>().To<UserWebserviceRepositoryProxy>();

            kernel.Bind<IPersistDomainModels<LogEntry>>().To<LogEntryWebserviceRepository>();
            kernel.Bind<IPersistDomainModels<Configuration>>().To<ConfigurationWebserviceRepository>();
            kernel.Bind<IPersistDomainModels<Holiday>>().To<HolidayWebserviceRepository>();
            kernel.Bind<IPersistDomainModels<Userdb>>().To<UserdbWebserviceRepository>();

            kernel.Bind<IInvokeBackgroundWorker<IEnumerable<CIProject>>>().To<AsyncClient<IEnumerable<CIProject>>>();
            kernel.Bind<IInvokeBackgroundWorker<IEnumerable<Changeset>>>().To<AsyncClient<IEnumerable<Changeset>>>();
            kernel.Bind<IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>>>().To<AsyncClient<IEnumerable<ProjectInfoServer>>>();

            
            kernel.Bind<ILog>().To<Logger>();
        }

        public override T GetInstance<T>()
        {
            return kernel.Get<T>();
        }

    }
}
