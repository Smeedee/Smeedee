using System.Windows;
using System.Windows.Controls;

namespace Smeedee.Widget.Admin.Users.SL.Views
{
    public partial class UserEditWindow : ChildWindow
    {
        private bool focusSet = false;

        public UserEditWindow()
        {
            InitializeComponent();
            GotFocus += (sender, args) => SetTextCursorToFirstField();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataForm.ValidateItem())
            {
                this.DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void SetTextCursorToFirstField()
        {
            TextBox usernameBox = dataForm.FindNameInContent("Username") as TextBox;
            if (!focusSet && usernameBox != null)
            {
                usernameBox.Focus();
                focusSet = true;
            }
        }
    }
}
