using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace NightscoutClientDotNet.Models
{
    public class Entry
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("device")]
        public string Device { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonIgnore]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("dateString")]
        public string DateString
        {
            get { return this.DateTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz"); }
            set { this.DateTime = DateTime.Parse(value); }
        }

        [JsonPropertyName("date")]
        public ulong Epoch { get; set; }

        [JsonPropertyName("sgv")]
        public double Sgv { get; set; }

        [JsonPropertyName("delta")]
        public double Delta { get; set; }

        [JsonPropertyName("direction")]
        public string Direction { get; set; }

        [JsonPropertyName("filtered")]
        public double Filtered { get; set; }

        [JsonPropertyName("unfiltered")]
        public double Unfiltered { get; set; }

        [JsonPropertyName("rssi")]
        public int Rssi { get; set; }

        [JsonPropertyName("noise")]
        public double Noise { get; set; }

        [JsonIgnore]
        public DateTime SystemTime { get; set; }

        [JsonPropertyName("sysTime")]
        public string SysTime
        {
            get { return this.SystemTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz"); }
            set { this.SystemTime = DateTime.Parse(value); }
        }

        public string Arrow
        {
            get
            {
                string arrow = "";

                switch (Direction)
                {
                    case "Flat":
                        arrow = "→";
                        break;
                    case "FortyFiveUp":
                        arrow = "↗";
                        break;
                    case "FortyFiveDown":
                        arrow = "↘";
                        break;
                    case "SingleUp":
                        arrow = "↑";
                        break;
                    case "SingleDown":
                        arrow = "↓";
                        break;
                    case "DoubleUp":
                        arrow = "⇈";
                        break;
                    case "DoubleDown":
                        arrow = "⇊";
                        break;
                    default:
                        arrow = "";
                        break;
                }

                return arrow;
            }
        }


        public Entry()
        {
            Id = string.Empty;
            Device = string.Empty;
            Type = string.Empty;
            Direction = string.Empty;
        }


        public override string ToString()
        {
            return String.Format("{0} {1} ({2} mg/dl, {3} min. ago)", Sgv.ToString(CultureInfo.InvariantCulture), Arrow, System.Math.Round(Delta, 1), (System.DateTime.Now - DateTime).Minutes.ToString(CultureInfo.InvariantCulture));
        }
    }
}
