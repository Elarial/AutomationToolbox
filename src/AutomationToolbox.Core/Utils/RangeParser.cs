using System.Net;

namespace AutomationToolbox.Core.Utils
{
    public static class RangeParser
    {
        public static IEnumerable<string> ParseIpRange(string range)
        {
             var ips = new List<string>();
             
             if (range.Contains("-"))
             {
                 var parts = range.Split('-');
                 if (parts.Length == 2)
                 {
                     if (IPAddress.TryParse(parts[0], out var startIp))
                     {
                         // Check if part 2 is just a number (e.g. 192.168.1.10-20)
                         if (int.TryParse(parts[1], out int endSuffix))
                         {
                             var bytes = startIp.GetAddressBytes();
                             // Simple validation: Ensure endSuffix is valid for last octet and >= start
                             if (endSuffix >= 0 && endSuffix <= 255 && bytes[3] <= endSuffix)
                             {
                                 for (int i = bytes[3]; i <= endSuffix; i++)
                                 {
                                     ips.Add($"{bytes[0]}.{bytes[1]}.{bytes[2]}.{i}");
                                 }
                             }
                         }
                         // Check if part 2 is a full IP (e.g. 192.168.1.10-192.168.1.20)
                         else if (IPAddress.TryParse(parts[1], out var endIp))
                         {
                             var b1 = startIp.GetAddressBytes();
                             var b2 = endIp.GetAddressBytes();
                             // MVP limitation: Only support range if first 3 octets match
                             if (b1[0] == b2[0] && b1[1] == b2[1] && b1[2] == b2[2] && b1[3] <= b2[3])
                             {
                                 for (int i = b1[3]; i <= b2[3]; i++)
                                 {
                                     ips.Add($"{b1[0]}.{b1[1]}.{b1[2]}.{i}");
                                 }
                             }
                         }
                     }
                 }
             }
             else
             {
                 // Single IP
                 if (IPAddress.TryParse(range, out _))
                 {
                     ips.Add(range);
                 }
             }
             return ips;
        }

        public static IEnumerable<int> ParsePortRange(string range)
        {
            var ports = new List<int>();
            if (string.IsNullOrWhiteSpace(range)) return ports;

            var parts = range.Split(',');
            foreach (var part in parts)
            {
                var p = part.Trim();
                if (p.Contains("-"))
                {
                    var rangeParts = p.Split('-');
                    if (rangeParts.Length == 2 && int.TryParse(rangeParts[0], out int start) && int.TryParse(rangeParts[1], out int end))
                    {
                        if (start <= end && start > 0 && end <= 65535)
                        {
                            for (int i = start; i <= end; i++) ports.Add(i);
                        }
                    }
                }
                else
                {
                    if (int.TryParse(p, out int port) && port > 0 && port <= 65535) ports.Add(port);
                }
            }
            return ports.Distinct();
        }
    }
}
