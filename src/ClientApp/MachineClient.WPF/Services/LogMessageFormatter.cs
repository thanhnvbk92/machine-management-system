using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MachineClient.WPF.Services
{
    public static class LogMessageFormatter
    {
        // Dictionary để map từ emoji sang text tương ứng
        private static readonly Dictionary<string, string> EmojiReplacements = new()
        {
            { "🔄", "[LOADING]" },
            { "✅", "[SUCCESS]" },
            { "❌", "[ERROR]" },
            { "⚠️", "[WARNING]" },
            { "🔌", "[CONNECTION]" },
            { "📝", "[REGISTRATION]" },
            { "📊", "[MONITORING]" },
            { "⏹️", "[STOP]" },
            { "�", "[REFRESH]" },
            { "⚙️", "[SETTINGS]" },
            { "📍", "[ELEMENT]" },
            { "📄", "[FILE]" },
            { "📁", "[FOLDER]" },
            { "💾", "[SAVE]" },
            { "📅", "[SCHEDULE]" }
        };

        /// <summary>
        /// Chuyển đổi log message từ emoji sang text format để tránh encoding issues
        /// </summary>
        /// <param name="message">Message chứa emoji</param>
        /// <returns>Message đã được clean không có emoji</returns>
        public static string CleanLogMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return message;

            var cleanedMessage = message;

            // Thay thế các emoji bằng text tương ứng
            foreach (var emoji in EmojiReplacements)
            {
                cleanedMessage = cleanedMessage.Replace(emoji.Key, emoji.Value);
            }

            // Loại bỏ các emoji khác không được map (sử dụng regex)
            cleanedMessage = RemoveRemainingEmojis(cleanedMessage);

            return cleanedMessage;
        }

        /// <summary>
        /// Loại bỏ tất cả emoji còn lại sử dụng regex pattern
        /// </summary>
        private static string RemoveRemainingEmojis(string input)
        {
            // Regex pattern để match emoji characters
            string emojiPattern = @"[\u2190-\u21FF\u2600-\u26FF\u2700-\u27BF\u3000-\u303F\u1F600-\u1F64F\u1F680-\u1F6FF\u1F700-\u1F77F\u1F780-\u1F7FF\u1F800-\u1F8FF\u1F900-\u1F9FF\u1FA00-\u1FA6F\u1FA70-\u1FAFF\u2000-\u206F\u20A0-\u20CF\u2100-\u214F\u2160-\u218F\u2190-\u21FF\u2200-\u22FF\u2300-\u23FF\u2400-\u243F\u2440-\u245F\u2460-\u24FF\u2500-\u257F\u2580-\u259F\u25A0-\u25FF\u2600-\u26FF\u2700-\u27BF]";
            
            return Regex.Replace(input, emojiPattern, string.Empty);
        }

        /// <summary>
        /// Tạo log message với timestamp và format chuẩn
        /// </summary>
        /// <param name="level">Log level (INFO, ERROR, WARNING, etc.)</param>
        /// <param name="message">Message content</param>
        /// <returns>Formatted log message</returns>
        public static string FormatLogMessage(string level, string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var cleanMessage = CleanLogMessage(message);
            return $"[{timestamp}] [{level}] {cleanMessage}";
        }

        /// <summary>
        /// Tạo log message với timestamp đơn giản cho UI display
        /// </summary>
        /// <param name="message">Message content</param>
        /// <returns>Formatted message với timestamp</returns>
        public static string FormatUILogMessage(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var cleanMessage = CleanLogMessage(message);
            return $"[{timestamp}] {cleanMessage}";
        }

        /// <summary>
        /// Tạo connection status message chuẩn
        /// </summary>
        /// <param name="isConnected">Connection status</param>
        /// <param name="details">Additional details</param>
        /// <returns>Formatted connection message</returns>
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
        /// <param name="operation">Operation name</param>
        /// <param name="success">Success status</param>
        /// <param name="details">Additional details</param>
        /// <returns>Formatted operation message</returns>
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