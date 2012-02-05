using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using TinyMVVM.Framework;
using Tskjortebutikken.Widgets.Controllers;
using Tskjortebutikken.Widgets.SL.Views;
using Tskjortebutikken.Widgets.ViewModel;

namespace Smeedee.Widget.Charting.SL
{
    [WidgetInfo(Name = "Dataset Visualizer",
        Author = "Gøran Hansen",
        Description = " Used together with Script Task to Visualize dataset as a graph (for advanced users)",
        Version = "0.1 Beta",
        Tags = new string[]{ CommonTags.Charting })]
    public class GraphWidget : Client.Framework.ViewModel.Widget
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
