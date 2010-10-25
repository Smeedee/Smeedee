using System;

namespace Smeedee.Widgets.TeamMembers.ViewModels
{
    partial class UserViewModel
    {
        partial void OnGetImageUrl(ref string value)
        {
            if (value == null)
            {
                value = DefaultPictureUri;
            }
        }
        
        public override bool Equals(object obj)
        {
            UserViewModel value = (obj as UserViewModel); 
 
            if (value == null) 
                return false;

            return Username.Equals(value.Username); 
        }
    }
}