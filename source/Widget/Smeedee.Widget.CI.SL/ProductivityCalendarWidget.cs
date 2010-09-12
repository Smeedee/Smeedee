//using System.Collections.Generic;
//using Smeedee.Client.Framework.Services;
//using Smeedee.Client.Framework.ViewModel;
//using Smeedee.DomainModel.CI;
//using Smeedee.DomainModel.Framework;
//using Smeedee.DomainModel.Framework.Logging;
//using Smeedee.DomainModel.Users;
//using Smeedee.Widget.CI.Controllers;
//using Smeedee.Widget.CI.SL.Views;
//using Smeedee.Widget.CI.ViewModels;
//using TinyMVVM.Framework;
//using TinyMVVM.Framework.Services;

//namespace Smeedee.Widget.CI.SL
//{
//    [WidgetInfo(Name = "Productivity Calendar")]
//    public class ProductivityCalendarSlide : Client.Framework.ViewModel.Widget
//    {
//        private ProductivityCalendarController controller;

//        public ProductivityCalendarSlide()
//        {
//            controller = NewController<ProductivityCalendarController>();
        
//            Title = "Productivity Calendar";

//            View = new ProductivityCalendarView { DataContext = controller.ViewModel };
//            SettingsView = new ProductivityCalendarView { DataContext = controller.ViewModel };

//            PropertyChanged += controller.ToggleRefreshInSettingsMode;
//        }

//        public override void Configure(DependencyConfigSemantics config)
//        {
//            config.Bind<ProductivityCalendarViewModel>().To<ProductivityCalendarViewModel>();
//        }
//    }
//}
