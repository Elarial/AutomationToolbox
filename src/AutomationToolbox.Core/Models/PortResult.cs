using System.Collections.Generic;

namespace AutomationToolbox.Core.Models
{
    /// <summary>
    /// Represents the result of a port scan on a specific host.
    /// </summary>
    public class PortResult
    {
        public string IpAddress { get; set; } = string.Empty;
        public List<int> OpenPorts { get; set; } = new List<int>();
    }
}
