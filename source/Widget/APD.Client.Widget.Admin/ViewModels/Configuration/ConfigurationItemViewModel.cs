using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.Client.Framework.ViewModels;
using APD.Client.Framework;
using System.Collections.ObjectModel;

namespace APD.Client.Widget.Admin.ViewModels.Configuration
{
    public abstract class ConfigurationItemViewModel : AbstractViewModel    
    {
        public string Name { get; set; }
        public string NamePrettyPrint { get; set; }
        public bool IsExpanded { get; set; }

        public ConfigurationItemViewModel(IInvokeUI uiInvoker) :
            base(uiInvoker)
        {
            
        }
    }
}
