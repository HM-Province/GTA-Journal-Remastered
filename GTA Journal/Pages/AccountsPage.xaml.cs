using GTA_Journal.Database;
using GTA_Journal.Repositories;
using GTA_Journal.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GTA_Journal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountsPage : Page
    {
        private class NewAccountInfo()
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public bool IsSavePassword { get; set; }
        }

        private NewAccountInfo _newAccountInfo = new NewAccountInfo 
        {
            Login = "",
            Password = "",
            IsSavePassword = false
        };

        public AccountsPage()
        {
            this.InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            var users = DataAccess.GetUsers();
            var viewModel = (App.Current as App).GlobalState;
            var settings = SettingsRepository.GetSettings();

            LoadingPanel.Visibility = Visibility.Collapsed;

            if (users.Count == 0)
            {
                NoAccountsPanel.Visibility = Visibility.Visible;
                UserListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoAccountsPanel.Visibility = Visibility.Collapsed;
                UserListView.Visibility = Visibility.Visible;
                UserListView.ItemsSource = users;

                var user = UserListView.Items.Where(user => (user as User).Id == settings.CurrentUserId).FirstOrDefault();
                if (user != null)
                    UserListView.SelectedItem = user;
            }
        }

        private void SetLoading()
        {
            LoadingPanel.Visibility = Visibility.Visible;
            NoAccountsPanel.Visibility = Visibility.Collapsed;
            UserListView.Visibility = Visibility.Collapsed;
        }

        private async void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await AddAccountDialog.ShowAsync();
            SetLoading();

            if (result == ContentDialogResult.Primary)
            {
                var credentials = await JournalRepository.GetUserCredentials(LoginTextBox.Text, PasswordTextBox.Password);
                if (credentials == null)
                {
                    AddAccountButton_Click(sender, e);
                    return;
                }

                var info = await JournalRepository.GetMainPageInfo(credentials.UserId, credentials.UsId);
                if (info == null)
                {
                    AddAccountButton_Click(sender, e);
                    return;
                }

                DataAccess.AddUser(
                    credentials.UserId,
                    credentials.UsId,
                    info.CurrentUser.Username,
                    SavePasswordCheckBox.IsChecked == true ? PasswordTextBox.Password : "",
                    info.CurrentUser.AvatarUrl,
                    info.CurrentUser.IsAdmin,
                    credentials.Expires ?? DateTime.Now
                );
            }

            LoadData();
        }

        private void UserListView_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            var settings = SettingsRepository.GetSettings();
            var viewModel = (App.Current as App).GlobalState;

            var selectedUser = UserListView.SelectedItem as User;
            if (selectedUser != null)
            {
                settings.CurrentUserId = selectedUser.Id;
                SettingsRepository.SaveSettingsChanges();
            }
        }

        private async void UserListViewDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var result = await DeleteAccountDialog.ShowAsync();
            SetLoading();

            if (result == ContentDialogResult.Primary)
            {
                var button = sender as Button;
                var user = button.Tag as User;
                if (user != null)
                {
                    DataAccess.DeleteUser(user.Id);
                }
            }

            LoadData();
        }

        private void AddAccountFields_TextChanged(object sender, object e)
        {
            if (LoginTextBox.Text.Trim().Length > 0 && PasswordTextBox.Password.Trim().Length > 0)
            {
                AddAccountDialog.IsPrimaryButtonEnabled = true;
            }
            else
            {
                AddAccountDialog.IsPrimaryButtonEnabled = false;
            }
        }
    }
}
