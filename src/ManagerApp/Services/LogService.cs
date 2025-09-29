using MachineManagement.ManagerApp.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MachineManagement.ManagerApp.Services;

/// <summary>
/// Service for log management operations
/// </summary>
public class LogService : ILogService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LogService> _logger;

    public LogService(IHttpClientFactory httpClientFactory, ILogger<LogService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
    }

    public async Task<PagedResponse<LogEntryDto>> GetLogsAsync(LogQueryRequest request)
    {
        try
        {
            _logger.LogInformation("Fetching logs with query: {@Request}", request);
            
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(request.MachineId))
                queryParams.Add($"machineId={Uri.EscapeDataString(request.MachineId)}");
            if (!string.IsNullOrEmpty(request.Level))
                queryParams.Add($"level={Uri.EscapeDataString(request.Level)}");
            if (request.StartTime.HasValue)
                queryParams.Add($"startTime={request.StartTime.Value:O}");
            if (request.EndTime.HasValue)
                queryParams.Add($"endTime={request.EndTime.Value:O}");
            if (!string.IsNullOrEmpty(request.SearchText))
                queryParams.Add($"search={Uri.EscapeDataString(request.SearchText)}");
            
            queryParams.Add($"page={request.Page}");
            queryParams.Add($"pageSize={request.PageSize}");

            var queryString = string.Join("&", queryParams);
            var url = $"/api/logs?{queryString}";

            var response = await _httpClient.GetFromJsonAsync<PagedResponse<LogEntryDto>>(url);
            return response ?? new PagedResponse<LogEntryDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching logs");
            return new PagedResponse<LogEntryDto>();
        }
    }

    public async Task<LogEntryDto?> GetLogByIdAsync(long id)
    {
        try
        {
            _logger.LogInformation("Fetching log {LogId}", id);
            return await _httpClient.GetFromJsonAsync<LogEntryDto>($"/api/logs/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching log {LogId}", id);
            return null;
        }
    }

    public async Task<bool> DeleteLogsAsync(DateTime beforeDate)
    {
        try
        {
            _logger.LogInformation("Deleting logs before {Date}", beforeDate);
            var response = await _httpClient.DeleteAsync($"/api/logs?before={beforeDate:O}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting logs");
            return false;
        }
    }

    public async Task<byte[]> ExportLogsAsync(LogQueryRequest request, string format)
    {
        try
        {
            _logger.LogInformation("Exporting logs in format {Format}", format);
            
            var content = JsonContent.Create(request);
            var response = await _httpClient.PostAsync($"/api/logs/export?format={format}", content);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting logs");
            return Array.Empty<byte>();
        }
    }
}