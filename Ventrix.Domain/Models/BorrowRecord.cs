using System;
using System.Collections.Generic;
using System.Text;

namespace Ventrix.Domain.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }
        public string BorrowerId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string Purpose { get; set; } 
        public string GradeLevel { get; set; }
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        public DateTime? ReturnDate { get; set; } 
        public string Status { get; set; } 
    }
}