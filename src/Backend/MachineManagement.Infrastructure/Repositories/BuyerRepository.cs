using Microsoft.EntityFrameworkCore;
using MachineManagement.Core.Entities;
using MachineManagement.Core.Interfaces;
using MachineManagement.Infrastructure.Data;

namespace MachineManagement.Infrastructure.Repositories
{
    public class BuyerRepository : Repository<Buyer>, IBuyerRepository
    {
        public BuyerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Buyer>> GetBuyersWithModelGroupsAsync()
        {
            return await _context.Buyers
                .Include(b => b.ModelGroups)
                .ToListAsync();
        }

        public async Task<Buyer?> GetBuyerWithModelGroupsAsync(int id)
        {
            return await _context.Buyers
                .Include(b => b.ModelGroups)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.Buyers
                .AnyAsync(b => b.Code == code);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Buyers
                .AnyAsync(b => b.Name == name);
        }

        public async Task<IEnumerable<Buyer>> SearchByNameOrCodeAsync(string searchTerm)
        {
            return await _context.Buyers
                .Where(b => b.Name.Contains(searchTerm) || b.Code.Contains(searchTerm))
                .ToListAsync();
        }
    }
}