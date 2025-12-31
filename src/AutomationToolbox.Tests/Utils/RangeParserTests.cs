using AutomationToolbox.Core.Utils;
using FluentAssertions;
using Xunit;

namespace AutomationToolbox.Tests.Utils
{
    public class RangeParserTests
    {
        [Fact]
        public void ParseIpRange_SingleIp_ReturnsOneIp()
        {
            var result = RangeParser.ParseIpRange("192.168.1.1");
            result.Should().HaveCount(1);
            result.First().Should().Be("192.168.1.1");
        }

        [Fact]
        public void ParseIpRange_RangeWithSuffix_ReturnsCorrectIps()
        {
            var result = RangeParser.ParseIpRange("192.168.1.10-12");
            result.Should().HaveCount(3);
            result.Should().ContainInOrder("192.168.1.10", "192.168.1.11", "192.168.1.12");
        }

        [Fact]
        public void ParseIpRange_RangeWithFullIp_ReturnsCorrectIps()
        {
            var result = RangeParser.ParseIpRange("192.168.1.10-192.168.1.12");
            result.Should().HaveCount(3);
            result.Should().ContainInOrder("192.168.1.10", "192.168.1.11", "192.168.1.12");
        }

        [Fact]
        public void ParseIpRange_InvalidRange_ReturnsEmpty()
        {
            var result = RangeParser.ParseIpRange("invalid");
            result.Should().BeEmpty();
        }

        [Fact]
        public void ParseIpRange_ReverseRange_ReturnsEmpty()
        {
            // 20-10 should be invalid or empty
            var result = RangeParser.ParseIpRange("192.168.1.20-10");
            result.Should().BeEmpty();
        }

        [Fact]
        public void ParsePortRange_SinglePort_ReturnsOnePort()
        {
            var result = RangeParser.ParsePortRange("80");
            result.Should().HaveCount(1);
            result.First().Should().Be(80);
        }

        [Fact]
        public void ParsePortRange_CommaSeparated_ReturnsPorts()
        {
            var result = RangeParser.ParsePortRange("80, 443, 8080");
            result.Should().HaveCount(3);
            result.Should().Contain(new[] { 80, 443, 8080 });
        }

        [Fact]
        public void ParsePortRange_Range_ReturnsPorts()
        {
            var result = RangeParser.ParsePortRange("500-505");
            result.Should().HaveCount(6); // 500, 501, 502, 503, 504, 505
            result.Should().Contain(new[] { 500, 501, 502, 503, 504, 505 });
        }

        [Fact]
        public void ParsePortRange_Mixed_ReturnsAll()
        {
            var result = RangeParser.ParsePortRange("80, 500-502");
            result.Should().HaveCount(4); // 80, 500, 501, 502
            result.Should().Contain(new[] { 80, 500, 501, 502 });
        }

        [Fact]
        public void ParsePortRange_InvalidRange_ReturnsEmpty()
        {
            var result = RangeParser.ParsePortRange("abc-def");
            result.Should().BeEmpty();
        }
        
         [Fact]
        public void ParsePortRange_Distinct_ReturnsUnique()
        {
            var result = RangeParser.ParsePortRange("80, 80, 80-82");
            result.Should().HaveCount(3); // 80, 81, 82
            result.Should().Contain(new[] { 80, 81, 82 });
        }
    }
}
