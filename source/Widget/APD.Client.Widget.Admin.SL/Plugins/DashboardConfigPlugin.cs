using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using APD.Client.Framework.Plugins;
using APD.DomainModel.Config;
using APD.Client.Widget.Admin.SL.Views;

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
            get { return new DashboardConfigItemView(); }
        }
    }
}
