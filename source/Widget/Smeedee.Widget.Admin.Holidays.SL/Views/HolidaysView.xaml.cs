using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Smeedee.Widget.Admin.Holidays.ViewModels;

namespace Smeedee.Widget.Admin.Holidays.SL.Views
{
    public partial class HolidaysView : UserControl
    {
        public HolidaysView()
        {
            InitializeComponent();
            gridHolidays.BeginEdit();

            KeyDown += HolidaysView_KeyDown;
        }

        void HolidaysView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var dataContext = (HolidaysViewModel)DataContext;
                dataContext.DeleteSelectedHoliday.Execute(null);
            }
        }

        private void ResizeColumns(object sender, System.EventArgs e)
        {
            try
            {
                var columns = gridHolidays.Columns.Where(column => column.Width.IsSizeToCells).ToList();

                foreach (var column in columns)
                {
                    column.Width = new DataGridLength(0);
                }
                gridHolidays.UpdateLayout();

                foreach (var column in columns)
                {
                    column.Width = DataGridLength.SizeToCells;
                }
                gridHolidays.UpdateLayout();
            }
            catch (Exception exception)
            {
                gridHolidays.UpdateLayout();
            }
        }
    }
}
