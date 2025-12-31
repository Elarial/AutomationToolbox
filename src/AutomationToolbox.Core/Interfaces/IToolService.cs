using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationToolbox.Core.Models;

namespace AutomationToolbox.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for retrieving automation tools.
    /// </summary>
    public interface IToolService
    {
         /// <summary>
        /// Retrieves the list of available automation tools asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="ToolDefinition"/>.</returns>
        Task<IEnumerable<ToolDefinition>> GetToolsAsync();
    }
}
