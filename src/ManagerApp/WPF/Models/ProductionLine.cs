using System.Collections.ObjectModel;
using System.ComponentModel;
using Serilog;

namespace MachineManagerApp.Models
{
    public class ProductionLine : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private bool _isSelected;
        private bool _isIndeterminate;
        private bool _isUpdatingCheckBoxState = false; // Flag to prevent infinite recursion

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

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    var oldValue = _isSelected;
                    _isSelected = value;
                    Log.Debug("📝 FOLDER MODEL - {Name}: IsSelected {Old} -> {New}", Name, oldValue, value);
                    OnPropertyChanged(nameof(IsSelected));
                    
                    if (!_isUpdatingCheckBoxState)
                    {
                        OnPropertyChanged(nameof(CheckBoxState));
                    }
                }
            }
        }

        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set
            {
                if (_isIndeterminate != value)
                {
                    var oldValue = _isIndeterminate;
                    _isIndeterminate = value;
                    Log.Debug("📝 FOLDER MODEL - {Name}: IsIndeterminate {Old} -> {New}", Name, oldValue, value);
                    OnPropertyChanged(nameof(IsIndeterminate));
                    
                    if (!_isUpdatingCheckBoxState)
                    {
                        OnPropertyChanged(nameof(CheckBoxState));
                    }
                }
            }
        }

        /// <summary>
        /// Checkbox state for three-state checkbox: true = checked, false = unchecked, null = indeterminate
        /// </summary>
        public bool? CheckBoxState
        {
            get
            {
                var result = _isIndeterminate ? (bool?)null : _isSelected;
                Log.Debug("📋 FOLDER MODEL - {Name}: CheckBoxState GET -> {State} (IsSelected={Selected}, IsIndeterminate={Indeterminate})", 
                    Name, result, _isSelected, _isIndeterminate);
                return result;
            }
            set
            {
                Log.Debug("📋 FOLDER MODEL - {Name}: CheckBoxState SET to {Value}", Name, value);
                _isUpdatingCheckBoxState = true;
                
                try
                {
                    if (value.HasValue)
                    {
                        Log.Debug("📋 FOLDER MODEL - {Name}: Setting IsSelected={Value}, IsIndeterminate=false", Name, value.Value);
                        IsSelected = value.Value;
                        IsIndeterminate = false;
                    }
                    else
                    {
                        // User clicked when indeterminate -> cycle to unchecked (false)
                        // Don't stay in indeterminate state for user clicks
                        Log.Debug("📋 FOLDER MODEL - {Name}: User clicked indeterminate -> Setting to unchecked (false)", Name);
                        IsSelected = false;
                        IsIndeterminate = false;
                    }
                    
                    // Explicitly notify that CheckBoxState changed
                    OnPropertyChanged(nameof(CheckBoxState));
                }
                finally
                {
                    _isUpdatingCheckBoxState = false;
                }
            }
        }

        public ObservableCollection<Machine> Machines { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}