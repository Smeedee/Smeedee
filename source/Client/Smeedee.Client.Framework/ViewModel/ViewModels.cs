
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Conventions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using TinyMVVM.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel.Dialogs;
using Smeedee.DomainModel.Config;
namespace Smeedee.Client.Framework.ViewModel
{
	public partial class ApplicationContext : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual DockBar DockBar 
		{ 
			get
			{
				OnGetDockBar(ref _DockBar);
				 
				return _DockBar; 
			}
			set 
			{
				OnSetDockBar(ref value); 
				_DockBar = value; 
			} 
		}

		private DockBar _DockBar;
		partial void OnGetDockBar(ref DockBar value);
		partial void OnSetDockBar(ref DockBar value);
		public virtual Traybar Traybar 
		{ 
			get
			{
				OnGetTraybar(ref _Traybar);
				 
				return _Traybar; 
			}
			set 
			{
				OnSetTraybar(ref value); 
				_Traybar = value; 
			} 
		}

		private Traybar _Traybar;
		partial void OnGetTraybar(ref Traybar value);
		partial void OnSetTraybar(ref Traybar value);
		public virtual Slideshow Slideshow 
		{ 
			get
			{
				OnGetSlideshow(ref _Slideshow);
				 
				return _Slideshow; 
			}
			set 
			{
				OnSetSlideshow(ref value); 
				_Slideshow = value; 
			} 
		}

		private Slideshow _Slideshow;
		partial void OnGetSlideshow(ref Slideshow value);
		partial void OnSetSlideshow(ref Slideshow value);
	
		
		//Commands
		
		public ApplicationContext()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class ErrorInfo : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual bool HasError
		{
			get 
			{
				OnGetHasError(ref _HasError);
				 
				return _HasError; 
			}
			set
			{
				if (value != _HasError)
				{
					OnSetHasError(ref value); 
					_HasError = value;
					TriggerPropertyChanged("HasError");
				}
			}
		}
		private bool _HasError;

		partial void OnGetHasError(ref bool value);
		partial void OnSetHasError(ref bool value);

		public virtual string ErrorMessage
		{
			get 
			{
				OnGetErrorMessage(ref _ErrorMessage);
				 
				return _ErrorMessage; 
			}
			set
			{
				if (value != _ErrorMessage)
				{
					OnSetErrorMessage(ref value); 
					_ErrorMessage = value;
					TriggerPropertyChanged("ErrorMessage");
				}
			}
		}
		private string _ErrorMessage;

		partial void OnGetErrorMessage(ref string value);
		partial void OnSetErrorMessage(ref string value);

	
		
		//Commands
		
		public ErrorInfo()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class Widget : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string Title
		{
			get 
			{
				OnGetTitle(ref _Title);
				 
				return _Title; 
			}
			set
			{
				if (value != _Title)
				{
					OnSetTitle(ref value); 
					_Title = value;
					TriggerPropertyChanged("Title");
				}
			}
		}
		private string _Title;

		partial void OnGetTitle(ref string value);
		partial void OnSetTitle(ref string value);

		public virtual FrameworkElement View
		{
			get 
			{
				OnGetView(ref _View);
				 
				return _View; 
			}
			set
			{
				if (value != _View)
				{
					OnSetView(ref value); 
					_View = value;
					TriggerPropertyChanged("View");
				}
			}
		}
		private FrameworkElement _View;

		partial void OnGetView(ref FrameworkElement value);
		partial void OnSetView(ref FrameworkElement value);

		public virtual FrameworkElement SettingsView
		{
			get 
			{
				OnGetSettingsView(ref _SettingsView);
				 
				return _SettingsView; 
			}
			set
			{
				if (value != _SettingsView)
				{
					OnSetSettingsView(ref value); 
					_SettingsView = value;
					TriggerPropertyChanged("SettingsView");
				}
			}
		}
		private FrameworkElement _SettingsView;

		partial void OnGetSettingsView(ref FrameworkElement value);
		partial void OnSetSettingsView(ref FrameworkElement value);

		public virtual ErrorInfo ErrorInfo 
		{ 
			get
			{
				OnGetErrorInfo(ref _ErrorInfo);
				 
				return _ErrorInfo; 
			}
			set 
			{
				OnSetErrorInfo(ref value); 
				_ErrorInfo = value; 
			} 
		}

