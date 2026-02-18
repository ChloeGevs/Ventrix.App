using Ventrix.Domain.Entities;

namespace Ventrix.Domain.Interfaces
{
    public interface IMaterialRepository
    {
        IEnumerable<Material> GetAll();
        IEnumerable<Material> GetByStatus(string status);
        void UpdateStatus(string id, string newStatus);
    }
}