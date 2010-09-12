using System;
using System.ComponentModel;
using System.Windows.Controls;
using Smeedee.Widget.BeerOMeter.SL.ViewModels;

namespace Smeedee.Widget.BeerOMeter.SL.Views
{
    public partial class BeerOMeterView : UserControl
    {
        public BeerOMeterView()
        {
            InitializeComponent();
            Animation.Begin();
            Animation.Completed += AnimationCompleted;

            AddPropertyChangedListener();

        }

        private void AddPropertyChangedListener()
        {
            try
            {
                var viewModel = DataContext as BeerOMeterViewModel;

                if (viewModel != null)
                {
                    viewModel.PropertyChanged += viewModelPropertyChanged;
                }
            }
            catch (Exception) {}
        }

        private void viewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Rate"))
            {
                try
                {
                    var viewModel = sender as BeerOMeterViewModel;
                    Animation.SpeedRatio = viewModel.Rate;
                }
                catch (Exception) {}
                
            }
        }

        private void AnimationCompleted(object sender, EventArgs e)
        {
            try
            {
                var viewModel = DataContext as BeerOMeterViewModel;

                if (viewModel != null)
                {
                    viewModel.AnimationCompleted.Execute();
                }

                Animation.Begin();
            }
            catch (Exception) {}
            
        }
    }
}
