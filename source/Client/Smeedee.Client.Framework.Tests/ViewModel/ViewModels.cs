
using System;
using NUnit.Framework;
using Smeedee.Client.Framework.ViewModel;
using TinyMVVM.Framework.Testing;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using TinyMVVM.Framework;

namespace Smeedee.Client.Tests.ViewModel
{
	public abstract class ApplicationContextTestContext : TestContext
	{
		protected ApplicationContext viewModel;

		[SetUp]
		public void Setup()
		{
			ServiceLocator.SetLocator(ServiceLocatorForTesting.GetServiceLocator());
			
			Context();
		}

		public abstract void Context();

		public void Given_ApplicationContext_is_created()
		{
			viewModel = new ApplicationContext();
		}

		public void And_ApplicationContext_is_created()
		{
			viewModel = new ApplicationContext();
		}
		
		public void And_Title_is_set(string value)
		{
			viewModel.Title = value;
		}

	
		public void And_Subtitle_is_set(string value)
		{
			viewModel.Subtitle = value;
		}

	
		public void And_Traybar_is_set(Traybar value)
		{
			viewModel.Traybar = value;
		}

	
		public void And_Slideshow_is_set(Slideshow value)
		{
			viewModel.Slideshow = value;
		}

	
		
	
		public void And_ApplicationContext_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public void When_Title_is_set(string value)
		{
			viewModel.Title = value;
		}
		
		public void When_Subtitle_is_set(string value)
		{
			viewModel.Subtitle = value;
		}
		
		public void When_Traybar_is_set(Traybar value)
		{
			viewModel.Traybar = value;
		}
		
