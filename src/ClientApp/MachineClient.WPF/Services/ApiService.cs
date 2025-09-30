using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MachineClient.WPF.Models;
using Microsoft.Extensions.Logging;

namespace MachineClient.WPF.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;

        public ApiService(IHttpClientFactory httpClientFactory, ILogger<ApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _logger = logger;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/health");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection test failed");
                return false;
            }
        }

        public async Task<MachineRegistrationResponse> RegisterMachineAsync(MachineRegistrationRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Sending registration request: {Request}", json);
                
                var response = await _httpClient.PostAsync("api/machines/register", content);
                
                var responseJson = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response - Status: {Status}, Body: {Body}", response.StatusCode, responseJson);
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var result = JsonSerializer.Deserialize<MachineRegistrationResponse>(responseJson, options) ?? new MachineRegistrationResponse();
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Machine registration successful for IP: {IP}, MAC: {MAC}", request.IP, request.MacAddress);
                }
                else
                {
                    _logger.LogWarning("Machine registration failed: {Message}", result.Message);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Machine registration failed for IP: {IP}", request.IP);
                return new MachineRegistrationResponse
                {
                    IsSuccess = false,
                    Message = $"Registration error: {ex.Message}"
                };
            }
        }

        [Obsolete("Use RegisterMachineAsync(MachineRegistrationRequest) instead")]
        public async Task RegisterMachineAsync(Machine machine)
        {
            try
            {
                var json = JsonSerializer.Serialize(machine);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("api/machines", content);
                response.EnsureSuccessStatusCode();
                
                _logger.LogInformation("Machine registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register machine");
                throw;
            }
        }

        public async Task SendHeartbeatAsync(string machineId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/machines/{machineId}/heartbeat", null);
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
                var json = JsonSerializer.Serialize(logList);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("api/logs", content);
                response.EnsureSuccessStatusCode();
                
                _logger.LogInformation("Sent {LogCount} logs successfully", logList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send logs");
                throw;
            }
        }

        public async Task<IEnumerable<Command>> GetPendingCommandsAsync(string machineId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/commands/{machineId}/pending");
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                var commands = JsonSerializer.Deserialize<List<Command>>(json) ?? new List<Command>();
                
                return commands;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending commands for machine {MachineId}", machineId);
                throw;
            }
        }

        public async Task UpdateCommandStatusAsync(int commandId, string status, string? result = null)
        {
            try
            {
                var data = new { Status = status, Result = result };
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"api/commands/{commandId}/status", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update command status for command {CommandId}", commandId);
                throw;
            }
        }

        public async Task<ClientConfiguration?> GetConfigurationAsync(string machineId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/configuration/{machineId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ClientConfiguration>(json);
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