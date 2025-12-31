using System;

namespace AutomationToolbox.Core.Models
{
    /// <summary>
    /// Represents a single automation tool available in the toolbox.
    /// </summary>
    public class ToolDefinition
    {
        /// <summary>
        /// Unique identifier for the tool.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Display name of the tool.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// A short description of what the tool does.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The category under which this tool is grouped (e.g., "Generators", "Formatters").
        /// </summary>
        public string Category { get; set; } = "General";

        /// <summary>
        /// FontAwesome class string for the tool's icon.
        /// </summary>
        public string Icon { get; set; } = "fa-solid fa-wrench"; 

        /// <summary>
        /// The relative URL to navigate to when opening the tool.
        /// </summary>
        public string Url { get; set; } = "#";
    }
}
