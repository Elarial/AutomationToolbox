using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using AutomationToolbox.Core.Interfaces;

namespace AutomationToolbox.Server.Services
{
    public class RealNetworkProbe : INetworkProbe
    {
        // P/Invoke for SendARP to get MAC address
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref int PhyAddrLen);

        public async Task<string> GetHostNameAsync(string ip)
        {
            try 
            {
                var entry = await Dns.GetHostEntryAsync(ip);
                return entry.HostName;
            } 
            catch
            {
                return "";
            }
        }

        public string GetMacAddress(IPAddress ipAddr, IPAddress sourceIp)
        {
             // Handle all loopback addresses (127.x.x.x)
             if (IPAddress.IsLoopback(ipAddr)) return "00:00:00:00:00:00";

            byte[] macAddr = new byte[6];
            int macAddrLen = macAddr.Length;
            // Note: BitConverter use assumes LittleEndian (Windows/Intel are), but SendARP is Windows only.
            // On Windows, IP address needs to be integer.
            // Obsolete 'Address' property is easy, but let's stick to GetAddressBytes which is portable-ish,
            // though SendARP expects specific int format.
            // Reusing existing logic from ScannerService.
#pragma warning disable CS0618
            int destIpInt = BitConverter.ToInt32(ipAddr.GetAddressBytes(), 0);
            int srcIpInt = BitConverter.ToInt32(sourceIp.GetAddressBytes(), 0);
#pragma warning restore CS0618

            if (SendARP(destIpInt, srcIpInt, macAddr, ref macAddrLen) == 0)
            {
                return string.Join(":", macAddr.Take(macAddrLen).Select(x => x.ToString("X2")));
            }
            return "";
        }

        public async Task<bool> PingAsync(string ip, int timeoutMs, CancellationToken ct)
        {
             try 
            {
                if (ct.IsCancellationRequested) return false;
                using var pinger = new Ping();
                // We assume timeoutMs usage for simplicity, matching original logic
                var reply = await pinger.SendPingAsync(ip, timeoutMs);
                return reply.Status == IPStatus.Success;
            }
            catch 
            {
                return false;
            }
        }

        public async Task<bool> TcpConnectAsync(string ip, int port, int timeoutMs, CancellationToken ct)
        {
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(ip, port);
                var timeoutTask = Task.Delay(timeoutMs, ct);
                
                var completed = await Task.WhenAny(connectTask, timeoutTask);
                if (completed == connectTask && client.Connected)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
