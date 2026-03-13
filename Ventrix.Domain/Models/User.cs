// File: Ventrix.Domain/Models/User.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ventrix.Domain.Enums;

namespace Ventrix.Domain.Models
{
    public class User
    {
        [Key]
        public string UserId { get; set; } = string.Empty; // Primary Key

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        public string? Suffix { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int Strikes { get; set; } = 0;

        // Business Logic: Lockout after 3 strikes
        public bool IsLockedOut => Strikes >= 3;

        public string FullName => $"{FirstName} {(string.IsNullOrEmpty(MiddleName) ? "" : MiddleName + " ")}{LastName} {Suffix}".Trim();

        // Navigation property for Borrowing History
        public virtual ICollection<BorrowRecord> BorrowingHistory { get; set; } = new List<BorrowRecord>();
    }
}