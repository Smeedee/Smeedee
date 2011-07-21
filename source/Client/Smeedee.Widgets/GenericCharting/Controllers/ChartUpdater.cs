using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.GenericCharting.Controllers
{
    public class ChartUpdater
    {

        private ChartViewModel viewModel;
        private IUIInvoker uiInvoker;
        private ChartConfig chartConfig;

        public ChartUpdater(ChartViewModel viewModel, ChartConfig config, IUIInvoker uiInvoker)
        {
            this.viewModel = viewModel;
            this.uiInvoker = uiInvoker;
            chartConfig = config;
        }

        public void Update()
        {
            if (chartConfig.IsValid)
                HideErrorMessage();
            else
                ShowErrorMessage("No chart configured.");
        }

        private void ShowErrorMessage(string message)
        {
            uiInvoker.Invoke(() =>
                                 {
                                     viewModel.ErrorMessage = message;
                                     viewModel.ShowErrorMessageInsteadOfChart = true;
                                 });
        }

        private void HideErrorMessage()
        {
            uiInvoker.Invoke(() =>
                                 {
                                     viewModel.ShowErrorMessageInsteadOfChart = false;
                                     viewModel.ErrorMessage = null;
                                 });
        }
    }
}
