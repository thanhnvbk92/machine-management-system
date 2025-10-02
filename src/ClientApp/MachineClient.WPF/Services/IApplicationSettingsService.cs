using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MachineClient.WPF.Services
{
    public interface IApplicationSettingsService
    {
        // API Configuration
        string ApiBaseUrl { get; set; }
        int ApiTimeoutSeconds { get; set; }
        int MaxRetryAttempts { get; set; }

        // Application Settings
        string ApplicationVersion { get; set; }
        bool AutoStartConnection { get; set; }
        bool SaveConnectionHistory { get; set; }
        string DefaultBackupPath { get; set; }

        // UI Automation Settings
        bool EnableUIAutomation { get; set; }
        int AutomationDelayMs { get; set; }
        int MaxAutomationRetries { get; set; }

        // Logging Settings
        string LogLevel { get; set; }
        bool EnableFileLogging { get; set; }
        int MaxLogFileSizeMB { get; set; }
        int LogRetentionDays { get; set; }

        // Machine Information
        string MachineName { get; set; }
        string MachineDescription { get; set; }
        string Department { get; set; }
        string Location { get; set; }

        // Network Settings
        string IPAddress { get; set; }
        string MacAddress { get; set; }
        int NetworkTimeoutMs { get; set; }

        // Methods
        Task LoadSettingsAsync();
        Task SaveSettingsAsync();
        Task ResetToDefaultsAsync();
        T GetSetting<T>(string key, T defaultValue = default);
        void SetSetting<T>(string key, T value);
        bool HasSetting(string key);
        Task<bool> ValidateSettingsAsync();

        // Events
        event EventHandler<SettingsChangedEventArgs>? SettingsChanged;
    }

    public class SettingsChangedEventArgs : EventArgs
    {
        public string SettingName { get; set; } = string.Empty;
        public object? OldValue { get; set; }
        public object? NewValue { get; set; }
    }
}