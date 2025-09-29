using MachineClient.WPF.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace MachineClient.WPF.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/health");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to test API connection");
                return false;
            }
        }

        public async Task RegisterMachineAsync(Machine machine)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/machines/register", machine, _jsonOptions);
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Machine {MachineId} registered successfully", machine.MachineId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register machine {MachineId}", machine.MachineId);
                throw;
            }
        }

        public async Task SendHeartbeatAsync(string machineId)
        {
            try
            {
                var heartbeat = new { MachineId = machineId, Timestamp = DateTime.UtcNow };
                var response = await _httpClient.PostAsJsonAsync("/api/machines/heartbeat", heartbeat, _jsonOptions);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send heartbeat for machine {MachineId}", machineId);
                throw;
            }
        }

        public async Task SendLogsAsync(IEnumerable<LogData> logs)
        {
            try
            {
                var logList = logs.ToList();
                if (!logList.Any()) return;

                var response = await _httpClient.PostAsJsonAsync("/api/logs/batch", logList, _jsonOptions);
                response.EnsureSuccessStatusCode();
                
                _logger.LogInformation("Sent {Count} log entries to server", logList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send logs to server");
                throw;
            }
        }

        public async Task<IEnumerable<Command>> GetPendingCommandsAsync(string machineId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/commands/pending/{machineId}");
                response.EnsureSuccessStatusCode();
                
                var commands = await response.Content.ReadFromJsonAsync<List<Command>>(_jsonOptions);
                return commands ?? new List<Command>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending commands for machine {MachineId}", machineId);
                return new List<Command>();
            }
        }

        public async Task UpdateCommandStatusAsync(int commandId, string status, string? result = null)
        {
            try
            {
                var update = new { Status = status, Result = result, ExecutedAt = DateTime.UtcNow };
                var response = await _httpClient.PutAsJsonAsync($"/api/commands/{commandId}/status", update, _jsonOptions);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update command {CommandId} status", commandId);
                throw;
            }
        }

        public async Task<ClientConfiguration?> GetConfigurationAsync(string machineId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/machines/{machineId}/configuration");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ClientConfiguration>(_jsonOptions);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get configuration for machine {MachineId}", machineId);
                return null;
            }
        }
    }
}