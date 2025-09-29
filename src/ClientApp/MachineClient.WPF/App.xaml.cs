using MachineClient.WPF.Services;
using MachineClient.WPF.ViewModels;
using MachineClient.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Windows;

namespace MachineClient.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        protected override async void OnStartup(StartupEventArgs e)
        {
            // Setup Serilog logging
            var logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MachineClient",
                "Logs",
                "machine-client-.log");

            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(logPath, 
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    shared: true)
                .CreateLogger();

            try
            {
                // Build host with dependency injection
                _host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        // Register services
                        services.AddSingleton<IApiService, ApiService>();
                        services.AddSingleton<ILogCollectionService, LogCollectionService>();
                        services.AddSingleton<IConfigurationService, ConfigurationService>();

                        // Register HttpClient for API service
                        services.AddHttpClient<IApiService, ApiService>();

                        // Register ViewModels
                        services.AddTransient<MainViewModel>();

                        // Register Views
                        services.AddTransient<MainWindow>();
                    })
                    .UseSerilog()
                    .Build();

                await _host.StartAsync();

                // Get the main window from DI
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
                MessageBox.Show($"Application failed to start: {ex.Message}", "Startup Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                if (_host != null)
                {
                    await _host.StopAsync();
                    _host.Dispose();
                }
            }
            finally
            {
                Log.CloseAndFlush();
                base.OnExit(e);
            }
        }
    }
}