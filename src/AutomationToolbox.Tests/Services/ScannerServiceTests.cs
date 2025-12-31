using System.Net;
using AutomationToolbox.Core.Interfaces;
using AutomationToolbox.Core.Models;
using AutomationToolbox.Server.Services;
using FluentAssertions;
using Xunit;

namespace AutomationToolbox.Tests.Services
{
    public class MockNetworkProbe : INetworkProbe
    {
        public HashSet<string> UpIps { get; set; } = new();
        public Dictionary<string, List<int>> OpenPorts { get; set; } = new();
        public int PingCallCount { get; private set; }
        public int TcpCallCount { get; private set; }

        public Task<string> GetHostNameAsync(string ip) => Task.FromResult($"host-{ip}");

        public string GetMacAddress(IPAddress ip, IPAddress sourceIp) => "00:11:22:33:44:55";

        public async Task<bool> PingAsync(string ip, int timeoutMs, CancellationToken ct)
        {
            await Task.Delay(1); // Simulate work
            PingCallCount++;
            return UpIps.Contains(ip);
        }

        public async Task<bool> TcpConnectAsync(string ip, int port, int timeoutMs, CancellationToken ct)
        {
             await Task.Delay(1);
             TcpCallCount++;
             if (OpenPorts.TryGetValue(ip, out var ports))
             {
                 return ports.Contains(port);
             }
             return false;
        }
    }

    public class ScannerServiceTests
    {
        [Fact]
        public async Task ScanSubnetAsync_ReturnsOnlyUpIps_WhenIncludeDownFalse()
        {
            // Arrange
            var mockProbe = new MockNetworkProbe();
            mockProbe.UpIps.Add("192.168.1.5");
            mockProbe.UpIps.Add("192.168.1.10");

            var service = new ScannerService(mockProbe);
            
            // Act
            // Range scan: 1-20
            var result = await service.ScanSubnetAsync("127.0.0.1", "192.168.1.1-20");

            // Assert
            result.Should().HaveCount(2);
            result.Select(r => r.IpAddress).Should().Contain(new[] { "192.168.1.5", "192.168.1.10" });
            result.All(r => r.IsUp).Should().BeTrue();
        }

        [Fact]
        public async Task ScanSubnetAsync_ReturnsAllIps_WhenIncludeDownTrue()
        {
             // Arrange
            var mockProbe = new MockNetworkProbe();
            mockProbe.UpIps.Add("192.168.1.5");

            var service = new ScannerService(mockProbe);
            
            // Act
            var result = await service.ScanSubnetAsync("127.0.0.1", "192.168.1.1-5", includeDown: true);

            // Assert
            // 1,2,3,4,5 = 5 hosts. Only .5 is UP.
            result.Should().HaveCount(5);
            var upHost = result.FirstOrDefault(r => r.IsUp);
            upHost.Should().NotBeNull();
            upHost!.IpAddress.Should().Be("192.168.1.5");
            
            result.Count(r => !r.IsUp).Should().Be(4);
        }

        [Fact]
        public async Task ScanPortsAsync_FindsOpenPorts()
        {
             // Arrange
            var mockProbe = new MockNetworkProbe();
            mockProbe.OpenPorts.Add("192.168.1.5", new List<int> { 80, 443 });

            var service = new ScannerService(mockProbe);

            // Act
            // Scan 80, 443, 8080
            var result = await service.ScanPortsAsync("192.168.1.5", "80, 443, 8080");

            // Assert
            result.OpenPorts.Should().HaveCount(2);
            result.OpenPorts.Should().Contain(new[] { 80, 443 });
            result.OpenPorts.Should().NotContain(8080);
        }
    }
}
