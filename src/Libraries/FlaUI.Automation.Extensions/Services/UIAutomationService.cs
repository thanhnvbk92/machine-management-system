using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Patterns;
using FlaUI.UIA3;
using Microsoft.Extensions.Logging;

namespace FlaUI.Automation.Extensions.Services
{
    /// <summary>
    /// Core UI Automation service implementation using FlaUI
    /// </summary>
    public class UIAutomationService : IUIAutomationService
    {
        private readonly ILogger<UIAutomationService> _logger;
        private UIA3Automation? _automation;
        private Application? _application;
        private Window? _mainWindow;
        private readonly Dictionary<string, Timer> _timers = new();
        private readonly Dictionary<string, string> _lastTextValues = new();

        public bool IsInitialized => _automation != null && _application != null && _mainWindow != null;
        public string ApplicationTitle { get; private set; } = "";
        public int ProcessId { get; private set; } = 0;

        public UIAutomationService(ILogger<UIAutomationService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> InitializeAsync(string? applicationTitle = null, int? processId = null)
        {
            try
            {
                _automation = new UIA3Automation();
                _logger.LogInformation("UIA3 Automation initialized");

                // Find application by process ID or title
                if (processId.HasValue)
                {
                    var process = Process.GetProcessById(processId.Value);
                    _application = Application.Attach(process);
                    ApplicationTitle = process.ProcessName;
                    ProcessId = processId.Value;
                }
                else if (!string.IsNullOrEmpty(applicationTitle))
                {
                    var processes = Process.GetProcessesByName(applicationTitle);
                    if (processes.Any())
                    {
                        _application = Application.Attach(processes.First());
                        ApplicationTitle = applicationTitle;
                        ProcessId = processes.First().Id;
                    }
                }
                else
                {
                    // Try to find current process or WPF application
                    var currentProcess = Process.GetCurrentProcess();
                    _application = Application.Attach(currentProcess);
                    ApplicationTitle = currentProcess.ProcessName;
                    ProcessId = currentProcess.Id;
                }

                if (_application != null)
                {
                    _mainWindow = _application.GetMainWindow(_automation);
                    if (_mainWindow != null)
                    {
                        _logger.LogInformation("Successfully connected to application: {Title} (PID: {ProcessId})", ApplicationTitle, ProcessId);
                        return true;
                    }
                }

                _logger.LogError("Failed to find application window");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize UI Automation");
                return false;
            }
        }

        public async Task<bool> ClickButtonAsync(string identifier, bool useAutomationId = true)
        {
            try
            {
                var button = await FindElementAsync<Button>(identifier, useAutomationId);
                if (button != null && button.IsEnabled)
                {
                    button.Click();
                    _logger.LogDebug("Clicked button: {Identifier}", identifier);
                    return true;
                }
                
                _logger.LogWarning("Button not found or not enabled: {Identifier}", identifier);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to click button: {Identifier}", identifier);
                return false;
            }
        }

        public async Task<string> ReadTextAsync(string identifier, bool useAutomationId = true)
        {
            try
            {
                var element = await FindElementAsync<AutomationElement>(identifier, useAutomationId);
                if (element != null)
                {
                    var text = element.Name ?? "";
                    _logger.LogDebug("Read text from {Identifier}: {Text}", identifier, text);
                    return text;
                }
                
                _logger.LogWarning("Text element not found: {Identifier}", identifier);
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read text from element: {Identifier}", identifier);
                return "";
            }
        }

        public async Task<string> GetTextBoxValueAsync(string identifier, bool useAutomationId = true)
        {
            try
            {
                var textBox = await FindElementAsync<TextBox>(identifier, useAutomationId);
                if (textBox != null)
                {
                    var text = textBox.Text ?? "";
                    _logger.LogDebug("Read TextBox value from {Identifier}: {Text}", identifier, text);
                    return text;
                }
                
                _logger.LogWarning("TextBox not found: {Identifier}", identifier);
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read TextBox value: {Identifier}", identifier);
                return "";
            }
        }

        public async Task<bool> SetTextBoxValueAsync(string identifier, string value, bool useAutomationId = true)
        {
            try
            {
                var textBox = await FindElementAsync<TextBox>(identifier, useAutomationId);
                if (textBox != null)
                {
                    textBox.Text = value;
                    _logger.LogDebug("Set TextBox value for {Identifier}: {Value}", identifier, value);
                    return true;
                }
                
                _logger.LogWarning("TextBox not found: {Identifier}", identifier);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set TextBox value: {Identifier}", identifier);
                return false;
            }
        }

        public async Task<string> GetComboBoxSelectionAsync(string identifier, bool useAutomationId = true)
        {
            try
            {
                var comboBox = await FindElementAsync<ComboBox>(identifier, useAutomationId);
                if (comboBox != null)
                {
                    var selectedItem = comboBox.SelectedItem;
                    var text = selectedItem?.Name ?? "";
                    _logger.LogDebug("Read ComboBox selection from {Identifier}: {Text}", identifier, text);
                    return text;
                }
                
                _logger.LogWarning("ComboBox not found: {Identifier}", identifier);
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read ComboBox selection: {Identifier}", identifier);
                return "";
            }
        }

        public async Task<bool> SelectComboBoxItemAsync(string identifier, string itemText, bool useAutomationId = true)
        {
            try
            {
                var comboBox = await FindElementAsync<ComboBox>(identifier, useAutomationId);
                if (comboBox != null)
                {
                    var items = comboBox.Items;
                    var targetItem = items.FirstOrDefault(item => item.Name.Contains(itemText, StringComparison.OrdinalIgnoreCase));
                    
                    if (targetItem != null)
                    {
                        targetItem.Select();
                        _logger.LogDebug("Selected ComboBox item {ItemText} in {Identifier}", itemText, identifier);
                        return true;
                    }
                    
                    _logger.LogWarning("ComboBox item not found: {ItemText} in {Identifier}", itemText, identifier);
                    return false;
                }
                
                _logger.LogWarning("ComboBox not found: {Identifier}", identifier);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to select ComboBox item: {ItemText} in {Identifier}", itemText, identifier);
                return false;
            }
        }

        public async Task<bool> ClickMenuItemAsync(string menuText)
        {
            try
            {
                if (_mainWindow != null)
                {
                    var menuItem = _mainWindow.FindFirstDescendant(cf => cf.ByName(menuText).And(cf.ByControlType(ControlType.MenuItem)));
                    if (menuItem != null)
                    {
                        menuItem.Click();
                        _logger.LogDebug("Clicked menu item: {MenuText}", menuText);
                        return true;
                    }
                }
                
                _logger.LogWarning("Menu item not found: {MenuText}", menuText);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to click menu item: {MenuText}", menuText);
                return false;
            }
        }

        public async Task<bool> StartTextBlockMonitoringAsync(string identifier, Action<string> onTextChanged, bool useAutomationId = true)
        {
            try
            {
                // Stop existing monitoring for this identifier
                await StopTextBlockMonitoringAsync(identifier);

                var element = await FindElementAsync<AutomationElement>(identifier, useAutomationId);
                if (element == null)
                {
                    _logger.LogWarning("Element not found for monitoring: {Identifier}", identifier);
                    return false;
                }
                
                // Initialize current value
                var currentText = element.Name ?? "";
                _lastTextValues[identifier] = currentText;

                // Create timer for polling
                var timer = new Timer(500); // Check every 500ms
                timer.Elapsed += async (sender, e) =>
                {
                    try
                    {
                        var newText = element.Name ?? "";
                        if (newText != _lastTextValues[identifier])
                        {
                            _lastTextValues[identifier] = newText;
                            onTextChanged?.Invoke(newText);
                            _logger.LogDebug("TextBlock changed for {Identifier}: {NewText}", identifier, newText);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in TextBlock monitoring for {Identifier}", identifier);
                    }
                };

                _timers[identifier] = timer;
                timer.Start();
                
                _logger.LogInformation("Started TextBlock monitoring for: {Identifier}", identifier);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start TextBlock monitoring for: {Identifier}", identifier);
                return false;
            }
        }

        public async Task<bool> StopTextBlockMonitoringAsync(string identifier)
        {
            try
            {
                if (_timers.TryGetValue(identifier, out var timer))
                {
                    timer.Stop();
                    timer.Dispose();
                    _timers.Remove(identifier);
                    _lastTextValues.Remove(identifier);
                    _logger.LogInformation("Stopped TextBlock monitoring for: {Identifier}", identifier);
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to stop TextBlock monitoring for: {Identifier}", identifier);
                return false;
            }
        }

        public async Task StopAllMonitoringAsync()
        {
            var identifiers = _timers.Keys.ToList();
            foreach (var identifier in identifiers)
            {
                await StopTextBlockMonitoringAsync(identifier);
            }
            _logger.LogInformation("Stopped all TextBlock monitoring");
        }

        private async Task<T?> FindElementAsync<T>(string identifier, bool useAutomationId = true) where T : AutomationElement
        {
            try
            {
                if (_mainWindow == null) return null;

                AutomationElement? element = null;
                
                if (useAutomationId)
                {
                    element = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(identifier));
                }
                else
                {
                    element = _mainWindow.FindFirstDescendant(cf => cf.ByName(identifier));
                }

                return element as T;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to find element: {Identifier}", identifier);
                return null;
            }
        }

        public void Dispose()
        {
            try
            {
                // Stop all monitoring
                Task.Run(async () => await StopAllMonitoringAsync()).Wait();
                
                // Clean up timers
                foreach (var timer in _timers.Values)
                {
                    timer?.Dispose();
                }
                _timers.Clear();
                _lastTextValues.Clear();

                // Dispose FlaUI resources
                _mainWindow = null;
                _application?.Dispose();
                _automation?.Dispose();

                _logger.LogInformation("UI Automation service disposed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing UI Automation service");
            }
        }
    }
}