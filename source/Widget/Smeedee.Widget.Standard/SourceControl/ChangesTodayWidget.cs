using System;
using System.ComponentModel.Composition;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Web.Services;
using Smeedee.Widget.Standard.SourceControl.Views;

namespace Smeedee.Widget.Standard.SourceControl
{
    public class ChangesTodayWidget : TraybarWidget
    {
        private SourceControlContext sourceControlContext;

        public ChangesTodayWidget()
        {
            sourceControlContext = new SourceControlContext();

            View = new ChangesTodayView()
            {
                DataContext = sourceControlContext.Changesets
            };

            var timer = new Timer((o) =>
            {
                LoadTodaysChangesets();
            }, null, 0, 30000);
        }

        private void LoadTodaysChangesets()
        {
            sourceControlContext.Load(
                sourceControlContext.GetAllQuery().
                    Where(entity => entity.Time.Day == DateTime.Today.Day &&
                        entity.Time.Month == DateTime.Today.Month &&
                        entity.Time.Year == DateTime.Today.Year));
        }
    }
}
