using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widget.Twitter.ViewModel
{
    public class TwitterViewModel : BindableViewModel<TweetViewModel>
    {
        

        public TwitterViewModel()
        {
            errorMessage = "";
        }

        private bool error;
        public bool Error
        {
            get { return error; }
            set
            {
                if (value != error)
                {
                    error = value;
                    TriggerPropertyChanged<TwitterViewModel>(t => t.Error);
                }
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                if (value != errorMessage)
                {
                    errorMessage = value;
                    TriggerPropertyChanged<TwitterViewModel>(t => t.ErrorMessage);
                }
            }
        }

    }
}
