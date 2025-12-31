using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationToolbox.Core.Models;

namespace AutomationToolbox.Core.Interfaces
{
    /// <summary>
    /// Manages the lifecycle and data of virtual Modbus TCP servers.
    /// </summary>
    public interface IModbusServerManager
    {
        /// <summary>
        /// Retrieves configuration and status for all managed servers.
        /// </summary>
        Task<IEnumerable<ModbusServerConfig>> GetAllServersAsync();

        /// <summary>
        /// Starts a new Modbus TCP server with the specified configuration.
        /// Validates that the port is not already in use.
        /// </summary>
        /// <param name="config">The server configuration.</param>
        /// <returns>The updated configuration with assigned ID and status.</returns>
        Task<ModbusServerConfig> StartServerAsync(ModbusServerConfig config);

        /// <summary>
        /// Stops and removes a server instance.
        /// </summary>
        /// <param name="serverId">The unique ID of the server to stop.</param>
        Task StopServerAsync(Guid serverId);
        
        /// <summary>
        /// Updates a single register or coil value in the server's data store (simulation).
        /// </summary>
        Task UpdateDataAsync(Guid serverId, ModbusDataType type, int address, ushort value);

        /// <summary>
        /// Reads a range of values from the server's data store for monitoring.
        /// </summary>
        Task<ushort[]> GetDataAsync(Guid serverId, ModbusDataType type, int startAddress, int count);

        /// <summary>
        /// Retrieves the recent traffic logs for a specific server.
        /// </summary>
        Task<IEnumerable<ModbusLogEntry>> GetLogsAsync(Guid serverId);
    }
}
