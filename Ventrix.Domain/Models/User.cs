// File: Ventrix.Domain/Models/User.cs
using System;
using System.ComponentModel.DataAnnotations;
using Ventrix.Domain.Enums;

namespace Ventrix.Domain.Models
{
    public class User
    {
        [Key]
        public string UserId { get; set; } = string.Empty; // Primary Key

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? Suffix { get; set; }
        public UserRole Role { get; set; }
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int Strikes { get; set; } = 0;

        // Business Logic: Lockout after 3 strikes
        public bool IsLockedOut => Strikes >= 3;

        public string FullName => $"{FirstName} {LastName} {Suffix}".Trim();
    }
}