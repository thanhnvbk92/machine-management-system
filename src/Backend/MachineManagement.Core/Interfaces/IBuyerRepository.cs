using MachineManagement.Core.Entities;

namespace MachineManagement.Core.Interfaces
{
    public interface IBuyerRepository : IRepository<Buyer>
    {
        Task<IEnumerable<Buyer>> GetBuyersWithModelGroupsAsync();
        Task<Buyer?> GetBuyerWithModelGroupsAsync(int id);
        Task<bool> ExistsByCodeAsync(string code);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Buyer>> SearchByNameOrCodeAsync(string searchTerm);
    }
}