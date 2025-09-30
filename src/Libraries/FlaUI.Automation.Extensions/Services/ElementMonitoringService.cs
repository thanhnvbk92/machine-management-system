using System;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;

namespace FlaUI.Automation.Extensions.Services
{
    /// <summary>
    /// Implementation of element monitoring service
    /// </summary>
    public class ElementMonitoringService : IElementMonitoringService
    {
        private readonly IUIAutomationService _uiAutomationService;
        private readonly ILogger<ElementMonitoringService> _logger;
        private Timer? _monitoringTimer;
        private string? _currentElementIdentifier;
        private bool _useAutomationId = true;
        private string? _lastKnownValue;

        public event EventHandler<ElementChangedEventArgs>? ElementChanged;
        public bool IsMonitoring => _monitoringTimer?.Enabled == true;
        public string? CurrentElementIdentifier => _currentElementIdentifier;
        public string? LastKnownValue => _lastKnownValue;

        public ElementMonitoringService(IUIAutomationService uiAutomationService, ILogger<ElementMonitoringService> logger)
        {
            _uiAutomationService = uiAutomationService;
            _logger = logger;
        }

        public async Task<bool> StartMonitoringAsync(string elementIdentifier, bool useAutomationId = true, int intervalMs = 500)
        {
            try
            {
                // Stop existing monitoring
                await StopMonitoringAsync();

                if (!_uiAutomationService.IsInitialized)
                {
                    _logger.LogWarning("UI Automation service not initialized");
                    return false;
                }

                _currentElementIdentifier = elementIdentifier;
                _useAutomationId = useAutomationId;

                // Get initial value
                _lastKnownValue = await _uiAutomationService.ReadTextAsync(elementIdentifier, useAutomationId);

                // Create monitoring timer
                _monitoringTimer = new Timer(intervalMs);
                _monitoringTimer.Elapsed += async (sender, e) =>
                {
                    await CheckForChangesAsync();
                };

                _monitoringTimer.Start();
                _logger.LogInformation("Started monitoring element: {ElementIdentifier}", elementIdentifier);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start monitoring element: {ElementIdentifier}", elementIdentifier);
                return false;
            }
        }

        public async Task<bool> StopMonitoringAsync()
        {
            try
            {
                if (_monitoringTimer != null)
                {
                    _monitoringTimer.Stop();
                    _monitoringTimer.Dispose();
                    _monitoringTimer = null;
                }

                var elementId = _currentElementIdentifier;
                _currentElementIdentifier = null;
                _lastKnownValue = null;

                if (!string.IsNullOrEmpty(elementId))
                {
                    _logger.LogInformation("Stopped monitoring element: {ElementIdentifier}", elementId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to stop monitoring");
                return false;
            }
        }

        private async Task CheckForChangesAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_currentElementIdentifier))
                    return;

                var currentValue = await _uiAutomationService.ReadTextAsync(_currentElementIdentifier, _useAutomationId);
                
                if (currentValue != _lastKnownValue)
                {
                    var args = new ElementChangedEventArgs
                    {
                        NewValue = currentValue,
                        PreviousValue = _lastKnownValue ?? "",
                        Timestamp = DateTime.Now,
                        ElementIdentifier = _currentElementIdentifier,
                        UseAutomationId = _useAutomationId
                    };

                    _lastKnownValue = currentValue;
                    ElementChanged?.Invoke(this, args);
                    
                    _logger.LogDebug("Element value changed for {ElementIdentifier}: {OldValue} -> {NewValue}", 
                        _currentElementIdentifier, args.PreviousValue, args.NewValue);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for element changes: {ElementIdentifier}", _currentElementIdentifier);
            }
        }

        public void Dispose()
        {
            try
            {
                Task.Run(async () => await StopMonitoringAsync()).Wait();
                _logger.LogInformation("Element monitoring service disposed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing element monitoring service");
            }
        }
    }
}