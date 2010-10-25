
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Conventions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TinyMVVM.Framework;
namespace Smeedee.Widgets.TeamMembers.ViewModels
{
	public partial class TeamMembersViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual bool HasDatabase
		{
			get 
			{
				OnGetHasDatabase(ref _HasDatabase);
				 
				return _HasDatabase; 
			}
			set
			{
				if (value != _HasDatabase)
				{
					OnSetHasDatabase(ref value); 
					_HasDatabase = value;
					TriggerPropertyChanged("HasDatabase");
				}
			}
		}
		private bool _HasDatabase;

		partial void OnGetHasDatabase(ref bool value);
		partial void OnSetHasDatabase(ref bool value);

		public virtual ObservableCollection<UserViewModel> TeamMembers
		{
			get 
			{
				OnGetTeamMembers(ref _TeamMembers);
				 
				return _TeamMembers; 
			}
			set
			{
				if (value != _TeamMembers)
				{
					OnSetTeamMembers(ref value); 
					_TeamMembers = value;
					TriggerPropertyChanged("TeamMembers");
				}
			}
		}
		private ObservableCollection<UserViewModel> _TeamMembers;

		partial void OnGetTeamMembers(ref ObservableCollection<UserViewModel> value);
		partial void OnSetTeamMembers(ref ObservableCollection<UserViewModel> value);

		public virtual bool IsLoading
		{
			get 
			{
				OnGetIsLoading(ref _IsLoading);
				 
				return _IsLoading; 
			}
			set
			{
				if (value != _IsLoading)
				{
					OnSetIsLoading(ref value); 
					_IsLoading = value;
					TriggerPropertyChanged("IsLoading");
				}
			}
		}
		private bool _IsLoading;

		partial void OnGetIsLoading(ref bool value);
		partial void OnSetIsLoading(ref bool value);

		public virtual bool HasConnectionProblems
		{
			get 
			{
				OnGetHasConnectionProblems(ref _HasConnectionProblems);
				 
				return _HasConnectionProblems; 
			}
			set
			{
				if (value != _HasConnectionProblems)
				{
					OnSetHasConnectionProblems(ref value); 
					_HasConnectionProblems = value;
					TriggerPropertyChanged("HasConnectionProblems");
				}
			}
		}
		private bool _HasConnectionProblems;

		partial void OnGetHasConnectionProblems(ref bool value);
		partial void OnSetHasConnectionProblems(ref bool value);

		public virtual string DeploymentPath 
		{ 
			get
			{
				OnGetDeploymentPath(ref _DeploymentPath);
				 
				return _DeploymentPath; 
			}
			set 
			{
				OnSetDeploymentPath(ref value); 
				_DeploymentPath = value; 
			} 
		}

		private string _DeploymentPath;
		partial void OnGetDeploymentPath(ref string value);
		partial void OnSetDeploymentPath(ref string value);
	
		
		//Commands
		
