using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MachineClient.WPF.Services
{
    public class ApplicationSettingsService : IApplicationSettingsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApplicationSettingsService> _logger;
        private readonly Dictionary<string, object> _settings;
        private readonly string _settingsFilePath;

        public event EventHandler<SettingsChangedEventArgs>? SettingsChanged;

        public ApplicationSettingsService(IConfiguration configuration, ILogger<ApplicationSettingsService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _settings = new Dictionary<string, object>();
            
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "MachineClient");
            Directory.CreateDirectory(appFolder);
            _settingsFilePath = Path.Combine(appFolder, "usersettings.json");
        }

        // API Configuration
        public string ApiBaseUrl
        {
            get => GetSetting("ApiBaseUrl", _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001");
            set => SetSetting("ApiBaseUrl", value);
        }

        public int ApiTimeoutSeconds
        {
            get => GetSetting("ApiTimeoutSeconds", _configuration.GetValue<int>("ApiSettings:TimeoutSeconds", 30));
            set => SetSetting("ApiTimeoutSeconds", value);
        }

        public int MaxRetryAttempts
        {
            get => GetSetting("MaxRetryAttempts", _configuration.GetValue<int>("ApiSettings:MaxRetryAttempts", 3));
            set => SetSetting("MaxRetryAttempts", value);
        }

        // Application Settings
        public string ApplicationVersion
        {
            get => GetSetting("ApplicationVersion", _configuration["Application:Version"] ?? "1.0.0");
            set => SetSetting("ApplicationVersion", value);
        }

        public bool AutoStartConnection
        {
            get => GetSetting("AutoStartConnection", _configuration.GetValue<bool>("Application:AutoStartConnection", false));
            set => SetSetting("AutoStartConnection", value);
        }

        public bool SaveConnectionHistory
        {
            get => GetSetting("SaveConnectionHistory", _configuration.GetValue<bool>("Application:SaveConnectionHistory", true));
            set => SetSetting("SaveConnectionHistory", value);
        }

        public string DefaultBackupPath
        {
            get => GetSetting("DefaultBackupPath", _configuration["Application:DefaultBackupPath"] ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MachineClient_Backup"));
            set => SetSetting("DefaultBackupPath", value);
        }

        // UI Automation Settings
        public bool EnableUIAutomation
        {
            get => GetSetting("EnableUIAutomation", _configuration.GetValue<bool>("UIAutomation:Enabled", true));
            set => SetSetting("EnableUIAutomation", value);
        }

        public int AutomationDelayMs
        {
            get => GetSetting("AutomationDelayMs", _configuration.GetValue<int>("UIAutomation:DelayMs", 500));
            set => SetSetting("AutomationDelayMs", value);
        }

        public int MaxAutomationRetries
        {
            get => GetSetting("MaxAutomationRetries", _configuration.GetValue<int>("UIAutomation:MaxRetries", 3));
            set => SetSetting("MaxAutomationRetries", value);
        }

        // Logging Settings
        public string LogLevel
        {
            get => GetSetting("LogLevel", _configuration["Logging:LogLevel:Default"] ?? "Information");
            set => SetSetting("LogLevel", value);
        }

        public bool EnableFileLogging
        {
            get => GetSetting("EnableFileLogging", _configuration.GetValue<bool>("Logging:EnableFileLogging", true));
            set => SetSetting("EnableFileLogging", value);
        }

        public int MaxLogFileSizeMB
        {
            get => GetSetting("MaxLogFileSizeMB", _configuration.GetValue<int>("Logging:MaxLogFileSizeMB", 10));
            set => SetSetting("MaxLogFileSizeMB", value);
        }

        public int LogRetentionDays
        {
            get => GetSetting("LogRetentionDays", _configuration.GetValue<int>("Logging:LogRetentionDays", 30));
            set => SetSetting("LogRetentionDays", value);
        }

        // Machine Information
        public string MachineName
        {
            get => GetSetting("MachineName", _configuration["Machine:Name"] ?? Environment.MachineName);
            set => SetSetting("MachineName", value);
        }

        public string MachineDescription
        {
            get => GetSetting("MachineDescription", _configuration["Machine:Description"] ?? "");
            set => SetSetting("MachineDescription", value);
        }

        public string Department
        {
            get => GetSetting("Department", _configuration["Machine:Department"] ?? "");
            set => SetSetting("Department", value);
        }

        public string Location
        {
            get => GetSetting("Location", _configuration["Machine:Location"] ?? "");
            set => SetSetting("Location", value);
        }

        // Network Settings
        public string IPAddress
        {
            get => GetSetting("IPAddress", _configuration["Network:IPAddress"] ?? "");
            set => SetSetting("IPAddress", value);
        }

        public string MacAddress
        {
            get => GetSetting("MacAddress", _configuration["Network:MacAddress"] ?? "");
            set => SetSetting("MacAddress", value);
        }

        public int NetworkTimeoutMs
        {
            get => GetSetting("NetworkTimeoutMs", _configuration.GetValue<int>("Network:TimeoutMs", 5000));
            set => SetSetting("NetworkTimeoutMs", value);
        }

        // Methods
        public async Task LoadSettingsAsync()
        {
            try
            {
                _logger.LogInformation("Loading user settings from: {SettingsPath}", _settingsFilePath);

                if (File.Exists(_settingsFilePath))
                {
                    var json = await File.ReadAllTextAsync(_settingsFilePath);
                    if (!string.IsNullOrEmpty(json))
                    {
                        var userSettings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                        if (userSettings != null)
                        {
                            foreach (var setting in userSettings)
                            {
                                _settings[setting.Key] = ConvertJsonElement(setting.Value);
                            }
                        }
                    }
                }

                _logger.LogInformation("Loaded {Count} user settings", _settings.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user settings");
            }
        }

        public async Task SaveSettingsAsync()
        {
            try
            {
                _logger.LogInformation("Saving user settings to: {SettingsPath}", _settingsFilePath);

                var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(_settingsFilePath, json);
                _logger.LogInformation("Saved {Count} user settings", _settings.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user settings");
            }
        }

        public async Task ResetToDefaultsAsync()
        {
            try
            {
                _logger.LogInformation("Resetting settings to defaults");
                
                _settings.Clear();
                
                if (File.Exists(_settingsFilePath))
                {
                    File.Delete(_settingsFilePath);
                }

                await SaveSettingsAsync();
                _logger.LogInformation("Settings reset to defaults successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting settings to defaults");
            }
        }

        public T GetSetting<T>(string key, T defaultValue = default)
        {
            try
            {
                if (_settings.TryGetValue(key, out var value))
                {
                    if (value is T directValue)
                        return directValue;

                    // Try to convert
                    if (typeof(T) == typeof(string))
                        return (T)(object)value.ToString()!;
                    
                    if (typeof(T) == typeof(int) && int.TryParse(value.ToString(), out var intValue))
                        return (T)(object)intValue;
                    
                    if (typeof(T) == typeof(bool) && bool.TryParse(value.ToString(), out var boolValue))
                        return (T)(object)boolValue;
                }

                return defaultValue;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting setting {Key}, returning default value", key);
                return defaultValue;
            }
        }

        public void SetSetting<T>(string key, T value)
        {
            try
            {
                var oldValue = _settings.TryGetValue(key, out var existing) ? existing : default(T);
                _settings[key] = value!;

                SettingsChanged?.Invoke(this, new SettingsChangedEventArgs
                {
                    SettingName = key,
                    OldValue = oldValue,
                    NewValue = value
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting {Key} to {Value}", key, value);
            }
        }

        public bool HasSetting(string key)
        {
            return _settings.ContainsKey(key);
        }

        public async Task<bool> ValidateSettingsAsync()
        {
            try
            {
                _logger.LogInformation("Validating application settings");

                var isValid = true;

                // Validate API URL
                if (string.IsNullOrEmpty(ApiBaseUrl) || !Uri.TryCreate(ApiBaseUrl, UriKind.Absolute, out _))
                {
                    _logger.LogWarning("Invalid API base URL: {ApiBaseUrl}", ApiBaseUrl);
                    isValid = false;
                }

                // Validate timeout values
                if (ApiTimeoutSeconds <= 0 || ApiTimeoutSeconds > 300)
                {
                    _logger.LogWarning("Invalid API timeout: {Timeout} seconds", ApiTimeoutSeconds);
                    isValid = false;
                }

                // Validate backup path
                if (!string.IsNullOrEmpty(DefaultBackupPath) && !Directory.Exists(Path.GetDirectoryName(DefaultBackupPath)))
                {
                    _logger.LogWarning("Invalid backup path: {BackupPath}", DefaultBackupPath);
                    isValid = false;
                }

                _logger.LogInformation("Settings validation result: {IsValid}", isValid);
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating settings");
                return false;
            }
        }

        private object ConvertJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString()!,
                JsonValueKind.Number => element.TryGetInt32(out var intValue) ? intValue : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => element.ToString()
            };
        }
    }
}