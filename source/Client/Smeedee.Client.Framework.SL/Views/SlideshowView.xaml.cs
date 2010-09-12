using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Client.Framework.SL.Views
{
	public partial class SlideshowView : UserControl
	{
	    private Dictionary<Key, Action> keyInputBindings = new Dictionary<Key,Action>();

	    protected Slideshow ViewModel
	    {
            get { return DataContext as Slideshow; }
	    }

        public SlideshowView()
        {
            // Required to initialize variables
			InitializeComponent();

            CreateInputBindings();

            KeyUp += SlideshowView_KeyUp;
            
            Loaded += (o, e) =>
            {
                ViewModel.Start.AfterExecute += (o2, e2) => Focus();
                ViewModel.Pause.AfterExecute += (o2, e2) => Focus();
                ViewModel.Edit.AfterExecute += (o2, e2) => Focus();
                ViewModel.AddSlide.AfterExecute += (o2, e2) => Focus();
            };
        }


	    private void CreateInputBindings()
	    {
	        keyInputBindings.Add(Key.Right, () => ViewModel.Next.Execute(null));
	        keyInputBindings.Add(Key.Left, () => ViewModel.Previous.Execute(null));
            keyInputBindings.Add(Key.P, () => TogglePause());
        }

        private void TogglePause()
        {
            if (!ViewModel.CurrentSlide.Widget.IsInSettingsMode)
            {
                if (ViewModel.IsRunning)
                    ViewModel.Pause.Execute(null);
                else
                    ViewModel.Start.Execute(null);
            }
        }

	    void SlideshowView_KeyUp(object sender, KeyEventArgs e)
        {
            if (keyInputBindings.ContainsKey(e.Key))
            {
                keyInputBindings[e.Key].Invoke();
            }
        }
	}
}