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
using TinyMVVM.IoC;

namespace Smeedee.Client.Framework.ViewModel
{
    public partial class Traybar : IPartImportsSatisfiedNotification
    {
        private IModuleLoader moduleLoader;

        partial void OnInitialize()
        {
        	moduleLoader = this.GetDependency<IModuleLoader>();

            Widgets = new ObservableCollection<Widget>();
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
