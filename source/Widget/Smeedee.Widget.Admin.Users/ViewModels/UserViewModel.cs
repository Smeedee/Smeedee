using System.ComponentModel.DataAnnotations;
using Smeedee.Widget.Admin.Users.InputValidators;

namespace Smeedee.Widget.Admin.Users.ViewModels
{
    public partial class UserViewModel
    {
        private string username;

        [Key]
        [Display(Name = "Username", Description = "Must be unique!")]
        [Required(ErrorMessage = "Username is required!")]
        [CustomValidation(typeof(UsernameValidator), "ValideUserViewModel")]
        public string Username
        {
            get { return username; }
            set
            {
                if (value != username)
                {
                    username = value;
                    TriggerPropertyChanged("Username");
                }
            }
        }

        private string firstName;

        [Required(ErrorMessage = "First name is required!")]
        public string Firstname
        {
            get { return firstName; }
            set
            {
                if (value != firstName)
                {
                    firstName = value;
                    TriggerPropertyChanged("Firstname");
                }
            }
        }

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
