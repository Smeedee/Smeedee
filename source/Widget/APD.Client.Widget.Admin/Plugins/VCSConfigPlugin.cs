using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using System.Windows.Controls;

namespace APD.Client.Widget.Admin.Plugins
{
    public class VCSConfigPlugin : ConfigPlugin
    {
        public override string Name
        {
            get { return "Version Control System"; }
        }

        public override UserControl UserInterface
        {
            get { return null; }
        }
    }
}
