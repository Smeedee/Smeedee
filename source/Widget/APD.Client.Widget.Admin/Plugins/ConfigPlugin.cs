using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.Client.Framework.Plugins;
using System.Windows.Controls;
using APD.DomainModel.Config;

namespace APD.Client.Widget.Admin.Plugins
{
    public abstract class ConfigPlugin : IConfigurationPlugin
    {
        protected Configuration configuration;

        public abstract string Name { get; }
        public abstract UserControl UserInterface { get; }

        public void Load(Configuration configuration)
        {
            this.configuration = configuration;

        }

        public Configuration Save()
        {
            return configuration;
        }
    }
}
