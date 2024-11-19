using Serilog;
using System;
using System.IO;
using System.Text.Json;

namespace GTA_Journal.Repositories
{
    public class ApplicationSettings
    {
        public int CurrentUserId { get; set; }
        public bool UseMicaStyle { get; set; }
        public int WatchCheckInterval { get; set; }
        public string WatchProcessName { get; set; }
    }

    public static class SettingsRepository
    {
        private static string _settingsPath {
            get
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = Environment.GetFolderPath(folder);
                Directory.CreateDirectory(Path.Join(path, "GTA Journal"));
                return Path.Join(path, "GTA Journal/settings.json");
            }
        }

        private static ApplicationSettings _cachedSettings = null;

        public static void InitializeRepository() {
            if (!File.Exists(_settingsPath)) {
                CreateSettings();
            }

            try
            {
                string jsonString = File.ReadAllText(_settingsPath);

                _cachedSettings = JsonSerializer.Deserialize<ApplicationSettings>(jsonString);

                Log.Information("Settings loaded");
            } catch (Exception ex) {
                Log.Error(ex, "Failed to parse settings, creating new");
                CreateSettings();
            }
        }

        public static ApplicationSettings GetSettings()
        {
            return _cachedSettings;
        }

        public static void SaveSettingsChanges()
        {
            try
            {
                File.WriteAllText(_settingsPath, JsonSerializer.Serialize(_cachedSettings));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save settings");
            }
        }

        private static void CreateSettings() {
            try
            {
                var settings = new ApplicationSettings
                {
                    CurrentUserId = -1,
                    UseMicaStyle = false,
                    WatchCheckInterval = 5000,
                    WatchProcessName = "notepad",
                };

                File.WriteAllText(_settingsPath, JsonSerializer.Serialize(settings));
                _cachedSettings = settings;
            } catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to create settings, exiting");
                Environment.Exit(1);
            }

            Log.Information("Created new settings");
        }
    }
}
