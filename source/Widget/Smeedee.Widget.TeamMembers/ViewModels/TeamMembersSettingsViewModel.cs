using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Smeedee.Widget.TeamMembers.ViewModels
{
    partial class TeamMembersSettingsViewModel
    {
        partial void OnInitialize ()
        {
            Userdbs = new ObservableCollection<string>(); 
        }
            
    }
}
