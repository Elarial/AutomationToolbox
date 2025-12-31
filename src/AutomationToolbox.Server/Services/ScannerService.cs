using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AutomationToolbox.Core.Interfaces;
using AutomationToolbox.Core.Models;

namespace AutomationToolbox.Server.Services
{
    public class ScannerService : IScannerService
    {
        private readonly INetworkProbe _probe;
        private static readonly int[] DefaultCommonPorts = new[] { 21, 22, 23, 25, 53, 80, 110, 135, 139, 143, 443, 445, 3389 };
        private static readonly int[] DefaultIndustrialPorts = new[] { 102, 502, 1080, 2404, 4000, 9600, 19132, 20000, 44818, 47808 };

        public ScannerService(INetworkProbe probe)
        {
            _probe = probe;
        }

        public Task<IEnumerable<NetworkInterfaceInfo>> GetInterfacesAsync()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback && ni.OperationalStatus == OperationalStatus.Up)
                .Select(ni => 
                {
                    var ipProps = ni.GetIPProperties();
                    var unicast = ipProps.UnicastAddresses.FirstOrDefault(ua => ua.Address.AddressFamily == AddressFamily.InterNetwork);
                    var gateway = ipProps.GatewayAddresses.FirstOrDefault()?.Address.ToString() ?? "";

                    if (unicast == null) return null;

                    return new NetworkInterfaceInfo
                    {
                        Id = ni.Id,
                        Name = ni.Name,
                        Description = ni.Description,
                        IpAddress = unicast.Address.ToString(),
                        NetMask = unicast.IPv4Mask.ToString(),
                        Gateway = gateway
                    };
                })
                .Where(ni => ni != null)
                .ToList();
            
            // Add Loopback manually for testing
            interfaces.Add(new NetworkInterfaceInfo
            {
                Id = "Loopback",
                Name = "Loopback",
                Description = "Localhost",
                IpAddress = "127.0.0.1",
                NetMask = "255.0.0.0",
                Gateway = ""
            });

            return Task.FromResult((IEnumerable<NetworkInterfaceInfo>)interfaces);
        }

        public async Task<IEnumerable<ScanResult>> ScanSubnetAsync(string interfaceIp, string? ipRange = null, bool includeDown = false, int timeoutMs = 500, CancellationToken ct = default)
        {
            var ipsToScanList = (ipRange != null 
                ? AutomationToolbox.Core.Utils.RangeParser.ParseIpRange(ipRange) 
                : GetSubnetIps(interfaceIp)).ToList();

            // Security: Limit scan size
            if (ipsToScanList.Count > 512) throw new ArgumentException("Scan range too large (max 512 hosts).");

            var results = new System.Collections.Concurrent.ConcurrentBag<ScanResult>();

            // Parallel Ping with Concurrency Limit
            var options = new ParallelOptions 
            { 
                MaxDegreeOfParallelism = 50, 
                CancellationToken = ct 
            };

            await Parallel.ForEachAsync(ipsToScanList, options, async (ip, token) =>
            {
                var result = await ScanSingleIpAsync(ip, interfaceIp, timeoutMs, token);
                if (result != null) results.Add(result);
            });

            return results.Where(r => r != null && (r.IsUp || includeDown));
        }

        public async Task<PortResult> ScanPortsAsync(string ip, string? portRange = null, int timeoutMs = 500, CancellationToken ct = default)
        {
            var ports = portRange != null ? AutomationToolbox.Core.Utils.RangeParser.ParsePortRange(portRange) : DefaultCommonPorts.Concat(DefaultIndustrialPorts).Distinct();
            
            // Security: limit port count
            var portList = ports.ToList();
            if (portList.Count > 1000) throw new ArgumentException("Port range too large (max 1000 ports).");

            var openPorts = new System.Collections.Concurrent.ConcurrentBag<int>();

            // Parallel Connect with Concurrency Limit
            var options = new ParallelOptions 
            { 
                MaxDegreeOfParallelism = 50, 
                CancellationToken = ct 
            };

            await Parallel.ForEachAsync(portList, options, async (port, token) =>
            {
                if (await _probe.TcpConnectAsync(ip, port, timeoutMs, token))
                {
                    openPorts.Add(port);
                }
            });

            return new PortResult { IpAddress = ip, OpenPorts = openPorts.OrderBy(p => p).ToList() };
        }

        private async Task<ScanResult> ScanSingleIpAsync(string ip, string sourceIp, int timeoutMs, CancellationToken ct)
        {
            if (await _probe.PingAsync(ip, timeoutMs, ct))
            {
                var mac = _probe.GetMacAddress(IPAddress.Parse(ip), IPAddress.Parse(sourceIp));
                var hostname = await _probe.GetHostNameAsync(ip);

                return new ScanResult
                {
                    IpAddress = ip,
                    Hostname = hostname,
                    MacAddress = mac,
                    IsUp = true
                };
            }
            
            // Return down result
            return new ScanResult
            {
                IpAddress = ip,
                IsUp = false
            };
        }



        private IEnumerable<string> GetSubnetIps(string interfaceIp)
        {
            // Simple /24 assumption for MVP based on interfaceIp. 
            // Real implementation should calculate from Mask.
            // Assuming "192.168.1.xxx" class C for now.
             var parts = interfaceIp.Split('.');
             if (parts.Length != 4) return new List<string>();
             
             var prefix = $"{parts[0]}.{parts[1]}.{parts[2]}.";
             var ips = new List<string>();
             for (int i = 1; i < 255; i++)
             {
                 if (prefix + i == interfaceIp) continue; // Skip self? No, scan self too.
                 ips.Add(prefix + i);
             }
             return ips;
        }


    }
}
