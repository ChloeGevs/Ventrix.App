using Ventrix.Domain.Interfaces;
using Ventrix.Domain.Models;

namespace Ventrix.Application.Services
{
    public class InventoryService
    {
        private readonly IInventoryRepository _inventoryRepo;

        public InventoryService(IInventoryRepository inventoryRepo)
        {
            _inventoryRepo = inventoryRepo;
        }

        public List<InventoryItem> GetAllItems()
        {
            return _inventoryRepo.GetAll().ToList();
        }

        public IEnumerable<InventoryItem> GetFilteredInventory(string search, string statusFilter)
        {
            var items = _inventoryRepo.GetAll();
            if (!string.IsNullOrEmpty(search))
                items = items.Where(i => i.Name.ToLower().Contains(search.ToLower()));

            if (statusFilter != "All")
                items = items.Where(i => i.Status == statusFilter);

            return items;
        }

        public void SaveItem(InventoryItem item)
        {
            if (item.Id == 0) _inventoryRepo.Add(item);
            else _inventoryRepo.Update(item);
        }

        public bool DeleteItem(int id)
        {
            var item = _inventoryRepo.GetById(id);
            if (item == null) return false;

            _inventoryRepo.Remove(item);
            return true;
        }

        public InventoryItem GetItemById(int id) => _inventoryRepo.GetById(id);
        public void AddItem(InventoryItem item) => _inventoryRepo.Add(item);
        public void UpdateItem(InventoryItem item) => _inventoryRepo.Update(item);

        public dynamic GetDashboardStats()
        {
            return new
            {
                Total = _inventoryRepo.GetTotalCount(),
                Available = _inventoryRepo.GetCountByStatus("Available"),
                Borrowed = _inventoryRepo.GetCountByStatus("Borrowed"),
                Damaged = _inventoryRepo.GetCountByCondition("Damaged")
            };
        }
    }
}