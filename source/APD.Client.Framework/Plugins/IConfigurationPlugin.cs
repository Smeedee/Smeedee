using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.DomainModel.Config;
using System.Windows.Controls;

namespace APD.Client.Framework.Plugins
{
    public interface IConfigurationPlugin
    {
        string Name { get; }
        UserControl UserInterface { get; }

        void Load(Configuration configuration);
        Configuration Save();
    }
}