		public TeamMembersViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Widgets.TeamMembers.ViewModels
{
	public partial class TeamMembersSettingsViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string CurrentDBName
		{
			get 
			{
				OnGetCurrentDBName(ref _CurrentDBName);
				 
				return _CurrentDBName; 
			}
			set
			{
				if (value != _CurrentDBName)
				{
					OnSetCurrentDBName(ref value); 
					_CurrentDBName = value;
					TriggerPropertyChanged("CurrentDBName");
				}
			}
		}
		private string _CurrentDBName;

		partial void OnGetCurrentDBName(ref string value);
		partial void OnSetCurrentDBName(ref string value);

		public virtual ObservableCollection<string> Userdbs
		{
			get 
			{
				OnGetUserdbs(ref _Userdbs);
				 
				return _Userdbs; 
			}
			set
			{
				if (value != _Userdbs)
				{
					OnSetUserdbs(ref value); 
					_Userdbs = value;
					TriggerPropertyChanged("Userdbs");
				}
			}
		}
		private ObservableCollection<string> _Userdbs;

		partial void OnGetUserdbs(ref ObservableCollection<string> value);
		partial void OnSetUserdbs(ref ObservableCollection<string> value);

		public virtual bool IsLoading
		{
			get 
			{
				OnGetIsLoading(ref _IsLoading);
				 
				return _IsLoading; 
			}
			set
			{
				if (value != _IsLoading)
				{
					OnSetIsLoading(ref value); 
					_IsLoading = value;
					TriggerPropertyChanged("IsLoading");
				}
			}
		}
		private bool _IsLoading;

		partial void OnGetIsLoading(ref bool value);
		partial void OnSetIsLoading(ref bool value);

		public virtual bool IsSaving
		{
			get 
			{
				OnGetIsSaving(ref _IsSaving);
				 
				return _IsSaving; 
			}
			set
			{
				if (value != _IsSaving)
				{
					OnSetIsSaving(ref value); 
					_IsSaving = value;
					TriggerPropertyChanged("IsSaving");
				}
			}
		}
		private bool _IsSaving;

		partial void OnGetIsSaving(ref bool value);
		partial void OnSetIsSaving(ref bool value);

		public virtual bool FirstnameIsChecked
		{
			get 
			{
				OnGetFirstnameIsChecked(ref _FirstnameIsChecked);
				 
				return _FirstnameIsChecked; 
			}
			set
			{
				if (value != _FirstnameIsChecked)
				{
					OnSetFirstnameIsChecked(ref value); 
					_FirstnameIsChecked = value;
					TriggerPropertyChanged("FirstnameIsChecked");
				}
			}
		}
		private bool _FirstnameIsChecked;

		partial void OnGetFirstnameIsChecked(ref bool value);
		partial void OnSetFirstnameIsChecked(ref bool value);

		public virtual bool MiddlenameIsChecked
		{
			get 
			{
				OnGetMiddlenameIsChecked(ref _MiddlenameIsChecked);
				 
				return _MiddlenameIsChecked; 
			}
			set
			{
				if (value != _MiddlenameIsChecked)
				{
					OnSetMiddlenameIsChecked(ref value); 
					_MiddlenameIsChecked = value;
					TriggerPropertyChanged("MiddlenameIsChecked");
				}
			}
		}
		private bool _MiddlenameIsChecked;

		partial void OnGetMiddlenameIsChecked(ref bool value);
		partial void OnSetMiddlenameIsChecked(ref bool value);

		public virtual bool SurnameIsChecked
		{
			get 
			{
				OnGetSurnameIsChecked(ref _SurnameIsChecked);
				 
				return _SurnameIsChecked; 
			}
			set
			{
				if (value != _SurnameIsChecked)
				{
					OnSetSurnameIsChecked(ref value); 
					_SurnameIsChecked = value;
					TriggerPropertyChanged("SurnameIsChecked");
				}
			}
		}
		private bool _SurnameIsChecked;

		partial void OnGetSurnameIsChecked(ref bool value);
		partial void OnSetSurnameIsChecked(ref bool value);

		public virtual bool UsernameIsChecked
		{
			get 
			{
				OnGetUsernameIsChecked(ref _UsernameIsChecked);
				 
				return _UsernameIsChecked; 
			}
			set
			{
				if (value != _UsernameIsChecked)
				{
					OnSetUsernameIsChecked(ref value); 
					_UsernameIsChecked = value;
					TriggerPropertyChanged("UsernameIsChecked");
				}
			}
		}
		private bool _UsernameIsChecked;

		partial void OnGetUsernameIsChecked(ref bool value);
		partial void OnSetUsernameIsChecked(ref bool value);

		public virtual string InformationField
		{
			get 
			{
				OnGetInformationField(ref _InformationField);
				 
				return _InformationField; 
			}
			set
			{
				if (value != _InformationField)
				{
					OnSetInformationField(ref value); 
					_InformationField = value;
					TriggerPropertyChanged("InformationField");
				}
			}
		}
		private string _InformationField;

		partial void OnGetInformationField(ref string value);
		partial void OnSetInformationField(ref string value);

	
		
		//Commands
		public DelegateCommand Refresh { get; set; }
		public DelegateCommand Save { get; set; }
		
		public TeamMembersSettingsViewModel()
		{
			Refresh = new DelegateCommand();
			Save = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Widgets.TeamMembers.ViewModels
{
	public partial class UserViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string DefaultPictureUri 
		{ 
			get
			{
				OnGetDefaultPictureUri(ref _DefaultPictureUri);
				 
				return _DefaultPictureUri; 
			}
			set 
			{
				OnSetDefaultPictureUri(ref value); 
				_DefaultPictureUri = value; 
			} 
		}

		private string _DefaultPictureUri;
		partial void OnGetDefaultPictureUri(ref string value);
		partial void OnSetDefaultPictureUri(ref string value);
		public virtual string Username
		{
			get 
			{
				OnGetUsername(ref _Username);
				 
				return _Username; 
			}
			set
			{
				if (value != _Username)
				{
					OnSetUsername(ref value); 
					_Username = value;
					TriggerPropertyChanged("Username");
				}
			}
		}
		private string _Username;

		partial void OnGetUsername(ref string value);
		partial void OnSetUsername(ref string value);

		public virtual string Firstname
		{
			get 
			{
				OnGetFirstname(ref _Firstname);
				 
				return _Firstname; 
			}
			set
			{
				if (value != _Firstname)
				{
					OnSetFirstname(ref value); 
					_Firstname = value;
					TriggerPropertyChanged("Firstname");
				}
			}
		}
		private string _Firstname;

		partial void OnGetFirstname(ref string value);
		partial void OnSetFirstname(ref string value);

		public virtual string Middlename
		{
			get 
			{
				OnGetMiddlename(ref _Middlename);
				 
				return _Middlename; 
			}
			set
			{
				if (value != _Middlename)
				{
					OnSetMiddlename(ref value); 
					_Middlename = value;
					TriggerPropertyChanged("Middlename");
				}
			}
		}
		private string _Middlename;

		partial void OnGetMiddlename(ref string value);
		partial void OnSetMiddlename(ref string value);

		public virtual string Surname
		{
			get 
			{
				OnGetSurname(ref _Surname);
				 
				return _Surname; 
			}
			set
			{
				if (value != _Surname)
				{
					OnSetSurname(ref value); 
					_Surname = value;
					TriggerPropertyChanged("Surname");
				}
			}
		}
		private string _Surname;

		partial void OnGetSurname(ref string value);
		partial void OnSetSurname(ref string value);

		public virtual string Email
		{
			get 
			{
				OnGetEmail(ref _Email);
				 
				return _Email; 
			}
			set
			{
				if (value != _Email)
				{
					OnSetEmail(ref value); 
					_Email = value;
					TriggerPropertyChanged("Email");
				}
			}
		}
		private string _Email;

		partial void OnGetEmail(ref string value);
		partial void OnSetEmail(ref string value);

		public virtual string ImageUrl
		{
			get 
			{
				OnGetImageUrl(ref _ImageUrl);
				 
				return _ImageUrl; 
			}
			set
			{
				if (value != _ImageUrl)
				{
					OnSetImageUrl(ref value); 
					_ImageUrl = value;
					TriggerPropertyChanged("ImageUrl");
				}
			}
		}
		private string _ImageUrl;

		partial void OnGetImageUrl(ref string value);
		partial void OnSetImageUrl(ref string value);

		public virtual Visibility UsernameIsVisible
		{
			get 
			{
				OnGetUsernameIsVisible(ref _UsernameIsVisible);
				 
				return _UsernameIsVisible; 
			}
			set
			{
				if (value != _UsernameIsVisible)
				{
					OnSetUsernameIsVisible(ref value); 
					_UsernameIsVisible = value;
					TriggerPropertyChanged("UsernameIsVisible");
				}
			}
		}
		private Visibility _UsernameIsVisible;

		partial void OnGetUsernameIsVisible(ref Visibility value);
		partial void OnSetUsernameIsVisible(ref Visibility value);

		public virtual Visibility FirstnameIsVisible
		{
			get 
			{
				OnGetFirstnameIsVisible(ref _FirstnameIsVisible);
				 
				return _FirstnameIsVisible; 
			}
			set
			{
				if (value != _FirstnameIsVisible)
				{
					OnSetFirstnameIsVisible(ref value); 
					_FirstnameIsVisible = value;
					TriggerPropertyChanged("FirstnameIsVisible");
				}
			}
		}
		private Visibility _FirstnameIsVisible;

		partial void OnGetFirstnameIsVisible(ref Visibility value);
		partial void OnSetFirstnameIsVisible(ref Visibility value);

		public virtual Visibility MiddlenameIsVisible
		{
			get 
			{
				OnGetMiddlenameIsVisible(ref _MiddlenameIsVisible);
				 
				return _MiddlenameIsVisible; 
			}
			set
			{
				if (value != _MiddlenameIsVisible)
				{
					OnSetMiddlenameIsVisible(ref value); 
					_MiddlenameIsVisible = value;
					TriggerPropertyChanged("MiddlenameIsVisible");
				}
			}
		}
		private Visibility _MiddlenameIsVisible;

		partial void OnGetMiddlenameIsVisible(ref Visibility value);
		partial void OnSetMiddlenameIsVisible(ref Visibility value);

		public virtual Visibility SurnameIsVisible
		{
			get 
			{
				OnGetSurnameIsVisible(ref _SurnameIsVisible);
				 
				return _SurnameIsVisible; 
			}
			set
			{
				if (value != _SurnameIsVisible)
				{
					OnSetSurnameIsVisible(ref value); 
					_SurnameIsVisible = value;
					TriggerPropertyChanged("SurnameIsVisible");
				}
			}
		}
		private Visibility _SurnameIsVisible;

		partial void OnGetSurnameIsVisible(ref Visibility value);
		partial void OnSetSurnameIsVisible(ref Visibility value);

	
		
		//Commands
		
		public UserViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

