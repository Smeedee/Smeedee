using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace Smeedee.Widget.TeamMembers.ViewModels
{
    partial class UserViewModel : TinyMVVM.Framework.ViewModelBase
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

            return this.Username.Equals(value.Username); 
        }
    }
}
