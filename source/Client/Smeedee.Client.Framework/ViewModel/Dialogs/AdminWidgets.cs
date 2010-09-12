using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Controller;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using TinyMVVM.IoC;

#if SILVERLIGHT
using Smeedee.Client.Framework.SL.Views.Dialogs;
#else
using Smeedee.Client.Framework.Views.Dialogs;
#endif

namespace Smeedee.Client.Framework.ViewModel.Dialogs
{
    partial class AdminWidgets
    {
        partial void OnInitialize()
        {
            AdminWidgets = new List<Slide>();
        }

    }
}
