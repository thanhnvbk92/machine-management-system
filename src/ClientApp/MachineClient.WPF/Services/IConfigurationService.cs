using System;
using System.Threading.Tasks;
using MachineClient.WPF.Models;

namespace MachineClient.WPF.Services
{
    public interface IConfigurationService
    {
        Task<ClientConfiguration?> LoadConfigurationAsync();
        Task SaveConfigurationAsync(ClientConfiguration configuration);
        Task<T?> GetSettingAsync<T>(string key, T? defaultValue = default);
        Task SetSettingAsync<T>(string key, T value);
        Task ResetToDefaultsAsync();
        string GetConfigFilePath();
    }
}