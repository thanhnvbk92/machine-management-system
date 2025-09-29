using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;

namespace MachineManagement.ManagerApp.Services
{
    public class CommandService : ICommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CommandService> _logger;

        public CommandService(IUnitOfWork unitOfWork, ILogger<CommandService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Command>> GetAllCommandsAsync()
        {
            try
            {
                return await _unitOfWork.Commands.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all commands");
                throw;
            }
        }

        public async Task<IEnumerable<Command>> GetCommandsByMachineIdAsync(int machineId)
        {
            try
            {
                return await _unitOfWork.Commands.FindAsync(c => c.MachineId == machineId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting commands for machine {MachineId}", machineId);
                throw;
            }
        }

        public async Task<IEnumerable<Command>> GetPendingCommandsAsync()
        {
            try
            {
                return await _unitOfWork.Commands.FindAsync(c => c.Status == "Pending");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending commands");
                throw;
            }
        }

        public async Task<IEnumerable<Command>> GetRecentCommandsAsync(int count = 50)
        {
            try
            {
                var commands = await _unitOfWork.Commands.GetAllAsync();
                return commands.OrderByDescending(c => c.CreatedAt).Take(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent commands");
                throw;
            }
        }

        public async Task<Command> CreateCommandAsync(Command command)
        {
            try
            {
                command.CreatedAt = DateTime.UtcNow;
                command.Status = "Pending";
                
                await _unitOfWork.Commands.AddAsync(command);
                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation("Created command {CommandId} for machine {MachineId}", command.CommandId, command.MachineId);
                return command;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating command for machine {MachineId}", command.MachineId);
                throw;
            }
        }

        public async Task<Command> UpdateCommandStatusAsync(int commandId, string status, string? response = null, string? errorMessage = null)
        {
            try
            {
                var command = await _unitOfWork.Commands.GetByIdAsync(commandId);
                if (command == null)
                    throw new ArgumentException($"Command with ID {commandId} not found");

                command.Status = status;
                command.Response = response;
                command.ErrorMessage = errorMessage;
                command.UpdatedAt = DateTime.UtcNow;

                if (status == "Completed" || status == "Failed")
                {
                    command.ExecutedAt = DateTime.UtcNow;
                }

                _unitOfWork.Commands.Update(command);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Updated command {CommandId} status to {Status}", commandId, status);
                return command;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating command {CommandId} status", commandId);
                throw;
            }
        }

        public async Task<bool> DeleteCommandAsync(int commandId)
        {
            try
            {
                var command = await _unitOfWork.Commands.GetByIdAsync(commandId);
                if (command == null)
                    return false;

                _unitOfWork.Commands.Remove(command);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Deleted command {CommandId}", commandId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting command {CommandId}", commandId);
                throw;
            }
        }

        public async Task<IEnumerable<Command>> GetCommandsByStatusAsync(string status)
        {
            try
            {
                return await _unitOfWork.Commands.FindAsync(c => c.Status == status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting commands by status {Status}", status);
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetCommandCountByStatusAsync()
        {
            try
            {
                var commands = await _unitOfWork.Commands.GetAllAsync();
                return commands.GroupBy(c => c.Status)
                             .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting command count by status");
                throw;
            }
        }

        public async Task<Command?> GetCommandByIdAsync(int commandId)
        {
            try
            {
                return await _unitOfWork.Commands.GetByIdAsync(commandId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting command {CommandId}", commandId);
                throw;
            }
        }

        public async Task<int> GetPendingCommandCountByMachineAsync(int machineId)
        {
            try
            {
                return await _unitOfWork.Commands.CountAsync(c => c.MachineId == machineId && c.Status == "Pending");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending command count for machine {MachineId}", machineId);
                throw;
            }
        }

        public async Task<IEnumerable<Command>> GetPagedCommandsAsync(int page, int size, string? status = null, int? machineId = null)
        {
            try
            {
                IEnumerable<Command> commands;

                if (!string.IsNullOrEmpty(status) && machineId.HasValue)
                {
                    commands = await _unitOfWork.Commands.FindAsync(c => c.Status == status && c.MachineId == machineId);
                }
                else if (!string.IsNullOrEmpty(status))
                {
                    commands = await _unitOfWork.Commands.FindAsync(c => c.Status == status);
                }
                else if (machineId.HasValue)
                {
                    commands = await _unitOfWork.Commands.FindAsync(c => c.MachineId == machineId);
                }
                else
                {
                    commands = await _unitOfWork.Commands.GetAllAsync();
                }

                return commands.OrderByDescending(c => c.CreatedAt)
                             .Skip((page - 1) * size)
                             .Take(size);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged commands");
                throw;
            }
        }
    }
}