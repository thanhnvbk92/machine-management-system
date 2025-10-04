using System.ComponentModel;

namespace MachineManagerApp.Models
{
    public enum MachineStatus
    {
        Online,
        Offline,
        Running,
        Stopped,
        Idle,
        Error,
        Maintenance
    }

    public class Machine : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _ipAddress = string.Empty;
        private MachineStatus _status;
        private DateTime _lastUpdated;
        private bool _isSelected;

        public int Id { get; set; }
        
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                OnPropertyChanged(nameof(IpAddress));
            }
        }

        public MachineStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(StatusColor));
            }
        }

        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set
            {
                _lastUpdated = value;
                OnPropertyChanged(nameof(LastUpdated));
            }
        }

        public int LineId { get; set; }
        public string LineName { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public string StatusColor => Status switch
        {
            MachineStatus.Online => "Green",
            MachineStatus.Offline => "Gray",
            MachineStatus.Error => "Red",
            MachineStatus.Maintenance => "Orange",
            _ => "Gray"
        };

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}