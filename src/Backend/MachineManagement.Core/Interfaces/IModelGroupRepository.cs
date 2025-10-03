using MachineManagement.Core.Entities;

namespace MachineManagement.Core.Interfaces
{
    public interface IModelGroupRepository : IRepository<ModelGroup>
    {
        Task<IEnumerable<ModelGroup>> GetModelGroupsWithBuyerAsync();
        Task<ModelGroup?> GetModelGroupWithBuyerAsync(int id);
        Task<IEnumerable<ModelGroup>> GetByBuyerIdAsync(int buyerId);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<ModelGroup>> SearchByNameAsync(string searchTerm);
    }
}