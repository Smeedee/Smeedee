using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

using APD.Client.Framework.ViewModels;
using APD.Client.Framework;
using System.Collections.ObjectModel;

namespace APD.Client.Widget.Admin.ViewModels.Configuration
{
    public class ConfigurationViewModel : BindableViewModel<ConfigurationItemViewModel>
    {
        public ICommand SaveUserdbUICommand { get; set; }
        public ICommand ReloadUserdbUICommand { get; set; }

        public ConfigurationViewModel(IInvokeUI uiInvoker, ITriggerCommand saveConfigCommandTrigger, ITriggerCommand refreshConfigCommandTrigger)
            : base(uiInvoker)
        {
            SaveUserdbUICommand = new UICommand(saveConfigCommandTrigger);
            ReloadUserdbUICommand = new UICommand(refreshConfigCommandTrigger);
        }
    }
}
