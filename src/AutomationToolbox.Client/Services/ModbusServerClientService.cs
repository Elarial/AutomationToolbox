using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutomationToolbox.Core.Models;

namespace AutomationToolbox.Client.Services
{
    /// <summary>
    /// Client-side service to interact with the Modbus Server API.
    /// </summary>
    public interface IModbusServerClientService
    {
        /// <summary>Fetches all servers.</summary>
        Task<IEnumerable<ModbusServerConfig>> GetAllServersAsync();
        /// <summary>Starts a new server.</summary>
        Task<ModbusServerConfig> StartServerAsync(ModbusServerConfig config);
        /// <summary>Stops a server.</summary>
        Task StopServerAsync(Guid serverId);
        /// <summary>Updates a simulated value.</summary>
        Task UpdateDataAsync(Guid serverId, ModbusDataType type, int address, ushort value);
        /// <summary>Reads simulated values.</summary>
        Task<ushort[]> GetDataAsync(Guid serverId, ModbusDataType type, int startAddress, int count);
        /// <summary>Gets traffic logs.</summary>
        Task<IEnumerable<ModbusLogEntry>> GetLogsAsync(Guid serverId);
        /// <summary>Gets available network interfaces.</summary>
        Task<IEnumerable<NetworkInterfaceInfo>> GetInterfacesAsync();
    }

    /// <inheritdoc />
    public class ModbusServerClientService : IModbusServerClientService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes the client service.
        /// </summary>
        public ModbusServerClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ModbusServerConfig>> GetAllServersAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ModbusServerConfig>>("api/modbus-servers") 
                   ?? Array.Empty<ModbusServerConfig>();
        }

        public async Task<ModbusServerConfig> StartServerAsync(ModbusServerConfig config)
        {
            var response = await _httpClient.PostAsJsonAsync("api/modbus-servers", config);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ModbusServerConfig>();
            return result ?? throw new InvalidOperationException("Failed to deserialize server config.");
        }

        public async Task StopServerAsync(Guid serverId)
        {
            var response = await _httpClient.DeleteAsync($"api/modbus-servers/{serverId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateDataAsync(Guid serverId, ModbusDataType type, int address, ushort value)
        {
            await _httpClient.PutAsJsonAsync($"api/modbus-servers/{serverId}/data", new ModbusRegisterUpdate
            {
                Type = type,
                Address = address,
                Value = value
            });
        }

        public async Task<ushort[]> GetDataAsync(Guid serverId, ModbusDataType type, int startAddress, int count)
        {
             return await _httpClient.GetFromJsonAsync<ushort[]>($"api/modbus-servers/{serverId}/data?type={type}&startAddress={startAddress}&count={count}") 
                    ?? Array.Empty<ushort>();
        }

        public async Task<IEnumerable<ModbusLogEntry>> GetLogsAsync(Guid serverId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ModbusLogEntry>>($"api/modbus-servers/{serverId}/logs") 
                   ?? Array.Empty<ModbusLogEntry>();
        }

        public async Task<IEnumerable<NetworkInterfaceInfo>> GetInterfacesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<NetworkInterfaceInfo>>("api/network-interfaces") 
                   ?? Array.Empty<NetworkInterfaceInfo>();
        }
    }
}
