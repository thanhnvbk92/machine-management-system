using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using MachineClient.WPF.Models;
using Microsoft.Extensions.Logging;

namespace MachineClient.WPF.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ILogger<ConfigurationService> _logger;
        private readonly string _configFilePath;

        public ConfigurationService(ILogger<ConfigurationService> logger)
        {
            _logger = logger;
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataPath, "MachineClient");
            Directory.CreateDirectory(appFolder);
            _configFilePath = Path.Combine(appFolder, "config.json");
        }

        public async Task<ClientConfiguration?> LoadConfigurationAsync()
        {
            try
            {
                if (!File.Exists(_configFilePath))
                {
                    return CreateDefaultConfiguration();
                }

                var json = await File.ReadAllTextAsync(_configFilePath);
                return JsonSerializer.Deserialize<ClientConfiguration>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load configuration");
                return CreateDefaultConfiguration();
            }
        }

        public async Task SaveConfigurationAsync(ClientConfiguration configuration)
        {
            try
            {
                var json = JsonSerializer.Serialize(configuration, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_configFilePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save configuration");
                throw;
            }
        }

        public async Task<T?> GetSettingAsync<T>(string key, T? defaultValue = default)
        {
            try
            {
                var config = await LoadConfigurationAsync();
                if (config != null && config.Key == key)
                {
                    return JsonSerializer.Deserialize<T>(config.Value);
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
                var config = new ClientConfiguration
                {
                    Key = key,
                    Value = JsonSerializer.Serialize(value)
                };
                await SaveConfigurationAsync(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set setting {Key}", key);
                throw;
            }
        }

        public async Task ResetToDefaultsAsync()
        {
            try
            {
                var defaultConfig = CreateDefaultConfiguration();
                await SaveConfigurationAsync(defaultConfig);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset to defaults");
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
                Key = "default",
                Value = JsonSerializer.Serialize(new
                {
                    MachineId = "MACHINE_001",
                    ApiUrl = "http://localhost:5275"
                })
            };
        }
    }
}