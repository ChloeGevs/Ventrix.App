using Ventrix.Domain.Models;

namespace Ventrix.Domain.Interfaces
{
    public interface IInventoryRepository
    {
        IEnumerable<InventoryItem> GetAll();
        InventoryItem GetById(int id);
        void Add(InventoryItem item);
        void Update(InventoryItem item);
        void Remove(InventoryItem item);
        int GetTotalCount();
        int GetCountByStatus(string status);
        int GetCountByCondition(string condition);
    }
}