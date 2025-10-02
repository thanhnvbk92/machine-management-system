using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MachineClient.WPF.ViewModels;

namespace MachineClient.WPF.ViewModels
{
    /// <summary>
    /// Main Shell ViewModel - coordinates child ViewModels and handles application-level concerns
    /// This is the primary ViewModel that ties together all page-specific ViewModels
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        private readonly ILogger<MainViewModel> _logger;

        #region Child ViewModels

        public HomeViewModel Home { get; }
        public SettingsViewModel Settings { get; }
        public AboutViewModel About { get; }
        public NavigationViewModel Navigation { get; }

        #endregion

        #region Application State Properties

        [ObservableProperty]
        private bool _isApplicationReady = false;

        [ObservableProperty]
        private string _applicationTitle = "Machine Management Client";

        [ObservableProperty]
        private DateTime _applicationStartTime = DateTime.Now;

        #endregion

        #region UI Proxy Properties for Backward Compatibility

        // Navigation Properties
        public string SelectedPage => Navigation.SelectedPage;
        public bool IsHomePage => Navigation.IsHomePage;
        public bool IsSettingsPage => Navigation.IsSettingsPage;
        public bool IsAboutPage => Navigation.IsAboutPage;

        // Home Page Properties (most commonly used in UI)
        public string MachineId => Home.MachineId;
        public string MacAddress => Home.MacAddress;
        public string IpAddress => Home.IpAddress;
        public string BuyerName => Home.BuyerName;
        public string LineName => Home.LineName;
        public string StationName => Home.StationName;
        public string ModelName => Home.ModelName;
        public string ProgramName => Home.ProgramName;
        public string Status => Home.Status;
        public bool IsConnected => Home.IsConnected;
        public string ConnectionStatusColor => Home.ConnectionStatusColor;
        public string ConnectionStatusIcon => Home.ConnectionStatusIcon;
        public bool IsMonitoring => Home.IsMonitoring;
        public string LogText => Home.LogText;

        // Collections
        public System.Collections.ObjectModel.ObservableCollection<Models.PinCountModel> PinCounts => Home.PinCounts;

        // Settings Properties (commonly used)
        public string ApiBaseUrl => Settings.ApiBaseUrl;
        public bool AutoStartConnection => Settings.AutoStartConnection;

        #endregion

        #region UI Proxy Commands for Backward Compatibility

        // Navigation Commands with debug logging
        public IRelayCommand NavigateToHomeCommand 
        {
            get
            {
                System.IO.File.AppendAllText(@"f:\Dev\Projects\C#\Project .NET\Machine Mangement System\src\ClientApp\MachineClient.WPF\navigation_debug.log", 
                    $"{DateTime.Now:HH:mm:ss} - NavigateToHomeCommand accessed\n");
                return Navigation.NavigateToHomeCommand;
            }
        }
        
        public IRelayCommand NavigateToSettingsCommand 
        {
            get
            {
                System.IO.File.AppendAllText(@"f:\Dev\Projects\C#\Project .NET\Machine Mangement System\src\ClientApp\MachineClient.WPF\navigation_debug.log", 
                    $"{DateTime.Now:HH:mm:ss} - NavigateToSettingsCommand accessed\n");
                return Navigation.NavigateToSettingsCommand;
            }
        }
        
        public IRelayCommand NavigateToAboutCommand 
        {
            get
            {
                System.IO.File.AppendAllText(@"f:\Dev\Projects\C#\Project .NET\Machine Mangement System\src\ClientApp\MachineClient.WPF\navigation_debug.log", 
                    $"{DateTime.Now:HH:mm:ss} - NavigateToAboutCommand accessed\n");
                return Navigation.NavigateToAboutCommand;
            }
        }

        // Home Commands (most commonly used)
        public IAsyncRelayCommand TestConnectionCommand => Home.TestConnectionCommand;
        public IAsyncRelayCommand StartMonitoringCommand => Home.StartMonitoringCommand;
        public IRelayCommand StopMonitoringCommand => Home.StopMonitoringCommand;
        public IAsyncRelayCommand StartBackupCommand => Home.StartBackupCommand;
        public IRelayCommand StopBackupCommand => Home.StopBackupCommand;
        public IRelayCommand ClearLogsCommand => Home.ClearLogsCommand;

