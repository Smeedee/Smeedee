using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.Client.Framework.ViewModels;
using APD.Client.Framework;
using APD.Client.Framework.Plugins;

namespace APD.Client.Widget.Admin.ViewModels.Configuration
{
    public class PluginConfigItemViewModel : ConfigurationItemViewModel
    {
        public IConfigurationPlugin Plugin { get; set; }
        public bool IsLoaded { get; set; }
        public string ErrorMessage { get; set; }

        public PluginConfigItemViewModel(IInvokeUI uiInvoker) :
            base(uiInvoker)
        {
            
        }
    }
}
