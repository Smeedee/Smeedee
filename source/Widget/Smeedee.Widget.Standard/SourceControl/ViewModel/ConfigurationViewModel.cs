using System;
using System.Linq;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Web.Services;
using Smeedee.DomainModel.Config;

namespace Smeedee.Widget.Standard.SourceControl.ViewModel
{
    public partial class ConfigurationViewModel
    {
        private ConfigurationContext configContext;

        public void OnInitialize()
        {
            configContext = new ConfigurationContext();

            configContext.Load(configContext.GetVCSConfigurationQuery(), (o) =>
            {
                VCSConfiguration = configContext.VCSConfigurations.SingleOrDefault();
            }, null);
        }
    }
}
