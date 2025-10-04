using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MachineManagerApp.Models;
using MachineManagerApp.Services;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MachineManagerApp.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IMachineService _machineService;
        private readonly ILogger<MainViewModel> _logger;

        [ObservableProperty]
        private ObservableCollection<ProductionLine> _productionLines = new();

        [ObservableProperty]
        private ProductionLine? _selectedLine;

        [ObservableProperty]
        private Machine? _selectedMachine;

        [ObservableProperty]
        private ObservableCollection<Machine> _selectedMachines = new();

        [ObservableProperty]
        private string _commandText = string.Empty;

        [ObservableProperty]
        private string _logMessages = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        public MainViewModel(IMachineService machineService, ILogger<MainViewModel> logger)
        {
            _machineService = machineService;
            _logger = logger;
            
            // Load initial data
            _ = LoadDataAsync();
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                _logger.LogInformation("Loading production lines data...");
                AddLogMessage("Loading production lines...");

                var lines = await _machineService.GetProductionLinesAsync();
                ProductionLines = lines;

                // Subscribe to property changes for ProductionLine checkboxes
                foreach (var line in ProductionLines)
                {
                    line.PropertyChanged += ProductionLine_PropertyChanged;
                    
                    // Subscribe to machine property changes within each line
                    foreach (var machine in line.Machines)
                    {
                        machine.PropertyChanged += Machine_PropertyChanged;
                    }
                }

                _logger.LogInformation("Loaded {LineCount} production lines", lines.Count);
                AddLogMessage($"Loaded {lines.Count} production lines successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading production lines");
                AddLogMessage($"Error loading data: {ex.Message}");
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ProductionLine_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                _logger.LogDebug("ProductionLine_PropertyChanged triggered: Property = {PropertyName}, _isUpdatingSelection = {Flag}, _isUpdatingFromMachines = {MachineFlag}", 
                    e.PropertyName, _isUpdatingSelection, _isUpdatingFromMachines);
                
                if (e.PropertyName == nameof(ProductionLine.IsSelected) && sender is ProductionLine line)
                {
                    // Log detailed state before processing
                    var selectedMachines = line.Machines.Count(m => m.IsSelected);
                    var totalMachines = line.Machines.Count;
                    _logger.LogDebug("🔍 FOLDER CLICK - Line {LineName}: IsSelected={State}, IsIndeterminate={Indeterminate}, Machines={Selected}/{Total}", 
                        line.Name, line.IsSelected, line.IsIndeterminate, selectedMachines, totalMachines);
                    Console.WriteLine($"🔍 FOLDER CLICK - {line.Name}: IsSelected={line.IsSelected}, IsIndeterminate={line.IsIndeterminate}, Machines={selectedMachines}/{totalMachines}");
                    
                    _logger.LogDebug("IsSelected changed for line {LineName}: {State}", line.Name, line.IsSelected);
                    
                    // Only cascade if this is NOT coming from machine updates
                    bool shouldProcess = !_isUpdatingSelection && !_isUpdatingFromMachines;
                    
                    _logger.LogDebug("🔧 PROCESS CHECK - Line {LineName}: shouldProcess={ShouldProcess} (_isUpdatingSelection={UpdatingFlag}, _isUpdatingFromMachines={MachineFlag})", 
                        line.Name, shouldProcess, _isUpdatingSelection, _isUpdatingFromMachines);
                    Console.WriteLine($"🔧 PROCESS CHECK - {line.Name}: shouldProcess={shouldProcess} (updating={_isUpdatingSelection}, fromMachines={_isUpdatingFromMachines})");
                    
                    if (shouldProcess)
                    {
                        _isUpdatingSelection = true; // Flag to prevent infinite recursion
                        
                        try
                        {
                            // Clear indeterminate state when user explicitly selects/deselects
                            line.IsIndeterminate = false;
                            
                            _logger.LogDebug("Cascading IsSelected={State} to {MachineCount} machines in line {LineName}", 
                                line.IsSelected, line.Machines.Count, line.Name);
                            
                            // Cascade to all machines
                            foreach (var machine in line.Machines)
                            {
                                var oldState = machine.IsSelected;
                                machine.IsSelected = line.IsSelected;
                                _logger.LogDebug("Machine {MachineName}: {OldState} -> {NewState}", 
                                    machine.Name, oldState, machine.IsSelected);
                            }
                            
                            // Log final state after cascade
                            var finalSelected = line.Machines.Count(m => m.IsSelected);
                            _logger.LogDebug("✅ CASCADE COMPLETE - Line {LineName}: Machines now {Selected}/{Total}", 
                                line.Name, finalSelected, line.Machines.Count);
                            Console.WriteLine($"✅ CASCADE COMPLETE - {line.Name}: Machines now {finalSelected}/{line.Machines.Count}");
                            
                            // If this line is currently selected in TreeView, refresh the ListView
                            if (SelectedLine == line)
                            {
                                // Trigger PropertyChanged for SelectedMachines to refresh ListView
                                OnPropertyChanged(nameof(SelectedMachines));
                                _logger.LogDebug("Triggered PropertyChanged for SelectedMachines to refresh ListView for line {LineName}", line.Name);
                            }
                            
                            _logger.LogInformation("Production line {LineName} checkbox clicked: {State}", 
                                line.Name, line.IsSelected ? "Selected" : "Deselected");
                            AddLogMessage($"Production line {line.Name}: {(line.IsSelected ? "Selected all machines" : "Deselected all machines")}");
                            
                            NotifyLineSelectionCommandsCanExecuteChanged();
                            NotifyMultiSelectionCommandsCanExecuteChanged();
                        }
                        finally
                        {
                            _isUpdatingSelection = false;
                        }
                    }
                    else
                    {
                        _logger.LogDebug("🚫 SKIPPING CASCADE for line {LineName} - _isUpdatingSelection={UpdatingFlag}, _isUpdatingFromMachines={MachineFlag}", 
                            line.Name, _isUpdatingSelection, _isUpdatingFromMachines);
                        Console.WriteLine($"🚫 SKIPPING CASCADE - {line.Name}: updating={_isUpdatingSelection}, fromMachines={_isUpdatingFromMachines}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProductionLine_PropertyChanged");
                AddLogMessage($"❌ Error updating production line: {ex.Message}");
                _isUpdatingSelection = false; // Reset flag on error
            }
        }

        private bool _isUpdatingSelection = false;
        private bool _isUpdatingFromMachines = false; // Separate flag for machine->line updates

        private void Machine_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == nameof(Machine.IsSelected) && !_isUpdatingSelection && sender is Machine machine)
                {
                    _logger.LogDebug("🔧 MACHINE CLICK - Machine {MachineName}: IsSelected={State}", machine.Name, machine.IsSelected);
                    Console.WriteLine($"🔧 MACHINE CLICK - {machine.Name}: IsSelected={machine.IsSelected}");
                    
                    // Update parent line selection based on machine selections
                    var parentLine = ProductionLines.FirstOrDefault(line => line.Machines.Contains(machine));
                    if (parentLine != null)
                    {
                        _isUpdatingFromMachines = true; // Use separate flag for machine->line updates
                        
                        try
                        {
                            // Check selection state of machines in the line
                            var selectedCount = parentLine.Machines.Count(m => m.IsSelected);
                            var totalCount = parentLine.Machines.Count;
                            
                            _logger.LogDebug("📊 UPDATING FOLDER STATE - Line {LineName}: {Selected}/{Total} machines selected", 
                                parentLine.Name, selectedCount, totalCount);
                            
                            var oldSelected = parentLine.IsSelected;
                            var oldIndeterminate = parentLine.IsIndeterminate;
                            
                            if (selectedCount == 0)
                            {
                                // No machines selected
                                parentLine.IsSelected = false;
                                parentLine.IsIndeterminate = false;
                                _logger.LogDebug("🔲 FOLDER STATE: None selected -> IsSelected=false, IsIndeterminate=false");
                            }
                            else if (selectedCount == totalCount)
                            {
                                // All machines selected
                                parentLine.IsSelected = true;
                                parentLine.IsIndeterminate = false;
                                _logger.LogDebug("☑️ FOLDER STATE: All selected -> IsSelected=true, IsIndeterminate=false");
                            }
                            else
                            {
                                // Partial selection - show indeterminate state
                                parentLine.IsSelected = false; // Checkbox not checked but indeterminate
                                parentLine.IsIndeterminate = true;
                                _logger.LogDebug("⬛ FOLDER STATE: Partial selected -> IsSelected=false, IsIndeterminate=true");
                            }
                            
                            _logger.LogDebug("🔄 FOLDER CHANGE - Line {LineName}: ({OldSel},{OldInd}) -> ({NewSel},{NewInd})", 
                                parentLine.Name, oldSelected, oldIndeterminate, parentLine.IsSelected, parentLine.IsIndeterminate);
                            Console.WriteLine($"🔄 FOLDER CHANGE - {parentLine.Name}: ({oldSelected},{oldIndeterminate}) -> ({parentLine.IsSelected},{parentLine.IsIndeterminate})");
                        }
                        finally
                        {
                            _isUpdatingFromMachines = false;
                        }
                    }
                    
                    NotifyMultiSelectionCommandsCanExecuteChanged();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Machine_PropertyChanged");
                AddLogMessage($"❌ Error updating machine selection: {ex.Message}");
                _isUpdatingFromMachines = false; // Reset flag on error
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteMachineCommand))]
        private async Task StartMachineAsync()
        {
            if (SelectedMachine == null) return;

            try
            {
                _logger.LogInformation("Starting machine: {MachineName}", SelectedMachine.Name);
                AddLogMessage($"Starting machine: {SelectedMachine.Name}...");

                var command = new Command { Type = "START", Parameters = "force=true" };
                var result = await _machineService.SendCommandAsync(SelectedMachine.Id, command);

                if (result.Success)
                {
                    SelectedMachine.Status = MachineStatus.Running;
                    _logger.LogInformation("Machine {MachineName} started successfully", SelectedMachine.Name);
                    AddLogMessage($"✅ Machine {SelectedMachine.Name} started successfully");
                }
                else
                {
                    _logger.LogWarning("Failed to start machine {MachineName}: {Error}", SelectedMachine.Name, result.ErrorMessage);
                    AddLogMessage($"❌ Failed to start machine: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting machine {MachineName}", SelectedMachine.Name);
                AddLogMessage($"❌ Error starting machine: {ex.Message}");
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteMachineCommand))]
        private async Task StopMachineAsync()
        {
            if (SelectedMachine == null) return;

            try
            {
                _logger.LogInformation("Stopping machine: {MachineName}", SelectedMachine.Name);
                AddLogMessage($"Stopping machine: {SelectedMachine.Name}...");

                var command = new Command { Type = "STOP", Parameters = "graceful=true" };
                var result = await _machineService.SendCommandAsync(SelectedMachine.Id, command);

                if (result.Success)
                {
                    SelectedMachine.Status = MachineStatus.Stopped;
                    _logger.LogInformation("Machine {MachineName} stopped successfully", SelectedMachine.Name);
                    AddLogMessage($"✅ Machine {SelectedMachine.Name} stopped successfully");
                }
                else
                {
                    _logger.LogWarning("Failed to stop machine {MachineName}: {Error}", SelectedMachine.Name, result.ErrorMessage);
                    AddLogMessage($"❌ Failed to stop machine: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping machine {MachineName}", SelectedMachine.Name);
                AddLogMessage($"❌ Error stopping machine: {ex.Message}");
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteMachineCommand))]
        private async Task RestartMachineAsync()
        {
            if (SelectedMachine == null) return;

            try
            {
                _logger.LogInformation("Restarting machine: {MachineName}", SelectedMachine.Name);
                AddLogMessage($"Restarting machine: {SelectedMachine.Name}...");

                var command = new Command { Type = "RESTART", Parameters = "delay=5" };
                var result = await _machineService.SendCommandAsync(SelectedMachine.Id, command);

                if (result.Success)
                {
                    SelectedMachine.Status = MachineStatus.Running;
                    _logger.LogInformation("Machine {MachineName} restarted successfully", SelectedMachine.Name);
                    AddLogMessage($"✅ Machine {SelectedMachine.Name} restarted successfully");
                }
                else
                {
                    _logger.LogWarning("Failed to restart machine {MachineName}: {Error}", SelectedMachine.Name, result.ErrorMessage);
                    AddLogMessage($"❌ Failed to restart machine: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restarting machine {MachineName}", SelectedMachine.Name);
                AddLogMessage($"❌ Error restarting machine: {ex.Message}");
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteMachineCommand))]
        private async Task ResetMachineAsync()
        {
            if (SelectedMachine == null) return;

            try
            {
                _logger.LogInformation("Resetting machine: {MachineName}", SelectedMachine.Name);
                AddLogMessage($"Resetting machine: {SelectedMachine.Name}...");

                var command = new Command { Type = "RESET", Parameters = "hard=true" };
                var result = await _machineService.SendCommandAsync(SelectedMachine.Id, command);

                if (result.Success)
                {
                    SelectedMachine.Status = MachineStatus.Offline;
                    _logger.LogInformation("Machine {MachineName} reset successfully", SelectedMachine.Name);
                    AddLogMessage($"✅ Machine {SelectedMachine.Name} reset successfully");
                }
                else
                {
                    _logger.LogWarning("Failed to reset machine {MachineName}: {Error}", SelectedMachine.Name, result.ErrorMessage);
                    AddLogMessage($"❌ Failed to reset machine: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting machine {MachineName}", SelectedMachine.Name);
                AddLogMessage($"❌ Error resetting machine: {ex.Message}");
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteMachineCommand))]
        private async Task UpdateStatusAsync()
        {
            if (SelectedMachine == null) return;

            try
            {
                _logger.LogInformation("Updating status for machine: {MachineName}", SelectedMachine.Name);
                AddLogMessage($"Updating status for machine: {SelectedMachine.Name}...");

                var command = new Command { Type = "STATUS", Parameters = "detailed=true" };
                var result = await _machineService.SendCommandAsync(SelectedMachine.Id, command);

                if (result.Success)
                {
                    // Simulate status update
                    var random = new Random();
                    var statuses = new[] { MachineStatus.Running, MachineStatus.Idle, MachineStatus.Error };
                    SelectedMachine.Status = statuses[random.Next(statuses.Length)];
                    
                    _logger.LogInformation("Status updated for machine {MachineName}: {Status}", SelectedMachine.Name, SelectedMachine.Status);
                    AddLogMessage($"✅ Status updated for {SelectedMachine.Name}: {SelectedMachine.Status}");
                }
                else
                {
                    _logger.LogWarning("Failed to update status for machine {MachineName}: {Error}", SelectedMachine.Name, result.ErrorMessage);
                    AddLogMessage($"❌ Failed to update status: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for machine {MachineName}", SelectedMachine.Name);
                AddLogMessage($"❌ Error updating status: {ex.Message}");
            }
        }

        [RelayCommand(CanExecute = nameof(CanSendCustomCommand))]
        private async Task SendCustomCommandAsync()
        {
            if (SelectedMachine == null || string.IsNullOrWhiteSpace(CommandText)) return;

            try
            {
                _logger.LogInformation("Sending custom command to machine {MachineName}: {Command}", SelectedMachine.Name, CommandText);
                AddLogMessage($"Sending command to {SelectedMachine.Name}: {CommandText}");

                var command = new Command { Type = "CUSTOM", Parameters = CommandText };
                var result = await _machineService.SendCommandAsync(SelectedMachine.Id, command);

                if (result.Success)
                {
                    _logger.LogInformation("Custom command sent successfully to machine {MachineName}", SelectedMachine.Name);
                    AddLogMessage($"✅ Command sent successfully to {SelectedMachine.Name}");
                    CommandText = string.Empty; // Clear the input
                }
                else
                {
                    _logger.LogWarning("Failed to send custom command to machine {MachineName}: {Error}", SelectedMachine.Name, result.ErrorMessage);
                    AddLogMessage($"❌ Failed to send command: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending custom command to machine {MachineName}", SelectedMachine.Name);
                AddLogMessage($"❌ Error sending command: {ex.Message}");
            }
        }

        // Multi-selection commands
        [RelayCommand(CanExecute = nameof(CanExecuteMultiMachineCommand))]
        private async Task StartSelectedMachinesAsync()
        {
            var selectedMachines = GetSelectedMachines();
            if (!selectedMachines.Any()) return;

            try
            {
                _logger.LogInformation("Starting {Count} selected machines", selectedMachines.Count);
                AddLogMessage($"Starting {selectedMachines.Count} selected machines...");

                var tasks = selectedMachines.Select(async machine =>
                {
                    var command = new Command { Type = "START", Parameters = "force=true" };
                    var result = await _machineService.SendCommandAsync(machine.Id, command);
                    
                    if (result.Success)
                    {
                        machine.Status = MachineStatus.Running;
                        AddLogMessage($"✅ {machine.Name} started successfully");
                    }
                    else
                    {
                        AddLogMessage($"❌ Failed to start {machine.Name}: {result.ErrorMessage}");
                    }
                });

                await Task.WhenAll(tasks);
                _logger.LogInformation("Completed starting selected machines");
                AddLogMessage("✅ Completed starting all selected machines");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting selected machines");
                AddLogMessage($"❌ Error starting machines: {ex.Message}");
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteMultiMachineCommand))]
        private async Task StopSelectedMachinesAsync()
        {
            var selectedMachines = GetSelectedMachines();
            if (!selectedMachines.Any()) return;

            try
            {
                _logger.LogInformation("Stopping {Count} selected machines", selectedMachines.Count);
                AddLogMessage($"Stopping {selectedMachines.Count} selected machines...");

                var tasks = selectedMachines.Select(async machine =>
                {
                    var command = new Command { Type = "STOP", Parameters = "graceful=true" };
                    var result = await _machineService.SendCommandAsync(machine.Id, command);
                    
                    if (result.Success)
                    {
                        machine.Status = MachineStatus.Stopped;
                        AddLogMessage($"✅ {machine.Name} stopped successfully");
                    }
                    else
                    {
                        AddLogMessage($"❌ Failed to stop {machine.Name}: {result.ErrorMessage}");
                    }
                });

                await Task.WhenAll(tasks);
                _logger.LogInformation("Completed stopping selected machines");
                AddLogMessage("✅ Completed stopping all selected machines");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping selected machines");
                AddLogMessage($"❌ Error stopping machines: {ex.Message}");
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteMultiMachineCommand))]
        private async Task RestartSelectedMachinesAsync()
        {
            var selectedMachines = GetSelectedMachines();
            if (!selectedMachines.Any()) return;

            try
            {
                _logger.LogInformation("Restarting {Count} selected machines", selectedMachines.Count);
                AddLogMessage($"Restarting {selectedMachines.Count} selected machines...");

                var tasks = selectedMachines.Select(async machine =>
                {
                    var command = new Command { Type = "RESTART", Parameters = "delay=5" };
                    var result = await _machineService.SendCommandAsync(machine.Id, command);
                    
                    if (result.Success)
                    {
                        machine.Status = MachineStatus.Running;
                        AddLogMessage($"✅ {machine.Name} restarted successfully");
                    }
                    else
                    {
                        AddLogMessage($"❌ Failed to restart {machine.Name}: {result.ErrorMessage}");
                    }
                });

                await Task.WhenAll(tasks);
                _logger.LogInformation("Completed restarting selected machines");
                AddLogMessage("✅ Completed restarting all selected machines");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restarting selected machines");
                AddLogMessage($"❌ Error restarting machines: {ex.Message}");
            }
        }

        [RelayCommand]
        private void SelectAllMachines()
        {
            foreach (var machine in SelectedMachines)
            {
                machine.IsSelected = true;
            }
            AddLogMessage($"Selected all {SelectedMachines.Count} machines");
            NotifyMultiSelectionCommandsCanExecuteChanged();
        }

        [RelayCommand]
        private void DeselectAllMachines()
        {
            foreach (var machine in SelectedMachines)
            {
                machine.IsSelected = false;
            }
            AddLogMessage("Deselected all machines");
            NotifyMultiSelectionCommandsCanExecuteChanged();
        }

        // ProductionLine multi-selection commands
        [RelayCommand(CanExecute = nameof(CanExecuteMultiLineCommand))]
        private async Task StartSelectedLinesAsync()
        {
            var selectedLines = GetSelectedProductionLines();
            if (!selectedLines.Any()) return;

            try
            {
                var totalMachines = selectedLines.SelectMany(line => line.Machines).Count();
                _logger.LogInformation("Starting {LineCount} production lines with {MachineCount} machines", 
                    selectedLines.Count, totalMachines);
                AddLogMessage($"Starting {selectedLines.Count} production lines ({totalMachines} machines)...");

                var tasks = selectedLines.SelectMany(line => line.Machines).Select(async machine =>
                {
                    var command = new Command { Type = "START", Parameters = "force=true" };
                    var result = await _machineService.SendCommandAsync(machine.Id, command);
                    
                    if (result.Success)
                    {
                        machine.Status = MachineStatus.Running;
                        AddLogMessage($"✅ {machine.Name} in {machine.LineName} started successfully");
                    }
                    else
                    {
                        AddLogMessage($"❌ Failed to start {machine.Name}: {result.ErrorMessage}");
                    }
                });

                await Task.WhenAll(tasks);
                _logger.LogInformation("Completed starting selected production lines");
                AddLogMessage("✅ Completed starting all selected production lines");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting selected production lines");
                AddLogMessage($"❌ Error starting production lines: {ex.Message}");
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteMultiLineCommand))]
        private async Task StopSelectedLinesAsync()
        {
            var selectedLines = GetSelectedProductionLines();
            if (!selectedLines.Any()) return;

            try
            {
                var totalMachines = selectedLines.SelectMany(line => line.Machines).Count();
                _logger.LogInformation("Stopping {LineCount} production lines with {MachineCount} machines", 
                    selectedLines.Count, totalMachines);
                AddLogMessage($"Stopping {selectedLines.Count} production lines ({totalMachines} machines)...");

                var tasks = selectedLines.SelectMany(line => line.Machines).Select(async machine =>
                {
                    var command = new Command { Type = "STOP", Parameters = "graceful=true" };
                    var result = await _machineService.SendCommandAsync(machine.Id, command);
                    
                    if (result.Success)
                    {
                        machine.Status = MachineStatus.Stopped;
                        AddLogMessage($"✅ {machine.Name} in {machine.LineName} stopped successfully");
                    }
                    else
                    {
                        AddLogMessage($"❌ Failed to stop {machine.Name}: {result.ErrorMessage}");
                    }
                });

                await Task.WhenAll(tasks);
                _logger.LogInformation("Completed stopping selected production lines");
                AddLogMessage("✅ Completed stopping all selected production lines");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping selected production lines");
                AddLogMessage($"❌ Error stopping production lines: {ex.Message}");
            }
        }

        [RelayCommand]
        private void SelectAllLines()
        {
            foreach (var line in ProductionLines)
            {
                line.IsSelected = true;
            }
            AddLogMessage($"Selected all {ProductionLines.Count} production lines");
            NotifyLineSelectionCommandsCanExecuteChanged();
        }

        [RelayCommand]
        private void DeselectAllLines()
        {
            foreach (var line in ProductionLines)
            {
                line.IsSelected = false;
            }
            AddLogMessage("Deselected all production lines");
            NotifyLineSelectionCommandsCanExecuteChanged();
        }

        private bool CanExecuteMachineCommand() => SelectedMachine != null;
        private bool CanSendCustomCommand() => SelectedMachine != null && !string.IsNullOrWhiteSpace(CommandText);
        private bool CanExecuteMultiMachineCommand() => GetSelectedMachines().Any();
        private bool CanExecuteMultiLineCommand() => GetSelectedProductionLines().Any();

        private List<Machine> GetSelectedMachines()
        {
            return SelectedMachines.Where(m => m.IsSelected).ToList();
        }

        private List<ProductionLine> GetSelectedProductionLines()
        {
            return ProductionLines.Where(line => line.IsSelected).ToList();
        }

        private void NotifyMultiSelectionCommandsCanExecuteChanged()
        {
            StartSelectedMachinesCommand.NotifyCanExecuteChanged();
            StopSelectedMachinesCommand.NotifyCanExecuteChanged();
            RestartSelectedMachinesCommand.NotifyCanExecuteChanged();
        }

        private void NotifyLineSelectionCommandsCanExecuteChanged()
        {
            StartSelectedLinesCommand.NotifyCanExecuteChanged();
            StopSelectedLinesCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedLineChanged(ProductionLine? value)
        {
            if (value != null)
            {
                SelectedMachines = value.Machines;
                
                _logger.LogInformation("Selected production line: {LineName} with {MachineCount} machines", 
                    value.Name, value.Machines.Count);
                AddLogMessage($"Selected production line: {value.Name} ({value.Machines.Count} machines)");
            }
            else
            {
                SelectedMachines = new ObservableCollection<Machine>();
            }
            
            // Update command states
            StartMachineCommand.NotifyCanExecuteChanged();
            StopMachineCommand.NotifyCanExecuteChanged();
            RestartMachineCommand.NotifyCanExecuteChanged();
            ResetMachineCommand.NotifyCanExecuteChanged();
            UpdateStatusCommand.NotifyCanExecuteChanged();
            NotifyMultiSelectionCommandsCanExecuteChanged();
        }

        partial void OnSelectedMachineChanged(Machine? value)
        {
            if (value != null)
            {
                _logger.LogInformation("Selected machine: {MachineName} (IP: {MachineIP}, Status: {Status})", 
                    value.Name, value.IpAddress, value.Status);
                AddLogMessage($"Selected machine: {value.Name} - {value.IpAddress} ({value.Status})");
            }
            
            // Update command states
            StartMachineCommand.NotifyCanExecuteChanged();
            StopMachineCommand.NotifyCanExecuteChanged();
            RestartMachineCommand.NotifyCanExecuteChanged();
            ResetMachineCommand.NotifyCanExecuteChanged();
            UpdateStatusCommand.NotifyCanExecuteChanged();
            SendCustomCommandCommand.NotifyCanExecuteChanged();
        }

        partial void OnCommandTextChanged(string value)
        {
            SendCustomCommandCommand.NotifyCanExecuteChanged();
        }

        private void AddLogMessage(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            LogMessages += $"[{timestamp}] {message}\n";
        }
    }
}