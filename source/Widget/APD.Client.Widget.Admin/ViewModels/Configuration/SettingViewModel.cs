using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.Client.Framework.ViewModels;
using APD.Client.Framework;

namespace APD.Client.Widget.Admin.ViewModels.Configuration
{
    public abstract class SettingViewModel : AbstractViewModel
    {
        public string Name { get; set; }

        public SettingViewModel(IInvokeUI uiInvoker) :
            base(uiInvoker)
        {
            
        }
    }

    public class KeyValueSettingViewModel : SettingViewModel
    {
        public string Value { get; set; }

        public KeyValueSettingViewModel(IInvokeUI uiInvoker) :
            base(uiInvoker)
        {
            
        }
    }
}
