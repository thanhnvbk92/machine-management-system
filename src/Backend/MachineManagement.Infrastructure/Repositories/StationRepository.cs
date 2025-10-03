using Microsoft.EntityFrameworkCore;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.Infrastructure.Data;

namespace MachineManagement.Infrastructure.Repositories
{
    public class StationRepository : Repository<Station>, IStationRepository
    {
        public StationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Station>> GetStationsWithRelatedDataAsync()
        {
            return await _context.Stations
                .Include(s => s.Line)
                .Include(s => s.ModelProcess)
                    .ThenInclude(mp => mp.ModelGroup)
                        .ThenInclude(mg => mg.Buyer)
                .Include(s => s.Machines)
                .ToListAsync();
        }

        public async Task<Station?> GetStationWithRelatedDataAsync(int id)
        {
            return await _context.Stations
                .Include(s => s.Line)
                .Include(s => s.ModelProcess)
                    .ThenInclude(mp => mp.ModelGroup)
                        .ThenInclude(mg => mg.Buyer)
                .Include(s => s.Machines)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Station>> GetByLineIdAsync(int lineId)
        {
            return await _context.Stations
                .Where(s => s.LineId == lineId)
                .Include(s => s.ModelProcess)
                .ToListAsync();
        }

        public async Task<IEnumerable<Station>> GetByModelProcessIdAsync(int modelProcessId)
        {
            return await _context.Stations
                .Where(s => s.ModelProcessId == modelProcessId)
                .Include(s => s.Line)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Stations
                .AnyAsync(s => s.Name == name);
        }

        public async Task<IEnumerable<Station>> SearchByNameAsync(string searchTerm)
        {
            return await _context.Stations
                .Where(s => s.Name.Contains(searchTerm))
                .Include(s => s.Line)
                .Include(s => s.ModelProcess)
                .ToListAsync();
        }
    }
}