using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationToolbox.Core.Interfaces;
using AutomationToolbox.Core.Models;

namespace AutomationToolbox.Core.Services
{
    /// <summary>
    /// A in-memory mock implementation of <see cref="IToolService"/> for testing and development.
    /// </summary>
    public class MockToolService : IToolService
    {
        /// <inheritdoc />
        public Task<IEnumerable<ToolDefinition>> GetToolsAsync()
        {
            var tools = new List<ToolDefinition>
            {
                new ToolDefinition 
                { 
                    Name = "JsonFormatterTitle", 
                    Description = "JsonFormatterDesc", 
                    Category = "DataCategory",
                    Icon = "fa-solid fa-code"
                },
                new ToolDefinition 
                { 
                    Name = "GuidGeneratorTitle", 
                    Description = "GuidGeneratorDesc", 
                    Category = "GeneratorsCategory",
                    Icon = "fa-solid fa-fingerprint"
                },
                new ToolDefinition 
                { 
                    Name = "Base64EncoderTitle", 
                    Description = "Base64EncoderDesc", 
                    Category = "EncodersCategory",
                    Icon = "fa-solid fa-file-code"
                },
                new ToolDefinition
                {
                    Name = "NumberFormatConverterTitle",
                    Description = "NumberFormatConverterDesc",
                    Category = "ConvertersCategory",
                    Icon = "fa-solid fa-right-left",
                    Url = "tools/number-format-converter"
                },
                new ToolDefinition
                {
                    Name = "IP Scanner",
                    Description = "Scan network for devices and ports",
                    Category = "Network",
                    Icon = "fa-solid fa-network-wired",
                    Url = "tools/ip-scanner"
                },
                new ToolDefinition
                {
                    Name = "Modbus Server",
                    Description = "Run virtual Modbus TCP servers",
                    Category = "Industrial",
                    Icon = "fa-solid fa-server",
                    Url = "tools/modbus-server"
                }
            };

            return Task.FromResult((IEnumerable<ToolDefinition>)tools);
        }
    }
}
