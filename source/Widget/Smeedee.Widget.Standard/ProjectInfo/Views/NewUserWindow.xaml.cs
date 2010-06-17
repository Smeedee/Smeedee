using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Web.Services.DAL;

namespace Smeedee.Widget.Standard.ProjectInfo.Views
{
    public partial class NewUserWindow : ChildWindow
    {
        public User NewUser { get; set; }

        public NewUserWindow()
        {
            InitializeComponent();
            NewUser = new User();
            NewUser.Userdb_fid = "default";
            dataForm.CurrentItem = NewUser;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataForm.ValidateItem())
            {
                dataForm.CommitEdit();
                this.DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NewUser = null;
            this.DialogResult = false;
        }
    }
}

