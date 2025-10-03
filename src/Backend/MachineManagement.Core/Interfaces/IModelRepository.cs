using MachineManagement.Core.Entities;

namespace MachineManagement.Core.Interfaces
{
    public interface IModelRepository : IRepository<Model>
    {
        Task<IEnumerable<Model>> GetModelsWithModelGroupAsync();
        Task<Model?> GetModelWithModelGroupAsync(int id);
        Task<IEnumerable<Model>> GetByModelGroupIdAsync(int modelGroupId);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Model>> SearchByNameAsync(string searchTerm);
    }
}