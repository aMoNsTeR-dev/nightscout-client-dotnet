using NightscoutClientDotNet;
using NightscoutClientDotNet.Interfaces;
using NightscoutClientDotNet.Models;
using NightscoutTrayWatcher.Models;
using NightscoutTrayWatcher.Utils;
using System;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows.Input;

namespace NightscoutTrayWatcher.ViewModels
{
    internal class TrayIconViewModel : INotifyPropertyChanged
    {
        #region members

        private const string _NIGHTSCOUT_BASE_WEB_ADDRESS = "https://androck.k.vu";
        private const string _NIGHTSCOUT_TOKEN_IN_URL = "ohmyposh-0a6cfb7ef9550ed6";
        private const double _TIMER_INTERVAL = 60000;
        private readonly INightscoutRestApiClient _connector;
        private bool _webBrowserLoaded;
        private string _cgmMonitorWebAddress;
        public bool _webBrowserKeepOpen;
        private string _lastEntryString;
        private readonly Timer _nsTimer;

        #endregion // members


        #region events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion // events


        #region properties

        public string CgmMonitorWebAddress
        {
            get { return _cgmMonitorWebAddress; }
            private set
            {
                _cgmMonitorWebAddress = value;
                OnPropertyChanged("CgmMonitorWebAddress");
            }
        }

        public bool WebBrowserLoaded
        {
            get { return _webBrowserLoaded; }
            set
            {
                _webBrowserLoaded = value;

                if (_webBrowserLoaded)
                {
                    CgmMonitorWebAddress = _NIGHTSCOUT_BASE_WEB_ADDRESS;
                }
            }
        }

        public bool WebBrowserKeepOpen
        {
            get { return _webBrowserKeepOpen; }
            set
            {
                if (_webBrowserKeepOpen != value)
                {
                    _webBrowserKeepOpen = value;
                    OnPropertyChanged("WebBrowserKeepOpen");
                }
            }
        }

        public string LastEntryString
        {
            get { return _lastEntryString; }
            private set
            {
                _lastEntryString = value;
                OnPropertyChanged("LastEntryString");
            }
        }

        #endregion // properties


        #region commands

        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => System.Windows.Application.Current.Shutdown() };
            }
        }

        #endregion // commands


        #region ctors

        public TrayIconViewModel()
        {
            Settings settings = App.GetSettingsService()?.Settings;

            if (!string.IsNullOrEmpty(settings?.NightscoutUrl))
            {
                _connector = new NightscoutRestApiV1Client(settings.NightscoutUrl, settings.NightscoutAuthenticationType, settings.NightscoutApiKey);
            }

            _webBrowserLoaded = false;
            _cgmMonitorWebAddress = string.Empty;
            _webBrowserKeepOpen = true;

            NsTimer_Elapsed(null, null);

            _nsTimer = new Timer();
            _nsTimer.Enabled = true;
            _nsTimer.Interval = settings.PollingTimerInterval;
            _nsTimer.Elapsed += NsTimer_Elapsed;
            _nsTimer.Start();
        }

        #endregion // ctors


        #region public functions
        #endregion // public functions


        #region private functions

        private async void NsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_connector == null)
            {
                return;
            }

            Entry entry = (await _connector.GetEntriesAsync("", 1))?.FirstOrDefault();
            if (entry != null && entry.Type.Equals("sgv"))
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    System.Drawing.Icon trayIcon = Utils.NightscoutUtils.GetIconFromEntry(entry);
                    if (trayIcon != null)
                    {
                        App.GetNotifyIcon().Icon = trayIcon;
                        LastEntryString = entry.ToString();
                    }
                }));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion // private functions
    }
}
