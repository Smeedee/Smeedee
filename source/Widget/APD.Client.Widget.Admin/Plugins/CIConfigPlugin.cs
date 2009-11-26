using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace APD.Client.Widget.Admin.Plugins
{
    public class CIConfigPlugin : ConfigPlugin
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
