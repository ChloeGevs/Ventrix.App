using System.Collections.Generic;
using System.Threading.Tasks;
using Ventrix.Domain.Models;

namespace Ventrix.Domain.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<InventoryItem>> GetAllAsync();

        Task<InventoryItem> GetByIdAsync(int id);

        Task AddAsync(InventoryItem item);

        Task UpdateAsync(InventoryItem item);

        Task RemoveAsync(InventoryItem item);

        Task DeleteAsync(int id);

        Task<int> GetTotalCountAsync();
        Task<int> GetCountByStatusAsync(string status);
        Task<int> GetCountByConditionAsync(string condition);
    }
}