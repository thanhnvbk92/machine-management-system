using MachineManagement.Core.Entities;

namespace MachineManagement.Core.Interfaces
{
    public interface ILineRepository : IRepository<Line>
    {
        Task<IEnumerable<Line>> GetLinesWithStationsAsync();
        Task<Line?> GetLineWithStationsAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Line>> SearchByNameAsync(string searchTerm);
    }
}