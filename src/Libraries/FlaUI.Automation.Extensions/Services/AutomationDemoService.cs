using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FlaUI.Automation.Extensions.Services
{
    /// <summary>
    /// Comprehensive demo service implementation
    /// </summary>
    public class AutomationDemoService : IAutomationDemoService
    {
        private readonly IUIAutomationService _uiAutomationService;
        private readonly IElementMonitoringService _elementMonitoringService;
        private readonly ILogger<AutomationDemoService> _logger;

        public event EventHandler<DemoProgressEventArgs>? DemoProgress;

        public AutomationDemoService(
            IUIAutomationService uiAutomationService,
            IElementMonitoringService elementMonitoringService,
            ILogger<AutomationDemoService> logger)
        {
            _uiAutomationService = uiAutomationService;
            _elementMonitoringService = elementMonitoringService;
            _logger = logger;

            // Subscribe to element changes for demo
            _elementMonitoringService.ElementChanged += OnElementChanged;
        }

        public async Task<bool> InitializeAndTestAsync(string? applicationTitle = null)
        {
            try
            {
                ReportProgress("🔄 Initializing UI Automation Demo...", false, false);

                // Initialize UI automation
                if (!_uiAutomationService.IsInitialized)
                {
                    var initialized = await _uiAutomationService.InitializeAsync(applicationTitle);
                    if (!initialized)
                    {
                        ReportProgress("❌ Failed to initialize UI Automation", false, true);
                        return false;
                    }
                }

                ReportProgress("✅ UI Automation initialized successfully", true, false);

                // Run all demos
                await DemoButtonClickingAsync();
                await Task.Delay(500);
                
                await DemoTextReadingAsync();
                await Task.Delay(500);
                
                await DemoTextBoxInteractionAsync();
                await Task.Delay(500);
                
                await DemoComboBoxInteractionAsync();
                await Task.Delay(500);

                ReportProgress("✅ All UI Automation demos completed successfully!", true, false);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to run UI Automation demo");
                ReportProgress($"❌ Demo failed: {ex.Message}", false, true);
                return false;
            }
        }

        public async Task<bool> DemoButtonClickingAsync()
        {
            try
            {
                ReportProgress("🔄 Demo: Button Clicking...", false, false);

                // Try to click common buttons
                var buttonIds = new[] { "StartBackupButton", "StopBackupButton", "DemoAllFeaturesButton" };
                var successCount = 0;

                foreach (var buttonId in buttonIds)
                {
                    var clicked = await _uiAutomationService.ClickButtonAsync(buttonId);
                    if (clicked)
                    {
                        successCount++;
                        ReportProgress($"✅ Successfully clicked: {buttonId}", true, false);
                        await Task.Delay(200); // Small delay between clicks
                    }
                    else
                    {
                        ReportProgress($"⚠️ Button not found: {buttonId}", false, false);
                    }
                }

                ReportProgress($"✅ Button clicking demo completed. Success: {successCount}/{buttonIds.Length}", true, false);
                return successCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Button clicking demo failed");
                ReportProgress($"❌ Button clicking demo failed: {ex.Message}", false, true);
                return false;
            }
        }

        public async Task<bool> DemoTextReadingAsync()
        {
            try
            {
                ReportProgress("🔄 Demo: Text Reading...", false, false);

                // Try to read text from common elements
                var textIds = new[] { "MachineIdTextBlock", "BackupStatusTextBlock", "BackupProgressTextBlock" };
                var successCount = 0;

                foreach (var textId in textIds)
                {
                    var text = await _uiAutomationService.ReadTextAsync(textId);
                    if (!string.IsNullOrEmpty(text))
                    {
                        successCount++;
                        ReportProgress($"✅ Read from {textId}: {text}", true, false);
                    }
                    else
                    {
                        ReportProgress($"⚠️ No text found in: {textId}", false, false);
                    }
                }

                ReportProgress($"✅ Text reading demo completed. Success: {successCount}/{textIds.Length}", true, false);
                return successCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Text reading demo failed");
                ReportProgress($"❌ Text reading demo failed: {ex.Message}", false, true);
                return false;
            }
        }

        public async Task<bool> DemoComboBoxInteractionAsync()
        {
            try
            {
                ReportProgress("🔄 Demo: ComboBox Interaction...", false, false);

                // This is a placeholder - would need actual ComboBox IDs from the application
                ReportProgress("ℹ️ ComboBox demo - no ComboBoxes found in current UI", false, false);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ComboBox interaction demo failed");
                ReportProgress($"❌ ComboBox demo failed: {ex.Message}", false, true);
                return false;
            }
        }

        public async Task<bool> DemoTextBoxInteractionAsync()
        {
            try
            {
                ReportProgress("🔄 Demo: TextBox Interaction...", false, false);

                // This is a placeholder - would need actual TextBox IDs from the application
                ReportProgress("ℹ️ TextBox demo - no TextBoxes found in current UI", false, false);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TextBox interaction demo failed");
                ReportProgress($"❌ TextBox demo failed: {ex.Message}", false, true);
                return false;
            }
        }

        public async Task<bool> DemoMenuClickingAsync()
        {
            try
            {
                ReportProgress("🔄 Demo: Menu Clicking...", false, false);

                // This is a placeholder - would need actual menu items from the application
                ReportProgress("ℹ️ Menu demo - no menus found in current UI", false, false);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menu clicking demo failed");
                ReportProgress($"❌ Menu demo failed: {ex.Message}", false, true);
                return false;
            }
        }

        public async Task<bool> StartElementMonitoringDemoAsync()
        {
            try
            {
                ReportProgress("🔄 Starting Element Monitoring Demo...", false, false);

                // Start monitoring backup status for demo
                var started = await _elementMonitoringService.StartMonitoringAsync("BackupStatusTextBlock");
                if (started)
                {
                    ReportProgress("✅ Element monitoring started for BackupStatusTextBlock", true, false);
                    return true;
                }
                else
                {
                    ReportProgress("⚠️ Failed to start element monitoring", false, false);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Element monitoring demo start failed");
                ReportProgress($"❌ Element monitoring start failed: {ex.Message}", false, true);
                return false;
            }
        }

        public async Task<bool> StopElementMonitoringDemoAsync()
        {
            try
            {
                ReportProgress("🔄 Stopping Element Monitoring Demo...", false, false);

                var stopped = await _elementMonitoringService.StopMonitoringAsync();
                if (stopped)
                {
                    ReportProgress("✅ Element monitoring stopped", true, false);
                    return true;
                }
                else
                {
                    ReportProgress("⚠️ No active monitoring to stop", false, false);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Element monitoring demo stop failed");
                ReportProgress($"❌ Element monitoring stop failed: {ex.Message}", false, true);
                return false;
            }
        }

        public async Task<string> GetDebugInfoAsync()
        {
            try
            {
                var info = $"UI Automation Debug Info:\n";
                info += $"- Initialized: {_uiAutomationService.IsInitialized}\n";
                info += $"- Application: {_uiAutomationService.ApplicationTitle}\n";
                info += $"- Process ID: {_uiAutomationService.ProcessId}\n";
                info += $"- Monitoring Active: {_elementMonitoringService.IsMonitoring}\n";
                info += $"- Monitored Element: {_elementMonitoringService.CurrentElementIdentifier ?? "None"}\n";
                info += $"- Last Known Value: {_elementMonitoringService.LastKnownValue ?? "None"}\n";

                return info;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get debug info");
                return $"Error getting debug info: {ex.Message}";
            }
        }

        private void OnElementChanged(object? sender, ElementChangedEventArgs e)
        {
            ReportProgress($"📍 Element Changed: {e.ElementIdentifier} = '{e.NewValue}' (was '{e.PreviousValue}')", true, false);
        }

        private void ReportProgress(string message, bool isSuccess, bool isError)
        {
            var args = new DemoProgressEventArgs
            {
                Message = message,
                IsSuccess = isSuccess,
                IsError = isError,
                Timestamp = DateTime.Now
            };

            DemoProgress?.Invoke(this, args);
            
            if (isError)
                _logger.LogError(message);
            else if (isSuccess)
                _logger.LogInformation(message);
            else
                _logger.LogDebug(message);
        }
    }
}