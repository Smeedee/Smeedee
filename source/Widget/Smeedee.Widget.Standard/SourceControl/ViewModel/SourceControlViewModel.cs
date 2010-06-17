using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Net;
using System.Reflection;
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
using APD.DomainModel.SourceControl;
using Smeedee.Client.Web.Services;
using Smeedee.Widget.Standard.Factories;

namespace Smeedee.Widget.Standard.SourceControl.ViewModel
{
    public partial class SourceControlViewModel
    {
        private SourceControlContext sourceControlContext;
        private EntityQuery<Changeset> lastSixChangesetsQuery;
        private Timer timer;

        public void OnInitialize()
        {
            Changesets = new ObservableCollection<ChangesetViewModel>();

            sourceControlContext = new SourceControlContext();
            lastSixChangesetsQuery = sourceControlContext.GetAllQuery().
                OrderByDescending(e => e.Time).Take(6);


            timer = new Timer((o) =>
            {
                LoadChangesets(lastSixChangesetsQuery);
            }, null, 0, 15000);
        }

        private void LoadChangesets(EntityQuery<Changeset> query)
        {
            sourceControlContext.Load(query, (o) =>
            {
                var data = sourceControlContext.Changesets.Select(c =>
                    new ChangesetViewModel()
                    {
                        AvatarUrl = "http://www.gravatar.com/avatar/0ca33046e6ce8c5d3a2d18c35db2e5cd.png",
                        Name = "goeran",
                        Comment = c.Comment,
                        Revision = string.Format("{0}", c.Revision),
                        Time = string.Format("{0}", c.Time)
                    }).ToList();
                Changesets.Clear();
                foreach (var item in data)
                    Changesets.Add(item);

            }, null);
        }
    }
}
