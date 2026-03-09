using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ventrix.Domain.Enums;

namespace Ventrix.Domain.Models
{
    public class BorrowRecord
    {
        [Key]
        public int Id { get; set; }

        public string BorrowerId { get; set; }

        [ForeignKey("BorrowerId")]
        public virtual User Borrower { get; set; }

        public int InventoryItemId { get; set; }

        [ForeignKey("InventoryItemId")]
        public virtual InventoryItem InventoryItem { get; set; }

        public int Quantity { get; set; }
        public string Purpose { get; set; }
        public GradeLevel GradeLevel { get; set; }

        public string ItemName { get; set; } 
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        public DateTime? ReturnDate { get; set; }
        public BorrowStatus Status { get; set; } = BorrowStatus.Active;

        public bool IsHiddenFromDashboard { get; set; } = false;
    }
}