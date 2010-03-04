using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Framework;
using APD.Client.Framework.ViewModels;


namespace APD.Client.Widget.CI.ViewModels
{
    public class BuildHistoryViewModel : BindableViewModel<ProjectBuildHistoryViewModel>
    {
        public BuildHistoryViewModel(IInvokeUI uiInvoker)
            : base(uiInvoker)
        {
            
        }
    }
}
