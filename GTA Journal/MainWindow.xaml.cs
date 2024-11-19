using GTA_Journal.Models;
using GTA_Journal.Repositories;
using Microsoft.UI.Windowing;
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
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GTA_Journal
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            (App.Current as App).GlobalState.PropertyChanged += OnGlobalStateChanged;
            (App.Current as App).GlobalState.UseMicaTheme = SettingsRepository.GetSettings().UseMicaStyle;

            rootFrame.Navigate(typeof(MainPage), rootFrame);
        }

        private void OnGlobalStateChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UseMicaTheme")
            {
                var modelValue = (App.Current as App).GlobalState;

                if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported() && modelValue.UseMicaTheme)
                {
                    this.SystemBackdrop = new MicaBackdrop()
                    {
                        Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base
                    };
                } 
                else
                {
                    this.SystemBackdrop = null;
                }
            }
        }
    }
}
