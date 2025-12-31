using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationToolbox.Core.Interfaces;
using AutomationToolbox.Core.Models;

namespace AutomationToolbox.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScannerController : ControllerBase
    {
        private readonly IScannerService _scannerService;

        public ScannerController(IScannerService scannerService)
        {
            _scannerService = scannerService;
        }

        [HttpGet("interfaces")]
        public async Task<IEnumerable<NetworkInterfaceInfo>> GetInterfaces()
        {
            return await _scannerService.GetInterfacesAsync();
        }

        [HttpGet("subnet")]
        public async Task<IActionResult> ScanSubnet([FromQuery] string interfaceIp, [FromQuery] string? range = null, [FromQuery] bool includeDown = false, [FromQuery] int timeoutMs = 500, CancellationToken ct = default)
        {
            var results = await _scannerService.ScanSubnetAsync(interfaceIp, range, includeDown, timeoutMs, ct);
            return Ok(results);
        }

        [HttpGet("ports")]
        public async Task<IActionResult> ScanPorts([FromQuery] string ip, [FromQuery] string? range = null, [FromQuery] int timeoutMs = 500, CancellationToken ct = default)
        {
            var result = await _scannerService.ScanPortsAsync(ip, range, timeoutMs, ct);
            return Ok(result);
        }
    }
}
