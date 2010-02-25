using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;


namespace APD.Client.Widget.Admin.ViewModels
{
    public class HolidayViewModel
    {
        public ICommand SaveUserdbUICommand { get; set; }
        public ICommand ReloadUserdbUICommand { get; set; }

        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
