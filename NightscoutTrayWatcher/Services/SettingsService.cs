using NightscoutTrayWatcher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Net.NetworkInformation;

namespace NightscoutTrayWatcher.Services
{
    internal class SettingsService
    {
        private const string _SETTINGS_FILE_NAME = "appsettings.json";
        private string _settingsFilePath;
        private Settings _settings;


        public Settings Settings { get { return _settings; } }


        public SettingsService() 
        { 
            _settingsFilePath = Path.Combine(Utils.NightscoutUtils.GetBasePath(), _SETTINGS_FILE_NAME);
            
            if (!File.Exists(_settingsFilePath))
            {
                _settings = new Settings();
                _ = SaveSettings();
            }
        }


        public async Task LoadSettings()
        {
            using var stream = File.OpenRead(_settingsFilePath);
            try
            {
                _settings = await JsonSerializer.DeserializeAsync<Settings>(stream);
                await stream.DisposeAsync();
            }
            catch (Exception ex)
            {
                await stream.DisposeAsync();
                _settings = new Settings();
                await SaveSettings();
            }
        }

        public async Task SaveSettings()
        {
            using var stream = File.Create(_settingsFilePath);
            await JsonSerializer.SerializeAsync(stream, _settings, new JsonSerializerOptions() { WriteIndented = true });
            await stream.DisposeAsync();
        }
    }
}
