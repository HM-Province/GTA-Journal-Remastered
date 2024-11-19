using GTA_Journal.Database;
using GTA_Journal.Models;
using GTA_Journal.Repositories;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GTA_Journal
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public GlobalStateModel GlobalState { get; } = new GlobalStateModel();

        public App()
        {
            this.InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            Directory.CreateDirectory(System.IO.Path.Join(path, "GTA Journal/Logs"));
            var logPath = System.IO.Path.Join(path, "GTA Journal/Logs");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(System.IO.Path.Join(logPath, $"log-{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")}.log"))
                .CreateLogger();

            SettingsRepository.InitializeRepository();
            DataAccess.InitializeDatabase();

            notificationManager = new NotificationManager();

            Log.Information("Application ready to render");
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();

            notificationManager.Init();

            var border = new Border {
                VerticalAlignment = VerticalAlignment.Top,
                Child = new TextBlock
                {
                    Text = "A",
                    VerticalAlignment = VerticalAlignment.Top
                }
            };

            m_window.ExtendsContentIntoTitleBar = true;
            m_window.SetTitleBar(border);

            Log.Information("Main window activated");
            m_window.Activate();
        }

        void OnProcessExit(object sender, EventArgs e)
        {
            Log.Information("App stopped");

            Log.CloseAndFlush();
            notificationManager.Unregister();
        }

        private Window m_window;
        private NotificationManager notificationManager;
    }
}