		public void When_Slideshow_is_set(Slideshow value)
		{
			viewModel.Slideshow = value;
		}
		
		
		public void When_ApplicationContext_is_spawned()
		{
			viewModel = new ApplicationContext();
		} 
		
			
		public void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class ErrorInfoTestContext : TestContext
	{
		protected ErrorInfo viewModel;

		[SetUp]
		public void Setup()
		{
			ServiceLocator.SetLocator(ServiceLocatorForTesting.GetServiceLocator());
			
			Context();
		}

		public abstract void Context();

		public void Given_ErrorInfo_is_created()
		{
			viewModel = new ErrorInfo();
		}

		public void And_ErrorInfo_is_created()
		{
			viewModel = new ErrorInfo();
		}
		
		public void And_HasError_is_set(bool value)
		{
			viewModel.HasError = value;
		}

	
		public void And_ErrorMessage_is_set(string value)
		{
			viewModel.ErrorMessage = value;
		}

	
		
	
		public void And_ErrorInfo_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public void When_HasError_is_set(bool value)
		{
			viewModel.HasError = value;
		}
		
		public void When_ErrorMessage_is_set(string value)
		{
			viewModel.ErrorMessage = value;
		}
		
		
		public void When_ErrorInfo_is_spawned()
		{
			viewModel = new ErrorInfo();
		} 
		
			
		public void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class WidgetTestContext : TestContext
	{
		protected Widget viewModel;

		[SetUp]
		public void Setup()
		{
			ServiceLocator.SetLocator(ServiceLocatorForTesting.GetServiceLocator());
			
			Context();
		}

		public abstract void Context();

		public void Given_Widget_is_created()
		{
			viewModel = new Widget();
		}

		public void And_Widget_is_created()
		{
			viewModel = new Widget();
		}
		
		public void And_View_is_set(FrameworkElement value)
		{
			viewModel.View = value;
		}

	
		public void And_ErrorInfo_is_set(ErrorInfo value)
		{
			viewModel.ErrorInfo = value;
		}

	
		
	
		public void And_Widget_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public void When_View_is_set(FrameworkElement value)
		{
			viewModel.View = value;
		}
		
		public void When_ErrorInfo_is_set(ErrorInfo value)
		{
			viewModel.ErrorInfo = value;
		}
		
		
		public void When_Widget_is_spawned()
		{
			viewModel = new Widget();
		} 
		
			
		public void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class TraybarWidgetTestContext : TestContext
	{
		protected TraybarWidget viewModel;

		[SetUp]
		public void Setup()
		{
			ServiceLocator.SetLocator(ServiceLocatorForTesting.GetServiceLocator());
			
			Context();
		}

		public abstract void Context();

		public void Given_TraybarWidget_is_created()
		{
			viewModel = new TraybarWidget();
		}

		public void And_TraybarWidget_is_created()
		{
			viewModel = new TraybarWidget();
		}
		
		
	
		public void And_TraybarWidget_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		
		public void When_TraybarWidget_is_spawned()
		{
			viewModel = new TraybarWidget();
		} 
		
			
		public void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class TraybarTestContext : TestContext
	{
		protected Traybar viewModel;

		[SetUp]
		public void Setup()
		{
			ServiceLocator.SetLocator(ServiceLocatorForTesting.GetServiceLocator());
			
			Context();
		}

		public abstract void Context();

		public void Given_Traybar_is_created()
		{
			viewModel = new Traybar();
		}

		public void And_Traybar_is_created()
		{
			viewModel = new Traybar();
		}
		
		public void And_Widgets_is_set(ObservableCollection<TraybarWidget> value)
		{
			viewModel.Widgets = value;
		}

		public void When_add_Widget(Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
	
		public void And_ErrorInfo_is_set(ErrorInfo value)
		{
			viewModel.ErrorInfo = value;
		}

	
		
	
		public void And_Traybar_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public void When_Widgets_is_set(ObservableCollection<TraybarWidget> value)
		{
			viewModel.Widgets = value;
		}
		
		public void When_ErrorInfo_is_set(ErrorInfo value)
		{
			viewModel.ErrorInfo = value;
		}
		
		
		public void When_Traybar_is_spawned()
		{
			viewModel = new Traybar();
		} 
		
			
		public void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class SlideshowTestContext : TestContext
	{
		protected Slideshow viewModel;

		[SetUp]
		public void Setup()
		{
			ServiceLocator.SetLocator(ServiceLocatorForTesting.GetServiceLocator());
			
			Context();
		}

		public abstract void Context();

		public void Given_Slideshow_is_created()
		{
			viewModel = new Slideshow();
		}

		public void And_Slideshow_is_created()
		{
			viewModel = new Slideshow();
		}
		
		public void And_Slides_is_set(ObservableCollection<Slide> value)
		{
			viewModel.Slides = value;
		}

		public void When_add_Slide(Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
	
		public void And_ErrorInfo_is_set(ErrorInfo value)
		{
			viewModel.ErrorInfo = value;
		}

	
		public void And_CurrentSlide_is_set(Slide value)
		{
			viewModel.CurrentSlide = value;
		}

	
		public void And_SlideshowInfo_is_set(string value)
		{
			viewModel.SlideshowInfo = value;
		}

	
		
	
		public void And_Slideshow_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public void When_Slides_is_set(ObservableCollection<Slide> value)
		{
			viewModel.Slides = value;
		}
		
		public void When_ErrorInfo_is_set(ErrorInfo value)
		{
			viewModel.ErrorInfo = value;
		}
		
		public void When_CurrentSlide_is_set(Slide value)
		{
			viewModel.CurrentSlide = value;
		}
		
		public void When_SlideshowInfo_is_set(string value)
		{
			viewModel.SlideshowInfo = value;
		}
		
		
		public void When_Slideshow_is_spawned()
		{
			viewModel = new Slideshow();
		} 
		
		public void And_Start_Command_is_executed()
		{
			viewModel.Start.Execute(null);
		}

		public void When_execute_Start_Command()
		{
			viewModel.Start.Execute(null);
		}
		public void And_Pause_Command_is_executed()
		{
			viewModel.Pause.Execute(null);
		}

		public void When_execute_Pause_Command()
		{
			viewModel.Pause.Execute(null);
		}
		public void And_Next_Command_is_executed()
		{
			viewModel.Next.Execute(null);
		}

		public void When_execute_Next_Command()
		{
			viewModel.Next.Execute(null);
		}
		public void And_Previous_Command_is_executed()
		{
			viewModel.Previous.Execute(null);
		}

		public void When_execute_Previous_Command()
		{
			viewModel.Previous.Execute(null);
		}
			
		public void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class SlideTestContext : TestContext
	{
		protected Slide viewModel;

		[SetUp]
		public void Setup()
		{
			ServiceLocator.SetLocator(ServiceLocatorForTesting.GetServiceLocator());
			
			Context();
		}

		public abstract void Context();

		public void Given_Slide_is_created()
		{
			viewModel = new Slide();
		}

		public void And_Slide_is_created()
		{
			viewModel = new Slide();
		}
		
		public void And_Title_is_set(string value)
		{
			viewModel.Title = value;
		}

	
		public void And_SettingsView_is_set(FrameworkElement value)
		{
			viewModel.SettingsView = value;
		}

	
		public void And_IsInSettingsMode_is_set(bool value)
		{
			viewModel.IsInSettingsMode = value;
		}

	
		
	
		public void And_Slide_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public void When_Title_is_set(string value)
		{
			viewModel.Title = value;
		}
		
		public void When_SettingsView_is_set(FrameworkElement value)
		{
			viewModel.SettingsView = value;
		}
		
		public void When_IsInSettingsMode_is_set(bool value)
		{
			viewModel.IsInSettingsMode = value;
		}
		
		
		public void When_Slide_is_spawned()
		{
			viewModel = new Slide();
		} 
		
		public void And_Settings_Command_is_executed()
		{
			viewModel.Settings.Execute(null);
		}

		public void When_execute_Settings_Command()
		{
			viewModel.Settings.Execute(null);
		}
			
		public void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class NotifierTestContext : TestContext
	{
		protected Notifier viewModel;

		[SetUp]
		public void Setup()
		{
			ServiceLocator.SetLocator(ServiceLocatorForTesting.GetServiceLocator());
			
			Context();
		}

		public abstract void Context();

		public void Given_Notifier_is_created()
		{
			viewModel = new Notifier();
		}

		public void And_Notifier_is_created()
		{
			viewModel = new Notifier();
		}
		
		public void And_Notifications_is_set(ObservableCollection<Notification> value)
		{
			viewModel.Notifications = value;
		}

		public void When_add_Notification(Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
	
		
	
		public void And_Notifier_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public void When_Notifications_is_set(ObservableCollection<Notification> value)
		{
			viewModel.Notifications = value;
		}
		
		
		public void When_Notifier_is_spawned()
		{
			viewModel = new Notifier();
		} 
		
			
		public void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

	public abstract class NotificationTestContext : TestContext
	{
		protected Notification viewModel;

		[SetUp]
		public void Setup()
		{
			ServiceLocator.SetLocator(ServiceLocatorForTesting.GetServiceLocator());
			
			Context();
		}

		public abstract void Context();

		public void Given_Notification_is_created()
		{
			viewModel = new Notification();
		}

		public void And_Notification_is_created()
		{
			viewModel = new Notification();
		}
		
		public void And_Message_is_set(string value)
		{
			viewModel.Message = value;
		}

	
		public void And_Displayed_is_set(bool value)
		{
			viewModel.Displayed = value;
		}

	
		
	
		public void And_Notification_PropertyChangeRecording_is_Started()
		{
			viewModel.PropertyChangeRecorder.Start();
		}

		public void When_Message_is_set(string value)
		{
			viewModel.Message = value;
		}
		
		public void When_Displayed_is_set(bool value)
		{
			viewModel.Displayed = value;
		}
		
		
		public void When_Notification_is_spawned()
		{
			viewModel = new Notification();
		} 
		
			
		public void And(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}

		public void When(string description, Action unitOfWork)
		{
			unitOfWork.Invoke();
		}
}

}

