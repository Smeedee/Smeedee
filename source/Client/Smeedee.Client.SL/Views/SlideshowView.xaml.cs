using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Client.SL
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
        }

	    private void CreateInputBindings()
	    {
	        keyInputBindings.Add(Key.Right, () => ViewModel.Next.Execute(null));
	        keyInputBindings.Add(Key.Left, () => ViewModel.Previous.Execute(null));
            keyInputBindings.Add(Key.Space, () => ViewModel.Start.Execute(null));
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