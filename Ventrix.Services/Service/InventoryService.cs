using Ventrix.Domain.Interfaces;
using Ventrix.Domain.Entities;

namespace Ventrix.Services.Service
{
    public class InventoryService
    {
        private readonly IMaterialRepository _repository;

        public InventoryService(IMaterialRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Material> GetFilteredInventory(string status)
        {
            if (string.IsNullOrEmpty(status) || status == "All")
            {
                return _repository.GetAll();
            }
            return _repository.GetByStatus(status);
        }
    }
}