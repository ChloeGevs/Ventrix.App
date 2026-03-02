using Ventrix.Domain.Models;

namespace Ventrix.Domain.Interfaces
{
    public interface IBorrowRepository
    {
        Task AddRecordAsync(BorrowRecord record);
        Task<BorrowRecord> GetRecordByIdAsync(int id);
        Task UpdateRecordAsync(BorrowRecord record);
        Task<IEnumerable<BorrowRecord>> GetAllAsync();
        Task ClearAllRecordsAsync();
    }
}