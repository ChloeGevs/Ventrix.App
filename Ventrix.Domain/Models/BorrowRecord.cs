using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ventrix.Domain.Models
{
    public enum BorrowStatus { Active, Returned, Overdue }

    public class BorrowRecord
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int InventoryItemId { get; set; }

        [ForeignKey("UserId")]
        public virtual User Borrower { get; set; }

        [ForeignKey("InventoryItemId")]
        public virtual InventoryItem Item { get; set; }
        public string BorrowerId { get; set; }
        public string ItemName { get; set; }
        public string GradeLevel { get; set; }

        public int Quantity { get; set; }
        public string Purpose { get; set; }
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        public DateTime? ReturnDate { get; set; }
        public BorrowStatus Status { get; set; } = BorrowStatus.Active;
    }
}