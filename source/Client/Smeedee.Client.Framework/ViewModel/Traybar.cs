using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using Smeedee.Client.Framework.Factories;
using Smeedee.Client.Framework.Services;

namespace Smeedee.Client.Framework.ViewModel
{
    public partial class Traybar : IPartImportsSatisfiedNotification
    {
        private IModuleLoader moduleLoader = new ModuleLoaderFactory().NewModuleLoader();

        public void OnInitialize()
        {
            Widgets = new ObservableCollection<TraybarWidget>();
            ErrorInfo = new ErrorInfo();

            LoadTraybarWidgets();
        }

        private void LoadTraybarWidgets()
        {
            try
            {
                moduleLoader.LoadTraybarWidgets(this);
            }
            catch (Exception ex)
            {
                ErrorInfo.HasError = true;
                ErrorInfo.ErrorMessage = ex.Message;
            }
        }

        public void OnImportsSatisfied()
        {
        }
    }

}
