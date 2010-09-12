using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Smeedee.Widget.Admin.Users.ViewModels
{
    public partial class UsersViewModel
    {
        public const string DEFAULT_IMAGE = "UserImages/default_user.jpg";
        private bool inEditMode;

        partial void OnInitialize()
        {
            Users = new ObservableCollection<UserViewModel>();
            Userdbs = new ObservableCollection<string>();
            EditedUser = new UserViewModel();
            inEditMode = false;
            EditedUser.PropertyChanged += EditedUserPropertyChanged;
            SetupDelegateCommands();
        }

        private void EditedUserPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Username"))
            {
                EditedUser.HasInvalidUsername = inEditMode ? !EditedUserIsUnique() : UserExists(EditedUser);
            }
        }

        public void AddUserViewModel(UserViewModel user)
        {
            user.DefaultPictureUri = DeploymentPath + DEFAULT_IMAGE;
            Users.Add(user);
            SelectedUser = user;
        }

        private void SetupDelegateCommands()
        {
            DeleteSelectedUser.AfterExecute += (sender, args) => OnDeleteSelectedUser();
            AddUser.AfterExecute += (sender, args) => OnAddUser();
            EditUser.AfterExecute += (sender, args) => OnEditUser();
            OpenUserEditWindow.AfterExecute += (sender, args) => OnOpenUserEditWindow();
            CloseUserEditWindow.AfterExecute += (sender, args) => OnCloseUserEditWindow();           
        }

        public void OnEditUser()
        {
            if (SelectedUser != null)
            {
                inEditMode = true;
                CopySelectedUSersValuesToEditedUser();
            }
        }

        private void CopySelectedUSersValuesToEditedUser()
        {
            EditedUser.Username = SelectedUser.Username;
            EditedUser.Firstname = SelectedUser.Firstname;
            EditedUser.Middlename = SelectedUser.Middlename;
            EditedUser.Surname = SelectedUser.Surname;
            EditedUser.Email = SelectedUser.Email;
            EditedUser.ImageUrl = SelectedUser.ImageUrl;
        }

        public void OnCloseUserEditWindow()
        {
            inEditMode = false;
        }

        public void OnOpenUserEditWindow()
        {
            ResetEditedUserValues();
        }

        private void ResetEditedUserValues()
        {
            EditedUser.Username = "";
            EditedUser.Firstname = "";
            EditedUser.Middlename = "";
            EditedUser.Surname = "";
            EditedUser.Email = "";
            EditedUser.ImageUrl = "";
        }

        public void OnAddUser()
        {
            if (inEditMode)
            {
                ReplaceSelectedUserWithEditedUser();
            }
            else
            {
                if (UserIsValidAndDoesNotExist(EditedUser))
                {
                    AddUserViewModel(CreateCopyOfUserViewModel(EditedUser));
                }
            }
        }

        private void ReplaceSelectedUserWithEditedUser()
        {
            if (UserIsValid(EditedUser) && EditedUserIsUnique())
            {
                var selectedIndex = Users.IndexOf(SelectedUser);
                var editedUserCopy = CreateCopyOfUserViewModel(EditedUser);

                Users.Remove(SelectedUser);
                Users.Insert(selectedIndex, editedUserCopy);
                SelectedUser = editedUserCopy;
                inEditMode = false;
            }
        }

        public UserViewModel CreateCopyOfUserViewModel(UserViewModel userViewModel)
        {
            var userViewModelCopy = new UserViewModel
            {
                Username = userViewModel.Username,
                Firstname = userViewModel.Firstname,
                Middlename = userViewModel.Middlename,
                Surname = userViewModel.Surname,
                Email = userViewModel.Email,
                ImageUrl = userViewModel.ImageUrl,
                DefaultPictureUri = userViewModel.DefaultPictureUri
            };
            return userViewModelCopy;
        }

        public void OnDeleteSelectedUser()
        {
            if (SelectedUser != null)
            {
                Users.Remove(SelectedUser);
                SelectedUser = null;
            }
        }

        private bool UserIsValidAndDoesNotExist(UserViewModel user)
        {
            var userIsValid = UserIsValid(user);
            var userExists = UserExists(user);

            return !userExists && userIsValid;
        }

        private static bool UserIsValid(UserViewModel user)
        {
            return Validator.TryValidateObject(
                user, new ValidationContext(user, null, null),
                    new List<ValidationResult>());
        }

        private bool UserExists(UserViewModel user)
        {
            var userExists = false;

            if (user != null && user.Username != null)
            {
                userExists = Users.Contains(user);
            }
            return userExists;
        }

        private bool IsDuplicateUser(UserViewModel user)
        {
            var userNames = (Users.Where(
                tm => tm.Username.Equals(user.Username))).Count();

            return userNames > 1;
        }

        private bool EditedUserIsUnique()
        {
            if (SelectedUser != null)
                return EditedUser.Username.Equals(SelectedUser.Username) ? !IsDuplicateUser(EditedUser) : !UserExists(EditedUser);

            return false;
        }

    }
}
