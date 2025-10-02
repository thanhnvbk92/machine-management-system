using System;

namespace MachineClient.WPF.Services
{
    public interface IUIStateManager
    {
        // Button states
        bool IsTestConnectionEnabled { get; set; }
        bool IsRegisterMachineEnabled { get; set; }
        bool IsBackupEnabled { get; set; }
        bool IsUIAutomationEnabled { get; set; }

        // UI visibility states
        bool IsLoadingVisible { get; set; }
        bool IsErrorPanelVisible { get; set; }
        bool IsSuccessPanelVisible { get; set; }
        bool IsConflictPanelVisible { get; set; }

        // Status display
        string StatusMessage { get; set; }
        string ConnectionStatus { get; set; }
        string LastOperationResult { get; set; }

        // Progress tracking
        int ProgressValue { get; set; }
        bool IsProgressVisible { get; set; }
        string ProgressText { get; set; }

        // Events for UI updates
        event EventHandler<UIStateChangedEventArgs>? UIStateChanged;
        event EventHandler<StatusMessageChangedEventArgs>? StatusMessageChanged;
        event EventHandler<ProgressUpdatedEventArgs>? ProgressUpdated;

        // State management methods
        void SetBusyState(string message);
        void SetIdleState();
        void SetErrorState(string errorMessage);
        void SetSuccessState(string successMessage);
        void SetConflictState(string conflictMessage);
        void UpdateProgress(int percentage, string text);
        void ResetAllStates();

        // Button state management
        void EnableAllButtons();
        void DisableAllButtons();
        void SetButtonStates(bool testConnection, bool registerMachine, bool backup, bool automation);
    }

    public class UIStateChangedEventArgs : EventArgs
    {
        public string PropertyName { get; set; } = string.Empty;
        public object? OldValue { get; set; }
        public object? NewValue { get; set; }
    }

    public class StatusMessageChangedEventArgs : EventArgs
    {
        public string Message { get; set; } = string.Empty;
        public MessageType Type { get; set; }
    }

    public class ProgressUpdatedEventArgs : EventArgs
    {
        public int Percentage { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsVisible { get; set; }
    }

    public enum MessageType
    {
        Info,
        Success,
        Warning,
        Error
    }
}