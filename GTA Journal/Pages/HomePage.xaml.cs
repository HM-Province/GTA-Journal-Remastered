using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Threading;
using GTA_Journal.Utils;
using System.Security.Principal;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GTA_Journal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private bool IsBusy = false;

        public HomePage()
        {
            this.InitializeComponent();

            var viewModel = (App.Current as App).GlobalState;

            if (!IsRunningAsAdministrator())
            {
                infoText.Text = "Запусти от имени администратора!";
                onlineButton.IsEnabled = false;
                afkButton.IsEnabled = false;
                offlineButton.IsEnabled = false;
            }
            Status.Text = $"{viewModel.CurrentUserStatus.ToString()}";

            viewModel.PropertyChanged += OnStatusChanged;
        }

        private void OnStatusChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentUserStatus")
            {
                Status.Text = (App.Current as App).GlobalState.CurrentUserStatus.ToString();
            }
        }

        private static bool IsRunningAsAdministrator() { using (WindowsIdentity identity = WindowsIdentity.GetCurrent()) { WindowsPrincipal principal = new WindowsPrincipal(identity); return principal.IsInRole(WindowsBuiltInRole.Administrator); } }

        private void ChangeStatus_OnClick(object sender, RoutedEventArgs e)
        {
            if ((Button)e.OriginalSource == onlineButton) {
                (App.Current as App).GlobalState.CurrentUserStatus = Models.GlobalStateModel.UserStatus.Online;
            } else if ((Button)e.OriginalSource == afkButton) {
                (App.Current as App).GlobalState.CurrentUserStatus = Models.GlobalStateModel.UserStatus.AFK;
            } else {
                (App.Current as App).GlobalState.CurrentUserStatus = Models.GlobalStateModel.UserStatus.Offline;
            }

            Thread.Sleep(10000);
            ushort[] codes = { 0xC0, 0x53, 0x41, 0x59, 0x20, 0x47, 0x41, 0x59, 0x0D };

            foreach (var code in codes)
            {
                KeyboardSimulator.PressKey(code, new Random().Next(30, 120));
            }
        }
    }
}