using System;

namespace MachineClient.WPF.Services
{
    public static class LogMessageFormatter
    {
        /// <summary>
        /// Tạo log message với timestamp đơn giản cho UI display
        /// </summary>
        /// <param name="message">Message content</param>
        /// <returns>Formatted message với timestamp</returns>
        public static string FormatUILogMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return "";
                
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            return $"[{timestamp}] {message}";
        }

        /// <summary>
        /// Tạo connection status message chuẩn
        /// </summary>
        public static string FormatConnectionMessage(bool isConnected, string details = "")
        {
            var status = isConnected ? "CONNECTED" : "DISCONNECTED";
            var message = $"Connection Status: {status}";
            
            if (!string.IsNullOrEmpty(details))
            {
                message += $" - {details}";
            }

            return FormatUILogMessage(message);
        }

        /// <summary>
        /// Tạo operation result message chuẩn
        /// </summary>
        public static string FormatOperationMessage(string operation, bool success, string details = "")
        {
            var status = success ? "SUCCESS" : "FAILED";
            var message = $"{operation}: {status}";
            
            if (!string.IsNullOrEmpty(details))
            {
                message += $" - {details}";
            }

            return FormatUILogMessage(message);
        }
    }
}