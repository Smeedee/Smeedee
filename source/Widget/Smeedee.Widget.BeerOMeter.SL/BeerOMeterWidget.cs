using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widget.BeerOMeter.SL.ViewModels;
using Smeedee.Widget.BeerOMeter.SL.Views;
using TinyMVVM.Framework;

namespace Smeedee.Widget.BeerOMeter.SL
{
//    [WidgetInfo(Name = "BeerOMeter",
//                Description = "Shows resources spent/earned in the most universally accepted measurement unit, namely beer",
//                Author = "Smeedee team",
//                Version = "1.0",
//                Tags = new[] { CommonTags.Fun })
//    ]
    public class BeerOMeterWidget : Client.Framework.ViewModel.Widget
    {
        public BeerOMeterWidget()
        {
            Title = "BeerO'Meter";

            var viewModel = GetInstance<BeerOMeterViewModel>();

            View = new BeerOMeterView {DataContext = viewModel};
            SettingsView = new BeerOMeterSettingsView {DataContext = viewModel};
            
        }

        public override void Configure(DependencyConfigSemantics config)
        {
            config.Bind<BindableViewModel<BeerOMeterViewModel>>().To<BindableViewModel<BeerOMeterViewModel>>().InSingletonScope();
        }
    }

}
