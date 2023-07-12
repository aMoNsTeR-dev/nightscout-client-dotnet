using NightscoutClientDotNet.Models.Enums;

namespace NightscoutTrayWatcher.Models
{
    internal class Settings
    {
        public string NightscoutUrl { get; set; }
        public ApiKeyType NightscoutAuthenticationType { get; set; }
        public string NightscoutApiKey { get; set; }
        public double PollingTimerInterval { get; set; }

        public Settings() 
        { 
            NightscoutUrl = string.Empty;
            NightscoutAuthenticationType = ApiKeyType.None;
            NightscoutApiKey = string.Empty;
            PollingTimerInterval = 60000;
        }
    }
}
