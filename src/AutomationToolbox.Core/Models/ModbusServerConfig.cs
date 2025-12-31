using System;

namespace AutomationToolbox.Core.Models
{
    /// <summary>
    /// Configuration for a Modbus TCP Server instance.
    /// </summary>
    public class ModbusServerConfig
    {
        /// <summary>
        /// Unique identifier for the server instance.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// User-friendly name for the server.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// TCP port to listen on (default 502).
        /// </summary>
        public int Port { get; set; } = 502;

        /// <summary>
        /// IP address of the network interface to bind to (or 0.0.0.0 for all).
        /// </summary>
        public string IpAddress { get; set; } = "0.0.0.0"; 

        /// <summary>
        /// Indicates if the server is currently running.
        /// </summary>
        public bool IsRunning { get; set; }
    }

    /// <summary>
    /// Request model for updating a Modbus register value.
    /// </summary>
    public class ModbusRegisterUpdate
    {
        /// <summary>
        /// The type of Modbus data to update (Coil, Input, Register, etc.).
        /// </summary>
        public ModbusDataType Type { get; set; }

        /// <summary>
        /// The register address (1-based index).
        /// </summary>
        public int Address { get; set; } 

        /// <summary>
        /// The new value to set. For Coils/DiscreteInputs, >0 is True.
        /// </summary>
        public ushort Value { get; set; }
    }

    /// <summary>
    /// Supported Modbus data types.
    /// </summary>
    public enum ModbusDataType
    {
        /// <summary>Read/Write Output (0xxxx)</summary>
        Coil = 0,
        /// <summary>Read-Only Input (1xxxx)</summary>
        DiscreteInput = 1,
        /// <summary>Read-Only Register (3xxxx)</summary>
        InputRegister = 3,
        /// <summary>Read/Write Register (4xxxx)</summary>
        HoldingRegister = 4
    }

    /// <summary>
    /// Represents a log entry for Modbus traffic.
    /// </summary>
    public class ModbusLogEntry
    {
        /// <summary>Timestamp of the event.</summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
        /// <summary>IP address of the client (if available).</summary>
        public string ClientIp { get; set; } = string.Empty;
        /// <summary>Modbus Function Code involved.</summary>
        public int FunctionCode { get; set; }
        /// <summary>Type of request (e.g., "Read", "Write", "Info").</summary>
        public string RequestType { get; set; } = string.Empty; 
        /// <summary>Detailed log message.</summary>
        public string Message { get; set; } = string.Empty; 
    }
}
