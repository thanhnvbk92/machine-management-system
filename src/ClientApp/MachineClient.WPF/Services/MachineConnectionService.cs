using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MachineClient.WPF.Models;

namespace MachineClient.WPF.Services
{
    public class MachineConnectionService : IMachineConnectionService
    {
        private readonly IApiService _apiService;
        private readonly ILogger<MachineConnectionService> _logger;

        public event EventHandler<ConnectionStatusChangedEventArgs>? ConnectionStatusChanged;
        public event EventHandler<MachineInfoUpdatedEventArgs>? MachineInfoUpdated;

        public bool IsConnected { get; private set; }
        public string ConnectionStatus { get; private set; } = "Disconnected";

        public MachineConnectionService(IApiService apiService, ILogger<MachineConnectionService> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                _logger.LogInformation("MachineConnectionService.TestConnectionAsync - Starting API connection test");
                var result = await _apiService.TestConnectionAsync();
                
                if (result)
                {
                    _logger.LogInformation("MachineConnectionService.TestConnectionAsync - API connection test successful");
                }
                else
                {
                    _logger.LogError("MachineConnectionService.TestConnectionAsync - API connection test failed");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MachineConnectionService.TestConnectionAsync - Error testing API connection: {Message}", ex.Message);
                return false;
            }
        }

        public async Task<MachineConnectionResult> ConnectAndRegisterAsync(MachineInfo machineInfo)
        {
            try
            {
                _logger.LogInformation("Starting machine connection and registration - IP: {IP}, MAC: {MAC}", 
                    machineInfo.IpAddress, machineInfo.MacAddress);

                // Test connection first
                var connectionResult = await TestConnectionAsync();
                if (!connectionResult)
                {
                    UpdateConnectionStatus(false, "Connection Failed", "Failed to connect to server");
                    return new MachineConnectionResult
                    {
                        IsSuccess = false,
                        Message = "Failed to connect to server"
                    };
                }

                // Register machine
                var registrationRequest = new MachineRegistrationRequest
                {
                    IP = machineInfo.IpAddress,
                    MacAddress = machineInfo.MacAddress,
                    MachineName = machineInfo.MachineName,
                    AppVersion = machineInfo.AppVersion
                };

                _logger.LogInformation("Sending machine registration request");
                var registrationResult = await _apiService.RegisterMachineAsync(registrationRequest);

                _logger.LogInformation("Registration result - Success: {Success}, RequiresMacUpdate: {RequiresMacUpdate}, Message: {Message}",
                    registrationResult.IsSuccess, registrationResult.RequiresMacUpdate, registrationResult.Message);

                if (registrationResult.IsSuccess)
                {
                    if (registrationResult.RequiresMacUpdate && registrationResult.ExistingMachine != null)
                    {
                        // IP conflict detected
                        UpdateConnectionStatus(false, "IP Conflict", "IP address conflict detected");
                        return new MachineConnectionResult
                        {
                            IsSuccess = true,
                            RequiresMacUpdate = true,
                            ExistingMachine = registrationResult.ExistingMachine,
                            Message = registrationResult.Message
                        };
                    }

                    // Successful registration
                    UpdateConnectionStatus(true, "Connected", "Machine registered successfully");
                    
                    if (registrationResult.MachineInfo != null)
                    {
                        MachineInfoUpdated?.Invoke(this, new MachineInfoUpdatedEventArgs 
                        { 
                            MachineInfo = ConvertToMachineInfo(registrationResult.MachineInfo)
                        });
                    }

                    return new MachineConnectionResult
                    {
                        IsSuccess = true,
                        IsNewMachine = registrationResult.IsNewMachine,
                        MachineInfo = registrationResult.MachineInfo != null ? ConvertToMachineInfo(registrationResult.MachineInfo) : null,
                        Message = registrationResult.Message
                    };
                }
                else
                {
                    UpdateConnectionStatus(false, "Registration Failed", registrationResult.Message);
                    return new MachineConnectionResult
                    {
                        IsSuccess = false,
                        Message = registrationResult.Message
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during machine connection and registration");
                UpdateConnectionStatus(false, "Connection Error", ex.Message);
                return new MachineConnectionResult
                {
                    IsSuccess = false,
                    Message = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<MacUpdateResult> UpdateMacAddressAsync(MacUpdateRequest request)
        {
            try
            {
                _logger.LogInformation("Updating MAC address for IP: {IP}", request.IP);
                
                var result = await _apiService.UpdateMacAddressAsync(request);
                
                if (result.IsSuccess)
                {
                    _logger.LogInformation("MAC address updated successfully");
                    UpdateConnectionStatus(true, "Connected", "MAC address updated successfully");
                    
                    if (result.MachineInfo != null)
                    {
                        MachineInfoUpdated?.Invoke(this, new MachineInfoUpdatedEventArgs 
                        { 
                            MachineInfo = ConvertToMachineInfo(result.MachineInfo)
                        });
                    }
                }
                else
                {
                    _logger.LogWarning("MAC address update failed: {Message}", result.Message);
                    UpdateConnectionStatus(false, "Update Failed", result.Message);
                }

                return new MacUpdateResult
                {
                    IsSuccess = result.IsSuccess,
                    Message = result.Message,
                    MachineInfo = result.MachineInfo != null ? ConvertToMachineInfo(result.MachineInfo) : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating MAC address");
                return new MacUpdateResult
                {
                    IsSuccess = false,
                    Message = $"Update error: {ex.Message}"
                };
            }
        }

        private void UpdateConnectionStatus(bool isConnected, string status, string message)
        {
            IsConnected = isConnected;
            ConnectionStatus = status;

            ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs
            {
                IsConnected = isConnected,
                Status = status,
                Message = message
            });
        }

        private MachineInfo ConvertToMachineInfo(Models.MachineDetailDto dto)
        {
            return new MachineInfo
            {
                Name = dto.Name,
                IpAddress = dto.IP,
                MacAddress = dto.MacAddress,
                BuyerName = dto.BuyerName,
                LineName = dto.LineName,
                StationName = dto.StationName,
                ModelName = dto.ModelName,
                ProgramName = dto.ProgramName
            };
        }
    }
}