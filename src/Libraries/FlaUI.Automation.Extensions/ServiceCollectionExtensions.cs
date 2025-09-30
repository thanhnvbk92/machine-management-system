using Microsoft.Extensions.DependencyInjection;
using FlaUI.Automation.Extensions.Services;

namespace FlaUI.Automation.Extensions
{
    /// <summary>
    /// Extension methods for registering UI Automation services with dependency injection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add all UI Automation services to the service collection
        /// </summary>
        public static IServiceCollection AddUIAutomation(this IServiceCollection services)
        {
            services.AddSingleton<IUIAutomationService, UIAutomationService>();
            services.AddSingleton<IElementMonitoringService, ElementMonitoringService>();
            services.AddSingleton<IAutomationDemoService, AutomationDemoService>();
            
            return services;
        }

        /// <summary>
        /// Add only core UI Automation service
        /// </summary>
        public static IServiceCollection AddUIAutomationCore(this IServiceCollection services)
        {
            services.AddSingleton<IUIAutomationService, UIAutomationService>();
            return services;
        }

        /// <summary>
        /// Add UI Automation with monitoring services
        /// </summary>
        public static IServiceCollection AddUIAutomationWithMonitoring(this IServiceCollection services)
        {
            services.AddSingleton<IUIAutomationService, UIAutomationService>();
            services.AddSingleton<IElementMonitoringService, ElementMonitoringService>();
            return services;
        }

        /// <summary>
        /// Add UI Automation with demo services
        /// </summary>
        public static IServiceCollection AddUIAutomationWithDemo(this IServiceCollection services)
        {
            services.AddSingleton<IUIAutomationService, UIAutomationService>();
            services.AddSingleton<IElementMonitoringService, ElementMonitoringService>();
            services.AddSingleton<IAutomationDemoService, AutomationDemoService>();
            return services;
        }
    }
}