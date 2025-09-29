using MachineManagement.ManagerApp.Models;
using System.Net.Http.Json;

namespace MachineManagement.ManagerApp.Services;

/// <summary>
/// Service for command management operations
/// </summary>
public class CommandService : ICommandService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CommandService> _logger;

    public CommandService(IHttpClientFactory httpClientFactory, ILogger<CommandService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
    }

    public async Task<IList<CommandDto>> GetCommandsAsync(string? machineId = null)
    {
        try
        {
            _logger.LogInformation("Fetching commands for machine {MachineId}", machineId ?? "all");
            
            var url = "/api/commands";
            if (!string.IsNullOrEmpty(machineId))
                url += $"?machineId={Uri.EscapeDataString(machineId)}";

            var response = await _httpClient.GetFromJsonAsync<IList<CommandDto>>(url);
            return response ?? new List<CommandDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching commands");
            return new List<CommandDto>();
        }
    }

    public async Task<CommandDto?> GetCommandByIdAsync(long id)
    {
        try
        {
            _logger.LogInformation("Fetching command {CommandId}", id);
            return await _httpClient.GetFromJsonAsync<CommandDto>($"/api/commands/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching command {CommandId}", id);
            return null;
        }
    }

    public async Task<CommandDto> CreateCommandAsync(CreateCommandRequest request)
    {
        try
        {
            _logger.LogInformation("Creating command for machine {MachineId}", request.MachineId);
            var response = await _httpClient.PostAsJsonAsync("/api/commands", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CommandDto>();
                return result ?? new CommandDto();
            }
            
            return new CommandDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating command");
            return new CommandDto();
        }
    }

    public async Task<bool> UpdateCommandStatusAsync(long id, string status, string? result = null)
    {
        try
        {
            _logger.LogInformation("Updating command {CommandId} status to {Status}", id, status);
            
            var updateRequest = new { Status = status, Result = result };
            var response = await _httpClient.PutAsJsonAsync($"/api/commands/{id}/status", updateRequest);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating command {CommandId}", id);
            return false;
        }
    }

    public async Task<bool> DeleteCommandAsync(long id)
    {
        try
        {
            _logger.LogInformation("Deleting command {CommandId}", id);
            var response = await _httpClient.DeleteAsync($"/api/commands/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting command {CommandId}", id);
            return false;
        }
    }
}