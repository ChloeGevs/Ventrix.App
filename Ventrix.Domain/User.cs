using System;

namespace Ventrix.Domain
{
    public class User
    {
        public int Id { get; set; } // Database internal ID
        public string UserId { get; set; } // The ID they type (e.g., "2024-0012" or "Admin")
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public string Role { get; set; } // "Student" or "Staff"
        public string Password { get; set; } // Only for Staff/Admin
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Helper to get full name
        public string FullName => $"{FirstName} {LastName} {Suffix}".Trim();
    }
}