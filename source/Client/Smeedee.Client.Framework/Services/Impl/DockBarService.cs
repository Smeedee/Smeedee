using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Framework;

namespace Smeedee.Client.Framework.Services.Impl
{
    public class DockBarService : IDockBarService
    {
        private DockBar dockBar;

        public DockBarService(DockBar dockBar)
        {
            this.dockBar = dockBar;
        }

        public void Show()
        {
            dockBar.IsVisible = true;
        }

        public void Hide()
        {
            dockBar.IsVisible = false;
        }

        public IEnumerable<DockBarItem> Items
        {
            get { return dockBar.Items.ToList(); }
        }

        public void AddItem(DockBarItem item)
        {
            Guard.Requires<ArgumentNullException>(item != null);

            dockBar.Items.Add(item);
        }

        public void RemoveItem(DockBarItem item)
        {
            Guard.Requires<ArgumentNullException>(item != null);
            Guard.Requires<ArgumentException>(dockBar.Items.Contains(item));

            dockBar.Items.Remove(item);
        }
    }
}
