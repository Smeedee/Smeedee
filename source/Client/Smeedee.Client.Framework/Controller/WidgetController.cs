using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;

namespace Smeedee.Client.Framework.Controller
{
    public class WidgetController 
    {
        private readonly Widget viewModel;
        private IPersistDomainModelsAsync<Configuration> configPersister;

        public WidgetController(Widget viewModel, IPersistDomainModelsAsync<Configuration> configPersister)
        {
            this.viewModel = viewModel;
            viewModel.SaveSettings.ExecuteDelegate = OnSaveSettings;
            this.configPersister = configPersister;
            this.configPersister.SaveCompleted += ConfigPersisterSaveCompleted;
            
        }

        protected void OnSaveSettings()
        {
            viewModel.ProgressbarService.ShowInSettingsView("Saving settings...");
            configPersister.Save(viewModel.Configuration);
        }

        void ConfigPersisterSaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            viewModel.ProgressbarService.HideInSettingsView();
            if (e.Error != null)
            {
                viewModel.ErrorInfo.ErrorMessage = "Failed to save settings :(";
                viewModel.ErrorInfo.HasError = true;
            }
            else
            {
                if (viewModel.IsInSettingsMode)
                    viewModel.Settings.Execute();
            }
        }
    }
}
