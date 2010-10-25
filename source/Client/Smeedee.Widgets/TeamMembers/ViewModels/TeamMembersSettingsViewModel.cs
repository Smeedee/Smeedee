using System;
using System.Collections.ObjectModel;

namespace Smeedee.Widgets.TeamMembers.ViewModels
{
    partial class TeamMembersSettingsViewModel
    {
        partial void OnInitialize()
        {
            Userdbs = new ObservableCollection<string>(); 
        }  
    }
}