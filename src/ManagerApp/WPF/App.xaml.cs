using MachineManagerApp.Services;
using MachineManagerApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;

namespace MachineManagerApp
{
    public partial class App : Application
    {
        private IHost? _host;

        protected override async void OnStartup(StartupEventArgs e)
        {
            // Fix for white screen issue - force software rendering
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/machine-manager-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting Machine Manager WPF Application");

                // Configure services
                _host = Host.CreateDefaultBuilder()
                    .UseSerilog() // Use Serilog as the logging provider
                    .ConfigureServices((context, services) =>
                    {
                        // Register HttpClient
                        services.AddHttpClient<IMachineService, MachineService>(client =>
                        {
                            client.BaseAddress = new Uri("http://localhost:5275/"); // Backend API URL
                            client.Timeout = TimeSpan.FromSeconds(30);
                        });

                        // Register services
                        services.AddSingleton<IMachineService, MachineService>();
                        
                        // Register ViewModels
                        services.AddTransient<MainViewModel>();
                        
                        // Register Views
                        services.AddTransient<MainWindow>();
                    })
                    .Build();

                await _host.StartAsync();

                // Show main window
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();

                Log.Information("Application started successfully");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application startup failed");
                MessageBox.Show($"Application failed to start: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                Log.Information("Application shutting down");
                
                if (_host != null)
                {
                    await _host.StopAsync();
                    _host.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during application shutdown");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            base.OnExit(e);
        }
    }
}