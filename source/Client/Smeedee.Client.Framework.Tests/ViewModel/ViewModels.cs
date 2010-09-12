
using System;
using NUnit.Framework;
using Moq;
using Smeedee.Client.Framework.Tests;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using TinyMVVM.IoC;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Testing.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using TinyMVVM.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel.Dialogs;

namespace Smeedee.Client.Tests.ViewModel
{
	public abstract class ApplicationContextTestContext 
	{
		protected ApplicationContext viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
		
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_ApplicationContext_is_created()
		{
			viewModel = new ApplicationContext();
		}

		public void And_ApplicationContext_is_created()
		{
			viewModel = new ApplicationContext();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_DockBar_is_set(DockBar value)
		{
			viewModel.DockBar = value;
		}

	
		public virtual void And_Traybar_is_set(Traybar value)
		{
			viewModel.Traybar = value;
		}

	
		public virtual void And_Slideshow_is_set(Slideshow value)
		{
			viewModel.Slideshow = value;
		}

	
		
	
		public virtual void And_ApplicationContext_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_DockBar_is_set(DockBar value)
		{
			viewModel.DockBar = value;
		}
		
		public virtual void When_Traybar_is_set(Traybar value)
		{
			viewModel.Traybar = value;
		}
		
		public virtual void When_Slideshow_is_set(Slideshow value)
		{
			viewModel.Slideshow = value;
		}
		
		
		public virtual void When_ApplicationContext_is_spawned()
		{
			viewModel = new ApplicationContext();
		} 
		
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class ErrorInfoTestContext 
	{
		protected ErrorInfo viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
		
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_ErrorInfo_is_created()
		{
			viewModel = new ErrorInfo();
		}

		public void And_ErrorInfo_is_created()
		{
			viewModel = new ErrorInfo();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_HasError_is_set(bool value)
		{
			viewModel.HasError = value;
		}

	
		public virtual void And_ErrorMessage_is_set(string value)
		{
			viewModel.ErrorMessage = value;
		}

	
		
	
		public virtual void And_ErrorInfo_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_HasError_is_set(bool value)
		{
			viewModel.HasError = value;
		}
		
		public virtual void When_ErrorMessage_is_set(string value)
		{
			viewModel.ErrorMessage = value;
		}
		
		
		public virtual void When_ErrorInfo_is_spawned()
		{
			viewModel = new ErrorInfo();
		} 
		
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class WidgetTestContext 
	{
		protected Widget viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
        protected Mock<IPersistDomainModelsAsync<Configuration>> ConfigurationPersisterFake = new Mock<IPersistDomainModelsAsync<Configuration>>();
        

		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
			    config.Bind<IPersistDomainModelsAsync<Configuration>>().ToInstance(ConfigurationPersisterFake.Object);
			    config.Bind<IManageConfigurations>().ToInstance(new Mock<IManageConfigurations>().Object);
			});
            

			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return viewModel.GetDependency<Mock<T>>();
		}

		public abstract void Context();

		public virtual void Given_Widget_is_created()
		{
			viewModel = new Widget();
		}

		public void And_Widget_is_created()
		{
			viewModel = new Widget();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_Title_is_set(string value)
		{
			viewModel.Title = value;
		}

	
		public virtual void And_View_is_set(FrameworkElement value)
		{
			viewModel.View = value;
		}

	
		public virtual void And_SettingsView_is_set(FrameworkElement value)
		{
			viewModel.SettingsView = value;
		}

	
		public virtual void And_ErrorInfo_is_set(ErrorInfo value)
		{
			viewModel.ErrorInfo = value;
		}

	
		public virtual void And_IsInSettingsMode_is_set(bool value)
		{
			viewModel.IsInSettingsMode = value;
		}

	
		public virtual void And_ViewProgressbar_is_set(Progressbar value)
		{
			viewModel.ViewProgressbar = value;
		}

	
		public virtual void And_SettingsProgressbar_is_set(Progressbar value)
		{
			viewModel.SettingsProgressbar = value;
		}

	
		public virtual void And_ProgressbarService_is_set(IProgressbar value)
		{
			viewModel.ProgressbarService = value;
		}

	
		
	
		public virtual void And_Widget_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_Title_is_set(string value)
		{
			viewModel.Title = value;
		}
		
		public virtual void When_View_is_set(FrameworkElement value)
		{
			viewModel.View = value;
		}
		
		public virtual void When_SettingsView_is_set(FrameworkElement value)
		{
			viewModel.SettingsView = value;
		}
		
		public virtual void When_ErrorInfo_is_set(ErrorInfo value)
		{
			viewModel.ErrorInfo = value;
		}
		
		public virtual void When_IsInSettingsMode_is_set(bool value)
		{
			viewModel.IsInSettingsMode = value;
		}
		
		public virtual void When_ViewProgressbar_is_set(Progressbar value)
		{
			viewModel.ViewProgressbar = value;
		}
		
		public virtual void When_SettingsProgressbar_is_set(Progressbar value)
		{
			viewModel.SettingsProgressbar = value;
		}
		
		public virtual void When_ProgressbarService_is_set(IProgressbar value)
		{
			viewModel.ProgressbarService = value;
		}
		
		
		public virtual void When_Widget_is_spawned()
		{
			viewModel = new Widget();
		} 
		
		public virtual void And_Settings_Command_is_executed()
		{
			viewModel.Settings.Execute(null);
		}

		public virtual void When_execute_Settings_Command()
		{
			viewModel.Settings.Execute(null);
		}
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class TraybarTestContext 
	{
		protected Traybar viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
		
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_Traybar_is_created()
		{
			viewModel = new Traybar();
		}

		public void And_Traybar_is_created()
		{
			viewModel = new Traybar();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_Widgets_is_set(ObservableCollection<Widget> value)
		{
			viewModel.Widgets = value;
		}

		public virtual void When_add_Widget(Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
	
		public virtual void And_ErrorInfo_is_set(ErrorInfo value)
		{
			viewModel.ErrorInfo = value;
		}

	
		
	
		public virtual void And_Traybar_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_Widgets_is_set(ObservableCollection<Widget> value)
		{
			viewModel.Widgets = value;
		}
		
		public virtual void When_ErrorInfo_is_set(ErrorInfo value)
		{
			viewModel.ErrorInfo = value;
		}
		
		
		public virtual void When_Traybar_is_spawned()
		{
			viewModel = new Traybar();
		} 
		
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class SlideshowTestContext 
	{
		protected Slideshow viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
		
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_Slideshow_is_created()
		{
			viewModel = new Slideshow();
		}

		public void And_Slideshow_is_created()
		{
			viewModel = new Slideshow();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_Slides_is_set(ObservableCollection<Slide> value)
		{
			viewModel.Slides = value;
		}

		public virtual void When_add_Slide(Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
	
		public virtual void And_ErrorInfo_is_set(ErrorInfo value)
		{
			viewModel.ErrorInfo = value;
		}

	
		public virtual void And_CurrentSlide_is_set(Slide value)
		{
			viewModel.CurrentSlide = value;
		}

	
		public virtual void And_SlideshowInfo_is_set(string value)
		{
			viewModel.SlideshowInfo = value;
		}

	
		public virtual void And_IsRunning_is_set(bool value)
		{
			viewModel.IsRunning = value;
		}

	
		public virtual void And_TimeLeftOfSlideInPercent_is_set(double value)
		{
			viewModel.TimeLeftOfSlideInPercent = value;
		}

	
		
	
		public virtual void And_Slideshow_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_Slides_is_set(ObservableCollection<Slide> value)
		{
			viewModel.Slides = value;
		}
				
		public virtual void When_ErrorInfo_is_set(ErrorInfo value)
		{
			viewModel.ErrorInfo = value;
		}
		
		public virtual void When_CurrentSlide_is_set(Slide value)
		{
			viewModel.CurrentSlide = value;
		}
		
		public virtual void When_SlideshowInfo_is_set(string value)
		{
			viewModel.SlideshowInfo = value;
		}
		
		public virtual void When_IsRunning_is_set(bool value)
		{
			viewModel.IsRunning = value;
		}
		
		public virtual void When_TimeLeftOfSlideInPercent_is_set(double value)
		{
			viewModel.TimeLeftOfSlideInPercent = value;
		}
		
		
		public virtual void When_Slideshow_is_spawned()
		{
			viewModel = new Slideshow();
		} 
		
		public virtual void And_Start_Command_is_executed()
		{
			viewModel.Start.Execute(null);
		}

		public virtual void When_execute_Start_Command()
		{
			viewModel.Start.Execute(null);
		}
		public virtual void And_Pause_Command_is_executed()
		{
			viewModel.Pause.Execute(null);
		}

		public virtual void When_execute_Pause_Command()
		{
			viewModel.Pause.Execute(null);
		}
		public virtual void And_Next_Command_is_executed()
		{
			viewModel.Next.Execute(null);
		}

		public virtual void When_execute_Next_Command()
		{
			viewModel.Next.Execute(null);
		}
		public virtual void And_Previous_Command_is_executed()
		{
			viewModel.Previous.Execute(null);
		}

		public virtual void When_execute_Previous_Command()
		{
			viewModel.Previous.Execute(null);
		}
		public virtual void And_AddSlide_Command_is_executed()
		{
			viewModel.AddSlide.Execute(null);
		}

		public virtual void When_execute_AddSlide_Command()
		{
			viewModel.AddSlide.Execute(null);
		}
		public virtual void And_Edit_Command_is_executed()
		{
			viewModel.Edit.Execute(null);
		}

		public virtual void When_execute_Edit_Command()
		{
			viewModel.Edit.Execute(null);
		}
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class SlideTestContext 
	{
		protected Slide viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	    protected Mock<IPersistDomainModelsAsync<Configuration>> ConfigurationPersisterFake = new Mock<IPersistDomainModelsAsync<Configuration>>();
        protected Mock<IManageConfigurations> ConfigManagerMock = new Mock<IManageConfigurations>();

		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
			    config.Bind<IPersistDomainModelsAsync<Configuration>>().ToInstance(ConfigurationPersisterFake.Object);
			    config.Bind<IManageConfigurations>().ToInstance(ConfigManagerMock.Object);
			});

			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_Slide_is_created()
		{
			viewModel = new Slide();
		}

		public void And_Slide_is_created()
		{
			viewModel = new Slide();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_Title_is_set(string value)
		{
			viewModel.Title = value;
		}

	
		public virtual void And_SecondsOnScreen_is_set(int value)
		{
			viewModel.SecondsOnScreen = value;
		}

	
		public virtual void And_Widget_is_set(Widget value)
		{
			viewModel.Widget = value;
		}

	
		
	
		public virtual void And_Slide_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_Title_is_set(string value)
		{
			viewModel.Title = value;
		}
		
		public virtual void When_SecondsOnScreen_is_set(int value)
		{
			viewModel.SecondsOnScreen = value;
		}
		
		public virtual void When_Widget_is_set(Widget value)
		{
			viewModel.Widget = value;
		}
		
		
		public virtual void When_Slide_is_spawned()
		{
			viewModel = new Slide();
		} 
		
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class SettingsViewModelBaseTestContext 
	{
		protected SettingsViewModelBase viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
		
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_SettingsViewModelBase_is_created()
		{
			viewModel = new SettingsViewModelBase();
		}

		public void And_SettingsViewModelBase_is_created()
		{
			viewModel = new SettingsViewModelBase();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_HasChanges_is_set(bool value)
		{
			viewModel.HasChanges = value;
		}

	
		
	
		public virtual void And_SettingsViewModelBase_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_HasChanges_is_set(bool value)
		{
			viewModel.HasChanges = value;
		}
		
		
		public virtual void When_SettingsViewModelBase_is_spawned()
		{
			viewModel = new SettingsViewModelBase();
		} 
		
		public virtual void And_Save_Command_is_executed()
		{
			viewModel.Save.Execute(null);
		}

		public virtual void When_execute_Save_Command()
		{
			viewModel.Save.Execute(null);
		}
		public virtual void And_ReloadSettings_Command_is_executed()
		{
			viewModel.ReloadSettings.Execute(null);
		}

		public virtual void When_execute_ReloadSettings_Command()
		{
			viewModel.ReloadSettings.Execute(null);
		}
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class NotifierTestContext 
	{
		protected Notifier viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
		
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_Notifier_is_created()
		{
			viewModel = new Notifier();
		}

		public void And_Notifier_is_created()
		{
			viewModel = new Notifier();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_Notifications_is_set(ObservableCollection<Notification> value)
		{
			viewModel.Notifications = value;
		}

		public virtual void When_add_Notification(Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
	
		
	
		public virtual void And_Notifier_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_Notifications_is_set(ObservableCollection<Notification> value)
		{
			viewModel.Notifications = value;
		}
		
		
		public virtual void When_Notifier_is_spawned()
		{
			viewModel = new Notifier();
		} 
		
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class NotificationTestContext 
	{
		protected Notification viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
		
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_Notification_is_created()
		{
			viewModel = new Notification();
		}

		public void And_Notification_is_created()
		{
			viewModel = new Notification();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_Message_is_set(string value)
		{
			viewModel.Message = value;
		}

	
		public virtual void And_Displayed_is_set(bool value)
		{
			viewModel.Displayed = value;
		}

	
		
	
		public virtual void And_Notification_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_Message_is_set(string value)
		{
			viewModel.Message = value;
		}
		
		public virtual void When_Displayed_is_set(bool value)
		{
			viewModel.Displayed = value;
		}
		
		
		public virtual void When_Notification_is_spawned()
		{
			viewModel = new Notification();
		} 
		
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class ProgressbarTestContext 
	{
		protected Progressbar viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
		
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_Progressbar_is_created()
		{
			viewModel = new Progressbar();
		}

		public void And_Progressbar_is_created()
		{
			viewModel = new Progressbar();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_Message_is_set(string value)
		{
			viewModel.Message = value;
		}

	
		public virtual void And_IsVisible_is_set(bool value)
		{
			viewModel.IsVisible = value;
		}

	
		
	
		public virtual void And_Progressbar_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_Message_is_set(string value)
		{
			viewModel.Message = value;
		}
		
		public virtual void When_IsVisible_is_set(bool value)
		{
			viewModel.IsVisible = value;
		}
		
		
		public virtual void When_Progressbar_is_spawned()
		{
			viewModel = new Progressbar();
		} 
		
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class WidgetMetadataTestContext 
	{
		protected WidgetMetadata viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
		
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_WidgetMetadata_is_created()
		{
			viewModel = new WidgetMetadata();
		}

		public void And_WidgetMetadata_is_created()
		{
			viewModel = new WidgetMetadata();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_Name_is_set(string value)
		{
			viewModel.Name = value;
		}

	
		public virtual void And_Description_is_set(string value)
		{
			viewModel.Description = value;
		}

	
		public virtual void And_Author_is_set(string value)
		{
			viewModel.Author = value;
		}

	
		public virtual void And_Tags_is_set(string[] value)
		{
			viewModel.Tags = value;
		}

	
		public virtual void And_Version_is_set(string value)
		{
			viewModel.Version = value;
		}

	
		public virtual void And_XAPName_is_set(string value)
		{
			viewModel.XAPName = value;
		}

	
		public virtual void And_Type_is_set(Type value)
		{
			viewModel.Type = value;
		}

	
		public virtual void And_UserSelectedTitle_is_set(string value)
		{
			viewModel.UserSelectedTitle = value;
		}

	
		public virtual void And_SecondsOnScreen_is_set(int value)
		{
			viewModel.SecondsOnScreen = value;
		}

	
		public virtual void And_IsSelected_is_set(bool value)
		{
			viewModel.IsSelected = value;
		}

	
		public virtual void And_IsDescriptionCapped_is_set(bool value)
		{
			viewModel.IsDescriptionCapped = value;
		}

	
		
	
		public virtual void And_WidgetMetadata_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_Name_is_set(string value)
		{
			viewModel.Name = value;
		}
		
		public virtual void When_Description_is_set(string value)
		{
			viewModel.Description = value;
		}
		
		public virtual void When_Author_is_set(string value)
		{
			viewModel.Author = value;
		}
		
		public virtual void When_Tags_is_set(string[] value)
		{
			viewModel.Tags = value;
		}
		
		public virtual void When_Version_is_set(string value)
		{
			viewModel.Version = value;
		}
		
		public virtual void When_XAPName_is_set(string value)
		{
			viewModel.XAPName = value;
		}
		
		public virtual void When_Type_is_set(Type value)
		{
			viewModel.Type = value;
		}
		
		public virtual void When_UserSelectedTitle_is_set(string value)
		{
			viewModel.UserSelectedTitle = value;
		}
		
		public virtual void When_SecondsOnScreen_is_set(int value)
		{
			viewModel.SecondsOnScreen = value;
		}
		
		public virtual void When_IsSelected_is_set(bool value)
		{
			viewModel.IsSelected = value;
		}
		
		public virtual void When_IsDescriptionCapped_is_set(bool value)
		{
			viewModel.IsDescriptionCapped = value;
		}
		
		
		public virtual void When_WidgetMetadata_is_spawned()
		{
			viewModel = new WidgetMetadata();
		} 
		
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class DockBarTestContext 
	{
		protected DockBar viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
		
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_DockBar_is_created()
		{
			viewModel = new DockBar();
		}

		public void And_DockBar_is_created()
		{
			viewModel = new DockBar();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_IsVisible_is_set(bool value)
		{
			viewModel.IsVisible = value;
		}

	
		public virtual void And_Items_is_set(ObservableCollection<DockBarItem> value)
		{
			viewModel.Items = value;
		}

		public virtual void When_add_Item(Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
	
		
	
		public virtual void And_DockBar_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_IsVisible_is_set(bool value)
		{
			viewModel.IsVisible = value;
		}
		
		public virtual void When_Items_is_set(ObservableCollection<DockBarItem> value)
		{
			viewModel.Items = value;
		}
		
		
		public virtual void When_DockBar_is_spawned()
		{
			viewModel = new DockBar();
		} 
		
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class DockBarItemTestContext 
	{
		protected DockBarItem viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
		
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_DockBarItem_is_created()
		{
			viewModel = new DockBarItem();
		}

		public void And_DockBarItem_is_created()
		{
			viewModel = new DockBarItem();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_Description_is_set(string value)
		{
			viewModel.Description = value;
		}

	
		public virtual void And_Icon_is_set(FrameworkElement value)
		{
			viewModel.Icon = value;
		}

	
		
	
		public virtual void And_DockBarItem_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_Description_is_set(string value)
		{
			viewModel.Description = value;
		}
		
		public virtual void When_Icon_is_set(FrameworkElement value)
		{
			viewModel.Icon = value;
		}
		
		
		public virtual void When_DockBarItem_is_spawned()
		{
			viewModel = new DockBarItem();
		} 
		
		public virtual void And_Click_Command_is_executed()
		{
			viewModel.Click.Execute(null);
		}

		public virtual void When_execute_Click_Command()
		{
			viewModel.Click.Execute(null);
		}
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class DialogTestContext 
	{
		protected Dialog viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
		
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_Dialog_is_created()
		{
			viewModel = new Dialog();
		}

		public void And_Dialog_is_created()
		{
			viewModel = new Dialog();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_Progressbar_is_set(Progressbar value)
		{
			viewModel.Progressbar = value;
		}

	
		public virtual void And_Width_is_set(int value)
		{
			viewModel.Width = value;
		}

	
		public virtual void And_Height_is_set(int value)
		{
			viewModel.Height = value;
		}

	
		public virtual void And_DisplayOkButton_is_set(bool value)
		{
			viewModel.DisplayOkButton = value;
		}

	
		public virtual void And_DisplayCancelButton_is_set(bool value)
		{
			viewModel.DisplayCancelButton = value;
		}

	
		public virtual void And_Title_is_set(string value)
		{
			viewModel.Title = value;
		}

	
		public virtual void And_View_is_set(FrameworkElement value)
		{
			viewModel.View = value;
		}

	
		public virtual void And_ButtonBarCommands_is_set(ObservableCollection<DelegateCommand> value)
		{
			viewModel.ButtonBarCommands = value;
		}

		public virtual void When_add_ButtonBarCommand(Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
	
		
	
		public virtual void And_Dialog_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_Progressbar_is_set(Progressbar value)
		{
			viewModel.Progressbar = value;
		}
		
		public virtual void When_Width_is_set(int value)
		{
			viewModel.Width = value;
		}
		
		public virtual void When_Height_is_set(int value)
		{
			viewModel.Height = value;
		}
		
		public virtual void When_DisplayOkButton_is_set(bool value)
		{
			viewModel.DisplayOkButton = value;
		}
		
		public virtual void When_DisplayCancelButton_is_set(bool value)
		{
			viewModel.DisplayCancelButton = value;
		}
		
		public virtual void When_Title_is_set(string value)
		{
			viewModel.Title = value;
		}
		
		public virtual void When_View_is_set(FrameworkElement value)
		{
			viewModel.View = value;
		}
		
		public virtual void When_ButtonBarCommands_is_set(ObservableCollection<DelegateCommand> value)
		{
			viewModel.ButtonBarCommands = value;
		}
		
		
		public virtual void When_Dialog_is_spawned()
		{
			viewModel = new Dialog();
		} 
		
		public virtual void And_Ok_Command_is_executed()
		{
			viewModel.Ok.Execute(null);
		}

		public virtual void When_execute_Ok_Command()
		{
			viewModel.Ok.Execute(null);
		}
		public virtual void And_Cancel_Command_is_executed()
		{
			viewModel.Cancel.Execute(null);
		}

		public virtual void When_execute_Cancel_Command()
		{
			viewModel.Cancel.Execute(null);
		}
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class SelectWidgetsDialogTestContext 
	{
		protected SelectWidgetsDialog viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
        protected Mock<IAsyncRepository<WidgetMetadata>> WidgetMetadataAsyncRepositoryFake = new Mock<IAsyncRepository<WidgetMetadata>>();
        protected Mock<IPersistDomainModelsAsync<SlideConfiguration>> SlideConfigurationPersister = new Mock<IPersistDomainModelsAsync<SlideConfiguration>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);

			    config.Bind<IAsyncRepository<WidgetMetadata>>().ToInstance(WidgetMetadataAsyncRepositoryFake.Object);
			    config.Bind<IPersistDomainModelsAsync<SlideConfiguration>>().ToInstance(SlideConfigurationPersister.Object);
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_SelectWidgetsDialog_is_created()
		{
			viewModel = new SelectWidgetsDialog();
		}

		public void And_SelectWidgetsDialog_is_created()
		{
			viewModel = new SelectWidgetsDialog();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_AvailableWidgets_is_set(ObservableCollection<WidgetMetadata> value)
		{
			viewModel.AvailableWidgets = value;
		}

		public virtual void When_add_AvailableWidget(Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
	
		public virtual void And_Progressbar_is_set(Progressbar value)
		{
			viewModel.Progressbar = value;
		}

	
		public virtual void And_SearchTerm_is_set(string value)
		{
			viewModel.SearchTerm = value;
		}

	
		
	
		public virtual void And_SelectWidgetsDialog_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_AvailableWidgets_is_set(ObservableCollection<WidgetMetadata> value)
		{
			viewModel.AvailableWidgets = value;
		}
		
		public virtual void When_Progressbar_is_set(Progressbar value)
		{
			viewModel.Progressbar = value;
		}
		
		public virtual void When_SearchTerm_is_set(string value)
		{
			viewModel.SearchTerm = value;
		}
		
		
		public virtual void When_SelectWidgetsDialog_is_spawned()
		{
			viewModel = new SelectWidgetsDialog();
		} 
		
		public virtual void And_SelectAll_Command_is_executed()
		{
			viewModel.SelectAll.Execute(null);
		}

		public virtual void When_execute_SelectAll_Command()
		{
			viewModel.SelectAll.Execute(null);
		}
		public virtual void And_DeselectAll_Command_is_executed()
		{
			viewModel.DeselectAll.Execute(null);
		}

		public virtual void When_execute_DeselectAll_Command()
		{
			viewModel.DeselectAll.Execute(null);
		}
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class EditSlideshowDialogTestContext 
	{
		protected EditSlideshowDialog viewModel;

		protected Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>> ApplicationContextRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ApplicationContext>>();
		protected Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>> ErrorInfoRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<ErrorInfo>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Widget>> WidgetRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Widget>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Traybar>> TraybarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Traybar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slideshow>> SlideshowRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slideshow>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Slide>> SlideRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Slide>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>> SettingsViewModelBaseRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notifier>> NotifierRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notifier>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Notification>> NotificationRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Notification>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Progressbar>> ProgressbarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Progressbar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>> WidgetMetadataRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<WidgetMetadata>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBar>> DockBarRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBar>>();
		protected Mock<TinyMVVM.Repositories.IRepository<DockBarItem>> DockBarItemRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<DockBarItem>>();
		protected Mock<TinyMVVM.Repositories.IRepository<Dialog>> DialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<Dialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>> SelectWidgetsDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>();
		protected Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>> EditSlideshowDialogRepositoryFake = new Mock<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>();
	
		[SetUp]
		public void Setup()
		{
			RemoveAllGlobalDependencies.ForAllViewModels();
			ConfigureGlobalDependencies.ForAllViewModels(config =>
			{
				config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

				config.Bind<TinyMVVM.Repositories.IRepository<ApplicationContext>>().ToInstance(ApplicationContextRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<ErrorInfo>>().ToInstance(ErrorInfoRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Widget>>().ToInstance(WidgetRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Traybar>>().ToInstance(TraybarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slideshow>>().ToInstance(SlideshowRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Slide>>().ToInstance(SlideRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SettingsViewModelBase>>().ToInstance(SettingsViewModelBaseRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notifier>>().ToInstance(NotifierRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Notification>>().ToInstance(NotificationRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Progressbar>>().ToInstance(ProgressbarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<WidgetMetadata>>().ToInstance(WidgetMetadataRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBar>>().ToInstance(DockBarRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<DockBarItem>>().ToInstance(DockBarItemRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<Dialog>>().ToInstance(DialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<SelectWidgetsDialog>>().ToInstance(SelectWidgetsDialogRepositoryFake.Object);
				config.Bind<TinyMVVM.Repositories.IRepository<EditSlideshowDialog>>().ToInstance(EditSlideshowDialogRepositoryFake.Object);
		
			});


			Context();
		}

		public Mock<T> GetFakeFor<T>() where T: class
		{
			return null;
		}

		public abstract void Context();

		public virtual void Given_EditSlideshowDialog_is_created()
		{
			viewModel = new EditSlideshowDialog();
		}

		public void And_EditSlideshowDialog_is_created()
		{
			viewModel = new EditSlideshowDialog();
		}

		public void And_PropertychangedRecorder_is_started()
		{
		    viewModel.PropertyChangeRecorder.Start();
		}
		
		public virtual void And_Slideshow_is_set(Slideshow value)
		{
			viewModel.Slideshow = value;
		}

	
		
	
		public virtual void And_EditSlideshowDialog_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public virtual void When_Slideshow_is_set(Slideshow value)
		{
			viewModel.Slideshow = value;
		}
		
		
		public virtual void When_EditSlideshowDialog_is_spawned()
		{
			viewModel = new EditSlideshowDialog();
		} 
		
		public virtual void And_Delete_Command_is_executed()
		{
			viewModel.Delete.Execute(null);
		}

		public virtual void When_execute_Delete_Command()
		{
			viewModel.Delete.Execute(null);
		}
		public virtual void And_MoveLeft_Command_is_executed()
		{
			viewModel.MoveLeft.Execute(null);
		}

		public virtual void When_execute_MoveLeft_Command()
		{
			viewModel.MoveLeft.Execute(null);
		}
		public virtual void And_MoveRight_Command_is_executed()
		{
			viewModel.MoveRight.Execute(null);
		}

		public virtual void When_execute_MoveRight_Command()
		{
			viewModel.MoveRight.Execute(null);
		}
			
		public virtual void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public virtual void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

}

