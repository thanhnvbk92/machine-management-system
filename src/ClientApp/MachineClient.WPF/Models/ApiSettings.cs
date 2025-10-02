namespace MachineClient.WPF.Models
{
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = "http://localhost:5275";
        public int Timeout { get; set; } = 30;
        public int RetryAttempts { get; set; } = 3;
        public int HeartbeatInterval { get; set; } = 30;
        
        /// <summary>
        /// Dải IP ưu tiên (ví dụ: "10.224" cho dải 10.224.xxx.xxx)
        /// </summary>
        public string PreferredIpPrefix { get; set; } = "10.224";
    }
}