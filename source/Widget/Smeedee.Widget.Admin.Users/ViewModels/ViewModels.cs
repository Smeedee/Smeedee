
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Conventions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TinyMVVM.Framework;
namespace Smeedee.Widget.Admin.Users.ViewModels
{
	public partial class UsersViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public bool HasDatabase
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

		public UserViewModel SelectedUser
		{
			get 
			{
				OnGetSelectedUser(ref _SelectedUser);
				 
				return _SelectedUser; 
			}
			set
			{
				if (value != _SelectedUser)
				{
					OnSetSelectedUser(ref value); 
					_SelectedUser = value;
					TriggerPropertyChanged("SelectedUser");
				}
			}
		}
		private UserViewModel _SelectedUser;

		partial void OnGetSelectedUser(ref UserViewModel value);
		partial void OnSetSelectedUser(ref UserViewModel value);

		public UserViewModel EditedUser
		{
			get 
			{
				OnGetEditedUser(ref _EditedUser);
				 
				return _EditedUser; 
			}
			set
			{
				if (value != _EditedUser)
				{
					OnSetEditedUser(ref value); 
					_EditedUser = value;
					TriggerPropertyChanged("EditedUser");
				}
			}
		}
		private UserViewModel _EditedUser;

		partial void OnGetEditedUser(ref UserViewModel value);
		partial void OnSetEditedUser(ref UserViewModel value);

		public string CurrentDBName
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

		public ObservableCollection<UserViewModel> Users
		{
			get 
			{
				OnGetUsers(ref _Users);
				 
				return _Users; 
			}
			set
			{
				if (value != _Users)
				{
					OnSetUsers(ref value); 
					_Users = value;
					TriggerPropertyChanged("Users");
				}
			}
		}
		private ObservableCollection<UserViewModel> _Users;

		partial void OnGetUsers(ref ObservableCollection<UserViewModel> value);
		partial void OnSetUsers(ref ObservableCollection<UserViewModel> value);

		public ObservableCollection<string> Userdbs
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

		public bool IsLoading
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

		public bool IsSaving
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

		public bool HasConnectionProblems
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

		public string DeploymentPath 
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
		public DelegateCommand AddUser { get; set; }
		public DelegateCommand EditUser { get; set; }
		public DelegateCommand OpenUserEditWindow { get; set; }
		public DelegateCommand CloseUserEditWindow { get; set; }
		public DelegateCommand DeleteSelectedUser { get; set; }
		public DelegateCommand Refresh { get; set; }
		public DelegateCommand Save { get; set; }
		
		public UsersViewModel()
		{
			AddUser = new DelegateCommand();
			EditUser = new DelegateCommand();
			OpenUserEditWindow = new DelegateCommand();
			CloseUserEditWindow = new DelegateCommand();
			DeleteSelectedUser = new DelegateCommand();
			Refresh = new DelegateCommand();
			Save = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Widget.Admin.Users.ViewModels
{
	public partial class UserViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public string DefaultPictureUri 
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
		public bool HasInvalidUsername 
		{ 
			get
			{
				OnGetHasInvalidUsername(ref _HasInvalidUsername);
				 
				return _HasInvalidUsername; 
			}
			set 
			{
				OnSetHasInvalidUsername(ref value); 
				_HasInvalidUsername = value; 
			} 
		}

		private bool _HasInvalidUsername;
		partial void OnGetHasInvalidUsername(ref bool value);
		partial void OnSetHasInvalidUsername(ref bool value);
		public string Middlename
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

		public string Surname
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

		public string Email
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

		public string ImageUrl
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

	
		
		//Commands
		
		public UserViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

