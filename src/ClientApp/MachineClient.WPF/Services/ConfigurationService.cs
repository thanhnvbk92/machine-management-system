using MachineClient.WPF.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace MachineClient.WPF.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ILogger<ConfigurationService> _logger;
        private readonly string _configFilePath;
        private readonly JsonSerializerOptions _jsonOptions;
        private ClientConfiguration? _cachedConfiguration;

        public ConfigurationService(ILogger<ConfigurationService> logger)
        {
            _logger = logger;
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataPath, "MachineClient");
            Directory.CreateDirectory(appFolder);
            
            _configFilePath = Path.Combine(appFolder, "config.json");
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<ClientConfiguration?> LoadConfigurationAsync()
        {
            try
            {
                if (_cachedConfiguration != null)
                    return _cachedConfiguration;

                if (!File.Exists(_configFilePath))
                {
                    _logger.LogInformation("Configuration file not found, creating default configuration");
                    _cachedConfiguration = CreateDefaultConfiguration();
                    await SaveConfigurationAsync(_cachedConfiguration);
                    return _cachedConfiguration;
                }

                var json = await File.ReadAllTextAsync(_configFilePath);
                _cachedConfiguration = JsonSerializer.Deserialize<ClientConfiguration>(json, _jsonOptions);
                
                if (_cachedConfiguration == null)
                {
                    _logger.LogWarning("Failed to deserialize configuration, using defaults");
                    _cachedConfiguration = CreateDefaultConfiguration();
                }

                _logger.LogInformation("Configuration loaded successfully");
                return _cachedConfiguration;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load configuration from {ConfigPath}", _configFilePath);
                _cachedConfiguration = CreateDefaultConfiguration();
                return _cachedConfiguration;
            }
        }

        public async Task SaveConfigurationAsync(ClientConfiguration configuration)
        {
            try
            {
                var json = JsonSerializer.Serialize(configuration, _jsonOptions);
                await File.WriteAllTextAsync(_configFilePath, json);
                
                _cachedConfiguration = configuration;
                _logger.LogInformation("Configuration saved successfully to {ConfigPath}", _configFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save configuration to {ConfigPath}", _configFilePath);
                throw;
            }
        }

        public async Task<T?> GetSettingAsync<T>(string key, T? defaultValue = default)
        {
            try
            {
                var config = await LoadConfigurationAsync();
                if (config == null) return defaultValue;

                var property = typeof(ClientConfiguration).GetProperty(key);
                if (property != null && property.PropertyType == typeof(T))
                {
                    var value = property.GetValue(config);
                    return value != null ? (T)value : defaultValue;
                }

                return defaultValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get setting {Key}", key);
                return defaultValue;
            }
        }

        public async Task SetSettingAsync<T>(string key, T value)
        {
            try
            {
                var config = await LoadConfigurationAsync();
                if (config == null) return;

                var property = typeof(ClientConfiguration).GetProperty(key);
                if (property != null && property.PropertyType == typeof(T))
                {
                    property.SetValue(config, value);
                    await SaveConfigurationAsync(config);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set setting {Key} = {Value}", key, value);
                throw;
            }
        }

        public async Task ResetToDefaultsAsync()
        {
            try
            {
                _cachedConfiguration = CreateDefaultConfiguration();
                await SaveConfigurationAsync(_cachedConfiguration);
                _logger.LogInformation("Configuration reset to defaults");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset configuration to defaults");
                throw;
            }
        }

        public string GetConfigFilePath()
        {
            return _configFilePath;
        }

        private ClientConfiguration CreateDefaultConfiguration()
        {
            return new ClientConfiguration
            {
                MachineId = Environment.MachineName,
                StationName = "Station_A",
                LineName = "Line_1",
                ApiUrl = "https://localhost:5001",
                LogCollectionInterval = 30,
                HeartbeatInterval = 60,
                AutoStart = true,
                LogFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MachineData", "Logs"),
                MaxLogFileSize = 10 * 1024 * 1024, // 10MB
                MaxLogFiles = 100,
                LogRetentionDays = 30,
                EnableDebugLogging = false,
                ConnectionTimeout = 30,
                RetryAttempts = 3,
                RetryDelay = 5000
            };
        }
    }
}