using Ventrix.Domain.Interfaces;
using Ventrix.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ventrix.Infrastructure.Repositories
{
    public class BorrowRepository : IBorrowRepository
    {
        private readonly AppDbContext _context;

        public BorrowRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRecordAsync(BorrowRecord record)
        {
            await _context.BorrowRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }

        public async Task<BorrowRecord> GetRecordByIdAsync(int id)
        {
            return await _context.BorrowRecords.FindAsync(id);
        }

        public async Task<IEnumerable<BorrowRecord>> GetAllAsync()
        {
            return await _context.BorrowRecords.ToListAsync();
        }

        public async Task<IEnumerable<BorrowRecord>> GetActiveRecordsAsync()
        {
            return await _context.BorrowRecords
                .Where(b => b.Status == "Active")
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRecord>> GetReturnedHistoryAsync()
        {
            return await _context.BorrowRecords
                .Where(b => b.Status == "Returned")
                .OrderByDescending(b => b.ReturnDate)
                .ToListAsync();
        }

        public async Task UpdateRecordAsync(BorrowRecord record)
        {
            _context.Entry(record).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task ClearAllRecordsAsync()
        {
            var allRecords = await _context.BorrowRecords.ToListAsync();
            _context.BorrowRecords.RemoveRange(allRecords);
            await _context.SaveChangesAsync();
        }
    }
}