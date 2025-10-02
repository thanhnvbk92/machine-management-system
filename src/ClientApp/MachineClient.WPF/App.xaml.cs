using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MachineClient.WPF.Models;
using MachineClient.WPF.Services;
using MachineClient.WPF.ViewModels;
using MachineClient.WPF.Views;
using FlaUI.Automation.Extensions;
using Serilog;
using Serilog.Extensions.Hosting;

namespace MachineClient.WPF
{
    public partial class App : Application
    {
        private IHost? _host;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]  
        static extern bool FreeConsole();

        static App()
        {
            try
            {
                System.IO.File.WriteAllText("startup.log", $"Static constructor called at {DateTime.Now}\n");
            }
            catch { }
        }

        public App()
        {
            try
            {
                System.IO.File.AppendAllText("startup.log", $"Constructor called at {DateTime.Now}\n");
                
                // Global exception handler
                DispatcherUnhandledException += App_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                
                System.IO.File.AppendAllText("startup.log", "Exception handlers registered\n");
                System.IO.File.AppendAllText("startup.log", $"App instance created successfully at {DateTime.Now}\n");
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("constructor_error.log", $"Constructor Error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            System.IO.File.WriteAllText("dispatcher_error.log", $"Dispatcher Error at {DateTime.Now}: {e.Exception.Message}\n\nStack Trace:\n{e.Exception.StackTrace}\n\nInner Exception: {e.Exception.InnerException?.Message}");
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            System.IO.File.WriteAllText("domain_error.log", $"Critical Error at {DateTime.Now}: {ex?.Message}\n\nStack Trace:\n{ex?.StackTrace}");
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Fix for white screen issue - force software rendering
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
                
                // File logging with detailed info
                var logFile = "detailed_debug.log";
                System.IO.File.WriteAllText(logFile, $"=== WPF APP STARTUP DEBUG ===\n");
                System.IO.File.AppendAllText(logFile, $"Starting application at {DateTime.Now}\n");
                System.IO.File.AppendAllText(logFile, $"Command line args: {string.Join(" ", e.Args)}\n");
                System.IO.File.AppendAllText(logFile, "✅ Software rendering mode enabled\n");
                
                System.IO.File.AppendAllText(logFile, "Creating host...\n");
                _host = CreateHostBuilder().Build();
                System.IO.File.AppendAllText(logFile, "✅ Host built successfully\n");
                
                System.IO.File.AppendAllText(logFile, "Starting host...\n");
                await _host.StartAsync();
                System.IO.File.AppendAllText(logFile, "✅ Host started successfully\n");

                System.IO.File.AppendAllText(logFile, "Getting MainWindow from DI...\n");
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                System.IO.File.AppendAllText(logFile, $"✅ MainWindow created: {mainWindow?.GetType().Name}\n");
                
                if (mainWindow != null)
                {
                    System.IO.File.AppendAllText(logFile, $"MainWindow properties - Title: {mainWindow.Title}, Width: {mainWindow.Width}, Height: {mainWindow.Height}\n");
                    
                    System.IO.File.AppendAllText(logFile, "Setting window properties...\n");
                    mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    mainWindow.WindowState = WindowState.Normal;
                    mainWindow.Topmost = true; // Force on top
                    
                    System.IO.File.AppendAllText(logFile, "Calling Show()...\n");
                    mainWindow.Show();
                    
                    System.IO.File.AppendAllText(logFile, $"After Show() - IsVisible: {mainWindow.IsVisible}, IsLoaded: {mainWindow.IsLoaded}\n");
                    
                    // Try to activate and focus
                    mainWindow.Activate();
                    mainWindow.Focus();
                    System.IO.File.AppendAllText(logFile, "Window activated and focused\n");
                    
                    // Check if window is actually visible
                    await Task.Delay(100); // Give time for window to appear
                    System.IO.File.AppendAllText(logFile, $"Final check - IsVisible: {mainWindow.IsVisible}, ActualWidth: {mainWindow.ActualWidth}, ActualHeight: {mainWindow.ActualHeight}\n");
                    
                    // Set as MainWindow
                    this.MainWindow = mainWindow;
                }
                else
                {
                    System.IO.File.AppendAllText(logFile, "❌ MainWindow is null!\n");
                }

                System.IO.File.AppendAllText(logFile, "Calling base.OnStartup...\n");
                base.OnStartup(e);
                System.IO.File.AppendAllText(logFile, "✅ Startup completed successfully!\n");
            }
            catch (Exception ex)
            {
                var errorMsg = $"❌ STARTUP ERROR: {ex.Message}\nStack Trace: {ex.StackTrace}\nInner Exception: {ex.InnerException?.Message}\n";
                System.IO.File.WriteAllText("startup_error.log", errorMsg);
                Current.Shutdown(1);
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            Console.WriteLine("Application exiting...");
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            FreeConsole();
            base.OnExit(e);
        }

        private IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .UseSerilog((context, loggerConfig) =>
                {
                    loggerConfig
                        .MinimumLevel.Information()
                        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                        .WriteTo.Debug()
                        .WriteTo.File(
                            path: "Logs/client-.log",
                            rollingInterval: RollingInterval.Day,
                            retainedFileCountLimit: 30,
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"
                        );
                })
                .ConfigureServices((context, services) =>
                {
                    // Configure ApiSettings with IOptions pattern
                    services.Configure<ApiSettings>(context.Configuration.GetSection("ApiSettings"));
                    
                    // Also register as singleton for backward compatibility
                    var apiSettings = new ApiSettings();
                    context.Configuration.GetSection("ApiSettings").Bind(apiSettings);
                    
                    // HTTP Client Factory
                    services.AddHttpClient("API", client =>
                    {
                        client.BaseAddress = new Uri(apiSettings.BaseUrl.TrimEnd('/') + "/");
                        client.Timeout = TimeSpan.FromSeconds(apiSettings.Timeout);
                    });
                    
                    // Services
                    services.AddSingleton<IApiService, ApiService>();
                    services.AddSingleton<ILogCollectionService, LogCollectionService>();
                    services.AddSingleton<IConfigurationService, ConfigurationService>();
                    services.AddSingleton<IMachineInfoService, MachineInfoService>();
                    services.AddSingleton<IBackupService, BackupService>();
                    
                    // Clean Architecture Services
                    services.AddSingleton<IMachineConnectionService, MachineConnectionService>();
                    services.AddSingleton<IBackupManager, BackupManager>();
                    services.AddSingleton<IUIStateManager, UIStateManager>();
                    services.AddSingleton<IApplicationSettingsService, ApplicationSettingsService>();
                    
                    // UI Automation Services (from library)
                    services.AddUIAutomation();
                    
                    // ViewModels (Page-based architecture)
                    services.AddTransient<HomeViewModel>();
                    services.AddTransient<SettingsViewModel>();
                    services.AddTransient<AboutViewModel>();
                    services.AddTransient<NavigationViewModel>();
                    services.AddTransient<MainViewModel>(); // Shell ViewModel
                    
                    // Views
                    services.AddTransient<MainWindow>();
                });
        }
    }
}