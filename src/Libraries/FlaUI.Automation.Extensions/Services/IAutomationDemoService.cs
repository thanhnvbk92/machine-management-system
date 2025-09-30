using System;
using System.Threading.Tasks;

namespace FlaUI.Automation.Extensions.Services
{
    /// <summary>
    /// Comprehensive demo service showcasing all UI automation capabilities
    /// </summary>
    public interface IAutomationDemoService
    {
        /// <summary>
        /// Initialize and test all UI automation features
        /// </summary>
        Task<bool> InitializeAndTestAsync(string? applicationTitle = null);

        /// <summary>
        /// Demo automatic button clicking
        /// </summary>
        Task<bool> DemoButtonClickingAsync();

        /// <summary>
        /// Demo reading text from various elements
        /// </summary>
        Task<bool> DemoTextReadingAsync();

        /// <summary>
        /// Demo ComboBox interaction
        /// </summary>
        Task<bool> DemoComboBoxInteractionAsync();

        /// <summary>
        /// Demo TextBox interaction
        /// </summary>
        Task<bool> DemoTextBoxInteractionAsync();

        /// <summary>
        /// Demo menu clicking
        /// </summary>
        Task<bool> DemoMenuClickingAsync();

        /// <summary>
        /// Start comprehensive element monitoring demo
        /// </summary>
        Task<bool> StartElementMonitoringDemoAsync();

        /// <summary>
        /// Stop element monitoring demo
        /// </summary>
        Task<bool> StopElementMonitoringDemoAsync();

        /// <summary>
        /// Get debug information about available UI elements
        /// </summary>
        Task<string> GetDebugInfoAsync();

        /// <summary>
        /// Event for demo progress updates
        /// </summary>
        event EventHandler<DemoProgressEventArgs>? DemoProgress;
    }

    /// <summary>
    /// Event arguments for demo progress notifications
    /// </summary>
    public class DemoProgressEventArgs : EventArgs
    {
        public required string Message { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsError { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Details { get; set; }
    }
}