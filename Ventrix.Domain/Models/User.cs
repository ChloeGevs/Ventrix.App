using System;

namespace Ventrix.Domain.Models
{
    public enum UserRole { Student, Faculty, Staff, Admin }

    public class User
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public UserRole Role { get; set; } 
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string FullName => $"{FirstName} {LastName} {Suffix}".Trim();
    }
}