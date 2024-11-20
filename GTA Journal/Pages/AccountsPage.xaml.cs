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
        }

        private async void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await AddAccountDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var credentials = await JournalRepository.GetUserCredentials(LoginTextBox.Text, PasswordTextBox.Password);
                if (credentials == null)
                {
                    AddAccountButton_Click(sender, e);
                    return;
                }

                
            }
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
