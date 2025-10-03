using MachineManagement.Core.Entities;

namespace MachineManagement.Core.Interfaces
{
    public interface IStationRepository : IRepository<Station>
    {
        Task<IEnumerable<Station>> GetStationsWithRelatedDataAsync();
        Task<Station?> GetStationWithRelatedDataAsync(int id);
        Task<IEnumerable<Station>> GetByLineIdAsync(int lineId);
        Task<IEnumerable<Station>> GetByModelProcessIdAsync(int modelProcessId);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Station>> SearchByNameAsync(string searchTerm);
    }
}