using System;
using System.Collections.Generic;
using Ninject;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.SL.Repositories;
using Smeedee.Client.Framework.SL.Repositories.Async;
using Smeedee.Client.Framework.SL.Services.Impl;
using Smeedee.Client.Framework.SL.ViewModel.Repositories;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Corkboard;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Holidays;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.DomainModel.SourceControl;
using Smeedee.DomainModel.TaskDefinition;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.DomainModel.TeamPicture;
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

            BindRepositories();
            BindAsyncRepositories();
            BindDomainModelPersisters();
            BindDomainModelDeleters();
            BindBackgroundWorkerInvokers();

            // Bind the other dependencies here that don't have any logical grouping
            kernel.Bind<IModuleLoader>().To<ModuleLoader>();
            kernel.Bind<IUIInvoker>().To<UIInvoker>();
            kernel.Bind<ITimer>().To<StandardTimer>();
        	kernel.Bind<IModalDialogService>().To<SLModalDialogService>();
            kernel.Bind<IAsyncRepository<WidgetMetadata>>().ToConstant(WidgetMetadataRepository.Instance);
            kernel.Bind<ILog>().To<Logger>();
            kernel.Bind<IMetadataService>().To<MetadataService>();
        }

        private void BindRepositories()
        {
            kernel.Bind<IRepository<Changeset>>().To<ChangesetWebserviceRepository>();
            kernel.Bind<IRepository<CIServer>>().To<CIServerWebserviceRepository>();
            kernel.Bind<IRepository<Holiday>>().To<HolidayWebserviceRepository>();
            kernel.Bind<IRepository<LogEntry>>().To<LogEntryWebserviceRepository>();
            kernel.Bind<IRepository<ProjectInfoServer>>().To<ProjectInfoRepository>();
            kernel.Bind<IRepository<Configuration>>().To<ConfigurationWebserviceRepository>();
            kernel.Bind<IRepository<Userdb>>().To<UserdbWebserviceRepository>();
            kernel.Bind<IRepository<User>>().To<UserWebserviceRepositoryProxy>();
            kernel.Bind<IRepository<RetrospectiveNote>>().To<RetrospectiveNoteWebServiceRepository>();
            kernel.Bind<IRepository<TeamPicture>>().To<TeamPictureWebserviceRepository>();
            kernel.Bind<IRepository<TaskDefinition>>().To<TaskDefinitionWebserviceRepository>();
            kernel.Bind<IRepository<TaskConfiguration>>().To<TaskConfigurationWebserviceRepository>();
        }

        private void BindAsyncRepositories()
        {
            kernel.Bind<IAsyncRepository<ProjectInfoServer>>().To<AsyncProjectInfoRepository>();
            kernel.Bind<IAsyncRepository<Changeset>>().To<AsyncChangesetRepository>(); 
            kernel.Bind<IAsyncRepository<Holiday>>().To<AsyncHolidayRepository>();
            kernel.Bind<IAsyncRepository<Configuration>>().To<AsyncConfigurationRepository>();
        }

        private void BindDomainModelPersisters()
        {
            kernel.Bind<IPersistDomainModels<LogEntry>>().To<LogEntryWebserviceRepository>();
            kernel.Bind<IPersistDomainModels<Configuration>>().To<ConfigurationWebserviceRepository>();
            kernel.Bind<IPersistDomainModels<Holiday>>().To<HolidayWebserviceRepository>();
            kernel.Bind<IPersistDomainModels<Userdb>>().To<UserdbWebserviceRepository>();
            kernel.Bind<IPersistDomainModels<RetrospectiveNote>>().To<RetrospectiveNoteWebServiceRepository>();
            kernel.Bind<IPersistDomainModels<CIServer>>().To<CIServerWebserviceRepository>();
            kernel.Bind<IPersistDomainModels<TeamPicture>>().To<TeamPictureWebserviceRepository>();
            kernel.Bind<IPersistDomainModels<TaskConfiguration>>().To<TaskConfigurationWebserviceRepository>();
        }

        private void BindDomainModelDeleters()
        {
            kernel.Bind<IDeleteDomainModels<RetrospectiveNote>>().To<RetrospectiveNoteWebServiceRepository>();
        }

        private void BindBackgroundWorkerInvokers()
        {
            kernel.Bind<IInvokeBackgroundWorker<IEnumerable<CIProject>>>().To<AsyncClient<IEnumerable<CIProject>>>();
            kernel.Bind<IInvokeBackgroundWorker<IEnumerable<Changeset>>>().To<AsyncClient<IEnumerable<Changeset>>>();
            kernel.Bind<IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>>>().To<AsyncClient<IEnumerable<ProjectInfoServer>>>();
            kernel.Bind<IInvokeBackgroundWorker<IEnumerable<RetrospectiveNote>>>().To<AsyncClient<IEnumerable<RetrospectiveNote>>>();
            kernel.Bind<IInvokeBackgroundWorker<IEnumerable<TeamPicture>>>().To<AsyncClient<IEnumerable<TeamPicture>>>();
            kernel.Bind<IInvokeBackgroundWorker<IEnumerable<TaskDefinition>>>().To<AsyncClient<IEnumerable<TaskDefinition>>>();
            kernel.Bind<IInvokeBackgroundWorker<IEnumerable<TaskConfiguration>>>().To<AsyncClient<IEnumerable<TaskConfiguration>>>();
        }

        public override T GetInstance<T>()
        {
            return kernel.Get<T>();
        }
    }
}