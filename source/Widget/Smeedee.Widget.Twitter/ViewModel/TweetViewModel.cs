using System;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widgets.SL.Twitter.ViewModel
{
    public class TweetViewModel : AbstractViewModel
    {
        private string username;
        public string Username
        {
            get { return username; }
            set
            {
                if(value != username)
                {
                    username = value;
                    TriggerPropertyChanged<TweetViewModel>(t => t.Username);
                }
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                if (value != message)
                {
                    message = value;
                    TriggerPropertyChanged<TweetViewModel>(t => t.Message);
                }
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                if (value != date)
                {
                    date = value;
                    TriggerPropertyChanged<TweetViewModel>(t => t.Date);
                }
            }
        }

        private string userImageUrl;
        public string UserImageUrl
        {
            get { return userImageUrl; }
            set
            {
                if (value != userImageUrl)
                {
                    userImageUrl = value;
                    TriggerPropertyChanged<TweetViewModel>(t => t.UserImageUrl);
                }
            }
        }

    }
}
