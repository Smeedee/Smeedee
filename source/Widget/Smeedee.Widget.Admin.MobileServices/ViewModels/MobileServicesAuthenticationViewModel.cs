using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyMVVM.Framework;

namespace Smeedee.Widget.Admin.MobileServices.ViewModels
{
    public class MobileServicesAuthenticationViewModel : ViewModelBase
    {
        public MobileServicesAuthenticationViewModel()
        {
            ApiKey = "Test";
        }

        public String ApiKey { get; set; }
    }
}
