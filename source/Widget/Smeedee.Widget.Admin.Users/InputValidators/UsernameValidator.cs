using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Smeedee.Widget.Admin.Users.ViewModels;

namespace Smeedee.Widget.Admin.Users.InputValidators
{
    public class UsernameValidator
    {
        public static ValidationResult ValideUserViewModel(object value, ValidationContext context)
        {
            var user = context.ObjectInstance as UserViewModel;

            if (user == null)
                return ValidationResult.Success;

            if (user.HasInvalidUsername)
                return new ValidationResult("Username must be unique!");

            return ValidationResult.Success;
        }
    }
}
