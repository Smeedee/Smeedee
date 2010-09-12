using System;
using System.Collections.ObjectModel;
using Smeedee.Client.Framework.Services;
using TinyMVVM.IoC;


namespace Smeedee.Client.Framework.ViewModel
{
    partial class DockBar
    {
        private IModuleLoader moduleLoader;

        partial void OnInitialize()
        {
            Items = new ObservableCollection<DockBarItem>();

            moduleLoader = this.GetDependency<IModuleLoader>();

            TryLoadAdminWidgets();
        }

        private void TryLoadAdminWidgets()
        {
            moduleLoader.LoadAdminWidgets(this);
        }
    }
}