		private ErrorInfo _ErrorInfo;
		partial void OnGetErrorInfo(ref ErrorInfo value);
		partial void OnSetErrorInfo(ref ErrorInfo value);
		public virtual bool IsInSettingsMode
		{
			get 
			{
				OnGetIsInSettingsMode(ref _IsInSettingsMode);
				 
				return _IsInSettingsMode; 
			}
			set
			{
				if (value != _IsInSettingsMode)
				{
					OnSetIsInSettingsMode(ref value); 
					_IsInSettingsMode = value;
					TriggerPropertyChanged("IsInSettingsMode");
				}
			}
		}
		private bool _IsInSettingsMode;

		partial void OnGetIsInSettingsMode(ref bool value);
		partial void OnSetIsInSettingsMode(ref bool value);

		public virtual bool IsDisplayed
		{
			get 
			{
				OnGetIsDisplayed(ref _IsDisplayed);
				 
				return _IsDisplayed; 
			}
			set
			{
				if (value != _IsDisplayed)
				{
					OnSetIsDisplayed(ref value); 
					_IsDisplayed = value;
					TriggerPropertyChanged("IsDisplayed");
				}
			}
		}
		private bool _IsDisplayed;

		partial void OnGetIsDisplayed(ref bool value);
		partial void OnSetIsDisplayed(ref bool value);

		public virtual Progressbar ViewProgressbar 
		{ 
			get
			{
				OnGetViewProgressbar(ref _ViewProgressbar);
				 
				return _ViewProgressbar; 
			}
			set 
			{
				OnSetViewProgressbar(ref value); 
				_ViewProgressbar = value; 
			} 
		}

		private Progressbar _ViewProgressbar;
		partial void OnGetViewProgressbar(ref Progressbar value);
		partial void OnSetViewProgressbar(ref Progressbar value);
		public virtual Progressbar SettingsProgressbar 
		{ 
			get
			{
				OnGetSettingsProgressbar(ref _SettingsProgressbar);
				 
				return _SettingsProgressbar; 
			}
			set 
			{
				OnSetSettingsProgressbar(ref value); 
				_SettingsProgressbar = value; 
			} 
		}

		private Progressbar _SettingsProgressbar;
		partial void OnGetSettingsProgressbar(ref Progressbar value);
		partial void OnSetSettingsProgressbar(ref Progressbar value);
		public virtual IProgressbar ProgressbarService 
		{ 
			get
			{
				OnGetProgressbarService(ref _ProgressbarService);
				 
				return _ProgressbarService; 
			}
			set 
			{
				OnSetProgressbarService(ref value); 
				_ProgressbarService = value; 
			} 
		}

		private IProgressbar _ProgressbarService;
		partial void OnGetProgressbarService(ref IProgressbar value);
		partial void OnSetProgressbarService(ref IProgressbar value);
	
		
		//Commands
		public DelegateCommand Settings { get; set; }
		public DelegateCommand SaveSettings { get; set; }
		