        // Pin Commands
        public IRelayCommand<Models.PinCountModel> ResetPinCommand => Home.ResetPinCommand;

        // About Commands
        public IAsyncRelayCommand CheckUpdateCommand => About.CheckForUpdatesCommand;

        #endregion

        public MainViewModel(
            HomeViewModel homeViewModel,
            SettingsViewModel settingsViewModel,
            AboutViewModel aboutViewModel,
            NavigationViewModel navigationViewModel,
            ILogger<MainViewModel> logger)
        {
            _logger = logger;

            // Initialize child ViewModels
            Home = homeViewModel;
            Settings = settingsViewModel;
            About = aboutViewModel;
            Navigation = navigationViewModel;

            InitializeApplication();
        }

        private void InitializeApplication()
        {
            try
            {
                _logger.LogInformation("MainViewModel Shell initialized with child ViewModels");
                _logger.LogInformation("Application start time: {StartTime}", ApplicationStartTime);

                // Subscribe to navigation events to coordinate between ViewModels if needed
                SubscribeToEvents();

                // Force navigate to Home to ensure correct initial state
                Navigation.NavigateToHomeCommand.Execute(null);
                _logger.LogInformation("Forced navigation to Home page");

                // Mark application as ready
                IsApplicationReady = true;

                // Verify NavigationViewModel state after setup
                _logger.LogInformation($"MainViewModel initialized. Navigation state: IsHomePage={Navigation.IsHomePage}, IsSettingsPage={Navigation.IsSettingsPage}, IsAboutPage={Navigation.IsAboutPage}");
                
                _logger.LogInformation("MainViewModel initialization completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during MainViewModel initialization");
                throw;
            }
        }

        private void SubscribeToEvents()
        {
            // Subscribe to property changes from child ViewModels to propagate to UI
            Home.PropertyChanged += (s, e) => 
            {
                // Forward property change notifications for proxy properties
                switch (e.PropertyName)
                {
                    case nameof(HomeViewModel.MachineId):
                        OnPropertyChanged(nameof(MachineId));
                        break;
                    case nameof(HomeViewModel.Status):
                        OnPropertyChanged(nameof(Status));
                        break;
                    case nameof(HomeViewModel.IsConnected):
                        OnPropertyChanged(nameof(IsConnected));
                        break;
                    case nameof(HomeViewModel.ConnectionStatusColor):
                        OnPropertyChanged(nameof(ConnectionStatusColor));
                        break;
                    case nameof(HomeViewModel.ConnectionStatusIcon):
                        OnPropertyChanged(nameof(ConnectionStatusIcon));
                        break;
                    case nameof(HomeViewModel.LogText):
                        OnPropertyChanged(nameof(LogText));
                        break;
                    case nameof(HomeViewModel.IsMonitoring):
                        OnPropertyChanged(nameof(IsMonitoring));
                        break;
                    case nameof(HomeViewModel.BuyerName):
                        OnPropertyChanged(nameof(BuyerName));
                        break;
                    case nameof(HomeViewModel.LineName):
                        OnPropertyChanged(nameof(LineName));
                        break;
                    case nameof(HomeViewModel.StationName):
                        OnPropertyChanged(nameof(StationName));
                        break;
                    case nameof(HomeViewModel.ModelName):
                        OnPropertyChanged(nameof(ModelName));
                        break;
                    case nameof(HomeViewModel.ProgramName):
                        OnPropertyChanged(nameof(ProgramName));
                        break;
                    case nameof(HomeViewModel.MacAddress):
                        OnPropertyChanged(nameof(MacAddress));
                        break;
                    case nameof(HomeViewModel.IpAddress):
                        OnPropertyChanged(nameof(IpAddress));
                        break;
                }
            };

            Navigation.PropertyChanged += (s, e) =>
            {
                // Force write to file immediately
                System.IO.File.AppendAllText(@"f:\Dev\Projects\C#\Project .NET\Machine Mangement System\src\ClientApp\MachineClient.WPF\navigation_debug.log", 
                    $"{DateTime.Now:HH:mm:ss} - Navigation.PropertyChanged: {e.PropertyName} - Value: {GetNavigationPropertyValue(e.PropertyName)}\n");
                
                // Forward navigation property changes
                switch (e.PropertyName)
                {
                    case nameof(NavigationViewModel.SelectedPage):
                        OnPropertyChanged(nameof(SelectedPage));
                        System.IO.File.AppendAllText(@"f:\Dev\Projects\C#\Project .NET\Machine Mangement System\src\ClientApp\MachineClient.WPF\navigation_debug.log", 
                            $"{DateTime.Now:HH:mm:ss} - Forwarded SelectedPage: {SelectedPage}\n");
                        break;
                    case nameof(NavigationViewModel.IsHomePage):
                        OnPropertyChanged(nameof(IsHomePage));
                        System.IO.File.AppendAllText(@"f:\Dev\Projects\C#\Project .NET\Machine Mangement System\src\ClientApp\MachineClient.WPF\navigation_debug.log", 
                            $"{DateTime.Now:HH:mm:ss} - Forwarded IsHomePage: {IsHomePage}\n");
                        break;
                    case nameof(NavigationViewModel.IsSettingsPage):
                        OnPropertyChanged(nameof(IsSettingsPage));
                        System.IO.File.AppendAllText(@"f:\Dev\Projects\C#\Project .NET\Machine Mangement System\src\ClientApp\MachineClient.WPF\navigation_debug.log", 
                            $"{DateTime.Now:HH:mm:ss} - Forwarded IsSettingsPage: {IsSettingsPage}\n");
                        break;
                    case nameof(NavigationViewModel.IsAboutPage):
                        OnPropertyChanged(nameof(IsAboutPage));
                        System.IO.File.AppendAllText(@"f:\Dev\Projects\C#\Project .NET\Machine Mangement System\src\ClientApp\MachineClient.WPF\navigation_debug.log", 
                            $"{DateTime.Now:HH:mm:ss} - Forwarded IsAboutPage: {IsAboutPage}\n");
                        break;
                }
            };

            Settings.PropertyChanged += (s, e) =>
            {
                // Forward settings property changes
                switch (e.PropertyName)
                {
                    case nameof(SettingsViewModel.ApiBaseUrl):
                        OnPropertyChanged(nameof(ApiBaseUrl));
                        break;
                    case nameof(SettingsViewModel.AutoStartConnection):
                        OnPropertyChanged(nameof(AutoStartConnection));
                        break;
                }
            };
            
            _logger.LogInformation("Event subscriptions configured for property change forwarding");
        }

