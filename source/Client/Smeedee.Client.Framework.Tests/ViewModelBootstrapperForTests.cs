using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Framework.ViewModel.Dialogs;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Testing.Services;
using TinyMVVM.IoC;

namespace Smeedee.Client.Framework.Tests
{
	public class ViewModelBootstrapperForTests
	{
		public static Mock<IUIInvoker> UIInvokerFake;
		public static Mock<IModuleLoader> ModuleLoaderFake;
		public static Mock<ITimer> TimerFake;
		public static Mock<IModalDialogService> ModalDialogServiceFake;
		public static Mock<IAsyncRepository<WidgetMetadata>> WidgetMetadataRepoFake;
        public static Mock<IPersistDomainModelsAsync<Configuration>> ConfigurationPersisterFake;
	    public static Mock<IPersistDomainModelsAsync<SlideConfiguration>> SlideConfigurationPersisterFake;

	    public static void Initialize()
		{
			UIInvokerFake = new Mock<IUIInvoker>();
			UIInvokerFake.Setup(s => s.Invoke(It.IsAny<Action>())).Callback((Action a) => a.Invoke());

			TimerFake = new Mock<ITimer>();

			ModuleLoaderFake = new Mock<IModuleLoader>();
			ModuleLoaderFake.Setup(s => s.LoadTraybarWidgets(It.IsAny<Traybar>())).
				Callback((Traybar traybar) =>
				{
					traybar.Widgets.Add(new Widget());
				});
			ModuleLoaderFake.Setup(s => s.LoadSlides(It.IsAny<Slideshow>())).
				Callback((Slideshow slideshow) =>
				{
					slideshow.Slides.Add(new Slide { Title = "First slide", Widget = new Widget(), SecondsOnScreen = 100});
					slideshow.Slides.Add(new Slide { Title = "Second slide", Widget = new Widget(), SecondsOnScreen = 110});
				});

			ModalDialogServiceFake = new Mock<IModalDialogService>();
			ModalDialogServiceFake.Setup(s => s.Show(
				It.IsAny<Dialog>(),
				It.IsAny<Action<bool>>())).Callback((Dialog dialog, Action<bool> dialogClosedCallback) =>
				{
					dialogClosedCallback.Invoke(true);
				});

			WidgetMetadataRepoFake = new Mock<IAsyncRepository<WidgetMetadata>>();
            ConfigurationPersisterFake = new Mock<IPersistDomainModelsAsync<Configuration>>();
            SlideConfigurationPersisterFake = new Mock<IPersistDomainModelsAsync<SlideConfiguration>>();

		    RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();
				config.Bind<IModalDialogService>().ToInstance(ModalDialogServiceFake.Object);
				config.Bind<ITimer>().ToInstance(TimerFake.Object);
				config.Bind<IModuleLoader>().ToInstance(ModuleLoaderFake.Object);
				config.Bind<IAsyncRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepoFake.Object);
			    config.Bind<IPersistDomainModelsAsync<Configuration>>().ToInstance(ConfigurationPersisterFake.Object);
			    config.Bind<IPersistDomainModelsAsync<SlideConfiguration>>().ToInstance(SlideConfigurationPersisterFake.Object);
			    config.Bind<IManageConfigurations>().ToInstance(new Mock<IManageConfigurations>().Object);
			});
		}
	}
}
