using Microsoft.EntityFrameworkCore;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.Infrastructure.Data;

namespace MachineManagement.Infrastructure.Repositories
{
    public class ModelGroupRepository : Repository<ModelGroup>, IModelGroupRepository
    {
        public ModelGroupRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ModelGroup>> GetModelGroupsWithBuyerAsync()
        {
            return await _context.ModelGroups
                .Include(mg => mg.Buyer)
                .ToListAsync();
        }

        public async Task<ModelGroup?> GetModelGroupWithBuyerAsync(int id)
        {
            return await _context.ModelGroups
                .Include(mg => mg.Buyer)
                .FirstOrDefaultAsync(mg => mg.Id == id);
        }

        public async Task<IEnumerable<ModelGroup>> GetByBuyerIdAsync(int buyerId)
        {
            return await _context.ModelGroups
                .Where(mg => mg.BuyerId == buyerId)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.ModelGroups
                .AnyAsync(mg => mg.Name == name);
        }

        public async Task<IEnumerable<ModelGroup>> SearchByNameAsync(string searchTerm)
        {
            return await _context.ModelGroups
                .Where(mg => mg.Name.Contains(searchTerm))
                .Include(mg => mg.Buyer)
                .ToListAsync();
        }
    }
}