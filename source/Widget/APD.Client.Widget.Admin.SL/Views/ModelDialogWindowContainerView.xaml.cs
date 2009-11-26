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

using APD.Client.Widget.Admin.ViewModels;
using APD.Client.Framework.SL;


namespace APD.Client.Widget.Admin.SL
{
    public partial class ModalDialogWindowContainerView : ChildWindow
    {
        public ModalDialogWindowContainerView()
        {
            DataContext = new ModalDialogViewModel(new SilverlightUIInvoker(Dispatcher), null);
            InitializeComponent();
        }

        public ModalDialogWindowContainerView(ModalDialogViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

