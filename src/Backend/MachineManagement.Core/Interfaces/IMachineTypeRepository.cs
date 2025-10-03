using MachineManagement.Core.Entities;

namespace MachineManagement.Core.Interfaces
{
    public interface IMachineTypeRepository : IRepository<MachineType>
    {
        Task<IEnumerable<MachineType>> GetMachineTypesWithMachinesAsync();
        Task<MachineType?> GetMachineTypeWithMachinesAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<MachineType>> SearchByNameAsync(string searchTerm);
    }
}