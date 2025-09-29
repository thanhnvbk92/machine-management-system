using Microsoft.Extensions.Logging;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.Core.Interfaces.Services;

namespace MachineManagement.Infrastructure.Services
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

        public async Task<Command> CreateCommandAsync(Command command)
        {
            try
            {
                command.CreatedAt = DateTime.UtcNow;
                command.Status = "Pending";
                command.IsActive = true;

                await _unitOfWork.Commands.AddAsync(command);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Command created for machine {MachineId}: {CommandType}", command.MachineId, command.CommandType);
                return command;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating command for machine {MachineId}", command.MachineId);
                throw;
            }
        }

        public async Task<IEnumerable<Command>> GetPendingCommandsAsync(int machineId)
        {
            try
            {
                var pendingCommands = await _unitOfWork.Commands.FindAsync(c => 
                    c.MachineId == machineId && 
                    c.Status == "Pending" && 
                    c.IsActive &&
                    (c.ScheduledAt == null || c.ScheduledAt <= DateTime.UtcNow));

                return pendingCommands.OrderBy(c => c.Priority).ThenBy(c => c.CreatedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending commands for machine {MachineId}", machineId);
                throw;
            }
        }

        public async Task<Command> UpdateCommandStatusAsync(int commandId, string status, string? response = null, string? errorMessage = null)
        {
            try
            {
                var command = await _unitOfWork.Commands.GetByIdAsync(commandId);
                if (command == null)
                {
                    _logger.LogWarning("Command with ID {CommandId} not found", commandId);
                    throw new ArgumentException($"Command with ID {commandId} not found");
                }

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

                _logger.LogInformation("Command {CommandId} status updated to {Status}", commandId, status);
                return command;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating command status for command {CommandId}", commandId);
                throw;
            }
        }

        public async Task<Command?> GetCommandByIdAsync(int id)
        {
            try
            {
                return await _unitOfWork.Commands.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting command by ID {CommandId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Command>> GetCommandsByMachineIdAsync(int machineId)
        {
            try
            {
                var commands = await _unitOfWork.Commands.FindAsync(c => c.MachineId == machineId);
                return commands.OrderByDescending(c => c.CreatedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting commands for machine {MachineId}", machineId);
                throw;
            }
        }

        public async Task<bool> DeleteCommandAsync(int id)
        {
            try
            {
                var command = await _unitOfWork.Commands.GetByIdAsync(id);
                if (command == null)
                {
                    return false;
                }

                _unitOfWork.Commands.Remove(command);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Command with ID {CommandId} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting command with ID {CommandId}", id);
                throw;
            }
        }
    }
}