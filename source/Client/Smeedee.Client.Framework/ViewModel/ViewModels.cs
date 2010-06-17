
using TinyMVVM.Framework.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using TinyMVVM.Framework;

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class ApplicationContext : TinyMVVM.Framework.ViewModelBase
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		public string Title
		{
			get { return _Title; }
			set
			{
				if (value != _Title)
				{
					UIInvoker.Invoke(() =>
					{
						_Title = value;
						TriggerPropertyChanged("Title");
					});
				}
			}
		}
		private string _Title;

		public string Subtitle
		{
			get { return _Subtitle; }
			set
			{
				if (value != _Subtitle)
				{
					UIInvoker.Invoke(() =>
					{
						_Subtitle = value;
						TriggerPropertyChanged("Subtitle");
					});
				}
			}
		}
		private string _Subtitle;

		public Traybar Traybar { get; set; } 
		public Slideshow Slideshow { get; set; } 
	
		
		//Commands
		
		public ApplicationContext()
		{
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
	public partial class ErrorInfo : TinyMVVM.Framework.ViewModelBase
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		public bool HasError
		{
			get { return _HasError; }
			set
			{
				if (value != _HasError)
				{
					UIInvoker.Invoke(() =>
					{
						_HasError = value;
						TriggerPropertyChanged("HasError");
					});
				}
			}
		}
		private bool _HasError;

		public string ErrorMessage
		{
			get { return _ErrorMessage; }
			set
			{
				if (value != _ErrorMessage)
				{
					UIInvoker.Invoke(() =>
					{
						_ErrorMessage = value;
						TriggerPropertyChanged("ErrorMessage");
					});
				}
			}
		}
		private string _ErrorMessage;

	
		
		//Commands
		
		public ErrorInfo()
		{
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
	public partial class Widget : TinyMVVM.Framework.ViewModelBase
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		public FrameworkElement View { get; set; } 
		public ErrorInfo ErrorInfo { get; set; } 
	
		
		//Commands
		
		public Widget()
		{
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
	public partial class TraybarWidget : Widget
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
	
		
		//Commands
		
		public TraybarWidget()
		{
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
	public partial class Traybar : TinyMVVM.Framework.ViewModelBase
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		[ImportMany(AllowRecomposition = true)]
		public ObservableCollection<TraybarWidget> Widgets { get; set; } 
		public ErrorInfo ErrorInfo
		{
			get { return _ErrorInfo; }
			set
			{
				if (value != _ErrorInfo)
				{
					UIInvoker.Invoke(() =>
					{
						_ErrorInfo = value;
						TriggerPropertyChanged("ErrorInfo");
					});
				}
			}
		}
		private ErrorInfo _ErrorInfo;

	
		
		//Commands
		
		public Traybar()
		{
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
	public partial class Slideshow : TinyMVVM.Framework.ViewModelBase
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		[ImportMany(AllowRecomposition = true)]
		public ObservableCollection<Slide> Slides { get; set; } 
		public ErrorInfo ErrorInfo { get; set; } 
		public Slide CurrentSlide
		{
			get { return _CurrentSlide; }
			set
			{
				if (value != _CurrentSlide)
				{
					UIInvoker.Invoke(() =>
					{
						_CurrentSlide = value;
						TriggerPropertyChanged("CurrentSlide");
					});
				}
			}
		}
		private Slide _CurrentSlide;

		public string SlideshowInfo
		{
			get { return _SlideshowInfo; }
			set
			{
				if (value != _SlideshowInfo)
				{
					UIInvoker.Invoke(() =>
					{
						_SlideshowInfo = value;
						TriggerPropertyChanged("SlideshowInfo");
					});
				}
			}
		}
		private string _SlideshowInfo;

	
		
		//Commands
		public DelegateCommand Start { get; set; }
		public DelegateCommand Pause { get; set; }
		public DelegateCommand Next { get; set; }
		public DelegateCommand Previous { get; set; }
		
		public Slideshow()
		{
			Start = new DelegateCommand();
			Pause = new DelegateCommand();
			Next = new DelegateCommand();
			Previous = new DelegateCommand();
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
	public partial class Slide : Widget
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		public string Title { get; set; } 
		public FrameworkElement SettingsView { get; set; } 
		public bool IsInSettingsMode
		{
			get { return _IsInSettingsMode; }
			set
			{
				if (value != _IsInSettingsMode)
				{
					UIInvoker.Invoke(() =>
					{
						_IsInSettingsMode = value;
						TriggerPropertyChanged("IsInSettingsMode");
					});
				}
			}
		}
		private bool _IsInSettingsMode;

	
		
		//Commands
		public DelegateCommand Settings { get; set; }
		
		public Slide()
		{
			Settings = new DelegateCommand();
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
	public partial class Notifier : TinyMVVM.Framework.ViewModelBase
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		public ObservableCollection<Notification> Notifications { get; set; } 
	
		
		//Commands
		
		public Notifier()
		{
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
	public partial class Notification : TinyMVVM.Framework.ViewModelBase
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		public string Message
		{
			get { return _Message; }
			set
			{
				if (value != _Message)
				{
					UIInvoker.Invoke(() =>
					{
						_Message = value;
						TriggerPropertyChanged("Message");
					});
				}
			}
		}
		private string _Message;

		public bool Displayed
		{
			get { return _Displayed; }
			set
			{
				if (value != _Displayed)
				{
					UIInvoker.Invoke(() =>
					{
						_Displayed = value;
						TriggerPropertyChanged("Displayed");
					});
				}
			}
		}
		private bool _Displayed;

	
		
		//Commands
		
		public Notification()
		{
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
}