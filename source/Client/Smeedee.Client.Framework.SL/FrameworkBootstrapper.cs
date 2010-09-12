using System;
using System.Collections.Generic;
using Ninject;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.SL.Repositories;
using Smeedee.Client.Framework.SL.Repositories.Async;
using Smeedee.Client.Framework.SL.RESTService;
using Smeedee.Client.Framework.SL.Services.Impl;
using Smeedee.Client.Framework.SL.ViewModel.Repositories;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
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
using TinyMVVM.IoC;

namespace Smeedee.Client.Framework
{
    public class FrameworkBootstrapper  
    {
        public static void Initialize()
        {
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
        	{
				// Bind the other dependencies here that don't have any logical grouping
                config.Bind<IAsyncRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepository.Instance);
                config.Bind<IFullScreenService>().To<SLFullScreenService>();
                config.Bind<ILog>().To<Logger>();
                config.Bind<IManageConfigurations>().ToInstance(new ConfigurationManager(new StandardTimer(), new AsyncConfigurationRepository()));
                config.Bind<IMessageBoxService>().To<SLMessageBoxService>();
                config.Bind<IMetadataService>().To<MetadataService>();
                config.Bind<IModalDialogService>().To<SLModalDialogService>();
                config.Bind<IModuleLoader>().To<ModuleLoader>().InSingletonScope();
                config.Bind<IProgressbar>().To<ProgressbarDummy>();
                config.Bind<ITimer>().To<StandardTimer>();
                config.Bind<IUIInvoker>().To<UIInvoker>();
        	    config.Bind<SmeedeeREST>().To<SmeedeeREST>();
                config.Bind<IDownloadStringService>().To<WebClientDownloadStringService>();

                BindAsyncIDomainModelDeleters(config);
                BindAsyncPersisters(config);
                BindAsyncRepositories(config);
                BindBackgroundWorkerInvokers(config);
                BindDomainModelDeleters(config);
                BindDomainModelPersisters(config);
                BindRepositories(config);
        	});
        }

        #region Synchronized Bindings
        private static void BindRepositories(DependencyConfigSemantics config)
        {
			config.Bind<IRepository<Changeset>>().To<ChangesetWebserviceRepository>();
			config.Bind<IRepository<CIServer>>().To<CIServerWebserviceRepository>();
            config.Bind<IRepository<Configuration>>().To<ConfigurationWebserviceRepository>();
            config.Bind<IRepository<Holiday>>().To<HolidayWebserviceRepository>();
			config.Bind<IRepository<LogEntry>>().To<LogEntryWebserviceRepository>();
			config.Bind<IRepository<ProjectInfoServer>>().To<ProjectInfoRepository>();
            config.Bind<IRepository<RetrospectiveNote>>().To<RetrospectiveNoteWebServiceRepository>();
            config.Bind<IRepository<TaskConfiguration>>().To<TaskConfigurationWebserviceRepository>();
            config.Bind<IRepository<TaskDefinition>>().To<TaskDefinitionWebserviceRepository>();
            config.Bind<IRepository<TeamPicture>>().To<TeamPictureWebserviceRepository>();
            config.Bind<IRepository<User>>().To<UserWebserviceRepositoryProxy>();
            config.Bind<IRepository<Userdb>>().To<UserdbWebserviceRepository>();			
		}

        private static void BindDomainModelPersisters(DependencyConfigSemantics config)
        {
            config.Bind<IPersistDomainModels<Configuration>>().To<ConfigurationWebserviceRepository>();
            config.Bind<IPersistDomainModels<CIServer>>().To<CIServerWebserviceRepository>();
            config.Bind<IPersistDomainModels<Holiday>>().To<HolidayWebserviceRepository>();
            config.Bind<IPersistDomainModels<LogEntry>>().To<LogEntryWebserviceRepository>();
            config.Bind<IPersistDomainModels<RetrospectiveNote>>().To<RetrospectiveNoteWebServiceRepository>();
            config.Bind<IPersistDomainModels<TaskConfiguration>>().To<TaskConfigurationWebserviceRepository>();
            config.Bind<IPersistDomainModels<TeamPicture>>().To<TeamPictureWebserviceRepository>();
            config.Bind<IPersistDomainModels<Userdb>>().To<UserdbWebserviceRepository>();            
        }

        private static void BindDomainModelDeleters(DependencyConfigSemantics config)
        {
            config.Bind<IDeleteDomainModels<RetrospectiveNote>>().To<RetrospectiveNoteWebServiceRepository>();
        }
        #endregion

        #region Asynchronized Bindings
        private static void BindAsyncRepositories(DependencyConfigSemantics config)
        {
            config.Bind<IAsyncRepository<Changeset>>().To<AsyncChangesetRepository>();
            config.Bind<IAsyncRepository<CIServer>>().To<AsyncCIServerRepository>();
            config.Bind<IAsyncRepository<Configuration>>().To<AsyncConfigurationRepository>();
            config.Bind<IAsyncRepository<Holiday>>().To<AsyncHolidayRepository>();
            config.Bind<IAsyncRepository<LogEntry>>().To<AsyncLogEntryRepository>();
            config.Bind<IAsyncRepository<ProjectInfoServer>>().To<AsyncProjectInfoRepository>();
			config.Bind<IAsyncRepository<RetrospectiveNote>>().To<AsyncRetrospectiveNoteRepository>();
            config.Bind<IAsyncRepository<SlideConfiguration>>().To<AsyncSlideConfigurationRepository>();
            config.Bind<IAsyncRepository<TaskConfiguration>>().To<AsyncTaskConfigurationRepository>();
            config.Bind<IAsyncRepository<TaskDefinition>>().To<AsyncTaskDefinitionRepository>();
            config.Bind<IAsyncRepository<TeamPicture>>().To<AsyncTeamPictureRepository>();
            config.Bind<IAsyncRepository<Userdb>>().To<AsyncUserdbRepository>();            
        }

        private static void BindAsyncPersisters(DependencyConfigSemantics config)
        {
            config.Bind<IPersistDomainModelsAsync<CIServer>>().To<AsyncCIServerRepository>();
            config.Bind<IPersistDomainModelsAsync<Configuration>>().To<AsyncConfigurationRepository>();
            config.Bind<IPersistDomainModelsAsync<Holiday>>().To<AsyncHolidayRepository>();
            config.Bind<IPersistDomainModelsAsync<LogEntry>>().To<AsyncLogEntryRepository>();
            config.Bind<IPersistDomainModelsAsync<RetrospectiveNote>>().To<AsyncRetrospectiveNoteRepository>();
            config.Bind<IPersistDomainModelsAsync<SlideConfiguration>>().To<AsyncSlideConfigurationRepository>();
            config.Bind<IPersistDomainModelsAsync<TaskConfiguration>>().To<AsyncTaskConfigurationRepository>();
            config.Bind<IPersistDomainModelsAsync<TeamPicture>>().To<AsyncTeamPictureRepository>();
            config.Bind<IPersistDomainModelsAsync<Userdb>>().To<AsyncUserdbRepository>();            
        }

        private static void BindAsyncIDomainModelDeleters(DependencyConfigSemantics config)
        {
            config.Bind<IDeleteDomainModelsAsync<RetrospectiveNote>>().To<AsyncRetrospectiveNoteRepository>();
            config.Bind<IDeleteDomainModelsAsync<SlideConfiguration>>().To<AsyncSlideConfigurationRepository>();
            config.Bind<IDeleteDomainModelsAsync<TaskConfiguration>>().To<AsyncTaskConfigurationRepository>();            
        }

        #endregion

        private static void BindBackgroundWorkerInvokers(DependencyConfigSemantics config)
        {
            config.Bind<IInvokeBackgroundWorker<IEnumerable<Changeset>>>().To<AsyncClient<IEnumerable<Changeset>>>();
            config.Bind<IInvokeBackgroundWorker<IEnumerable<CIProject>>>().To<AsyncClient<IEnumerable<CIProject>>>();
            config.Bind<IInvokeBackgroundWorker<IEnumerable<Holiday>>>().To<AsyncClient<IEnumerable<Holiday>>>();
            config.Bind<IInvokeBackgroundWorker<IEnumerable<ProjectInfoServer>>>().To<AsyncClient<IEnumerable<ProjectInfoServer>>>();
			config.Bind<IInvokeBackgroundWorker<IEnumerable<RetrospectiveNote>>>().To<AsyncClient<IEnumerable<RetrospectiveNote>>>();
            config.Bind<IInvokeBackgroundWorker<IEnumerable<TaskConfiguration>>>().To<AsyncClient<IEnumerable<TaskConfiguration>>>();
            config.Bind<IInvokeBackgroundWorker<IEnumerable<TaskDefinition>>>().To<AsyncClient<IEnumerable<TaskDefinition>>>();
            config.Bind<IInvokeBackgroundWorker<IEnumerable<TeamPicture>>>().To<AsyncClient<IEnumerable<TeamPicture>>>();
			config.Bind<IInvokeBackgroundWorker<IEnumerable<Userdb>>>().To<AsyncClient<IEnumerable<Userdb>>>();            
        }
    }
}