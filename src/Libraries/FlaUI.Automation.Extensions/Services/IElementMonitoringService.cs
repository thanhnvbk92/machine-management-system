using System;
using System.Threading.Tasks;

namespace FlaUI.Automation.Extensions.Services
{
    /// <summary>
    /// Specialized service for monitoring UI element changes and triggering events
    /// </summary>
    public interface IElementMonitoringService : IDisposable
    {
        /// <summary>
        /// Start monitoring element value changes
        /// </summary>
        Task<bool> StartMonitoringAsync(string elementIdentifier, bool useAutomationId = true, int intervalMs = 500);

        /// <summary>
        /// Stop monitoring element changes
        /// </summary>
        Task<bool> StopMonitoringAsync();

        /// <summary>
        /// Event triggered when monitored element value changes
        /// </summary>
        event EventHandler<ElementChangedEventArgs>? ElementChanged;

        /// <summary>
        /// Check if monitoring is currently active
        /// </summary>
        bool IsMonitoring { get; }

        /// <summary>
        /// Get current monitored element identifier
        /// </summary>
        string? CurrentElementIdentifier { get; }

        /// <summary>
        /// Get last known value of monitored element
        /// </summary>
        string? LastKnownValue { get; }
    }

    /// <summary>
    /// Event arguments for element change notifications
    /// </summary>
    public class ElementChangedEventArgs : EventArgs
    {
        public required string NewValue { get; set; }
        public required string PreviousValue { get; set; }
        public DateTime Timestamp { get; set; }
        public required string ElementIdentifier { get; set; }
        public bool UseAutomationId { get; set; }
    }
}