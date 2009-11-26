using System;
using System.Windows;
using System.Windows.Controls;

using APD.Client.Framework.Settings.Repository;


namespace APD.Admin.Silverlight
{
    public partial class Page : UserControl
    {
        private const string sound_setting_name = "CI_play_sound";
        private SettingsManager settingsManager;

        public Page()
        {
            InitializeComponent();

            var localSettingsRepository = new IsolatedStorageSettingsRepository();
            var serverSettingsRepository = new ServerSettingsRepository();

            settingsManager =
                new SettingsManager(localSettingsRepository, serverSettingsRepository);

            RegisterSettingsManagerEventHandlers();
        }

        private void RegisterSettingsManagerEventHandlers()
        {
            settingsManager.LoadSettingsCompleted += new EventHandler(LoadSettingsCompleted);
            settingsManager.SaveSettingsCompleted += new EventHandler(SaveSettingsCompleted);

            settingsManager.LoadingSettings += new EventHandler(LoadingSettings);
            settingsManager.SavingSettings += new EventHandler(SavingSettings);
        }

        void SaveSettingsCompleted(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                SetGuiEnabled(true);
                lblMessage.Text = "Save completed";
            });
        }

        void LoadSettingsCompleted(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                SetGuiEnabled(true);
                InitSettings();
                lblMessage.Text = "Got settings:";
            });
        }

        void LoadingSettings(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                SetGuiEnabled(false);
                lblMessage.Text = "Loading settings...";
            });
        }

        void SavingSettings(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                SetGuiEnabled(false);
                lblMessage.Text = "Saving...";
            });
        }

        private void InitSettings()
        {
            object value = settingsManager.GetSetting(sound_setting_name);
            if (value == null)
                return;

            bool bValue = (bool) value;
            chkCIPlaySound.IsChecked = bValue;
        }

        private void SetGuiEnabled(bool state)
        {
            Dispatcher.BeginInvoke(() =>
            {
                btnSave.IsEnabled = state;
                radioClientSettings.IsEnabled = state;
                radioAdminSettings.IsEnabled = state;
                chkCIPlaySound.IsEnabled = state;
            });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            settingsManager.SaveCurrentSettingsAsync();
        }

        private void chkCIPlaySound_Click(object sender, RoutedEventArgs e)
        {
            settingsManager.SetSetting(sound_setting_name, chkCIPlaySound.IsChecked);
        }

        private void radio_modeChanged(object sender, RoutedEventArgs e)
        {
                    
            if( this.radioAdminSettings.IsChecked.Value == true )
            {
                btnSave.Content = "Save settings to server";
                settingsManager.ChangeToAdminMode();                
            }
            else
            {
                btnSave.Content = "Save settings to my computer";
                settingsManager.ChangeToUserMode();
            }
        }

    }
}
