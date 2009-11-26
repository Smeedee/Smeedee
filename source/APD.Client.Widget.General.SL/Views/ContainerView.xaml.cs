using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using APD.Client.Widget.General.ViewModels;
using System.ComponentModel;

namespace APD.Client.Widget.General.SL.Views
{
    public partial class ContainerView : UserControl
    {
        public ContainerView()
        {
            InitializeComponent();

            GotoDisplay();

            this.Loaded += new RoutedEventHandler(ContainerView_Loaded);
        }

        void ContainerView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoadingAnimationViewModel)
            {
                var vm = DataContext as LoadingAnimationViewModel;

                vm.PropertyChanged += new PropertyChangedEventHandler(vm_PropertyChanged);
            }
        }

        void vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = sender as LoadingAnimationViewModel;

            if (e.PropertyName.Equals("Display"))
            {
                if (vm.Display == true)
                    GotoDisplay();
                else if (vm.Display == false)
                    GotoHide();
            }
        }

        private void GotoDisplay()
        {
            VisualStateManager.GoToState(this, "Display", true);
        }

        private void GotoHide()
        {
            VisualStateManager.GoToState(this, "Hide", true);
        }
    }
}
