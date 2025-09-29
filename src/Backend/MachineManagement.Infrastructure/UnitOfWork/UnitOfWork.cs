using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.Infrastructure.Data;
using MachineManagement.Infrastructure.Repositories;

namespace MachineManagement.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        private IRepository<Machine>? _machines;
        private IRepository<LogData>? _logData;
        private IRepository<Command>? _commands;
        private IRepository<ClientConfig>? _clientConfigs;
        private IRepository<Station>? _stations;
        private IRepository<Line>? _lines;
        private IRepository<ModelProcess>? _modelProcesses;
        private IRepository<Model>? _models;
        private IRepository<ModelGroup>? _modelGroups;
        private IRepository<Buyer>? _buyers;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<Machine> Machines =>
            _machines ??= new Repository<Machine>(_context);

        public IRepository<LogData> LogData =>
            _logData ??= new Repository<LogData>(_context);

        public IRepository<Command> Commands =>
            _commands ??= new Repository<Command>(_context);

        public IRepository<ClientConfig> ClientConfigs =>
            _clientConfigs ??= new Repository<ClientConfig>(_context);

        public IRepository<Station> Stations =>
            _stations ??= new Repository<Station>(_context);

        public IRepository<Line> Lines =>
            _lines ??= new Repository<Line>(_context);

        public IRepository<ModelProcess> ModelProcesses =>
            _modelProcesses ??= new Repository<ModelProcess>(_context);

        public IRepository<Model> Models =>
            _models ??= new Repository<Model>(_context);

        public IRepository<ModelGroup> ModelGroups =>
            _modelGroups ??= new Repository<ModelGroup>(_context);

        public IRepository<Buyer> Buyers =>
            _buyers ??= new Repository<Buyer>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}