using Microsoft.EntityFrameworkCore;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.Infrastructure.Data;

namespace MachineManagement.Infrastructure.Repositories
{
    public class LineRepository : Repository<Line>, ILineRepository
    {
        public LineRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Line>> GetLinesWithStationsAsync()
        {
            return await _context.Lines
                .Include(l => l.Stations)
                .ToListAsync();
        }

        public async Task<Line?> GetLineWithStationsAsync(int id)
        {
            return await _context.Lines
                .Include(l => l.Stations)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Lines
                .AnyAsync(l => l.Name == name);
        }

        public async Task<IEnumerable<Line>> SearchByNameAsync(string searchTerm)
        {
            return await _context.Lines
                .Where(l => l.Name.Contains(searchTerm))
                .ToListAsync();
        }
    }
}