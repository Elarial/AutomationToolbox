using System.Net;

namespace AutomationToolbox.Core.Interfaces
{
    public interface INetworkProbe
    {
        Task<bool> PingAsync(string ip, int timeoutMs, CancellationToken ct);
        Task<bool> TcpConnectAsync(string ip, int port, int timeoutMs, CancellationToken ct);
        string GetMacAddress(IPAddress ip, IPAddress sourceIp);
        Task<string> GetHostNameAsync(string ip);
    }
}