		public Widget()
		{
			Settings = new DelegateCommand();
			SaveSettings = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class Traybar : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual ObservableCollection<Widget> Widgets 
		{ 
			get
			{
				OnGetWidgets(ref _Widgets);
				 
				return _Widgets; 
			}
			set 
			{
				OnSetWidgets(ref value); 
				_Widgets = value; 
			} 
		}

		private ObservableCollection<Widget> _Widgets;
		partial void OnGetWidgets(ref ObservableCollection<Widget> value);
		partial void OnSetWidgets(ref ObservableCollection<Widget> value);
		public virtual ErrorInfo ErrorInfo
		{
			get 
			{
				OnGetErrorInfo(ref _ErrorInfo);
				 
				return _ErrorInfo; 
			}
			set
			{
				if (value != _ErrorInfo)
				{
					OnSetErrorInfo(ref value); 
					_ErrorInfo = value;
					TriggerPropertyChanged("ErrorInfo");
				}
			}
		}
		private ErrorInfo _ErrorInfo;

		partial void OnGetErrorInfo(ref ErrorInfo value);
		partial void OnSetErrorInfo(ref ErrorInfo value);

	
		
		//Commands
		
		public Traybar()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class Slideshow : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual ObservableCollection<Slide> Slides 
		{ 
			get
			{
				OnGetSlides(ref _Slides);
				 
				return _Slides; 
			}
			set 
			{
				OnSetSlides(ref value); 
				_Slides = value; 
			} 
		}

		private ObservableCollection<Slide> _Slides;
		partial void OnGetSlides(ref ObservableCollection<Slide> value);
		partial void OnSetSlides(ref ObservableCollection<Slide> value);
		public virtual ErrorInfo ErrorInfo 
		{ 
			get
			{
				OnGetErrorInfo(ref _ErrorInfo);
				 
				return _ErrorInfo; 
			}
			set 
			{
				OnSetErrorInfo(ref value); 
				_ErrorInfo = value; 
			} 
		}

		private ErrorInfo _ErrorInfo;
		partial void OnGetErrorInfo(ref ErrorInfo value);
		partial void OnSetErrorInfo(ref ErrorInfo value);
		public virtual Slide CurrentSlide
		{
			get 
			{
				OnGetCurrentSlide(ref _CurrentSlide);
				 
				return _CurrentSlide; 
			}
			set
			{
				if (value != _CurrentSlide)
				{
					OnSetCurrentSlide(ref value); 
					_CurrentSlide = value;
					TriggerPropertyChanged("CurrentSlide");
				}
			}
		}
		private Slide _CurrentSlide;

		partial void OnGetCurrentSlide(ref Slide value);
		partial void OnSetCurrentSlide(ref Slide value);

		public virtual string SlideshowInfo
		{
			get 
			{
				OnGetSlideshowInfo(ref _SlideshowInfo);
				 
				return _SlideshowInfo; 
			}
			set
			{
				if (value != _SlideshowInfo)
				{
					OnSetSlideshowInfo(ref value); 
					_SlideshowInfo = value;
					TriggerPropertyChanged("SlideshowInfo");
				}
			}
		}
		private string _SlideshowInfo;

		partial void OnGetSlideshowInfo(ref string value);
		partial void OnSetSlideshowInfo(ref string value);

		public virtual bool IsRunning
		{
			get 
			{
				OnGetIsRunning(ref _IsRunning);
				 
				return _IsRunning; 
			}
			set
			{
				if (value != _IsRunning)
				{
					OnSetIsRunning(ref value); 
					_IsRunning = value;
					TriggerPropertyChanged("IsRunning");
				}
			}
		}
		private bool _IsRunning;

		partial void OnGetIsRunning(ref bool value);
		partial void OnSetIsRunning(ref bool value);

		public virtual double TimeLeftOfSlideInPercent
		{
			get 
			{
				OnGetTimeLeftOfSlideInPercent(ref _TimeLeftOfSlideInPercent);
				 
				return _TimeLeftOfSlideInPercent; 
			}
			set
			{
				if (value != _TimeLeftOfSlideInPercent)
				{
					OnSetTimeLeftOfSlideInPercent(ref value); 
					_TimeLeftOfSlideInPercent = value;
					TriggerPropertyChanged("TimeLeftOfSlideInPercent");
				}
			}
		}
		private double _TimeLeftOfSlideInPercent;

		partial void OnGetTimeLeftOfSlideInPercent(ref double value);
		partial void OnSetTimeLeftOfSlideInPercent(ref double value);

	
		
		//Commands
		public DelegateCommand Start { get; set; }
		public DelegateCommand Pause { get; set; }
		public DelegateCommand Next { get; set; }
		public DelegateCommand Previous { get; set; }
		public DelegateCommand AddSlide { get; set; }
		public DelegateCommand Edit { get; set; }
		
		public Slideshow()
		{
			Start = new DelegateCommand();
			Pause = new DelegateCommand();
			Next = new DelegateCommand();
			Previous = new DelegateCommand();
			AddSlide = new DelegateCommand();
			Edit = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class Slide : Widget
	{
		//State
		public virtual string Title
		{
			get 
			{
				OnGetTitle(ref _Title);
				 
				return _Title; 
			}
			set
			{
				if (value != _Title)
				{
					OnSetTitle(ref value); 
					_Title = value;
					TriggerPropertyChanged("Title");
				}
			}
		}
		private string _Title;

		partial void OnGetTitle(ref string value);
		partial void OnSetTitle(ref string value);

		public virtual int SecondsOnScreen
		{
			get 
			{
				OnGetSecondsOnScreen(ref _SecondsOnScreen);
				 
				return _SecondsOnScreen; 
			}
			set
			{
				if (value != _SecondsOnScreen)
				{
					OnSetSecondsOnScreen(ref value); 
					_SecondsOnScreen = value;
					TriggerPropertyChanged("SecondsOnScreen");
				}
			}
		}
		private int _SecondsOnScreen;

		partial void OnGetSecondsOnScreen(ref int value);
		partial void OnSetSecondsOnScreen(ref int value);

		public virtual Widget Widget 
		{ 
			get
			{
				OnGetWidget(ref _Widget);
				 
				return _Widget; 
			}
			set 
			{
				OnSetWidget(ref value); 
				_Widget = value; 
			} 
		}

		private Widget _Widget;
		partial void OnGetWidget(ref Widget value);
		partial void OnSetWidget(ref Widget value);
	
		
		//Commands
		
		public Slide()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class SettingsViewModelBase : ViewModelBase
	{
		//State
		public virtual bool HasChanges
		{
			get 
			{
				OnGetHasChanges(ref _HasChanges);
				 
				return _HasChanges; 
			}
			set
			{
				if (value != _HasChanges)
				{
					OnSetHasChanges(ref value); 
					_HasChanges = value;
					TriggerPropertyChanged("HasChanges");
				}
			}
		}
		private bool _HasChanges;

		partial void OnGetHasChanges(ref bool value);
		partial void OnSetHasChanges(ref bool value);

	
		
		//Commands
		public DelegateCommand Save { get; set; }
		public DelegateCommand ReloadSettings { get; set; }
		
		public SettingsViewModelBase()
		{
			Save = new DelegateCommand();
			ReloadSettings = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class Notifier : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual ObservableCollection<Notification> Notifications 
		{ 
			get
			{
				OnGetNotifications(ref _Notifications);
				 
				return _Notifications; 
			}
			set 
			{
				OnSetNotifications(ref value); 
				_Notifications = value; 
			} 
		}

		private ObservableCollection<Notification> _Notifications;
		partial void OnGetNotifications(ref ObservableCollection<Notification> value);
		partial void OnSetNotifications(ref ObservableCollection<Notification> value);
	
		
		//Commands
		
		public Notifier()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class Notification : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string Message
		{
			get 
			{
				OnGetMessage(ref _Message);
				 
				return _Message; 
			}
			set
			{
				if (value != _Message)
				{
					OnSetMessage(ref value); 
					_Message = value;
					TriggerPropertyChanged("Message");
				}
			}
		}
		private string _Message;

		partial void OnGetMessage(ref string value);
		partial void OnSetMessage(ref string value);

		public virtual bool Displayed
		{
			get 
			{
				OnGetDisplayed(ref _Displayed);
				 
				return _Displayed; 
			}
			set
			{
				if (value != _Displayed)
				{
					OnSetDisplayed(ref value); 
					_Displayed = value;
					TriggerPropertyChanged("Displayed");
				}
			}
		}
		private bool _Displayed;

		partial void OnGetDisplayed(ref bool value);
		partial void OnSetDisplayed(ref bool value);

	
		
		//Commands
		
		public Notification()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class Progressbar : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string Message
		{
			get 
			{
				OnGetMessage(ref _Message);
				 
				return _Message; 
			}
			set
			{
				if (value != _Message)
				{
					OnSetMessage(ref value); 
					_Message = value;
					TriggerPropertyChanged("Message");
				}
			}
		}
		private string _Message;

		partial void OnGetMessage(ref string value);
		partial void OnSetMessage(ref string value);

		public virtual bool IsVisible
		{
			get 
			{
				OnGetIsVisible(ref _IsVisible);
				 
				return _IsVisible; 
			}
			set
			{
				if (value != _IsVisible)
				{
					OnSetIsVisible(ref value); 
					_IsVisible = value;
					TriggerPropertyChanged("IsVisible");
				}
			}
		}
		private bool _IsVisible;

		partial void OnGetIsVisible(ref bool value);
		partial void OnSetIsVisible(ref bool value);

	
		
		//Commands
		
		public Progressbar()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class WidgetMetadata : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string Name 
		{ 
			get
			{
				OnGetName(ref _Name);
				 
				return _Name; 
			}
			set 
			{
				OnSetName(ref value); 
				_Name = value; 
			} 
		}

		private string _Name;
		partial void OnGetName(ref string value);
		partial void OnSetName(ref string value);
		public virtual string Description 
		{ 
			get
			{
				OnGetDescription(ref _Description);
				 
				return _Description; 
			}
			set 
			{
				OnSetDescription(ref value); 
				_Description = value; 
			} 
		}

		private string _Description;
		partial void OnGetDescription(ref string value);
		partial void OnSetDescription(ref string value);
		public virtual string Author 
		{ 
			get
			{
				OnGetAuthor(ref _Author);
				 
				return _Author; 
			}
			set 
			{
				OnSetAuthor(ref value); 
				_Author = value; 
			} 
		}

		private string _Author;
		partial void OnGetAuthor(ref string value);
		partial void OnSetAuthor(ref string value);
		public virtual string[] Tags 
		{ 
			get
			{
				OnGetTags(ref _Tags);
				 
				return _Tags; 
			}
			set 
			{
				OnSetTags(ref value); 
				_Tags = value; 
			} 
		}

		private string[] _Tags;
		partial void OnGetTags(ref string[] value);
		partial void OnSetTags(ref string[] value);
		public virtual string Version 
		{ 
			get
			{
				OnGetVersion(ref _Version);
				 
				return _Version; 
			}
			set 
			{
				OnSetVersion(ref value); 
				_Version = value; 
			} 
		}

		private string _Version;
		partial void OnGetVersion(ref string value);
		partial void OnSetVersion(ref string value);
		public virtual string XAPName 
		{ 
			get
			{
				OnGetXAPName(ref _XAPName);
				 
				return _XAPName; 
			}
			set 
			{
				OnSetXAPName(ref value); 
				_XAPName = value; 
			} 
		}

		private string _XAPName;
		partial void OnGetXAPName(ref string value);
		partial void OnSetXAPName(ref string value);
		public virtual Type Type 
		{ 
			get
			{
				OnGetType(ref _Type);
				 
				return _Type; 
			}
			set 
			{
				OnSetType(ref value); 
				_Type = value; 
			} 
		}

		private Type _Type;
		partial void OnGetType(ref Type value);
		partial void OnSetType(ref Type value);
		public virtual string UserSelectedTitle 
		{ 
			get
			{
				OnGetUserSelectedTitle(ref _UserSelectedTitle);
				 
				return _UserSelectedTitle; 
			}
			set 
			{
				OnSetUserSelectedTitle(ref value); 
				_UserSelectedTitle = value; 
			} 
		}

		private string _UserSelectedTitle;
		partial void OnGetUserSelectedTitle(ref string value);
		partial void OnSetUserSelectedTitle(ref string value);
		public virtual int SecondsOnScreen
		{
			get 
			{
				OnGetSecondsOnScreen(ref _SecondsOnScreen);
				 
				return _SecondsOnScreen; 
			}
			set
			{
				if (value != _SecondsOnScreen)
				{
					OnSetSecondsOnScreen(ref value); 
					_SecondsOnScreen = value;
					TriggerPropertyChanged("SecondsOnScreen");
				}
			}
		}
		private int _SecondsOnScreen;

		partial void OnGetSecondsOnScreen(ref int value);
		partial void OnSetSecondsOnScreen(ref int value);

		public virtual bool IsSelected
		{
			get 
			{
				OnGetIsSelected(ref _IsSelected);
				 
				return _IsSelected; 
			}
			set
			{
				if (value != _IsSelected)
				{
					OnSetIsSelected(ref value); 
					_IsSelected = value;
					TriggerPropertyChanged("IsSelected");
				}
			}
		}
		private bool _IsSelected;

		partial void OnGetIsSelected(ref bool value);
		partial void OnSetIsSelected(ref bool value);

		public virtual bool IsDescriptionCapped
		{
			get 
			{
				OnGetIsDescriptionCapped(ref _IsDescriptionCapped);
				 
				return _IsDescriptionCapped; 
			}
			set
			{
				if (value != _IsDescriptionCapped)
				{
					OnSetIsDescriptionCapped(ref value); 
					_IsDescriptionCapped = value;
					TriggerPropertyChanged("IsDescriptionCapped");
				}
			}
		}
		private bool _IsDescriptionCapped;

		partial void OnGetIsDescriptionCapped(ref bool value);
		partial void OnSetIsDescriptionCapped(ref bool value);

	
		
		//Commands
		
		public WidgetMetadata()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class DockBar : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual bool IsVisible
		{
			get 
			{
				OnGetIsVisible(ref _IsVisible);
				 
				return _IsVisible; 
			}
			set
			{
				if (value != _IsVisible)
				{
					OnSetIsVisible(ref value); 
					_IsVisible = value;
					TriggerPropertyChanged("IsVisible");
				}
			}
		}
		private bool _IsVisible;

		partial void OnGetIsVisible(ref bool value);
		partial void OnSetIsVisible(ref bool value);

		public virtual ObservableCollection<DockBarItem> Items 
		{ 
			get
			{
				OnGetItems(ref _Items);
				 
				return _Items; 
			}
			set 
			{
				OnSetItems(ref value); 
				_Items = value; 
			} 
		}

		private ObservableCollection<DockBarItem> _Items;
		partial void OnGetItems(ref ObservableCollection<DockBarItem> value);
		partial void OnSetItems(ref ObservableCollection<DockBarItem> value);
	
		
		//Commands
		
		public DockBar()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class DockBarItem : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string Description
		{
			get 
			{
				OnGetDescription(ref _Description);
				 
				return _Description; 
			}
			set
			{
				if (value != _Description)
				{
					OnSetDescription(ref value); 
					_Description = value;
					TriggerPropertyChanged("Description");
				}
			}
		}
		private string _Description;

		partial void OnGetDescription(ref string value);
		partial void OnSetDescription(ref string value);

		public virtual FrameworkElement Icon
		{
			get 
			{
				OnGetIcon(ref _Icon);
				 
				return _Icon; 
			}
			set
			{
				if (value != _Icon)
				{
					OnSetIcon(ref value); 
					_Icon = value;
					TriggerPropertyChanged("Icon");
				}
			}
		}
		private FrameworkElement _Icon;

		partial void OnGetIcon(ref FrameworkElement value);
		partial void OnSetIcon(ref FrameworkElement value);

	
		
		//Commands
		public DelegateCommand Click { get; set; }
		
		public DockBarItem()
		{
			Click = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel
{
	public partial class WelcomeWidget : Widget
	{
		//State
	
		
		//Commands
		
		public WelcomeWidget()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel.DockBarItems
{
	public partial class WidgetDockBarItem : DockBarItem
	{
		//State
		public virtual Widget Widget
		{
			get 
			{
				OnGetWidget(ref _Widget);
				 
				return _Widget; 
			}
			set
			{
				if (value != _Widget)
				{
					OnSetWidget(ref value); 
					_Widget = value;
					TriggerPropertyChanged("Widget");
				}
			}
		}
		private Widget _Widget;

		partial void OnGetWidget(ref Widget value);
		partial void OnSetWidget(ref Widget value);

		public virtual string ItemName 
		{ 
			get
			{
				OnGetItemName(ref _ItemName);
				 
				return _ItemName; 
			}
			set 
			{
				OnSetItemName(ref value); 
				_ItemName = value; 
			} 
		}

		private string _ItemName;
		partial void OnGetItemName(ref string value);
		partial void OnSetItemName(ref string value);
	
		
		//Commands
		
		public WidgetDockBarItem()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel.DockBarItems
{
	public partial class AddWidgetDockBarItem : DockBarItem
	{
		//State
		public virtual Slideshow SlideShow
		{
			get 
			{
				OnGetSlideShow(ref _SlideShow);
				 
				return _SlideShow; 
			}
			set
			{
				if (value != _SlideShow)
				{
					OnSetSlideShow(ref value); 
					_SlideShow = value;
					TriggerPropertyChanged("SlideShow");
				}
			}
		}
		private Slideshow _SlideShow;

		partial void OnGetSlideShow(ref Slideshow value);
		partial void OnSetSlideShow(ref Slideshow value);

		public virtual string ItemName 
		{ 
			get
			{
				OnGetItemName(ref _ItemName);
				 
				return _ItemName; 
			}
			set 
			{
				OnSetItemName(ref value); 
				_ItemName = value; 
			} 
		}

		private string _ItemName;
		partial void OnGetItemName(ref string value);
		partial void OnSetItemName(ref string value);
	
		
		//Commands
		
		public AddWidgetDockBarItem()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel.DockBarItems
{
	public partial class EditSlideshowDockBarItem : DockBarItem
	{
		//State
		public virtual Slideshow SlideShow
		{
			get 
			{
				OnGetSlideShow(ref _SlideShow);
				 
				return _SlideShow; 
			}
			set
			{
				if (value != _SlideShow)
				{
					OnSetSlideShow(ref value); 
					_SlideShow = value;
					TriggerPropertyChanged("SlideShow");
				}
			}
		}
		private Slideshow _SlideShow;

		partial void OnGetSlideShow(ref Slideshow value);
		partial void OnSetSlideShow(ref Slideshow value);

		public virtual string ItemName 
		{ 
			get
			{
				OnGetItemName(ref _ItemName);
				 
				return _ItemName; 
			}
			set 
			{
				OnSetItemName(ref value); 
				_ItemName = value; 
			} 
		}

		private string _ItemName;
		partial void OnGetItemName(ref string value);
		partial void OnSetItemName(ref string value);
	
		
		//Commands
		
		public EditSlideshowDockBarItem()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel.Dialogs
{
	public partial class Dialog : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual Progressbar Progressbar 
		{ 
			get
			{
				OnGetProgressbar(ref _Progressbar);
				 
				return _Progressbar; 
			}
			set 
			{
				OnSetProgressbar(ref value); 
				_Progressbar = value; 
			} 
		}

		private Progressbar _Progressbar;
		partial void OnGetProgressbar(ref Progressbar value);
		partial void OnSetProgressbar(ref Progressbar value);
		public virtual int Width 
		{ 
			get
			{
				OnGetWidth(ref _Width);
				 
				return _Width; 
			}
			set 
			{
				OnSetWidth(ref value); 
				_Width = value; 
			} 
		}

		private int _Width;
		partial void OnGetWidth(ref int value);
		partial void OnSetWidth(ref int value);
		public virtual int Height 
		{ 
			get
			{
				OnGetHeight(ref _Height);
				 
				return _Height; 
			}
			set 
			{
				OnSetHeight(ref value); 
				_Height = value; 
			} 
		}

		private int _Height;
		partial void OnGetHeight(ref int value);
		partial void OnSetHeight(ref int value);
		public virtual bool DisplayOkButton
		{
			get 
			{
				OnGetDisplayOkButton(ref _DisplayOkButton);
				 
				return _DisplayOkButton; 
			}
			set
			{
				if (value != _DisplayOkButton)
				{
					OnSetDisplayOkButton(ref value); 
					_DisplayOkButton = value;
					TriggerPropertyChanged("DisplayOkButton");
				}
			}
		}
		private bool _DisplayOkButton;

		partial void OnGetDisplayOkButton(ref bool value);
		partial void OnSetDisplayOkButton(ref bool value);

		public virtual bool DisplayCancelButton
		{
			get 
			{
				OnGetDisplayCancelButton(ref _DisplayCancelButton);
				 
				return _DisplayCancelButton; 
			}
			set
			{
				if (value != _DisplayCancelButton)
				{
					OnSetDisplayCancelButton(ref value); 
					_DisplayCancelButton = value;
					TriggerPropertyChanged("DisplayCancelButton");
				}
			}
		}
		private bool _DisplayCancelButton;

		partial void OnGetDisplayCancelButton(ref bool value);
		partial void OnSetDisplayCancelButton(ref bool value);

		public virtual string Title
		{
			get 
			{
				OnGetTitle(ref _Title);
				 
				return _Title; 
			}
			set
			{
				if (value != _Title)
				{
					OnSetTitle(ref value); 
					_Title = value;
					TriggerPropertyChanged("Title");
				}
			}
		}
		private string _Title;

		partial void OnGetTitle(ref string value);
		partial void OnSetTitle(ref string value);

		public virtual FrameworkElement View
		{
			get 
			{
				OnGetView(ref _View);
				 
				return _View; 
			}
			set
			{
				if (value != _View)
				{
					OnSetView(ref value); 
					_View = value;
					TriggerPropertyChanged("View");
				}
			}
		}
		private FrameworkElement _View;

		partial void OnGetView(ref FrameworkElement value);
		partial void OnSetView(ref FrameworkElement value);

		public virtual ObservableCollection<DelegateCommand> ButtonBarCommands 
		{ 
			get
			{
				OnGetButtonBarCommands(ref _ButtonBarCommands);
				 
				return _ButtonBarCommands; 
			}
			set 
			{
				OnSetButtonBarCommands(ref value); 
				_ButtonBarCommands = value; 
			} 
		}

		private ObservableCollection<DelegateCommand> _ButtonBarCommands;
		partial void OnGetButtonBarCommands(ref ObservableCollection<DelegateCommand> value);
		partial void OnSetButtonBarCommands(ref ObservableCollection<DelegateCommand> value);
	
		
		//Commands
		public DelegateCommand Ok { get; set; }
		public DelegateCommand Cancel { get; set; }
		
		public Dialog()
		{
			Ok = new DelegateCommand();
			Cancel = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel.Dialogs
{
	public partial class WidgetDialog : Dialog
	{
		//State
		public virtual Widget Widget 
		{ 
			get
			{
				OnGetWidget(ref _Widget);
				 
				return _Widget; 
			}
			set 
			{
				OnSetWidget(ref value); 
				_Widget = value; 
			} 
		}

		private Widget _Widget;
		partial void OnGetWidget(ref Widget value);
		partial void OnSetWidget(ref Widget value);
	
		
		//Commands
		
		public WidgetDialog()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel.Dialogs
{
	public partial class SelectWidgetsDialog : Dialog
	{
		//State
		public virtual ObservableCollection<WidgetMetadata> AvailableWidgets 
		{ 
			get
			{
				OnGetAvailableWidgets(ref _AvailableWidgets);
				 
				return _AvailableWidgets; 
			}
			set 
			{
				OnSetAvailableWidgets(ref value); 
				_AvailableWidgets = value; 
			} 
		}

		private ObservableCollection<WidgetMetadata> _AvailableWidgets;
		partial void OnGetAvailableWidgets(ref ObservableCollection<WidgetMetadata> value);
		partial void OnSetAvailableWidgets(ref ObservableCollection<WidgetMetadata> value);
		public virtual Progressbar Progressbar 
		{ 
			get
			{
				OnGetProgressbar(ref _Progressbar);
				 
				return _Progressbar; 
			}
			set 
			{
				OnSetProgressbar(ref value); 
				_Progressbar = value; 
			} 
		}

		private Progressbar _Progressbar;
		partial void OnGetProgressbar(ref Progressbar value);
		partial void OnSetProgressbar(ref Progressbar value);
		public virtual string SearchTerm
		{
			get 
			{
				OnGetSearchTerm(ref _SearchTerm);
				 
				return _SearchTerm; 
			}
			set
			{
				if (value != _SearchTerm)
				{
					OnSetSearchTerm(ref value); 
					_SearchTerm = value;
					TriggerPropertyChanged("SearchTerm");
				}
			}
		}
		private string _SearchTerm;

		partial void OnGetSearchTerm(ref string value);
		partial void OnSetSearchTerm(ref string value);

		public virtual List<Slide> NewSlides 
		{ 
			get
			{
				OnGetNewSlides(ref _NewSlides);
				 
				return _NewSlides; 
			}
			set 
			{
				OnSetNewSlides(ref value); 
				_NewSlides = value; 
			} 
		}

		private List<Slide> _NewSlides;
		partial void OnGetNewSlides(ref List<Slide> value);
		partial void OnSetNewSlides(ref List<Slide> value);
	
		
		//Commands
		public DelegateCommand SelectAll { get; set; }
		public DelegateCommand DeselectAll { get; set; }
		
		public SelectWidgetsDialog()
		{
			SelectAll = new DelegateCommand();
			DeselectAll = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Client.Framework.ViewModel.Dialogs
{
	public partial class EditSlideshowDialog : Dialog
	{
		//State
		public virtual Slideshow Slideshow
		{
			get 
			{
				OnGetSlideshow(ref _Slideshow);
				 
				return _Slideshow; 
			}
			set
			{
				if (value != _Slideshow)
				{
					OnSetSlideshow(ref value); 
					_Slideshow = value;
					TriggerPropertyChanged("Slideshow");
				}
			}
		}
		private Slideshow _Slideshow;

		partial void OnGetSlideshow(ref Slideshow value);
		partial void OnSetSlideshow(ref Slideshow value);

	
		
		//Commands
		public DelegateCommand Delete { get; set; }
		public DelegateCommand MoveLeft { get; set; }
		public DelegateCommand MoveRight { get; set; }
		
		public EditSlideshowDialog()
		{
			Delete = new DelegateCommand();
			MoveLeft = new DelegateCommand();
			MoveRight = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

