using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationToolbox.Core.Models;

namespace AutomationToolbox.Core.Interfaces
{
    /// <summary>
    /// Defines operations for network scanning.
    /// </summary>
    public interface IScannerService
    {
        /// <summary>
        /// Retrieves all available network interfaces on the host.
        /// </summary>
        Task<IEnumerable<NetworkInterfaceInfo>> GetInterfacesAsync();

        /// <summary>
        /// Scans a subnet for active devices.
        /// </summary>
        /// <param name="interfaceIp">The IP of the interface to use (determines the subnet).</param>
        /// <param name="ipRange">Optional custom IP range (e.g. "192.168.1.10-20"). If null, scans the full subnet.</param>
        Task<IEnumerable<ScanResult>> ScanSubnetAsync(string interfaceIp, string? ipRange = null, bool includeDown = false, int timeoutMs = 500, CancellationToken ct = default);

        /// <summary>
        /// Scans specific ports on a target IP.
        /// </summary>
        /// <param name="ip">The target IP address.</param>
        /// <param name="portRange">Optional custom port range (e.g. "80,443,500-600"). If null, scans default ports.</param>
        Task<PortResult> ScanPortsAsync(string ip, string? portRange = null, int timeoutMs = 500, CancellationToken ct = default);
    }
}
