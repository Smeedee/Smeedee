using System;
using System.ComponentModel.Composition;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Repositories;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using TinyMVVM.Framework;
using TinyMVVM.IoC;

namespace Smeedee.Client.Framework.ViewModel
{
    public partial class Widget
    {
        private IPersistDomainModelsAsync<Configuration> configRepo;

        public event EventHandler ConfigurationChanged;

        private Configuration configuration = null;
        private WidgetController controller;

        public Configuration Configuration 
        { 
            get
            {
                return configuration;
            }
            internal set
            {
               if( value != configuration )
               {
                   configuration = value;
                   if (ConfigurationChanged != null)
                        ConfigurationChanged(this, EventArgs.Empty);
               }
            } 
        }

        partial void OnInitialize()
        {
            ErrorInfo = new ErrorInfo();
			ViewProgressbar = new Progressbar();
			SettingsProgressbar = new Progressbar();
        	ProgressbarService = new WidgetProgressbar(UIInvoker, ViewProgressbar, SettingsProgressbar);
            
            Configuration = NewConfiguration();
            var configurationPersister = this.GetDependency<IPersistDomainModelsAsync<Configuration>>();
            
            var widgetConfigRepository = new WidgetConfigurationRepository(this, configurationPersister);
            //ConfigurationChanged += (o, e) => widgetConfigRepository.BeginGet(All.ItemsOf<Configuration>()); // temporary patch to support users of config repository
			
            this.ConfigureDependencies(config =>
			{
				config.MergeInGlobalDependenciesConfig = true;
				config.Bind<IProgressbar>().ToInstance(ProgressbarService);
			    config.Bind<Configuration>().ToInstance(Configuration);
			    config.Bind<Widget>().ToInstance(this);
			    config.Bind<IAsyncRepository<Configuration>>().ToInstance(widgetConfigRepository);
			    config.Bind<IRepository<Configuration>>().ToInstance(widgetConfigRepository);
			    config.Bind<IPersistDomainModelsAsync<Configuration>>().ToInstance(widgetConfigRepository);
			    config.Bind<IPersistDomainModels<Configuration>>().ToInstance(widgetConfigRepository);
				Configure(config);
			});

            controller = this.CreateController<WidgetController>();

        }

        public void SetConfigurationId(Guid id)
        {
            Configuration.Id = id;
            var configManager = this.GetDependency<IManageConfigurations>();
            configManager.RegisterForConfigurationUpdates(this);
        }

		public virtual void Configure(DependencyConfigSemantics config)
		{
			
		}

		protected virtual T GetInstance<T>() where T : class
		{
			return this.GetDependency<T>();
		}

		protected virtual T NewController<T>()
		{
			return this.CreateController<T>();
		}

        protected void ToggleSettings(object sender, EventArgs evt)
        {
            OnSettings();
        }

        public void OnSettings()
        {
            IsInSettingsMode = !IsInSettingsMode;
        }

        protected void ReportFailure(string message)
        {
            ErrorInfo.HasError = true;
            ErrorInfo.ErrorMessage = message;
        }

        protected void NoFailure()
        {
            ErrorInfo.HasError = false;
            ErrorInfo.ErrorMessage = string.Empty;
        }

        protected virtual Configuration NewConfiguration()
        {
            return new Configuration() {IsConfigured = false};
        }
    }
}
