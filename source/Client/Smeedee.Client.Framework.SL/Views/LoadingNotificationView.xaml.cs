using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Smeedee.Client.Framework.SL.Views
{
    public partial class LoadingNotificationView : UserControl
    {
        public LoadingNotificationView()
        {
            InitializeComponent();
            Storyboard standardLoadingAnimation = Resources["LoadingAnimation"] as Storyboard;
            if( standardLoadingAnimation != null )
                standardLoadingAnimation.Begin(); 
        }
    }
}
