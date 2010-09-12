using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Smeedee.Widget.Admin.Tasks.ViewModels;

namespace Smeedee.Widget.Admin.Tasks.SL.Views
{
    
    public partial class TasksView : UserControl
    {
        private Dictionary<Key, Action> keyInputBindings = new Dictionary<Key, Action>();

        protected TasksViewModel ViewModel
	    {
            get { return DataContext as TasksViewModel; }
	    }

        public TasksView()
        {
            InitializeComponent();

            CreateInputBindings();

            KeyUp += TaskView_KeyUp;

            AddButton.Click += (o, e) => RunningTaskName.Focus();
        }

        private void CreateInputBindings()
        {
            keyInputBindings.Add(Key.Enter, () => ViewModel.ActivateSelectedTask.Execute(null));
            keyInputBindings.Add(Key.Delete, () => ViewModel.DeactivateSelectedTask.Execute(null));
        }

        void TaskView_KeyUp(object sender, KeyEventArgs e)
        {
            if (keyInputBindings.ContainsKey(e.Key))
            {
                keyInputBindings[e.Key].Invoke();
            }
        }
    }
}