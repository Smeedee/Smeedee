using System.ComponentModel.DataAnnotations;

namespace Smeedee.Widget.BeerOMeter.SL.ViewModels
{
    public partial class BeerOMeterViewModel : TinyMVVM.Framework.ViewModelBase
    {
        public const int AnimationDuration = 10;

        private int cashEarned; 

        partial void  OnInitialize()
        {
            cashEarned = 0;
            rate = 200;
            price = 50;
            AnimationCompleted.AfterExecute += (sender, args) =>  OnAnimationCompleted();
            Reset.AfterExecute += (sender, args) => OnReset();

        }

        private void OnAnimationCompleted()
        {
            cashEarned += Price;
            TriggerPropertyChanged("NumberOfBeers");
        }

        private void OnReset()
        {
            cashEarned = 0;
        }

        public double Ratio
        {
            get
            {
                return AnimationDuration / ((Rate / Price) * 3600);
            }
        }

        partial void OnGetNumberOfBeers(ref int value)
        {
            value = cashEarned/Price;
        }


        private int rate;

        [Required]
        [Display(Name = "Rate", Description = "Hour rate")]
        [Range(1, 1000, ErrorMessage = "Hour rate must be between 1-100")]
        public int Rate
        {
            get { return rate; }
            set
            {
                if (value != rate)
                {
                    rate = value;
                    TriggerPropertyChanged("Rate");
                    TriggerPropertyChanged("Ratio");
                }
            }
        }

        private int price;

        [Required]
        [Display(Name = "Price", Description = "Beer price pr 0.5 L")]
        [Range(1, 125, ErrorMessage = "Price must be between 1-100")]
        public int Price
        {
            get { return price; }
            set
            {
                if (value != price)
                {
                    price = value;
                    TriggerPropertyChanged("Price");
                    TriggerPropertyChanged("Ratio");
                }
            }
        }

    }
}
