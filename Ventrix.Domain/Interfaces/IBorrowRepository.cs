using Ventrix.Domain.Models;

namespace Ventrix.Domain.Interfaces
{
    public interface IBorrowRepository
    {
        void AddRecord(BorrowRecord record);
        BorrowRecord GetRecordById(int id);
        IEnumerable<BorrowRecord> GetActiveRecords();
        IEnumerable<BorrowRecord> GetReturnedHistory();
        void UpdateRecord(BorrowRecord record);
    }
}