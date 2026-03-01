using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Required for Task
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

        public async Task<List<InventoryItem>> GetAllItemsAsync()
        {
            var items = await _inventoryRepo.GetAllAsync(); // Updated to async
            return items.ToList();
        }

        public async Task<IEnumerable<InventoryItem>> GetFilteredInventoryAsync(string search, string statusFilter)
        {
            var items = await _inventoryRepo.GetAllAsync(); 

            if (!string.IsNullOrEmpty(search))
                items = items.Where(i => i.Name.ToLower().Contains(search.ToLower()));

            if (statusFilter != "All")
                items = items.Where(i => i.Status == statusFilter);

            return items;
        }

        public async Task SaveItemAsync(InventoryItem item)
        {
            if (item.Id == 0)
                await _inventoryRepo.AddAsync(item); // Updated to async
            else
                await _inventoryRepo.UpdateAsync(item); // Updated to async
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await _inventoryRepo.GetByIdAsync(id); // Updated to async
            if (item == null) return false;

            await _inventoryRepo.RemoveAsync(item); // Updated to async
            return true;
        }

        public async Task<InventoryItem> GetItemByIdAsync(int id) =>
            await _inventoryRepo.GetByIdAsync(id); // Updated to async

        public async Task AddItemAsync(InventoryItem item) =>
            await _inventoryRepo.AddAsync(item); // Updated to async

        public async Task UpdateItemAsync(InventoryItem item) =>
            await _inventoryRepo.UpdateAsync(item); // Updated to async

        public async Task<dynamic> GetDashboardStatsAsync()
        {
            return new
            {
                Total = await _inventoryRepo.GetTotalCountAsync(), // Updated to async
                Available = await _inventoryRepo.GetCountByStatusAsync("Available"), // Updated to async
                Borrowed = await _inventoryRepo.GetCountByStatusAsync("Borrowed"), // Updated to async
                Damaged = await _inventoryRepo.GetCountByConditionAsync("Damaged") // Updated to async
            };
        }
    }
}