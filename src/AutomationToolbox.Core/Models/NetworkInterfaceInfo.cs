namespace AutomationToolbox.Core.Models
{
    /// <summary>
    /// Represents information about a network interface.
    /// </summary>
    public class NetworkInterfaceInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string NetMask { get; set; } = string.Empty;
        public string Gateway { get; set; } = string.Empty;
    }
}
