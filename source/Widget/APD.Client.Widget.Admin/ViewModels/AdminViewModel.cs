using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.Client.Framework;
using APD.Client.Framework.Controllers;
using APD.Client.Framework.ViewModels;
using System.Collections.ObjectModel;
using APD.Client.Widget.Admin.ViewModels.Configuration;

namespace APD.Client.Widget.Admin.ViewModels
{
    public class AdminViewModel : AbstractViewModel
    {
        public UserdbViewModel Userdb { get; set; }
        public ConfigurationViewModel Configuration { get; set; }

        public AdminViewModel(IInvokeUI uiInvoker, ITriggerCommand saveUserDbNotifier, ITriggerCommand reloadUserdbNotifier, 
            ITriggerEvent<EventArgs> editUserdbNotifier,
            ITriggerCommand saveConfigNotifier,
            ITriggerCommand refreshConfigNotifier) : 
            base(uiInvoker)
        {
            Userdb = new UserdbViewModel(uiInvoker, saveUserDbNotifier, reloadUserdbNotifier, editUserdbNotifier);
            Configuration = new ConfigurationViewModel(uiInvoker, saveConfigNotifier, refreshConfigNotifier);
        }
    }
}
