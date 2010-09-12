using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.ViewModel;
using TinyMVVM.Framework;

namespace Smeedee.Widget.CI.ViewModels
{
    class ProductivityCalendarViewModel : BindableViewModel<ProductivityCalendarDayViewModel>
    {
        public DelegateCommand Update { get; set; }

        public ProductivityCalendarViewModel()
        {
            Update = new DelegateCommand();
            Update.CanExecuteDelegate = (() => true);
        }
    }
}
