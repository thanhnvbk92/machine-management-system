using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace MachineClient.WPF.Services
{
    public class UIStateManager : IUIStateManager, INotifyPropertyChanged
    {
        private readonly ILogger<UIStateManager> _logger;

        // Button states
        private bool _isTestConnectionEnabled = true;
        private bool _isRegisterMachineEnabled = true;
        private bool _isBackupEnabled = true;
        private bool _isUIAutomationEnabled = true;

        // UI visibility states
        private bool _isLoadingVisible = false;
        private bool _isErrorPanelVisible = false;
        private bool _isSuccessPanelVisible = false;
        private bool _isConflictPanelVisible = false;

        // Status display
        private string _statusMessage = "Ready";
        private string _connectionStatus = "Disconnected";
        private string _lastOperationResult = "";

        // Progress tracking
        private int _progressValue = 0;
        private bool _isProgressVisible = false;
        private string _progressText = "";

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<UIStateChangedEventArgs>? UIStateChanged;
        public event EventHandler<StatusMessageChangedEventArgs>? StatusMessageChanged;
        public event EventHandler<ProgressUpdatedEventArgs>? ProgressUpdated;

        public UIStateManager(ILogger<UIStateManager> logger)
        {
            _logger = logger;
        }

        // Button states
        public bool IsTestConnectionEnabled
        {
            get => _isTestConnectionEnabled;
            set => SetProperty(ref _isTestConnectionEnabled, value);
        }

        public bool IsRegisterMachineEnabled
        {
            get => _isRegisterMachineEnabled;
            set => SetProperty(ref _isRegisterMachineEnabled, value);
        }

        public bool IsBackupEnabled
        {
            get => _isBackupEnabled;
            set => SetProperty(ref _isBackupEnabled, value);
        }

        public bool IsUIAutomationEnabled
        {
            get => _isUIAutomationEnabled;
            set => SetProperty(ref _isUIAutomationEnabled, value);
        }

        // UI visibility states
        public bool IsLoadingVisible
        {
            get => _isLoadingVisible;
            set => SetProperty(ref _isLoadingVisible, value);
        }

        public bool IsErrorPanelVisible
        {
            get => _isErrorPanelVisible;
            set => SetProperty(ref _isErrorPanelVisible, value);
        }

        public bool IsSuccessPanelVisible
        {
            get => _isSuccessPanelVisible;
            set => SetProperty(ref _isSuccessPanelVisible, value);
        }

        public bool IsConflictPanelVisible
        {
            get => _isConflictPanelVisible;
            set => SetProperty(ref _isConflictPanelVisible, value);
        }

        // Status display
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => SetProperty(ref _connectionStatus, value);
        }

        public string LastOperationResult
        {
            get => _lastOperationResult;
            set => SetProperty(ref _lastOperationResult, value);
        }

        // Progress tracking
        public int ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        public bool IsProgressVisible
        {
            get => _isProgressVisible;
            set => SetProperty(ref _isProgressVisible, value);
        }

        public string ProgressText
        {
            get => _progressText;
            set => SetProperty(ref _progressText, value);
        }

        // State management methods
        public void SetBusyState(string message)
        {
            _logger.LogInformation("Setting UI to busy state: {Message}", message);
            
            StatusMessage = message;
            IsLoadingVisible = true;
            IsErrorPanelVisible = false;
            IsSuccessPanelVisible = false;
            IsConflictPanelVisible = false;
            DisableAllButtons();

            StatusMessageChanged?.Invoke(this, new StatusMessageChangedEventArgs
            {
                Message = message,
                Type = MessageType.Info
            });
        }

        public void SetIdleState()
        {
            _logger.LogInformation("Setting UI to idle state");
            
            StatusMessage = "Ready";
            IsLoadingVisible = false;
            IsProgressVisible = false;
            EnableAllButtons();
        }

        public void SetErrorState(string errorMessage)
        {
            _logger.LogWarning("Setting UI to error state: {ErrorMessage}", errorMessage);
            
            StatusMessage = errorMessage;
            LastOperationResult = errorMessage;
            IsLoadingVisible = false;
            IsErrorPanelVisible = true;
            IsSuccessPanelVisible = false;
            IsConflictPanelVisible = false;
            IsProgressVisible = false;
            EnableAllButtons();

            StatusMessageChanged?.Invoke(this, new StatusMessageChangedEventArgs
            {
                Message = errorMessage,
                Type = MessageType.Error
            });
        }

        public void SetSuccessState(string successMessage)
        {
            _logger.LogInformation("Setting UI to success state: {SuccessMessage}", successMessage);
            
            StatusMessage = successMessage;
            LastOperationResult = successMessage;
            IsLoadingVisible = false;
            IsErrorPanelVisible = false;
            IsSuccessPanelVisible = true;
            IsConflictPanelVisible = false;
            IsProgressVisible = false;
            EnableAllButtons();

            StatusMessageChanged?.Invoke(this, new StatusMessageChangedEventArgs
            {
                Message = successMessage,
                Type = MessageType.Success
            });
        }

        public void SetConflictState(string conflictMessage)
        {
            _logger.LogWarning("Setting UI to conflict state: {ConflictMessage}", conflictMessage);
            
            StatusMessage = conflictMessage;
            LastOperationResult = conflictMessage;
            IsLoadingVisible = false;
            IsErrorPanelVisible = false;
            IsSuccessPanelVisible = false;
            IsConflictPanelVisible = true;
            IsProgressVisible = false;
            EnableAllButtons();

            StatusMessageChanged?.Invoke(this, new StatusMessageChangedEventArgs
            {
                Message = conflictMessage,
                Type = MessageType.Warning
            });
        }

        public void UpdateProgress(int percentage, string text)
        {
            ProgressValue = percentage;
            ProgressText = text;
            IsProgressVisible = true;

            ProgressUpdated?.Invoke(this, new ProgressUpdatedEventArgs
            {
                Percentage = percentage,
                Text = text,
                IsVisible = true
            });
        }

        public void ResetAllStates()
        {
            _logger.LogInformation("Resetting all UI states");
            
            IsLoadingVisible = false;
            IsErrorPanelVisible = false;
            IsSuccessPanelVisible = false;
            IsConflictPanelVisible = false;
            IsProgressVisible = false;
            StatusMessage = "Ready";
            LastOperationResult = "";
            ProgressValue = 0;
            ProgressText = "";
            EnableAllButtons();
        }

        public void EnableAllButtons()
        {
            SetButtonStates(true, true, true, true);
        }

        public void DisableAllButtons()
        {
            SetButtonStates(false, false, false, false);
        }

        public void SetButtonStates(bool testConnection, bool registerMachine, bool backup, bool automation)
        {
            IsTestConnectionEnabled = testConnection;
            IsRegisterMachineEnabled = registerMachine;
            IsBackupEnabled = backup;
            IsUIAutomationEnabled = automation;
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            var oldValue = field;
            field = value;
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
            UIStateChanged?.Invoke(this, new UIStateChangedEventArgs
            {
                PropertyName = propertyName ?? string.Empty,
                OldValue = oldValue,
                NewValue = value
            });
        }
    }
}