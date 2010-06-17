using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Web.Services;
using Smeedee.Client.Web.Services.DAL;

namespace Smeedee.Widget.Standard.ProjectInfo.Views
{
    public partial class UserdbSettingsView : UserControl
    {
        public UserdbSettingsView()
        {
            InitializeComponent();
        }

        private void userDomainDataSource_LoadedData(object sender, LoadedDataEventArgs e)
        {

            if (e.HasError)
            {
                System.Windows.MessageBox.Show(e.Error.ToString(), "Load Error", System.Windows.MessageBoxButton.OK);
                e.MarkErrorAsHandled();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            userDomainDataSource.SubmitChanges();
        }

        private void userDomainDataSource_LoadedData_1(object sender, LoadedDataEventArgs e)
        {

            if (e.HasError)
            {
                System.Windows.MessageBox.Show(e.Error.ToString(), "Load Error", System.Windows.MessageBoxButton.OK);
                e.MarkErrorAsHandled();
            }
        }

        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {
            var userWindow = new NewUserWindow();
            userWindow.Closed += new EventHandler(userWindow_Closed);
            userWindow.Show();
        }

        void userWindow_Closed(object sender, EventArgs e)
        {
            var userWindow = sender as NewUserWindow;

            if (userWindow.NewUser != null)
            {
                userDomainDataSource.DataView.Add(userWindow.NewUser);
                userDomainDataSource.SubmitChanges();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var ctx = userDomainDataSource.DomainContext as UserdbContext;

            var selectedUser = userDataGrid.SelectedItem as User;

            if (selectedUser != null)
            {
                userDomainDataSource.DataView.Remove(selectedUser);
                userDomainDataSource.SubmitChanges();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (userDomainDataSource.CanLoad)
                userDomainDataSource.Load();
        }
    }
}
