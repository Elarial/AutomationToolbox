using System.Collections.Generic;

namespace AutomationToolbox.Core.Models
{
    /// <summary>
    /// Represents the result of an IP scan for a single host.
    /// </summary>
    public class ScanResult
    {
        public string IpAddress { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public bool IsUp { get; set; }
        public List<int> OpenPorts { get; set; } = new List<int>();
    }
}
