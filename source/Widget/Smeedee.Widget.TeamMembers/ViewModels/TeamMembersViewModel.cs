using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using Smeedee.Client.Framework.ViewModel;
using TinyMVVM.Framework;

namespace Smeedee.Widget.TeamMembers.ViewModels
{
    partial class TeamMembersViewModel 
    {
        public const string DEFAULT_IMAGE = "UserImages/default_user.jpg";

        partial void OnInitialize()
        {
            TeamMembers = new ObservableCollection<UserViewModel>();
        }

        private bool firstnameIsChecked;

        public bool FirstnameIsChecked
        {
            get { return firstnameIsChecked; }
            set
            {
                if (value != firstnameIsChecked)
                {
                    var visibilityToSet = value ? Visibility.Visible : Visibility.Collapsed;
                    foreach (var teamMember in TeamMembers)
                    {
                        teamMember.FirstnameIsVisible = visibilityToSet;
                    }
                    firstnameIsChecked = value;
                    TriggerPropertyChanged<TeamMembersViewModel>(t => t.FirstnameIsChecked);
                }
            }
        }

        private bool middlenameIsChecked;

        public bool MiddlenameIsChecked
        {
            get { return middlenameIsChecked; }
            set
            {
                if (value != middlenameIsChecked)
                {
                    var visibilityToSet = value ? Visibility.Visible : Visibility.Collapsed;
                    foreach (var teamMember in TeamMembers)
                    {
                        teamMember.MiddlenameIsVisible = visibilityToSet;
                    }
                    middlenameIsChecked = value;
                    TriggerPropertyChanged<TeamMembersViewModel>(t => t.MiddlenameIsChecked);
                }
            }
        }

        private bool surnameIsChecked;

        public bool SurnameIsChecked
        {
            get { return surnameIsChecked; }
            set
            {
                if (value != surnameIsChecked)
                {
                    var visibilityToSet = value ? Visibility.Visible : Visibility.Collapsed;
                    foreach (var teamMember in TeamMembers)
                    {
                        teamMember.SurnameIsVisible = visibilityToSet;
                    }
                    surnameIsChecked = value;
                    TriggerPropertyChanged<TeamMembersViewModel>(t => t.SurnameIsChecked);
                }
            }
        }

        private bool usernameIsChecked;

        public bool UsernameIsChecked
        {
            get { return usernameIsChecked; }
            set
            {
                if (value != usernameIsChecked)
                {
                    var visibilityToSet = value ? Visibility.Visible : Visibility.Collapsed;
                    foreach (var teamMember in TeamMembers)
                    {
                        teamMember.UsernameIsVisible = visibilityToSet;
                    }
                    usernameIsChecked = value;
                    TriggerPropertyChanged<TeamMembersViewModel>(t => t.UsernameIsChecked);
                }
            }
        }

        public void AddTeamMember(UserViewModel teamMember)
        {
            teamMember.DefaultPictureUri = DeploymentPath + DEFAULT_IMAGE;
            SetVisibilitiesToUserViewModel(teamMember);
            TeamMembers.Add(teamMember);
        }

        private void SetVisibilitiesToUserViewModel(UserViewModel user)
        {
            user.FirstnameIsVisible = FirstnameIsChecked ? Visibility.Visible : Visibility.Collapsed;
            user.MiddlenameIsVisible = MiddlenameIsChecked ? Visibility.Visible : Visibility.Collapsed;
            user.SurnameIsVisible = SurnameIsChecked ? Visibility.Visible : Visibility.Collapsed;
            user.UsernameIsVisible = UsernameIsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        public UserViewModel CreateCopy(UserViewModel userViewModel)
        {
            var userViewModelCopy = new UserViewModel()
            {
                Username = userViewModel.Username,
                Firstname = userViewModel.Firstname,
                Middlename = userViewModel.Middlename,
                Surname = userViewModel.Surname,
                Email = userViewModel.Email,
                ImageUrl = userViewModel.ImageUrl,
                DefaultPictureUri = userViewModel.DefaultPictureUri
            };
            SetVisibilitiesToUserViewModel(userViewModelCopy);
            return userViewModelCopy;
        }
    }
}