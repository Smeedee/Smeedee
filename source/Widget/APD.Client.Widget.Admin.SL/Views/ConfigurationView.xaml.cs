using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using APD.Client.Framework.SL.Controls;
using APD.Client.Widget.Admin.ViewModels.Configuration;

namespace APD.Client.Widget.Admin.SL.Views
{
    public partial class ConfigurationView : UserControl, IDataTemplateSelector
    {
        public ConfigurationView()
        {
            InitializeComponent();

            configsItemsControl.ItemTemplateSelector = this;
        }

        public DataTemplate SelectTemplate(object item, DependencyObject element)
        {
            if (item is ProviderConfigItemViewModel)
            {
                return Resources["providerConfigItemViewModelDataTemplate"] as DataTemplate;
            }
            else if (item is GenericConfigItemViewModel)
                return Resources["genericConfigItemViewModelDataTemplate"] as DataTemplate;
            else if (item is DashboardConfigItemViewModel)
                return Resources["dashboardConfigItemViewModelDataTemplate"] as DataTemplate;
            else if (item is PluginConfigItemViewModel)
                return Resources["pluginConfigItemViewModelDataTemplate"] as DataTemplate;
            else
                return null;
        }
    }
}
