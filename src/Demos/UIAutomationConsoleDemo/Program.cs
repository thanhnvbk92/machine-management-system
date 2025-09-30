using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FlaUI.Automation.Extensions;
using FlaUI.Automation.Extensions.Services;

namespace UIAutomationConsoleDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== UI Automation Console Demo ===");
            Console.WriteLine("This demo shows how to use FlaUI.Automation.Extensions library in any .NET application");
            Console.WriteLine();

            // Create host with DI
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Add UI Automation services from library
                    services.AddUIAutomation();
                    
                    // Add logging
                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                        builder.SetMinimumLevel(LogLevel.Information);
                    });
                })
                .Build();

            // Get services from DI
            var automationService = host.Services.GetRequiredService<IUIAutomationService>();
            var monitoringService = host.Services.GetRequiredService<IElementMonitoringService>();
            var demoService = host.Services.GetRequiredService<IAutomationDemoService>();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            // Subscribe to demo progress
            demoService.DemoProgress += (sender, e) =>
            {
                Console.WriteLine($"[{e.Timestamp:HH:mm:ss}] {e.Message}");
            };

            try
            {
                Console.WriteLine("1. Searching for WPF applications...");
                
                // Try to find Machine Client WPF app
                var initialized = await automationService.InitializeAsync("MachineClient.WPF");
                
                if (!initialized)
                {
                    Console.WriteLine("⚠️ MachineClient.WPF not found. Please start the WPF application first.");
                    Console.WriteLine("Attempting to connect to any available WPF application...");
                    
                    // Try to connect to any available application
                    initialized = await automationService.InitializeAsync();
                }

                if (initialized)
                {
                    Console.WriteLine($"✅ Connected to: {automationService.ApplicationTitle} (PID: {automationService.ProcessId})");
                    Console.WriteLine();

                    // Demo 1: Basic automation
                    Console.WriteLine("2. Running basic automation demo...");
                    await demoService.InitializeAndTestAsync();
                    Console.WriteLine();

                    // Demo 2: Element monitoring
                    Console.WriteLine("3. Starting element monitoring demo...");
                    
                    monitoringService.ElementChanged += (sender, e) =>
                    {
                        Console.WriteLine($"📍 ELEMENT CHANGED: {e.ElementIdentifier}");
                        Console.WriteLine($"   Old Value: '{e.PreviousValue}'");
                        Console.WriteLine($"   New Value: '{e.NewValue}'");
                        Console.WriteLine($"   Timestamp: {e.Timestamp}");
                        Console.WriteLine();
                    };

                    await demoService.StartElementMonitoringDemoAsync();
                    
                    Console.WriteLine("🔄 Monitoring active for 10 seconds...");
                    Console.WriteLine("   Try clicking buttons in the WPF app to see changes!");
                    await Task.Delay(10000);
                    
                    await demoService.StopElementMonitoringDemoAsync();
                    Console.WriteLine();

                    // Demo 3: Manual automation
                    Console.WriteLine("4. Manual automation operations...");
                    
                    // Try to click backup button
                    Console.WriteLine("   Attempting to click Start Backup button...");
                    var clicked = await automationService.ClickButtonAsync("StartBackupButton");
                    Console.WriteLine($"   Result: {(clicked ? "✅ Success" : "⚠️ Button not found")}");
                    
                    // Try to read machine ID
                    Console.WriteLine("   Attempting to read Machine ID...");
                    var machineId = await automationService.ReadTextAsync("MachineIdTextBlock");
                    Console.WriteLine($"   Machine ID: {(string.IsNullOrEmpty(machineId) ? "⚠️ Not found" : $"✅ {machineId}")}");
                    
                    // Get debug info
                    Console.WriteLine();
                    Console.WriteLine("5. Debug Information:");
                    var debugInfo = await demoService.GetDebugInfoAsync();
                    Console.WriteLine(debugInfo);
                }
                else
                {
                    Console.WriteLine("❌ Failed to connect to any application.");
                    Console.WriteLine("Please ensure a WPF application is running.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Demo failed");
                Console.WriteLine($"❌ Demo failed: {ex.Message}");
            }
            finally
            {
                // Cleanup
                automationService?.Dispose();
                monitoringService?.Dispose();
                await host.StopAsync();
            }

            Console.WriteLine();
            Console.WriteLine("Demo completed. Press any key to exit...");
            Console.ReadKey();
        }
    }
}