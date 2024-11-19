using GTA_Journal.Repositories;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI;
using Serilog;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GTA_Journal.Pages
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            // Пофиксить баг при навигации
            micaToggleSwitch.IsOn = SettingsRepository.GetSettings().UseMicaStyle;
        }

        private void MicaToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = sender as ToggleSwitch;
            var settings = SettingsRepository.GetSettings();

            var modelValue = (App.Current as App).GlobalState;

            if (toggleSwitch != null)
            {
                Log.Information($"Mica backdrop changed: {toggleSwitch.IsOn}");

                if (toggleSwitch.IsOn)
                {
                    settings.UseMicaStyle = true;
                    modelValue.UseMicaTheme = true;
                }
                else
                {
                    settings.UseMicaStyle = false;
                    modelValue.UseMicaTheme = false;
                }

                SettingsRepository.SaveSettingsChanges();
            }
        }
    }
}
