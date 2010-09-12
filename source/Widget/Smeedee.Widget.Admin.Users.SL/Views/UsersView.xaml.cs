using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Smeedee.Widget.Admin.Users.SL.Views
{
    public partial class UsersView : UserControl
    {
        public UsersView()
        {
            InitializeComponent();
        }

        private void AddUserClick(object sender, RoutedEventArgs e)
        {
            var userWindow = new UserEditWindow {Title = "Add new user", DataContext = this.DataContext};
            userWindow.Show();
        }

        private void ResizeColumns(object sender, System.EventArgs e)
        {
            try
            {
                var columns = userDataGrid.Columns.Where(column => column.Width.IsSizeToCells).ToList();

                foreach (var column in columns)
                {
                    column.Width = new DataGridLength(0);
                }
                userDataGrid.UpdateLayout();

                foreach (var column in columns)
                {
                    column.Width = DataGridLength.SizeToCells;
                }
                userDataGrid.UpdateLayout();
            }
            catch (Exception exception)
            {
                userDataGrid.UpdateLayout();
            }
        }

        private void EditUserClick(object sender, RoutedEventArgs e)
        {
            if (userDataGrid.SelectedItem != null)
            {
                var userWindow = new UserEditWindow {Title = "Edit user", DataContext = this.DataContext};
                userWindow.Show();
            }
        }
    }
}
