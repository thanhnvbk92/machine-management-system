using System;
using System.Threading.Tasks;

namespace FlaUI.Automation.Extensions.Services
{
    /// <summary>
    /// Core UI Automation service interface providing fundamental automation operations
    /// </summary>
    public interface IUIAutomationService : IDisposable
    {
        /// <summary>
        /// Initialize the automation service with target application
        /// </summary>
        Task<bool> InitializeAsync(string? applicationTitle = null, int? processId = null);

        /// <summary>
        /// Click a button by AutomationId or Name
        /// </summary>
        Task<bool> ClickButtonAsync(string identifier, bool useAutomationId = true);

        /// <summary>
        /// Read text from UI element (TextBlock, Label, TextBox)
        /// </summary>
        Task<string> ReadTextAsync(string identifier, bool useAutomationId = true);

        /// <summary>
        /// Get current value from TextBox
        /// </summary>
        Task<string> GetTextBoxValueAsync(string identifier, bool useAutomationId = true);

        /// <summary>
        /// Set value to TextBox
        /// </summary>
        Task<bool> SetTextBoxValueAsync(string identifier, string value, bool useAutomationId = true);

        /// <summary>
        /// Get selected item from ComboBox
        /// </summary>
        Task<string> GetComboBoxSelectionAsync(string identifier, bool useAutomationId = true);

        /// <summary>
        /// Select item in ComboBox by text
        /// </summary>
        Task<bool> SelectComboBoxItemAsync(string identifier, string itemText, bool useAutomationId = true);

        /// <summary>
        /// Click menu item by text
        /// </summary>
        Task<bool> ClickMenuItemAsync(string menuText);

        /// <summary>
        /// Start monitoring TextBlock for changes
        /// </summary>
        Task<bool> StartTextBlockMonitoringAsync(string identifier, Action<string> onTextChanged, bool useAutomationId = true);

        /// <summary>
        /// Stop monitoring TextBlock
        /// </summary>
        Task<bool> StopTextBlockMonitoringAsync(string identifier);

        /// <summary>
        /// Stop all monitoring activities
        /// </summary>
        Task StopAllMonitoringAsync();

        /// <summary>
        /// Check if service is initialized and ready
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Get current application title
        /// </summary>
        string ApplicationTitle { get; }

        /// <summary>
        /// Get current process ID
        /// </summary>
        int ProcessId { get; }
    }
}