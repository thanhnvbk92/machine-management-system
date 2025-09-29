using MachineManagement.ManagerApp.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MachineManagement.ManagerApp.Services;

/// <summary>
/// Service for machine management operations
/// </summary>
public class MachineService : IMachineService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MachineService> _logger;

    public MachineService(IHttpClientFactory httpClientFactory, ILogger<MachineService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
    }

    public async Task<IList<MachineDto>> GetAllMachinesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all machines from API");
            var response = await _httpClient.GetFromJsonAsync<IList<MachineDto>>("/api/machines");
            return response ?? new List<MachineDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching machines");
            return new List<MachineDto>();
        }
    }

    public async Task<MachineDto?> GetMachineByIdAsync(string machineId)
    {
        try
        {
            _logger.LogInformation("Fetching machine {MachineId}", machineId);
            return await _httpClient.GetFromJsonAsync<MachineDto>($"/api/machines/{machineId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching machine {MachineId}", machineId);
            return null;
        }
    }

    public async Task<bool> UpdateMachineAsync(string machineId, MachineDto machine)
    {
        try
        {
            _logger.LogInformation("Updating machine {MachineId}", machineId);
            var response = await _httpClient.PutAsJsonAsync($"/api/machines/{machineId}", machine);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating machine {MachineId}", machineId);
            return false;
        }
    }

    public async Task<bool> DeleteMachineAsync(string machineId)
    {
        try
        {
            _logger.LogInformation("Deleting machine {MachineId}", machineId);
            var response = await _httpClient.DeleteAsync($"/api/machines/{machineId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting machine {MachineId}", machineId);
            return false;
        }
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching dashboard stats");
            var response = await _httpClient.GetFromJsonAsync<DashboardStatsDto>("/api/machines/stats");
            return response ?? new DashboardStatsDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching dashboard stats");
            return new DashboardStatsDto();
        }
    }
}