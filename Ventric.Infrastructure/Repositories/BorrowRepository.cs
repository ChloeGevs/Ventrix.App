using Ventrix.Domain.Interfaces;
using Ventrix.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Ventrix.Infrastructure.Repositories
{
    public class BorrowRepository : IBorrowRepository
    {
        private readonly AppDbContext _context;

        public BorrowRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddRecord(BorrowRecord record)
        {
            _context.BorrowRecords.Add(record);
            _context.SaveChanges();
        }

        public BorrowRecord GetRecordById(int id)
        {
            return _context.BorrowRecords.Find(id);
        }

        public IEnumerable<BorrowRecord> GetAll()
        {
            return _context.BorrowRecords.ToList(); 
        }
        public IEnumerable<BorrowRecord> GetActiveRecords()
        {
            return _context.BorrowRecords
                .Where(b => b.Status == "Active")
                .ToList();
        }

        public IEnumerable<BorrowRecord> GetReturnedHistory()
        {
            return _context.BorrowRecords
                .Where(b => b.Status == "Returned")
                .OrderByDescending(b => b.ReturnDate)
                .ToList();
        }

        public void UpdateRecord(BorrowRecord record)
        {
            _context.Entry(record).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}