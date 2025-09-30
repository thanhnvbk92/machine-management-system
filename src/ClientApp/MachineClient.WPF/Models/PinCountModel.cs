using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MachineClient.WPF.Models
{
    public class PinCountModel : INotifyPropertyChanged
    {
        private string _pinName = string.Empty;
        private int _usage;
        private int _lifetime;
        private string _warning = string.Empty;
        private bool _isWarning;
        private bool _isError;

        public string PinName
        {
            get => _pinName;
            set
            {
                _pinName = value;
                OnPropertyChanged();
            }
        }

        public int Usage
        {
            get => _usage;
            set
            {
                _usage = value;
                UpdateWarningStatus();
                OnPropertyChanged();
            }
        }

        public int Lifetime
        {
            get => _lifetime;
            set
            {
                _lifetime = value;
                UpdateWarningStatus();
                OnPropertyChanged();
            }
        }

        public string Warning
        {
            get => _warning;
            set
            {
                _warning = value;
                OnPropertyChanged();
            }
        }

        public bool IsWarning
        {
            get => _isWarning;
            set
            {
                _isWarning = value;
                OnPropertyChanged();
            }
        }

        public bool IsError
        {
            get => _isError;
            set
            {
                _isError = value;
                OnPropertyChanged();
            }
        }

        public double UsagePercentage => _lifetime > 0 ? (double)_usage / _lifetime * 100 : 0;

        private void UpdateWarningStatus()
        {
            var percentage = UsagePercentage;
            
            if (percentage >= 100)
            {
                IsError = true;
                IsWarning = false;
                Warning = "EXCEEDED LIFETIME";
            }
            else if (percentage >= 95)
            {
                IsError = false;
                IsWarning = true;
                Warning = "NEAR LIFETIME";
            }
            else
            {
                IsError = false;
                IsWarning = false;
                Warning = "OK";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}