using MachineManagerApp.Models;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace MachineManagerApp.Services
{
    public interface IMachineService
    {
        Task<ObservableCollection<ProductionLine>> GetProductionLinesAsync();
        Task<ObservableCollection<Machine>> GetMachinesByLineAsync(int lineId);
        Task<CommandResult> SendCommandAsync(int machineId, Command command);
        Task<bool> UpdateMachineStatusAsync(int machineId, MachineStatus status);
    }

    public class MachineService : IMachineService
    {
        private readonly HttpClient _httpClient;

        public MachineService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ObservableCollection<ProductionLine>> GetProductionLinesAsync()
        {
            // Tạm thời sử dụng dữ liệu mẫu
            // Trong thực tế sẽ gọi API: await _httpClient.GetFromJsonAsync<List<ProductionLine>>("api/lines");
            
            await Task.Delay(100); // Simulate network delay

            var lines = new ObservableCollection<ProductionLine>
            {
                new ProductionLine { Id = 1, Name = "Line 1 - Assembly" },
                new ProductionLine { Id = 2, Name = "Line 2 - Testing" },
                new ProductionLine { Id = 3, Name = "Line 3 - Packaging" },
                new ProductionLine { Id = 4, Name = "Line 4 - Quality Control" }
            };

            // Thêm máy móc vào từng line
            lines[0].Machines = new ObservableCollection<Machine>
            {
                new Machine { Id = 1, Name = "CNC-001", IpAddress = "192.168.1.10", Status = MachineStatus.Online, LineId = 1, LineName = "Line 1 - Assembly", LastUpdated = DateTime.Now },
                new Machine { Id = 2, Name = "ROBOT-001", IpAddress = "192.168.1.11", Status = MachineStatus.Online, LineId = 1, LineName = "Line 1 - Assembly", LastUpdated = DateTime.Now },
                new Machine { Id = 3, Name = "CONVEYOR-001", IpAddress = "192.168.1.12", Status = MachineStatus.Offline, LineId = 1, LineName = "Line 1 - Assembly", LastUpdated = DateTime.Now.AddMinutes(-5) }
            };

            lines[1].Machines = new ObservableCollection<Machine>
            {
                new Machine { Id = 4, Name = "TESTER-001", IpAddress = "192.168.1.20", Status = MachineStatus.Online, LineId = 2, LineName = "Line 2 - Testing", LastUpdated = DateTime.Now },
                new Machine { Id = 5, Name = "SCANNER-001", IpAddress = "192.168.1.21", Status = MachineStatus.Error, LineId = 2, LineName = "Line 2 - Testing", LastUpdated = DateTime.Now.AddMinutes(-2) }
            };

            lines[2].Machines = new ObservableCollection<Machine>
            {
                new Machine { Id = 6, Name = "PACK-001", IpAddress = "192.168.1.30", Status = MachineStatus.Maintenance, LineId = 3, LineName = "Line 3 - Packaging", LastUpdated = DateTime.Now.AddHours(-1) },
                new Machine { Id = 7, Name = "LABEL-001", IpAddress = "192.168.1.31", Status = MachineStatus.Online, LineId = 3, LineName = "Line 3 - Packaging", LastUpdated = DateTime.Now },
                new Machine { Id = 8, Name = "SEAL-001", IpAddress = "192.168.1.32", Status = MachineStatus.Online, LineId = 3, LineName = "Line 3 - Packaging", LastUpdated = DateTime.Now }
            };

            lines[3].Machines = new ObservableCollection<Machine>
            {
                new Machine { Id = 9, Name = "QC-001", IpAddress = "192.168.1.40", Status = MachineStatus.Online, LineId = 4, LineName = "Line 4 - Quality Control", LastUpdated = DateTime.Now },
                new Machine { Id = 10, Name = "INSPECT-001", IpAddress = "192.168.1.41", Status = MachineStatus.Offline, LineId = 4, LineName = "Line 4 - Quality Control", LastUpdated = DateTime.Now.AddMinutes(-10) }
            };

            return lines;
        }

        public async Task<ObservableCollection<Machine>> GetMachinesByLineAsync(int lineId)
        {
            var lines = await GetProductionLinesAsync();
            var line = lines.FirstOrDefault(l => l.Id == lineId);
            return line?.Machines ?? new ObservableCollection<Machine>();
        }

        public async Task<CommandResult> SendCommandAsync(int machineId, Command command)
        {
            // Simulate network delay
            await Task.Delay(500);

            // Trong thực tế sẽ gọi API để gửi command
            // var response = await _httpClient.PostAsJsonAsync($"api/machines/{machineId}/commands", command);

            // Simulate command result
            var random = new Random();
            var success = random.NextDouble() > 0.1; // 90% success rate

            return new CommandResult
            {
                Success = success,
                Message = success ? $"Command '{command.Name}' executed successfully" : $"Command '{command.Name}' failed to execute",
                Timestamp = DateTime.Now,
                MachineId = machineId.ToString()
            };
        }

        public async Task<bool> UpdateMachineStatusAsync(int machineId, MachineStatus status)
        {
            // Simulate network delay
            await Task.Delay(200);

            // Trong thực tế sẽ gọi API để cập nhật trạng thái
            // var response = await _httpClient.PutAsJsonAsync($"api/machines/{machineId}/status", status);

            return true; // Simulate success
        }
    }
}