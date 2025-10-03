using Microsoft.EntityFrameworkCore;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.Infrastructure.Data;

namespace MachineManagement.Infrastructure.Repositories
{
    public class ModelRepository : Repository<Model>, IModelRepository
    {
        public ModelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Model>> GetModelsWithModelGroupAsync()
        {
            return await _context.Models
                .Include(m => m.ModelGroup)
                    .ThenInclude(mg => mg.Buyer)
                .ToListAsync();
        }

        public async Task<Model?> GetModelWithModelGroupAsync(int id)
        {
            return await _context.Models
                .Include(m => m.ModelGroup)
                    .ThenInclude(mg => mg.Buyer)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Model>> GetByModelGroupIdAsync(int modelGroupId)
        {
            return await _context.Models
                .Where(m => m.ModelGroupId == modelGroupId)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Models
                .AnyAsync(m => m.Name == name);
        }

        public async Task<IEnumerable<Model>> SearchByNameAsync(string searchTerm)
        {
            return await _context.Models
                .Where(m => m.Name.Contains(searchTerm))
                .Include(m => m.ModelGroup)
                    .ThenInclude(mg => mg.Buyer)
                .ToListAsync();
        }
    }
}