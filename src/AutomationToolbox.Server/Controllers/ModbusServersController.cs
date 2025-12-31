using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomationToolbox.Core.Interfaces;
using AutomationToolbox.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutomationToolbox.Server.Controllers
{
    /// <summary>
    /// API Controller for managing Modbus TCP Server instances.
    /// </summary>
    [ApiController]
    [Route("api/modbus-servers")]
    public class ModbusServersController : ControllerBase
    {
        private readonly IModbusServerManager _serverManager;
        private readonly IScannerService _scannerService;

        /// <summary>
        /// Initializes a new instance of the controller.
        /// </summary>
        public ModbusServersController(IModbusServerManager serverManager, IScannerService scannerService)
        {
            _serverManager = serverManager;
            _scannerService = scannerService;
        }

        /// <summary>
        /// Gets all configured Modbus servers.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModbusServerConfig>>> GetAll()
        {
            return Ok(await _serverManager.GetAllServersAsync());
        }

        /// <summary>
        /// Starts a new Modbus server.
        /// </summary>
        /// <param name="config">The server configuration.</param>
        [HttpPost]
        public async Task<ActionResult<ModbusServerConfig>> StartServer([FromBody] ModbusServerConfig config)
        {
            try
            {
                var result = await _serverManager.StartServerAsync(config);
                return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> StopServer(Guid id)
        {
            await _serverManager.StopServerAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/data")]
        public async Task<IActionResult> UpdateData(Guid id, [FromBody] ModbusRegisterUpdate update)
        {
            await _serverManager.UpdateDataAsync(id, update.Type, update.Address, update.Value);
            return Ok();
        }

        [HttpGet("{id}/data")]
        public async Task<ActionResult<ushort[]>> GetData(Guid id, [FromQuery] ModbusDataType type, [FromQuery] int startAddress, [FromQuery] int count)
        {
             // Validate count mostly
             if (count <= 0 || count > 125) return BadRequest("Count must be between 1 and 125.");
             
             var data = await _serverManager.GetDataAsync(id, type, startAddress, count);
             return Ok(data);
        }

        [HttpGet("{id}/logs")]
        public async Task<ActionResult<IEnumerable<ModbusLogEntry>>> GetLogs(Guid id)
        {
            var logs = await _serverManager.GetLogsAsync(id);
            return Ok(logs);
        }
    }
    
    [ApiController]
    [Route("api/network-interfaces")]
    public class NetworkInterfacesController : ControllerBase
    {
        private readonly IScannerService _scannerService;

        public NetworkInterfacesController(IScannerService scannerService)
        {
            _scannerService = scannerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NetworkInterfaceInfo>>> GetInterfaces()
        {
            var interfaces = await _scannerService.GetInterfacesAsync();
            return Ok(interfaces);
        }
    }
}
