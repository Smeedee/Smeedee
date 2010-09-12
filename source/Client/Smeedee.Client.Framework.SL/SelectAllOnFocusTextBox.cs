using System.Windows.Controls;

namespace Smeedee.Client.Framework.SL
{
    public class SelectAllOnFocusTextBox : TextBox
    {
        public SelectAllOnFocusTextBox()
        {
            GotFocus += (o, e) => SelectAll();
        }
    }
}