        private string GetNavigationPropertyValue(string propertyName)
        {
            return propertyName switch
            {
                "SelectedPage" => Navigation.SelectedPage ?? "null",
                "IsHomePage" => Navigation.IsHomePage.ToString(),
                "IsSettingsPage" => Navigation.IsSettingsPage.ToString(),
                "IsAboutPage" => Navigation.IsAboutPage.ToString(),
                _ => "unknown"
            };
        }

        #region Application-Level Methods

        /// <summary>
        /// Get the currently active page ViewModel
        /// </summary>
        /// <returns>The active page ViewModel</returns>
        public object GetCurrentPageViewModel()
        {
            return Navigation.SelectedPage switch
            {
                "Home" => Home,
                "Settings" => Settings,
                "About" => About,
                _ => Home // Default to Home
            };
        }

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageName">Name of the page to navigate to</param>
        public void NavigateToPage(string pageName)
        {
            Navigation.NavigateToPage(pageName);
            _logger.LogInformation("Navigated to page: {PageName}", pageName);
        }

        /// <summary>
        /// Get application uptime
        /// </summary>
        /// <returns>TimeSpan representing how long the application has been running</returns>
        public TimeSpan GetApplicationUptime()
        {
            return DateTime.Now - ApplicationStartTime;
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Cleanup resources when the application is shutting down
        /// </summary>
        public void Cleanup()
        {
            try
            {
                _logger.LogInformation("Starting application cleanup");

                // Cleanup child ViewModels if they implement IDisposable
                if (Home is IDisposable homeDisposable)
                {
                    homeDisposable.Dispose();
                }

                // Add cleanup for other ViewModels if needed

                _logger.LogInformation("Application cleanup completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during application cleanup");
            }
        }

        #endregion
    }
}