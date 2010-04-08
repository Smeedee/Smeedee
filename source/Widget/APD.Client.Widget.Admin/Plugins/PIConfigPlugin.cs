using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace APD.Client.Widget.Admin.Plugins
{
    class PIConfigPlugin : ConfigPlugin
    {
        public override string Name
        {
            get { return "Continuous Integration"; }
        }

        public override UserControl UserInterface
        {
            get { return null; }
        }
    }
}
