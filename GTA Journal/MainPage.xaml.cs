using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using GTA_Journal.Pages;
using GTA_Journal.Utils;
using System.Speech.Synthesis;
using Serilog;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GTA_Journal
{
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _timer;
        private Frame _rootFrame = null;

        public MainPage()
        {
            this.InitializeComponent();

            contentFrame.Navigate(typeof(HomePage));

            StartTimer();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null && e.Parameter is Frame frame)
            {
                _rootFrame = frame;
            }
        }

        private void navigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
            var selectedItem = (NavigationViewItem)args.SelectedItem;

            if (selectedItem != null) {
                string pageName = $"GTA_Journal.Pages.{selectedItem.Tag}";
                Type pageType = Type.GetType(pageName);
                contentFrame.Navigate(pageType);

                Log.Information($"Navigated to {pageName}");
            }
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            // myButton.Content = "Clicked";

            var toast = new AppNotificationBuilder()
                .AddText("Hui")
                .BuildNotification();

            AppNotificationManager.Default.Show(toast);
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(10);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            var processes = Process.GetProcessesByName("notepad");

            if (processes.Length == 0)
            {
                (App.Current as App).GlobalState.CurrentUserStatus = Models.GlobalStateModel.UserStatus.Offline;
            }
        }
    }
}