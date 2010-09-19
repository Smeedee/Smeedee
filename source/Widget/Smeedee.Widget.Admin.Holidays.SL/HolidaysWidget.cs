using System.ComponentModel;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widget.Admin.Holidays.Controllers;
using Smeedee.Widget.Admin.Holidays.SL.Views;
using Smeedee.Widget.Admin.Holidays.ViewModels;
using TinyMVVM.Framework;

namespace Smeedee.Widget.Admin.Holidays.SL
{
    [WidgetInfo(Name = "Holidays",
                Description = "Contains the holidays for usage by other widgets.",
                Author = "Smeedee team",
                Version = "1.0",
                Tags = new[] { CommonTags.SourceControl, CommonTags.Fun })
    ]
    public class HolidaysWidget : Client.Framework.ViewModel.Widget
    {
        private const int REFRESH_INTERVAL_IN_MS = 60 * 60 * 1000;   //Needs this at all?

        public HolidaysWidget()
        {
            Title = "Holidays";

            var controller = NewController<HolidaysController>();
            PropertyChanged += controller.ToggleRefreshInSettingsMode;

            controller.ViewModel.PropertyChanged += ViewModelPropertyChanged;

            View = new HolidaysView()
            {
                DataContext = controller.ViewModel
            };
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = sender as AbstractViewModel;
            var isDoneSaving = (viewModel != null && e.PropertyName.Equals("IsSaving") && !viewModel.IsSaving);

            if (isDoneSaving && IsInSettingsMode)
                OnSettings();
        }

        public override void Configure(DependencyConfigSemantics config)
        {
            config.Bind<BindableViewModel<HolidaysViewModel>>().To<BindableViewModel<HolidaysViewModel>>().InSingletonScope();
            config.Bind<int>().ToInstance(REFRESH_INTERVAL_IN_MS);
        }
    }
}