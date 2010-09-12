using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Client.Framework.Services
{
    public interface IManageConfigurations
    {
        void RegisterForConfigurationUpdates(Widget widgetToRegister);
    }
}
