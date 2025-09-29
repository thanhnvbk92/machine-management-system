using MachineManagement.Core.Entities;

namespace MachineManagement.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Machine> Machines { get; }
        IRepository<LogData> LogData { get; }
        IRepository<Command> Commands { get; }
        IRepository<ClientConfig> ClientConfigs { get; }
        IRepository<Station> Stations { get; }
        IRepository<Line> Lines { get; }
        IRepository<ModelProcess> ModelProcesses { get; }
        IRepository<Model> Models { get; }
        IRepository<ModelGroup> ModelGroups { get; }
        IRepository<Buyer> Buyers { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}