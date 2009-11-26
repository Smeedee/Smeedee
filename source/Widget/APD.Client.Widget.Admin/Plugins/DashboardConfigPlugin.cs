using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using APD.Client.Framework.Plugins;
using APD.DomainModel.Config;

namespace APD.Client.Widget.Admin.Plugins
{
    public class DashboardConfigPlugin : ConfigPlugin
    {
        public override string Name
        {
            get { return "Dashboard"; }
        }

        public override UserControl UserInterface
        {
            get { return null; }
        }
    }
}
