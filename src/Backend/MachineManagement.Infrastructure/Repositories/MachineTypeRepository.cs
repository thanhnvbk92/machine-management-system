using Microsoft.EntityFrameworkCore;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.Infrastructure.Data;

namespace MachineManagement.Infrastructure.Repositories
{
    public class MachineTypeRepository : Repository<MachineType>, IMachineTypeRepository
    {
        public MachineTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MachineType>> GetMachineTypesWithMachinesAsync()
        {
            return await _context.MachineTypes
                .Include(mt => mt.Machines)
                .ToListAsync();
        }

        public async Task<MachineType?> GetMachineTypeWithMachinesAsync(int id)
        {
            return await _context.MachineTypes
                .Include(mt => mt.Machines)
                .FirstOrDefaultAsync(mt => mt.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.MachineTypes
                .AnyAsync(mt => mt.Name == name);
        }

        public async Task<IEnumerable<MachineType>> SearchByNameAsync(string searchTerm)
        {
            return await _context.MachineTypes
                .Where(mt => mt.Name.Contains(searchTerm))
                .ToListAsync();
        }
    }
}