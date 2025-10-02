using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MachineClient.WPF.ViewModels
{
    /// <summary>
    /// ViewModel for navigation management - handles page switching and navigation state
    /// </summary>
    public partial class NavigationViewModel : ObservableObject
    {
        private readonly ILogger<NavigationViewModel> _logger;

        #region Navigation Properties

        [ObservableProperty]
        private string _selectedPage = "Home";

        [ObservableProperty]
        private bool _isHomePage = true;

        [ObservableProperty]
        private bool _isSettingsPage = false;

        [ObservableProperty]
        private bool _isAboutPage = false;

        #endregion

        public NavigationViewModel(ILogger<NavigationViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("NavigationViewModel initialized with Home page selected");
            
            // Debug initial state
            _logger.LogInformation($"Initial state: IsHomePage={IsHomePage}, IsSettingsPage={IsSettingsPage}, IsAboutPage={IsAboutPage}");
        }

        #region Navigation Commands

        [RelayCommand]
        private void NavigateToHome()
        {
            _logger.LogInformation("=== NAVIGATION: NavigateToHome command executed ===");
            
            SelectedPage = "Home";
            IsHomePage = true;
            IsSettingsPage = false;
            IsAboutPage = false;
            
            // Manually fire PropertyChanged events
            OnPropertyChanged(nameof(SelectedPage));
            OnPropertyChanged(nameof(IsHomePage));
            OnPropertyChanged(nameof(IsSettingsPage));
            OnPropertyChanged(nameof(IsAboutPage));
            
            _logger.LogInformation($"Navigation state set: IsHomePage={IsHomePage}, IsSettingsPage={IsSettingsPage}, IsAboutPage={IsAboutPage}");
            
            // Force write to file immediately
            System.IO.File.AppendAllText(@"f:\Dev\Projects\C#\Project .NET\Machine Mangement System\src\ClientApp\MachineClient.WPF\navigation_debug.log", 
                $"{DateTime.Now:HH:mm:ss} - NavigateToHome executed, IsHomePage={IsHomePage}\n");
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            _logger.LogInformation("=== NAVIGATION: NavigateToSettings command executed ===");
            
            SelectedPage = "Settings";
            IsHomePage = false;
            IsSettingsPage = true;
            IsAboutPage = false;
            
            // Manually fire PropertyChanged events
            OnPropertyChanged(nameof(SelectedPage));
            OnPropertyChanged(nameof(IsHomePage));
            OnPropertyChanged(nameof(IsSettingsPage));
            OnPropertyChanged(nameof(IsAboutPage));
            
            _logger.LogInformation($"Navigation state set: IsHomePage={IsHomePage}, IsSettingsPage={IsSettingsPage}, IsAboutPage={IsAboutPage}");
            
            // Force write to file immediately
            System.IO.File.AppendAllText(@"f:\Dev\Projects\C#\Project .NET\Machine Mangement System\src\ClientApp\MachineClient.WPF\navigation_debug.log", 
                $"{DateTime.Now:HH:mm:ss} - NavigateToSettings executed, IsSettingsPage={IsSettingsPage}\n");
        }

        [RelayCommand]
        private void NavigateToAbout()
        {
            _logger.LogInformation("=== NAVIGATION: NavigateToAbout command executed ===");
            
            SelectedPage = "About";
            IsHomePage = false;
            IsSettingsPage = false;
            IsAboutPage = true;
            
            // Manually fire PropertyChanged events
            OnPropertyChanged(nameof(SelectedPage));
            OnPropertyChanged(nameof(IsHomePage));
            OnPropertyChanged(nameof(IsSettingsPage));
            OnPropertyChanged(nameof(IsAboutPage));
            
            _logger.LogInformation($"Navigation state set: IsHomePage={IsHomePage}, IsSettingsPage={IsSettingsPage}, IsAboutPage={IsAboutPage}");
            
            // Force write to file immediately
            System.IO.File.AppendAllText(@"f:\Dev\Projects\C#\Project .NET\Machine Mangement System\src\ClientApp\MachineClient.WPF\navigation_debug.log", 
                $"{DateTime.Now:HH:mm:ss} - NavigateToAbout executed, IsAboutPage={IsAboutPage}\n");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Navigate to a specific page by name
        /// </summary>
        /// <param name="pageName">Name of the page to navigate to</param>
        public void NavigateToPage(string pageName)
        {
            switch (pageName?.ToLower())
            {
                case "home":
                    NavigateToHome();
                    break;
                case "settings":
                    NavigateToSettings();
                    break;
                case "about":
                    NavigateToAbout();
                    break;
                default:
                    _logger.LogWarning("Unknown page name: {PageName}. Defaulting to Home", pageName);
                    NavigateToHome();
                    break;
            }
        }

        /// <summary>
        /// Get the current active page name
        /// </summary>
        /// <returns>Name of the currently active page</returns>
        public string GetCurrentPage()
        {
            return SelectedPage;
        }

        /// <summary>
        /// Check if a specific page is currently active
        /// </summary>
        /// <param name="pageName">Name of the page to check</param>
        /// <returns>True if the page is active, false otherwise</returns>
        public bool IsPageActive(string pageName)
        {
            return string.Equals(SelectedPage, pageName, System.StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}