using System;
using System.Windows;
using System.Windows.Input;

using APD.Client.Framework.Commands;

using Microsoft.Practices.Composite.Events;


namespace APD.Client.Silverlight
{
    public partial class NiceShell : IShell
    {
        private NextSlideCommandPublisher nextSlideCommandPublisher;
        private TogglePauseSlideShowCommandPublisher togglePauseCommandPublisher;
        private PreviousSlideCommandPublisher previousSlideCommandPublisher;

        public NiceShell(       
                            NextSlideCommandPublisher nextSlideCommandPublisher, 
                            TogglePauseSlideShowCommandPublisher togglePauseSlideShowCommandPublisher, 
                            PreviousSlideCommandPublisher previousSlideCommandPublisher)
        {
            InitializeComponent();

            this.nextSlideCommandPublisher = nextSlideCommandPublisher;
            this.togglePauseCommandPublisher = togglePauseSlideShowCommandPublisher;
            this.previousSlideCommandPublisher = previousSlideCommandPublisher;

            this.logoSpin.Begin();
        }

        #region Implementation of IShellView

        public void ShowView()
        {
            Application.Current.RootVisual = this;
        }

        public void SetTitle(string title)
        {
            this.slideTitle.Text = title;
        }


        public void  SetInfoString(string info)
        {
            slideInformation.Text = info;
        }

        #endregion

        private void UserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    nextSlideCommandPublisher.Notify();
                    break;

                case Key.Left:
                    previousSlideCommandPublisher.Notify();
                    break;

                case Key.Space:
                    togglePauseCommandPublisher.Notify();
                    break;
            }
        }
    }
}