using Hardcodet.Wpf.TaskbarNotification;
using NightscoutTrayWatcher.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NightscoutTrayWatcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static TaskbarIcon _notifyIcon;
        private static SettingsService _settingsService;


        internal static TaskbarIcon GetNotifyIcon() 
        { 
            return _notifyIcon; 
        }

        internal static SettingsService GetSettingsService()
        {
            return _settingsService;
        }


        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _settingsService = new SettingsService();
            await _settingsService.LoadSettings();

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }
    }
}
