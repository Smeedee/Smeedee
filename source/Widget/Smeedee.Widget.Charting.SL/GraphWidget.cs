using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using TinyMVVM.Framework;
using Tskjortebutikken.Widgets.Controllers;
using Tskjortebutikken.Widgets.SL.Views;
using Tskjortebutikken.Widgets.ViewModel;

namespace Tskjortebutikken.Widgets.SL
{
    [WidgetInfo(Name = "Graf",
        Author = "Gøran Hansen",
        Description = "Visualiserer et dataset som en graf",
        Tags = new string[]{ "t-skjortebutikken.no", "Charting" })]
    public class GraphWidget : Widget
    {
        private GraphController controller;
        private GraphSettingsController settingsController;

        public GraphWidget()
        {
            Title = "Graf";

            controller = NewController<GraphController>();
            settingsController = NewController<GraphSettingsController>();

            PropertyChanged += (o, e) =>
            {
                if (IsInSettingsMode)
                    controller.StopFetchingDataInBackground();
                else
                    controller.StartFetchingDataInBackground();
            };

            ConfigurationChanged += (o, e) =>
            {
                controller.UpdateConfiguration(Configuration);
                settingsController.UpdateConfiguration(Configuration);
            };

            View = new GraphView()
            {
                DataContext = controller.ViewModel
            };
            SettingsView = new GraphSettingsView()
            {
                DataContext = settingsController.GraphSettingsViewModel
            };
        }

        public override void Configure(DependencyConfigSemantics config)
        {
            config.Bind<Graph>().To<Graph>().InSingletonScope();
            config.Bind<GraphSettings>().To<GraphSettings>().InSingletonScope();
            config.Bind<GraphController>().To<GraphController>().InSingletonScope();
            config.Bind<GraphSettingsController>().To<GraphSettingsController>().InSingletonScope();
        }

        protected override Configuration NewConfiguration()
        {
            return GraphConfig.NewDefaultConfiguration();
        }
    }
}
